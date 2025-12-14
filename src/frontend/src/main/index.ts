import { app, shell, BrowserWindow, ipcMain, dialog } from 'electron'
import { join } from 'path'
import { electronApp, optimizer, is } from '@electron-toolkit/utils'
import * as path from 'node:path'
import * as fs from 'node:fs/promises'
import { randomUUID } from 'node:crypto'
import { HTMLElement, parse } from 'node-html-parser'

type SummaryConfig = {
  model?: string
  systemPrompt?: string
  userPrompt?: string
  reasoningEffort?: string
  reasoningMaxTokens?: number
}

const defaultSummaryModel = 'openai/gpt-5'
const defaultSummarySystemPrompt =
  'You summarize Advent of Code problem descriptions into concise HTML while preserving important formatting such as code blocks and distinct input examples. Use any provided earlier parts only as context to interpret references; the output must cover only the target part.'
const defaultSummaryUserPrompt =
  'Summarize only the target Advent of Code problem part below into shorter HTML that keeps the essential details needed to solve that part. If earlier parts are provided, treat them purely as context and do not re-summarize them.'

type SummaryRequest = {
  model: string
  messages: { role: 'system' | 'user'; content: string }[]
  reasoning?: { effort?: string; max_tokens?: number }
}

export type ChatMessage = { role: 'system' | 'user' | 'assistant'; content: string }
export type DiscussionReasoning = { effort?: string; max_tokens?: number }

const descriptionUnavailableText = 'Description unavailable.'
const submittedAnswerCache = new Map<string, string[]>()

function buildReasoningConfig(
  summaryConfig?: SummaryConfig
): { effort?: string; max_tokens?: number } | undefined {
  const maxTokens = summaryConfig?.reasoningMaxTokens
  const effort = summaryConfig?.reasoningEffort?.trim().toLowerCase()

  if (typeof maxTokens === 'number' && Number.isFinite(maxTokens) && maxTokens > 0) {
    return { max_tokens: Math.floor(maxTokens) }
  }

  if (effort != null && effort.length > 0) {
    return { effort }
  }

  return undefined
}

function buildSummaryRequest(
  article: string,
  previousArticles: string[] = [],
  summaryConfig?: SummaryConfig
): SummaryRequest {
  const model =
    summaryConfig?.model != null && summaryConfig.model.trim().length > 0
      ? summaryConfig.model
      : defaultSummaryModel

  const systemPrompt =
    summaryConfig?.systemPrompt != null && summaryConfig.systemPrompt.trim().length > 0
      ? summaryConfig.systemPrompt
      : defaultSummarySystemPrompt

  const userPrompt =
    summaryConfig?.userPrompt != null && summaryConfig.userPrompt.trim().length > 0
      ? summaryConfig.userPrompt
      : defaultSummaryUserPrompt

  const usableContext = previousArticles.filter(
    (content) => typeof content === 'string' && content.trim().length > 0
  )
  const contextSection =
    usableContext.length > 0
      ? `Context from earlier parts (do not summarize these; use only to resolve references):\n${usableContext
          .map((content, idx) => `Part ${idx + 1}:\n${content}`)
          .join('\n\n---\n\n')}`
      : undefined

  const userContent =
    contextSection != null
      ? `${userPrompt}\n\n${contextSection}\n\nTarget part HTML to summarize:\n${article}`
      : `${userPrompt}\n\nTarget part HTML to summarize:\n${article}`

  return {
    model,
    reasoning: buildReasoningConfig(summaryConfig),
    messages: [
      {
        role: 'system',
        content: systemPrompt
      },
      {
        role: 'user',
        content: userContent
      }
    ]
  }
}

function parseMainElement(html: string): HTMLElement | undefined {
  try {
    const root = parse(html)
    return root.querySelector('main') ?? undefined
  } catch (error) {
    console.error('Failed to parse AoC HTML', error)
    return undefined
  }
}

function collectArticlesFromMain(mainElement: HTMLElement): HTMLElement[] {
  return mainElement.querySelectorAll('article')
}

function extractSubmittedAnswers(mainElement: HTMLElement): string[] {
  return mainElement
    .querySelectorAll('p')
    .map((paragraph) => {
      const text = paragraph.text?.toLowerCase() ?? ''
      if (!text.includes('your puzzle answer was')) return undefined

      const answer = paragraph.querySelector('code')?.text?.trim()
      return answer != null && answer.length > 0 ? answer : undefined
    })
    .filter((answer): answer is string => answer != null)
}

function clampIndex(index: number, length: number): number {
  if (length === 0) return 0
  if (!Number.isFinite(index)) return 0
  return Math.min(Math.max(index, 0), length - 1)
}

