import { Badge, Button, Form, InputGroup, ListGroup, Modal, Stack } from 'react-bootstrap'
import { FC, useCallback, useEffect, useMemo, useState } from 'react'
import { BsBoxArrowUpRight, BsSearch, BsSortDown, BsSortUp, BsX } from 'react-icons/bs'
import './StorageModal.css'

export interface StorageModalProps {
  show: boolean
  hide: () => void
}

interface StorageEntry {
  key: string
  size: number
  typeLabel: string
  description?: string
  preview: string
  storedAt?: number
}

type SortOption = 'key-asc' | 'key-desc' | 'size-asc' | 'size-desc' | 'type-asc'
const META_KEY = '__storage_meta__'

interface SearchFields {
  key: boolean
  description: boolean
  type: boolean
  size: boolean
  date: boolean
  time: boolean
}

const formatBytes = (bytes: number): string => {
  const units = ['B', 'KB', 'MB', 'GB', 'TB']
  let size = bytes
  let unitIndex = 0

  while (size >= 1024 && unitIndex < units.length - 1) {
    size = size / 1024
    unitIndex++
  }

  const precision = size >= 10 || unitIndex === 0 ? 0 : 1
  return `${size.toFixed(precision)} ${units[unitIndex]}`
}

const getTimestampParts = (value?: number): { date: string; time: string; full: string } => {
  if (value == null || Number.isNaN(value))
    return { date: 'Unknown', time: 'Unknown', full: 'Unknown' }
  const dateObj = new Date(value)
  if (Number.isNaN(dateObj.getTime())) return { date: 'Unknown', time: 'Unknown', full: 'Unknown' }

  const date = dateObj.toLocaleDateString()
  const time = dateObj.toLocaleTimeString()
  return { date, time, full: `${date} ${time}` }
}

const clampPreview = (value: string, maxLength: number = 1200): string => {
  if (value.length <= maxLength) return value
  return `${value.slice(0, maxLength)}\n... truncated ...`
}

const describeKey = (key: string): string | undefined => {
  if (key === 'settings') return 'App settings, tokens, and AI preferences.'
  if (key === 'backend_options') return 'Saved backend endpoints and the active selection.'
  if (key === 'settings_modal_ui') return 'Settings dialog accordion layout.'
  if (key === 'author') return 'Last selected author.'

  const discussionMatch = key.match(/^(.*)-discussion-part-(\d+)$/)
  if (discussionMatch != null) {
    const part = Number.parseInt(discussionMatch[2] ?? '0', 10) + 1
    return `AI discussion history for part ${Number.isNaN(part) ? '?' : part}.`
  }

  const descriptionMatch = key.match(/^(.*)-part-(\d+)$/)
  if (descriptionMatch != null) {
    const part = Number.parseInt(descriptionMatch[2] ?? '0', 10) + 1
    return `Problem description cache for part ${Number.isNaN(part) ? '?' : part}.`
  }

  if (key.startsWith('input-')) {
    return 'Cached problem inputs for this puzzle set.'
  }

  return undefined
}

const describeValue = (value: string): { typeLabel: string; preview: string } => {
  if (value.length === 0) {
    return { typeLabel: 'Empty', preview: '[empty]' }
  }

  try {
    const parsed = JSON.parse(value)
    const typeLabel = Array.isArray(parsed)
      ? 'JSON array'
      : parsed != null && typeof parsed === 'object'
        ? 'JSON object'
        : `JSON ${typeof parsed}`
    return {
      typeLabel,
      preview: JSON.stringify(parsed, null, 2)
    }
  } catch {
    return {
      typeLabel: 'Text',
      preview: value
    }
  }
}

const getByteSize = (value: string): number => {
  try {
    return new TextEncoder().encode(value).length
  } catch {
    return value.length
  }
}

