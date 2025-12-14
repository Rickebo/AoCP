import { FC, useEffect, useMemo, useState } from 'react'
import { Button, ButtonGroup, Form, OverlayTrigger, Popover, Table } from 'react-bootstrap'
import { TableColumnAlignment } from '../data/ProblemUpdate'
import { BsFilter, BsPlus, BsSortDown, BsSortUp, BsX } from 'react-icons/bs'
import './ProblemTable.css'

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

type FilterOperator =
  | 'contains'
  | 'doesNotContain'
  | 'equals'
  | 'doesNotEqual'
  | 'startsWith'
  | 'endsWith'
  | 'gt'
  | 'lt'
  | 'gte'
  | 'lte'
  | 'isEmpty'
  | 'isNotEmpty'

interface ColumnCondition {
  id: number
  op: FilterOperator
  value: string
}

interface ColumnFilter {
  conditions: ColumnCondition[]
}

const createCondition = (op: FilterOperator = 'contains'): ColumnCondition => ({
  id: Math.floor(Math.random() * Number.MAX_SAFE_INTEGER),
  op,
  value: ''
})

const defaultFilter = (): ColumnFilter => ({
  conditions: [createCondition()]
})

const alignmentClass: Record<TableColumnAlignment, string> = {
  left: 'text-start',
  center: 'text-center',
  right: 'text-end'
}

const filterOptions: { value: FilterOperator; label: string }[] = [
  { value: 'contains', label: 'Contains' },
  { value: 'doesNotContain', label: "Doesn't contain" },
  { value: 'equals', label: 'Equals' },
  { value: 'doesNotEqual', label: "Doesn't equal" },
  { value: 'startsWith', label: 'Starts with' },
  { value: 'endsWith', label: 'Ends with' },
  { value: 'gt', label: '> (number)' },
  { value: 'gte', label: '>= (number)' },
  { value: 'lt', label: '< (number)' },
  { value: 'lte', label: '<= (number)' },
  { value: 'isEmpty', label: 'Is empty' },
  { value: 'isNotEmpty', label: 'Is not empty' }
]