function serializeArticles(articles: HTMLElement[]): string {
  return articles.map((article) => article.toString()).join('')
}

async function fetchAocArticle(
  year: number,
  day: number,
  token: string,
  partIndex?: number
): Promise<string> {
  const resp = await fetch(`https://adventofcode.com/${year}/day/${day}`, {
    headers: {
      cookie: `session=${token}`
    }
  })

  const html = await resp.text()

  const mainElement = parseMainElement(html)
  if (mainElement == null) {
    return descriptionUnavailableText
  }

  const submittedAnswers = extractSubmittedAnswers(mainElement)
  submittedAnswerCache.set(`${year}-${day}`, submittedAnswers)

  const articles = collectArticlesFromMain(mainElement)
  if (articles.length === 0) {
    return descriptionUnavailableText
  }

  if (partIndex == null || partIndex < 0) {
    return serializeArticles(articles)
  }

  const clampedIndex = clampIndex(partIndex, articles.length)
  return articles[clampedIndex].toString()
}

async function summarizeDescriptionWithOpenRouter(
  article: string,
  previousArticles: string[],
  openRouterToken: string,
  summaryConfig?: SummaryConfig
): Promise<string | undefined> {
  try {
    const request = buildSummaryRequest(article, previousArticles, summaryConfig)

    const response = await fetch('https://openrouter.ai/api/v1/chat/completions', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${openRouterToken}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(
        request.reasoning != null
          ? {
              model: request.model,
              messages: request.messages,
              reasoning: request.reasoning
            }
          : {
              model: request.model,
              messages: request.messages
            }
      )
    })

    if (!response.ok) {
      console.error('OpenRouter API request failed', response.status, await response.text())
      return undefined
    }

    type ChatCompletionResponse = {
      choices?: { message?: { content?: string } }[]
    }

    const data: ChatCompletionResponse = await response.json()
    const content = data.choices?.[0]?.message?.content
    if (typeof content !== 'string') {
      return undefined
    }

    return content
  } catch (error) {
    console.error('Failed to call OpenRouter API', error)
    return undefined
  }
}

async function summarizeDescriptionWithOpenRouterStream(
  article: string,
  previousArticles: string[],
  openRouterToken: string,
  onChunk: (chunk: string) => void,
  signal?: AbortSignal,
  summaryConfig?: SummaryConfig
): Promise<string | undefined> {
  try {
    const request = buildSummaryRequest(article, previousArticles, summaryConfig)

    const response = await fetch('https://openrouter.ai/api/v1/chat/completions', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${openRouterToken}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(
        request.reasoning != null
          ? {
              model: request.model,
              stream: true,
              messages: request.messages,
              reasoning: request.reasoning
            }
          : {
              model: request.model,
              stream: true,
              messages: request.messages
            }
      ),
      signal
    })

    if (!response.ok) {
      console.error('OpenRouter API request failed', response.status, await response.text())
      return undefined
    }

    const reader = response.body?.getReader()
    if (reader == null) {
      return undefined
    }

    const decoder = new TextDecoder()
    let buffer = ''
    let content = ''
    let finished = false

    while (!finished) {
      const { value, done } = await reader.read()
      finished = done
      if (done) break

      buffer += decoder.decode(value, { stream: true })
      const lines = buffer.split('\n')
      buffer = lines.pop() ?? ''

      for (const line of lines) {
        const trimmed = line.trim()
        if (trimmed.length === 0) continue
        if (trimmed === 'data: [DONE]') {
          return content.length > 0 ? content : undefined
        }

        if (!trimmed.startsWith('data:')) continue

        const payload = trimmed.slice('data:'.length).trimStart()
        if (payload.length === 0) continue

        try {
          type StreamPayload = { choices?: { delta?: { content?: string } }[] }
          const parsed: StreamPayload = JSON.parse(payload)
          const delta = parsed.choices?.[0]?.delta?.content
          if (typeof delta === 'string' && delta.length > 0) {
            content += delta
            onChunk(delta)
          }
        } catch (err) {
          console.error('Failed to parse OpenRouter stream chunk', err)
        }
      }
    }

    return content.length > 0 ? content : undefined
  } catch (error) {
    if ((error as Error)?.name === 'AbortError') {
      return undefined
    }
    console.error('Failed to call OpenRouter API (streaming)', error)
    return undefined
  }
}