const StorageModal: FC<StorageModalProps> = (props) => {
  const [entries, setEntries] = useState<StorageEntry[]>([])
  const [sort, setSort] = useState<SortOption>('key-asc')
  const [search, setSearch] = useState<string>('')
  const [storedAfter, setStoredAfter] = useState<string>('')
  const [storedBefore, setStoredBefore] = useState<string>('')
  const [searchFields, setSearchFields] = useState<SearchFields>({
    key: true,
    description: true,
    type: true,
    size: true,
    date: true,
    time: true
  })
  const [viewingEntry, setViewingEntry] = useState<{ key: string; value: string } | undefined>(
    undefined
  )

  const loadMeta = (): Record<string, number> => {
    try {
      const raw = localStorage.getItem(META_KEY)
      if (raw == null) return {}
      const parsed = JSON.parse(raw)
      return typeof parsed === 'object' && parsed != null ? (parsed as Record<string, number>) : {}
    } catch {
      return {}
    }
  }

  const saveMeta = (meta: Record<string, number>): void => {
    try {
      localStorage.setItem(META_KEY, JSON.stringify(meta))
    } catch (error) {
      console.error('Failed to persist storage metadata', error)
    }
  }

  const refreshEntries = useCallback((): void => {
    const result: StorageEntry[] = []
    const meta = loadMeta()
    let metaChanged = false

    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i)
      if (key == null) continue
      if (key === META_KEY) continue

      const value = localStorage.getItem(key) ?? ''
      const { typeLabel, preview } = describeValue(value)
      const storedAt = meta[key]
      if (storedAt == null) {
        meta[key] = Date.now()
        metaChanged = true
      }

      result.push({
        key,
        size: getByteSize(value),
        typeLabel,
        description: describeKey(key),
        preview: clampPreview(preview),
        storedAt: meta[key]
      })
    }

    result.sort((a, b) => a.key.localeCompare(b.key))
    setEntries(result)
    if (metaChanged) saveMeta(meta)
  }, [])

  useEffect(() => {
    if (props.show) {
      refreshEntries()
    }
  }, [props.show, refreshEntries])

  const totalSize = useMemo(() => entries.reduce((sum, entry) => sum + entry.size, 0), [entries])

  const filteredEntries = useMemo(() => {
    const query = search.trim().toLowerCase()
    const after = storedAfter.trim().length > 0 ? new Date(storedAfter).getTime() : undefined
    const before = storedBefore.trim().length > 0 ? new Date(storedBefore).getTime() : undefined
    const activeSearchFields =
      searchFields.key ||
      searchFields.description ||
      searchFields.type ||
      searchFields.size ||
      searchFields.date ||
      searchFields.time

    const matchesFilters = (entry: StorageEntry): boolean => {
      const matchesAfter = after == null || (entry.storedAt != null && entry.storedAt >= after)
      const matchesBefore = before == null || (entry.storedAt != null && entry.storedAt <= before)

      if (query.length === 0) {
        return matchesAfter && matchesBefore
      }

      const searchables: string[] = []
      const ts = getTimestampParts(entry.storedAt)
      if (searchFields.key || !activeSearchFields) searchables.push(entry.key)
      if (searchFields.description || !activeSearchFields) {
        if (entry.description != null) searchables.push(entry.description)
        searchables.push(entry.preview)
      }
      if (searchFields.type || !activeSearchFields) searchables.push(entry.typeLabel)
      if (searchFields.size || !activeSearchFields) {
        searchables.push(`${entry.size}`)
        searchables.push(formatBytes(entry.size))
      }
      if (searchFields.date || !activeSearchFields) searchables.push(ts.date)
      if (searchFields.time || !activeSearchFields) searchables.push(ts.time)

      const matchesQuery = searchables.some((s) => s.toLowerCase().includes(query))

      return matchesQuery && matchesAfter && matchesBefore
    }

    const filtered = entries.filter(matchesFilters)

    filtered.sort((a, b) => {
      switch (sort) {
        case 'key-asc':
          return a.key.localeCompare(b.key)
        case 'key-desc':
          return b.key.localeCompare(a.key)
        case 'size-asc':
          return a.size - b.size
        case 'size-desc':
          return b.size - a.size
        case 'type-asc': {
          const typeCompare = a.typeLabel.localeCompare(b.typeLabel)
          if (typeCompare !== 0) return typeCompare
          return a.key.localeCompare(b.key)
        }
        default:
          return 0
      }
    })

    return filtered
  }, [entries, search, sort, storedAfter, storedBefore, searchFields])

  const filteredSize = useMemo(
    () => filteredEntries.reduce((sum, entry) => sum + entry.size, 0),
    [filteredEntries]
  )
  const hasFilters =
    search.trim().length > 0 || storedAfter.trim().length > 0 || storedBefore.trim().length > 0
  const sortIsDesc = sort.endsWith('-desc')

  const handleClearKey = (key: string): void => {
    localStorage.removeItem(key)
    refreshEntries()
  }

  const handleClearAll = (): void => {
    const confirmed = window.confirm(
      'This will remove all stored settings, inputs, cached descriptions, and discussions on this device. Are you sure?'
    )
    if (!confirmed) return

    localStorage.clear()
    refreshEntries()
  }

  const handleClearFiltered = (): void => {
    if (filteredEntries.length === 0) return
    const confirmed = window.confirm(
      `This will remove ${filteredEntries.length} filtered entr${filteredEntries.length === 1 ? 'y' : 'ies'} from storage. Are you sure?`
    )
    if (!confirmed) return

    filteredEntries.forEach((entry) => {
      localStorage.removeItem(entry.key)
    })

    const meta = loadMeta()
    filteredEntries.forEach((entry) => {
      delete meta[entry.key]
    })
    saveMeta(meta)

    refreshEntries()
  }

  const resetFilters = (): void => {
    setSearch('')
    setTypeFilters([])
    setSort('key-asc')
    setStoredAfter('')
    setStoredBefore('')
    setSearchFields({
      key: true,
      description: true,
      type: true,
      size: true,
      date: true,
      time: true
    })
  }

  const handleViewKey = (key: string): void => {
    try {
      const rawValue = localStorage.getItem(key) ?? ''
      let formatted = rawValue

      try {
        const parsed = JSON.parse(rawValue)
        formatted = JSON.stringify(parsed, null, 2)
      } catch {
        formatted = rawValue
      }

      setViewingEntry({ key, value: formatted })
    } catch (error) {
      console.error('Failed to read cached entry', error)
      setViewingEntry({ key, value: '[Could not read entry]' })
    }
  }

  return (
    <Modal show={props.show} onHide={props.hide} size="lg" scrollable centered>
      <Modal.Header closeButton>
        <Modal.Title>Storage & Cache</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Stack gap={3}>
          <div className="d-flex align-items-center flex-wrap gap-2">
            <span className="fw-semibold">Local data on this device</span>
            <Badge bg="secondary">
              Showing {filteredEntries.length} / {entries.length} keys
            </Badge>
            <Badge bg="dark">
              {formatBytes(filteredSize)} / {formatBytes(totalSize)}
            </Badge>
            <div className="ms-auto d-flex align-items-center gap-2">
              {hasFilters && (
                <Button
                  variant="outline-warning"
                  size="sm"
                  onClick={handleClearFiltered}
                  disabled={filteredEntries.length === 0}
                >
                  Clear filtered
                </Button>
              )}
              <Button
                variant="outline-danger"
                size="sm"
                onClick={handleClearAll}
                disabled={entries.length === 0}
              >
                Clear everything
              </Button>
            </div>
          </div>

          <Stack direction="vertical" gap={3}>
            <Stack gap={3}>
              <div className="d-flex flex-column gap-2">
                <InputGroup style={{ minWidth: '220px' }}>
                  <InputGroup.Text>
                    <BsSearch />
                  </InputGroup.Text>
                  <Form.Control
                    placeholder="Search entries"
                    value={search}
                    onChange={(e) => setSearch(e.currentTarget.value)}
                  />
                  {search.trim().length > 0 && (
                    <Button
                      variant="outline-secondary"
                      size="sm"
                      onClick={resetFilters}
                      title="Clear search"
                    >
                      <BsX />
                    </Button>
                  )}
                </InputGroup>

                <div className="d-flex flex-wrap align-items-center gap-3 search-divider">
                  <span className="text-muted small">Fields:</span>
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Keys"
                    checked={searchFields.key}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, key: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Descriptions"
                    checked={searchFields.description}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, description: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Types"
                    checked={searchFields.type}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, type: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Sizes"
                    checked={searchFields.size}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, size: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Dates"
                    checked={searchFields.date}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, date: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                  <Form.Check
                    inline
                    type="checkbox"
                    label="Times"
                    checked={searchFields.time}
                    onChange={(e) => {
                      const checked = e.currentTarget.checked
                      setSearchFields((prev) => {
                        const next = { ...prev, time: checked }
                        return Object.values(next).some(Boolean) ? next : prev
                      })
                    }}
                  />
                </div>
              </div>

              <div className="search-divider d-flex flex-wrap align-items-center gap-2">
                <InputGroup style={{ minWidth: '200px' }}>
                  <InputGroup.Text>{sortIsDesc ? <BsSortDown /> : <BsSortUp />}</InputGroup.Text>
                  <Form.Select
                    value={sort}
                    onChange={(e) => setSort(e.currentTarget.value as SortOption)}
                  >
                    <option value="key-asc">Key (A -&gt; Z)</option>
                    <option value="key-desc">Key (Z -&gt; A)</option>
                    <option value="size-asc">Size (small -&gt; large)</option>
                    <option value="size-desc">Size (large -&gt; small)</option>
                    <option value="type-asc">Type (A -&gt; Z)</option>
                    <option value="type-desc">Type (Z -&gt; A)</option>
                  </Form.Select>
                </InputGroup>
              </div>

              <Stack direction="horizontal" gap={2} className="flex-wrap search-divider">
                <Form.Group>
                  <Form.Label className="mb-1 small text-muted">Stored after</Form.Label>
                  <Form.Control
                    type="datetime-local"
                    value={storedAfter}
                    onChange={(e) => setStoredAfter(e.currentTarget.value)}
                    style={{ minWidth: '230px' }}
                  />
                </Form.Group>
                <Form.Group>
                  <Form.Label className="mb-1 small text-muted">Stored before</Form.Label>
                  <Form.Control
                    type="datetime-local"
                    value={storedBefore}
                    onChange={(e) => setStoredBefore(e.currentTarget.value)}
                    style={{ minWidth: '230px' }}
                  />
                </Form.Group>
              </Stack>
            </Stack>
          </Stack>

          {entries.length === 0 ? (
            <div className="text-muted">Nothing cached yet.</div>
          ) : (
            <div className="storage-entry-container">
              <ListGroup variant="flush" className="storage-entry-list">
                {filteredEntries.map((entry) => (
                  <ListGroup.Item key={entry.key} className="storage-entry">
                    <Stack gap={0} className="storage-entry-stack">
                      <div className="d-flex align-items-center gap-2 storage-entry-header">
                        <div className="storage-key">
                          <code>{entry.key}</code>
                        </div>
                        <div className="ms-auto d-flex align-items-center gap-2">
                          <Button
                            variant="outline-secondary"
                            size="sm"
                            className="d-flex align-items-center gap-1"
                            onClick={() => handleViewKey(entry.key)}
                          >
                            <BsBoxArrowUpRight /> Open
                          </Button>
                          <Button
                            variant="outline-danger"
                            size="sm"
                            onClick={() => handleClearKey(entry.key)}
                          >
                            Clear
                          </Button>
                        </div>
                      </div>

                      <Stack className="storage-entry-body">
                        {entry.description != null && (
                          <div className="text-muted small">{entry.description}</div>
                        )}

                        <div className="d-flex flex-wrap align-items-center gap-2">
                          <Badge className="badge-type">{entry.typeLabel}</Badge>
                          <Badge className="badge-size">{formatBytes(entry.size)}</Badge>
                          {((): JSX.Element => {
                            const ts = getTimestampParts(entry.storedAt)
                            return (
                              <>
                                <Badge className="badge-date">{ts.date}</Badge>
                                <Badge className="badge-time">{ts.time}</Badge>
                              </>
                            )
                          })()}
                        </div>

                        <pre className="storage-entry-preview mb-0">{entry.preview}</pre>
                      </Stack>
                    </Stack>
                  </ListGroup.Item>
                ))}
              </ListGroup>
            </div>
          )}
        </Stack>
      </Modal.Body>
      <Modal.Footer className="d-flex flex-wrap gap-2 align-items-center">
        <span className="text-muted small">
          Clearing affects only this device. Tokens and cached descriptions will be removed locally.
        </span>
        <div className="ms-auto d-flex gap-2">
          <Button variant="secondary" onClick={props.hide}>
            Close
          </Button>
        </div>
      </Modal.Footer>

      <Modal
        show={viewingEntry != null}
        onHide={() => setViewingEntry(undefined)}
        size="lg"
        centered
      >
        <Modal.Header closeButton>
          <Modal.Title>View cached entry</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Stack gap={3}>
            <div className="storage-key">
              <code>{viewingEntry?.key}</code>
            </div>
            <Form.Control
              as="textarea"
              rows={12}
              value={viewingEntry?.value ?? ''}
              readOnly
              style={{ fontFamily: 'Source Code Pro, monospace' }}
            />
          </Stack>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setViewingEntry(undefined)}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </Modal>
  )
}

export default StorageModal