const ProblemTable: FC<ProblemTableProps> = ({ columns, rows }) => {
  const [sort, setSort] = useState<SortState | null>(null)
  const [filters, setFilters] = useState<ColumnFilter[]>(columns.map(() => defaultFilter()))
  const [activePopover, setActivePopover] = useState<number | null>(null)

  useEffect(() => {
    setFilters(columns.map(() => defaultFilter()))
    setSort(null)
    setActivePopover(null)
  }, [columns])

  const updateCondition = (
    columnIndex: number,
    conditionId: number,
    condition: Partial<ColumnCondition>
  ): void => {
    setFilters((current) => {
      const next = [...current]
      const filter = next[columnIndex]
      const updatedConditions = filter.conditions.map((c) =>
        c.id === conditionId ? { ...c, ...condition } : c
      )
      next[columnIndex] = { ...filter, conditions: updatedConditions }
      return next
    })
  }

  const addCondition = (columnIndex: number): void => {
    setFilters((current) => {
      const next = [...current]
      const filter = next[columnIndex]
      next[columnIndex] = { ...filter, conditions: [...filter.conditions, createCondition()] }
      return next
    })
  }

  const removeCondition = (columnIndex: number, conditionId: number): void => {
    setFilters((current) => {
      const next = [...current]
      const filter = next[columnIndex]
      const remaining = filter.conditions.filter((c) => c.id !== conditionId)
      next[columnIndex] = {
        ...filter,
        conditions: remaining.length > 0 ? remaining : [createCondition()]
      }
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

  const matchesCondition = (cell: string, condition: ColumnCondition): boolean => {
    const target = cell ?? ''
    const value = condition.value
    switch (condition.op) {
      case 'contains':
        return target.toLowerCase().includes(value.toLowerCase())
      case 'doesNotContain':
        return !target.toLowerCase().includes(value.toLowerCase())
      case 'equals':
        return target.toLowerCase() === value.toLowerCase()
      case 'doesNotEqual':
        return target.toLowerCase() !== value.toLowerCase()
      case 'startsWith':
        return target.toLowerCase().startsWith(value.toLowerCase())
      case 'endsWith':
        return target.toLowerCase().endsWith(value.toLowerCase())
      case 'gt': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a > b
      }
      case 'gte': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a >= b
      }
      case 'lt': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a < b
      }
      case 'lte': {
        const a = Number(target)
        const b = Number(value)
        return Number.isFinite(a) && Number.isFinite(b) && a <= b
      }
      case 'isEmpty':
        return target.trim().length === 0
      case 'isNotEmpty':
        return target.trim().length > 0
      default:
        return true
    }
  }

  const matchesFilter = (cell: string, filter: ColumnFilter): boolean => {
    const activeConditions = filter.conditions.filter((c) => {
      const requiresValue = !['isEmpty', 'isNotEmpty'].includes(c.op)
      return requiresValue ? c.value !== '' : true
    })

    if (activeConditions.length === 0) return true

    return activeConditions.every((condition) => matchesCondition(cell, condition))
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
    return null
  }

  return (
    <div className="d-flex flex-column h-100">
      <div
        className="table-responsive flex-grow-1 problem-table-wrapper rounded-3"
        style={{ overflow: 'auto' }}
      >
        <Table striped hover size="sm" className="mb-0 w-100 problem-table">
          <thead className="position-sticky top-0 bg-body z-3">
            <tr>
              {columns.map((col, index) => {
                const isSorted = sort?.columnIndex === index
                const direction = isSorted ? sort?.direction : undefined
                const filter = filters[index] ?? defaultFilter()
                const hasFilter = filter.conditions.some(
                  (c) => c.value !== '' || c.op === 'isEmpty' || c.op === 'isNotEmpty'
                )
                const controlsOpen = activePopover === index
                const buttonVariant = isSorted || hasFilter ? 'primary' : 'outline-secondary'

                const popover = (
                  <Popover id={`column-controls-${index}`} className="problem-table-popover">
                    <Popover.Body>
                      <div className="d-flex align-items-center gap-2 mb-3">
                        <ButtonGroup size="sm">
                          <Button
                            variant={direction === 'asc' ? 'primary' : 'outline-secondary'}
                            onClick={() => setSort({ columnIndex: index, direction: 'asc' })}
                            aria-label={`Sort ${col.header} ascending`}
                          >
                            <BsSortUp />
                          </Button>
                          <Button
                            variant={direction === 'desc' ? 'primary' : 'outline-secondary'}
                            onClick={() => setSort({ columnIndex: index, direction: 'desc' })}
                            aria-label={`Sort ${col.header} descending`}
                          >
                            <BsSortDown />
                          </Button>
                        </ButtonGroup>
                        <Button
                          size="sm"
                          variant="outline-secondary"
                          onClick={() => clearSort(index)}
                          disabled={!isSorted}
                          aria-label={`Clear sort for ${col.header}`}
                        >
                          <BsX />
                          <span className="ms-1">Clear sort</span>
                        </Button>
                      </div>
                      <div className="d-flex flex-column gap-2">
                        {filter.conditions.map((condition) => {
                          const requiresValue = !['isEmpty', 'isNotEmpty'].includes(condition.op)
                          return (
                            <div
                              key={condition.id}
                              className="d-flex align-items-center gap-2 problem-table-condition"
                            >
                              <Form.Select
                                size="sm"
                                value={condition.op}
                                onChange={(e) =>
                                  updateCondition(index, condition.id, {
                                    op: e.currentTarget.value as FilterOperator
                                  })
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
                                value={condition.value}
                                disabled={!requiresValue}
                                onChange={(e) =>
                                  updateCondition(index, condition.id, {
                                    value: e.currentTarget.value
                                  })
                                }
                                placeholder="Filter"
                              />
                              <Button
                                size="sm"
                                variant="outline-secondary"
                                onClick={() => removeCondition(index, condition.id)}
                                aria-label={`Remove filter condition for ${col.header}`}
                              >
                                <BsX />
                              </Button>
                            </div>
                          )
                        })}
                        <div className="d-flex align-items-center gap-2">
                          <Button
                            size="sm"
                            variant="outline-secondary"
                            onClick={() => addCondition(index)}
                            aria-label={`Add filter condition for ${col.header}`}
                          >
                            <BsPlus />
                            <span className="ms-1">Add condition</span>
                          </Button>
                          <Button
                            size="sm"
                            variant="outline-secondary"
                            onClick={() => clearFilter(index)}
                            disabled={!hasFilter}
                            aria-label={`Clear all filters for ${col.header}`}
                          >
                            <BsX />
                            <span className="ms-1">Clear filters</span>
                          </Button>
                        </div>
                      </div>
                    </Popover.Body>
                  </Popover>
                )

                return (
                  <th key={col.header + index} className="align-top">
                    <div className="d-flex align-items-center gap-2">
                      <span className="fw-semibold">{col.header}</span>
                      <OverlayTrigger
                        trigger="click"
                        placement="bottom"
                        rootClose
                        show={controlsOpen}
                        onToggle={(next) => setActivePopover(next ? index : null)}
                        overlay={popover}
                      >
                        <Button
                          size="sm"
                          variant={buttonVariant}
                          className="problem-table-filter-btn"
                          aria-label={`Show sort and filter controls for ${col.header}`}
                        >
                          <BsFilter />
                        </Button>
                      </OverlayTrigger>
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