async function chatWithOpenRouterStream(
  messages: ChatMessage[],
  openRouterToken: string,
  onChunk: (chunk: string) => void,
  signal?: AbortSignal,
  model?: string,
  reasoning?: DiscussionReasoning
): Promise<string | undefined> {
  try {
    const safeModel = model != null && model.trim().length > 0 ? model.trim() : defaultSummaryModel

    const response = await fetch('https://openrouter.ai/api/v1/chat/completions', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${openRouterToken}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(
        reasoning != null
          ? {
              model: safeModel,
              stream: true,
              messages,
              reasoning
            }
          : {
              model: safeModel,
              stream: true,
              messages
            }
      ),
      signal
    })

    if (!response.ok) {
      console.error('OpenRouter API request failed', response.status, await response.text())
      return undefined
    }

    const reader = response.body?.getReader()
    if (reader == null) {
      return undefined
    }

    const decoder = new TextDecoder()
    let buffer = ''
    let content = ''
    let finished = false

    while (!finished) {
      const { value, done } = await reader.read()
      finished = done
      if (done) break

      buffer += decoder.decode(value, { stream: true })
      const lines = buffer.split('\n')
      buffer = lines.pop() ?? ''

      for (const line of lines) {
        const trimmed = line.trim()
        if (trimmed.length === 0) continue
        if (trimmed === 'data: [DONE]') {
          return content.length > 0 ? content : undefined
        }

        if (!trimmed.startsWith('data:')) continue

        const payload = trimmed.slice('data:'.length).trimStart()
        if (payload.length === 0) continue

        try {
          type StreamPayload = { choices?: { delta?: { content?: string } }[] }
          const parsed: StreamPayload = JSON.parse(payload)
          const delta = parsed.choices?.[0]?.delta?.content
          if (typeof delta === 'string' && delta.length > 0) {
            content += delta
            onChunk(delta)
          }
        } catch (err) {
          console.error('Failed to parse OpenRouter stream chunk', err)
        }
      }
    }

    return content.length > 0 ? content : undefined
  } catch (error) {
    if ((error as Error)?.name === 'AbortError') {
      return undefined
    }
    console.error('Failed to call OpenRouter API (streaming)', error)
    return undefined
  }
}

