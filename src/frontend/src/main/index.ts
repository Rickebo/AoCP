import { app, shell, BrowserWindow, ipcMain, dialog } from 'electron'
import { join } from 'path'
import { electronApp, optimizer, is } from '@electron-toolkit/utils'
import * as path from 'node:path'
import * as fs from 'node:fs/promises'

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

  const firstArticleIndex = html.indexOf('<article')
  if (firstArticleIndex === -1) {
    return html
  }

  if (partIndex == null || partIndex < 0) {
    const lastArticleEndIndex = html.lastIndexOf('</article>')
    if (lastArticleEndIndex === -1) {
      return html
    }

    return html.slice(firstArticleIndex, lastArticleEndIndex + '</article>'.length)
  }

  const articles: string[] = []
  let searchIndex = 0

  while (true) {
    const openIndex = html.indexOf('<article', searchIndex)
    if (openIndex === -1) {
      break
    }

    const closeIndex = html.indexOf('</article>', openIndex)
    if (closeIndex === -1) {
      break
    }

    const articleHtml = html.slice(openIndex, closeIndex + '</article>'.length)
    articles.push(articleHtml)
    searchIndex = closeIndex + '</article>'.length
  }

  if (articles.length === 0) {
    return html
  }

  const dayDescArticles = articles.filter((articleHtml) => {
    const openingTagEnd = articleHtml.indexOf('>')
    if (openingTagEnd === -1) {
      return false
    }

    const openingTag = articleHtml.slice(0, openingTagEnd)
    return (
      openingTag.includes('day-desc') ||
      openingTag.includes('class="day-desc"') ||
      openingTag.includes("class='day-desc'")
    )
  })

  const candidates = dayDescArticles.length > 0 ? dayDescArticles : articles
  const clampedIndex = Math.min(Math.max(partIndex, 0), candidates.length - 1)

  return candidates[clampedIndex]
}

async function summarizeDescriptionWithOpenRouter(
  article: string,
  openRouterToken: string
): Promise<string | undefined> {
  try {
    const response = await fetch('https://openrouter.ai/api/v1/chat/completions', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${openRouterToken}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        model: 'openai/gpt-5',
        messages: [
          {
            role: 'system',
            content:
              'You summarize Advent of Code problem descriptions into concise HTML while preserving important formatting such as code blocks and distinct input examples.'
          },
          {
            role: 'user',
            content:
              'Summarize the following Advent of Code problem description HTML into a shorter HTML explanation that keeps the essential details needed to solve the puzzle:\n\n' +
              article
          }
        ]
      })
    })

    if (!response.ok) {
      console.error('OpenRouter API request failed', response.status, await response.text())
      return undefined
    }

    const data: any = await response.json()
    const content = data?.choices?.[0]?.message?.content
    if (typeof content !== 'string') {
      return undefined
    }

    return content
  } catch (error) {
    console.error('Failed to call OpenRouter API', error)
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

  ipcMain.handle(
    'get-raw-description',
    async (_, year, day, token, partIndex): Promise<string> => {
      return await fetchAocArticle(year, day, token, partIndex)
    }
  )

  ipcMain.handle(
    'get-processed-description',
    async (_, article: string, openRouterToken: string): Promise<string | undefined> => {
      try {
        if (openRouterToken == null || openRouterToken.trim().length === 0) {
          return undefined
        }

        const processed = await summarizeDescriptionWithOpenRouter(article, openRouterToken)
        return processed ?? undefined
      } catch (error) {
        console.error('Failed to handle get-processed-description', error)
        return undefined
      }
    }
  )

  ipcMain.handle('save-gif', async (_, defaultFileName: string, bytes: Uint8Array): Promise<boolean> => {
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
  })

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
