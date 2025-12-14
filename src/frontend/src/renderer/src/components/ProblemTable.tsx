import { FC, useEffect, useMemo, useState } from 'react'
import { Button, ButtonGroup, Form, Table } from 'react-bootstrap'
import { TableColumnAlignment } from '../data/ProblemUpdate'

export interface ProblemTableProps {
  columns: {
    header: string
    alignment: TableColumnAlignment
  }[]
  rows: string[][]
}

type SortDirection = 'asc' | 'desc'

interface SortState {
  columnIndex: number
  direction: SortDirection
}

type FilterOperator = 'contains' | 'equals' | 'startsWith' | 'endsWith' | 'gt' | 'lt'

interface ColumnFilter {
  op: FilterOperator
  value: string
}

const defaultFilter = (): ColumnFilter => ({
  op: 'contains',
  value: ''
})

const alignmentClass: Record<TableColumnAlignment, string> = {
  left: 'text-start',
  center: 'text-center',
  right: 'text-end'
}

const filterOptions: { value: FilterOperator; label: string }[] = [
  { value: 'contains', label: 'Contains' },
  { value: 'equals', label: 'Equals' },
  { value: 'startsWith', label: 'Starts with' },
  { value: 'endsWith', label: 'Ends with' },
  { value: 'gt', label: '> (number)' },
  { value: 'lt', label: '< (number)' }
]

const ProblemTable: FC<ProblemTableProps> = ({ columns, rows }) => {
  const [sort, setSort] = useState<SortState | null>(null)
  const [filters, setFilters] = useState<ColumnFilter[]>(columns.map(() => defaultFilter()))

  useEffect(() => {
    setFilters(columns.map(() => defaultFilter()))
    setSort(null)
  }, [columns])

  const updateFilter = (index: number, filter: Partial<ColumnFilter>): void => {
    setFilters((current) => {
      const next = [...current]
      next[index] = { ...next[index], ...filter }
      return next
    })
  }

  const clearFilter = (index: number): void => {
    setFilters((current) => {
      const next = [...current]
      next[index] = defaultFilter()
      return next
    })
  }

  const clearSort = (index?: number): void => {
    if (sort == null) return
    if (index == null || sort.columnIndex === index) setSort(null)
  }

  const matchesFilter = (cell: string, filter: ColumnFilter): boolean => {
    if (!filter?.value) return true
    const target = cell ?? ''
    const value = filter.value
    switch (filter.op) {
      case 'contains':
        return target.toLowerCase().includes(value.toLowerCase())
      case 'equals':
        return target.toLowerCase() === value.toLowerCase()
      case 'startsWith':
        return target.toLowerCase().startsWith(value.toLowerCase())
      case 'endsWith':
        return target.toLowerCase().endsWith(value.toLowerCase())
      case 'gt': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a > b
      }
      case 'lt': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a < b
      }
      default:
        return true
    }
  }

  const applyFiltersAndSort = useMemo(() => {
    const filterRows = rows.filter((row) =>
      columns.every((_, colIndex) =>
        matchesFilter(row[colIndex] ?? '', filters[colIndex] ?? defaultFilter())
      )
    )

    if (sort == null) return filterRows

    const { columnIndex, direction } = sort
    const factor = direction === 'asc' ? 1 : -1
    return [...filterRows].sort((a, b) => {
      const av = a[columnIndex] ?? ''
      const bv = b[columnIndex] ?? ''
      const an = Number(av)
      const bn = Number(bv)
      if (Number.isFinite(an) && Number.isFinite(bn)) return factor * (an - bn)
      return factor * av.localeCompare(bv, undefined, { numeric: true, sensitivity: 'base' })
    })
  }, [columns, filters, rows, sort])

  if (columns.length === 0) {
    return <div className="text-muted small p-3">No table data yet.</div>
  }

  return (
    <div className="d-flex flex-column h-100">
      <div className="table-responsive flex-grow-1" style={{ overflow: 'auto' }}>
        <Table striped hover size="sm" className="mb-0">
          <thead className="position-sticky top-0 bg-body z-3">
            <tr>
              {columns.map((col, index) => {
                const isSorted = sort?.columnIndex === index
                const direction = isSorted ? sort?.direction : undefined
                const filter = filters[index] ?? defaultFilter()
                return (
                  <th key={col.header + index} className="align-top">
                    <div className="d-flex align-items-center gap-2">
                      <span className="fw-semibold">{col.header}</span>
                      <ButtonGroup size="sm">
                        <Button
                          variant={direction === 'asc' ? 'primary' : 'outline-secondary'}
                          onClick={() => setSort({ columnIndex: index, direction: 'asc' })}
                          aria-label={`Sort ${col.header} ascending`}
                        >
                          ↑
                        </Button>
                        <Button
                          variant={direction === 'desc' ? 'primary' : 'outline-secondary'}
                          onClick={() => setSort({ columnIndex: index, direction: 'desc' })}
                          aria-label={`Sort ${col.header} descending`}
                        >
                          ↓
                        </Button>
                        <Button
                          variant="outline-secondary"
                          onClick={() => clearSort(index)}
                          disabled={!isSorted}
                          aria-label={`Clear sort for ${col.header}`}
                        >
                          ✕
                        </Button>
                      </ButtonGroup>
                    </div>
                    <div className="d-flex align-items-center gap-1 mt-2">
                      <Form.Select
                        size="sm"
                        value={filter.op}
                        onChange={(e) =>
                          updateFilter(index, { op: e.currentTarget.value as FilterOperator })
                        }
                      >
                        {filterOptions.map((option) => (
                          <option key={option.value} value={option.value}>
                            {option.label}
                          </option>
                        ))}
                      </Form.Select>
                      <Form.Control
                        size="sm"
                        value={filter.value}
                        onChange={(e) => updateFilter(index, { value: e.currentTarget.value })}
                        placeholder="Filter"
                      />
                      <Button
                        size="sm"
                        variant="outline-secondary"
                        onClick={() => clearFilter(index)}
                        disabled={!filter.value}
                        aria-label={`Clear filter for ${col.header}`}
                      >
                        Clear
                      </Button>
                    </div>
                  </th>
                )
              })}
            </tr>
          </thead>
          <tbody>
            {applyFiltersAndSort.map((row, rowIndex) => (
              <tr key={`row-${rowIndex}`}>
                {columns.map((col, colIndex) => (
                  <td
                    key={`cell-${rowIndex}-${colIndex}`}
                    className={alignmentClass[col.alignment]}
                  >
                    {row[colIndex]}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </Table>
      </div>
    </div>
  )
}

export default ProblemTable