function createWindow(): void {
  // Create the browser window.
  const mainWindow = new BrowserWindow({
    title: 'AoCP',
    icon: path.join(__dirname, 'src', 'assets', 'icon.png'),
    width: 900,
    height: 670,
    show: false,
    autoHideMenuBar: true,
    ...(process.platform === 'linux' ? {} : {}),
    webPreferences: {
      preload: join(__dirname, '../preload/index.js'),
      sandbox: false
    }
  })

  mainWindow.on('ready-to-show', () => {
    mainWindow.show()
  })

  mainWindow.webContents.setWindowOpenHandler((details) => {
    shell.openExternal(details.url)
    return {
      action: 'deny',
      overrideBrowserWindowOptions: {
        show: false,
        backgroundColor: 'black',
        webPreferences: {
          preload: 'my-child-window-preload-script.js'
        }
      }
    }
  })

  mainWindow.webContents.session.webRequest.onHeadersReceived((details, callback) => {
    callback({
      responseHeaders: {
        'Access-Control-Allow-Origin': [
          details.webContents?.mainFrame.origin ?? details.referrer.replace(/\/$/, '')
        ],
        'Access-Control-Allow-Credentials': ['true'],
        ...details.responseHeaders
      }
    })
  })

  ipcMain.handle('set-cookie', (_, url, cookie): Promise<void> => {
    return mainWindow.webContents.session.cookies.set({
      url: url,
      name: 'session',
      value: cookie
    })
  })

  ipcMain.handle('get-input', async (_, year, day, token): Promise<string> => {
    const resp = await fetch(`https://adventofcode.com/${year}/day/${day}/input`, {
      headers: {
        cookie: `session=${token}`
      }
    })

    return resp.text()
  })

  ipcMain.handle('get-article', async (_, year, day, token): Promise<string> => {
    return await fetchAocArticle(year, day, token)
  })

  ipcMain.handle('get-raw-description', async (_, year, day, token, partIndex): Promise<string> => {
    return await fetchAocArticle(year, day, token, partIndex)
  })

  ipcMain.handle('read-file', async (_, filePath: string): Promise<string | undefined> => {
    try {
      const content = await fs.readFile(filePath, { encoding: 'utf-8' })
      return content
    } catch (error) {
      console.error('Failed to read file', filePath, error)
      return undefined
    }
  })

  ipcMain.handle(
    'get-processed-description',
    async (
      _,
      article: string,
      openRouterToken: string,
      previousArticles: string[] = [],
      summaryConfig?: SummaryConfig
    ): Promise<string | undefined> => {
      try {
        if (openRouterToken == null || openRouterToken.trim().length === 0) {
          return undefined
        }

        const processed = await summarizeDescriptionWithOpenRouter(
          article,
          previousArticles,
          openRouterToken,
          summaryConfig
        )
        return processed ?? undefined
      } catch (error) {
        console.error('Failed to handle get-processed-description', error)
        return undefined
      }
    }
  )

  const activeSummaryStreams = new Map<string, AbortController>()
  const activeDiscussionStreams = new Map<string, AbortController>()

  ipcMain.handle(
    'start-processed-description-stream',
    async (
      event,
      article: string,
      openRouterToken: string,
      previousArticles: string[] = [],
      summaryConfig?: SummaryConfig
    ): Promise<string | undefined> => {
      if (openRouterToken == null || openRouterToken.trim().length === 0) {
        return undefined
      }

      const channel = `processed-description-stream-${randomUUID()}`
      const abortController = new AbortController()
      activeSummaryStreams.set(channel, abortController)
      const sender = event.sender

      summarizeDescriptionWithOpenRouterStream(
        article,
        previousArticles,
        openRouterToken,
        (chunk) => {
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'chunk', content: chunk })
        },
        abortController.signal,
        summaryConfig
      )
        .then((finalContent) => {
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'done', content: finalContent })
        })
        .catch((error) => {
          if (abortController.signal.aborted) return
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'error', message: error?.message ?? String(error) })
        })
        .finally(() => {
          activeSummaryStreams.delete(channel)
        })

      return channel
    }
  )

  ipcMain.handle('cancel-processed-description-stream', (_, channel: string): void => {
    const controller = activeSummaryStreams.get(channel)
    if (controller != null) {
      controller.abort()
      activeSummaryStreams.delete(channel)
    }
  })

  ipcMain.handle(
    'start-discussion-stream',
    async (
      event,
      messages: ChatMessage[],
      openRouterToken: string,
      model?: string,
      reasoning?: DiscussionReasoning
    ): Promise<string | undefined> => {
      if (openRouterToken == null || openRouterToken.trim().length === 0) {
        return undefined
      }

      const channel = `discussion-stream-${randomUUID()}`
      const abortController = new AbortController()
      activeDiscussionStreams.set(channel, abortController)
      const sender = event.sender

      chatWithOpenRouterStream(
        messages,
        openRouterToken,
        (chunk) => {
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'chunk', content: chunk })
        },
        abortController.signal,
        model,
        reasoning
      )
        .then((finalContent) => {
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'done', content: finalContent })
        })
        .catch((error) => {
          if (abortController.signal.aborted) return
          if (sender.isDestroyed()) return
          sender.send(channel, { type: 'error', message: error?.message ?? String(error) })
        })
        .finally(() => {
          activeDiscussionStreams.delete(channel)
        })

      return channel
    }
  )

  ipcMain.handle('cancel-discussion-stream', (_, channel: string): void => {
    const controller = activeDiscussionStreams.get(channel)
    if (controller != null) {
      controller.abort()
      activeDiscussionStreams.delete(channel)
    }
  })

  ipcMain.handle(
    'save-gif',
    async (_, defaultFileName: string, bytes: Uint8Array): Promise<boolean> => {
      const result = await dialog.showSaveDialog(mainWindow, {
        title: 'Save grid animation',
        defaultPath: defaultFileName,
        filters: [
          {
            name: 'WebM Video',
            extensions: ['webm']
          }
        ]
      })

      if (result.canceled || !result.filePath) {
        return false
      }

      await fs.writeFile(result.filePath, Buffer.from(bytes))

      return true
    }
  )

  // HMR for renderer base on electron-vite cli.
  // Load the remote URL for development or the local html file for production.
  if (is.dev && process.env['ELECTRON_RENDERER_URL']) {
    mainWindow.loadURL(process.env['ELECTRON_RENDERER_URL'])
  } else {
    mainWindow.loadFile(join(__dirname, '../renderer/index.html'))
  }
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  // Set app user model id for windows
  electronApp.setAppUserModelId('com.electron')

  // Default open or close DevTools by F12 in development
  // and ignore CommandOrControl + R in production.
  // see https://github.com/alex8088/electron-toolkit/tree/master/packages/utils
  app.on('browser-window-created', (_, window) => {
    optimizer.watchWindowShortcuts(window)
  })

  // IPC test
  ipcMain.on('ping', () => console.log('pong'))

  createWindow()

  app.on('activate', function () {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) createWindow()
  })
})

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

// In this file you can include the rest of your app"s specific main process
// code. You can also put them in separate files and require them here.
