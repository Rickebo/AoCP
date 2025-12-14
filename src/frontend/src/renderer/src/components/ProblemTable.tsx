import { FC, useEffect, useMemo, useRef, useState } from 'react'
import {
  ColumnDef,
  ColumnFiltersState,
  FilterFn,
  RowData,
  SortingState,
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  useReactTable
} from '@tanstack/react-table'
import { useVirtualizer } from '@tanstack/react-virtual'
import { Button, ButtonGroup, Form, Overlay, Popover, Table } from 'react-bootstrap'
import { BsFilter, BsSortDown, BsSortUp, BsX } from 'react-icons/bs'
import { TableColumnAlignment } from '../data/ProblemUpdate'
import './ProblemTable.css'

/* eslint-disable @typescript-eslint/no-unused-vars */
declare module '@tanstack/react-table' {
  interface ColumnMeta<TData extends RowData, TValue> {
    alignment?: TableColumnAlignment
  }
}
/* eslint-enable @typescript-eslint/no-unused-vars */

export interface ProblemTableProps {
  columns: {
    header: string
    alignment: TableColumnAlignment
  }[]
  rows: string[][]
}

type ProblemRow = Record<string, string>

const alignmentClass: Record<TableColumnAlignment, string> = {
  left: 'text-start',
  center: 'text-center',
  right: 'text-end'
}

type FilterOperator =
  | 'contains'
  | 'equals'
  | 'startsWith'
  | 'endsWith'
  | 'gt'
  | 'lt'
  | 'gte'
  | 'lte'
  | 'isEmpty'
  | 'isNotEmpty'

interface FilterValue {
  op: FilterOperator
  value: string
}

interface FilterCondition extends FilterValue {
  id: string
}

const filterOptions: { value: FilterOperator; label: string }[] = [
  { value: 'contains', label: 'Contains' },
  { value: 'equals', label: 'Equals' },
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
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [activeControl, setActiveControl] = useState<string | null>(null)
  const [conditionDrafts, setConditionDrafts] = useState<Record<string, FilterCondition[]>>({})
  const tableContainerRef = useRef<HTMLDivElement>(null)
  const controlRefs = useRef<Record<string, HTMLButtonElement | null>>({})

  const buildCondition = (): FilterCondition => ({
    id: crypto.randomUUID ? crypto.randomUUID() : `${Date.now()}-${Math.random()}`,
    op: 'contains',
    value: ''
  })

  const normalizeConditions = (value: unknown): FilterCondition[] => {
    const raw = Array.isArray(value) ? (value as Partial<FilterCondition>[]) : []
    const mapped = raw.map((c, index) => ({
      id: c.id ?? `${index}-${Date.now()}`,
      op: c.op ?? 'contains',
      value: c.value ?? ''
    }))
    if (mapped.length === 0) mapped.push(buildCondition())
    return mapped
  }

  const getConditionsForColumn = (columnId: string, rawValue: unknown): FilterCondition[] => {
    return conditionDrafts[columnId] ?? normalizeConditions(rawValue)
  }

  useEffect(() => {
    setSorting([])
    setColumnFilters([])
    setActiveControl(null)
    setConditionDrafts({})
  }, [columns])

  const data = useMemo<ProblemRow[]>(() => {
    return rows.map((row) => {
      const record: ProblemRow = {}
      columns.forEach((_, index) => {
        record[`col-${index}`] = row[index] ?? ''
      })
      return record
    })
  }, [columns, rows])

  const conditionalFilter: FilterFn<ProblemRow> = (row, columnId, filterValue) => {
    if (filterValue == null) return true

    const conditions = normalizeConditions(filterValue)
    const activeConditions = conditions.filter((c) => {
      const requiresValue = !['isEmpty', 'isNotEmpty'].includes(c.op)
      return requiresValue ? c.value.trim().length > 0 : true
    })

    if (activeConditions.length === 0) return true

    const cellValue = (row.getValue(columnId) ?? '').toString()
    const compareTarget = cellValue.toLowerCase()

    return activeConditions.every((condition) => {
      const compareValue = condition.value.toLowerCase()
      switch (condition.op) {
        case 'contains':
          return compareTarget.includes(compareValue)
        case 'equals':
          return compareTarget === compareValue
        case 'startsWith':
          return compareTarget.startsWith(compareValue)
        case 'endsWith':
          return compareTarget.endsWith(compareValue)
        case 'gt': {
          const a = Number(cellValue)
          const b = Number(condition.value)
          return Number.isFinite(a) && Number.isFinite(b) && a > b
        }
        case 'gte': {
          const a = Number(cellValue)
          const b = Number(condition.value)
          return Number.isFinite(a) && Number.isFinite(b) && a >= b
        }
        case 'lt': {
          const a = Number(cellValue)
          const b = Number(condition.value)
          return Number.isFinite(a) && Number.isFinite(b) && a < b
        }
        case 'lte': {
          const a = Number(cellValue)
          const b = Number(condition.value)
          return Number.isFinite(a) && Number.isFinite(b) && a <= b
        }
        case 'isEmpty':
          return cellValue.trim().length === 0
        case 'isNotEmpty':
          return cellValue.trim().length > 0
        default:
          return true
      }
    })
  }

  const columnDefs = useMemo<ColumnDef<ProblemRow, string>[]>(() => {
    return columns.map((col, index) => ({
      id: `col-${index}`,
      header: col.header,
      accessorKey: `col-${index}`,
      enableSorting: true,
      enableColumnFilter: true,
      filterFn: conditionalFilter,
      meta: {
        alignment: col.alignment
      }
    }))
  }, [columns])

  const table = useReactTable({
    data,
    columns: columnDefs,
    state: {
      sorting,
      columnFilters
    },
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel()
  })

  const rowModel = table.getRowModel()
  const rowVirtualizer = useVirtualizer({
    count: rowModel.rows.length,
    getScrollElement: () => tableContainerRef.current,
    estimateSize: () => 36,
    overscan: 8,
    measureElement: (element) => element?.getBoundingClientRect().height ?? 0
  })

  if (columns.length === 0) {
    return null
  }

  const virtualRows = rowVirtualizer.getVirtualItems()
  const paddingTop = virtualRows.length > 0 ? virtualRows[0].start : 0
  const paddingBottom =
    virtualRows.length > 0
      ? rowVirtualizer.getTotalSize() - virtualRows[virtualRows.length - 1].end
      : 0
  const filteredCount = table.getFilteredRowModel().rows.length
  const totalCount = table.getPreFilteredRowModel().rows.length

  return (
    <div className="d-flex flex-column h-100">
      <div
        className="table-responsive flex-grow-1 problem-table-wrapper rounded-3"
        style={{ overflow: 'auto' }}
        ref={tableContainerRef}
      >
        <Table striped hover size="sm" className="mb-0 w-100 problem-table">
          <thead className="position-sticky top-0 bg-body z-3">
            {table.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  const sortedState = header.column.getIsSorted()
                  const conditions = getConditionsForColumn(
                    header.column.id,
                    header.column.getFilterValue()
                  )
                  const filterActive = conditions.some((condition) => {
                    const requiresValue = !['isEmpty', 'isNotEmpty'].includes(condition.op)
                    return requiresValue ? condition.value.trim().length > 0 : true
                  })
                  const isActive = activeControl === header.id
                  const hasState = Boolean(sortedState) || filterActive
                  const applyConditions = (next: FilterCondition[]): void => {
                    setConditionDrafts((current) => ({
                      ...current,
                      [header.column.id]: next
                    }))
                    header.column.setFilterValue(next)
                  }

                  const handleUpdateCondition = (
                    id: string,
                    updates: Partial<FilterCondition>
                  ): void => {
                    const current = getConditionsForColumn(
                      header.column.id,
                      header.column.getFilterValue()
                    )
                    const next = current.map((condition) =>
                      condition.id === id ? { ...condition, ...updates } : condition
                    )
                    applyConditions(next)
                  }
                  const handleRemoveCondition = (id: string): void => {
                    const current = getConditionsForColumn(
                      header.column.id,
                      header.column.getFilterValue()
                    )
                    const remaining = current.filter((condition) => condition.id !== id)
                    applyConditions(remaining.length > 0 ? remaining : [buildCondition()])
                  }
                  const handleAddCondition = (): void => {
                    const current = getConditionsForColumn(
                      header.column.id,
                      header.column.getFilterValue()
                    )
                    applyConditions([...current, buildCondition()])
                  }
                  const handleClearConditions = (): void => {
                    const defaultConditions = [buildCondition()]
                    setConditionDrafts((current) => ({
                      ...current,
                      [header.column.id]: defaultConditions
                    }))
                    header.column.setFilterValue(undefined)
                  }

                  return (
                    <th key={header.id} className="align-middle">
                      <div className="problem-table-header">
                        <div className="d-flex align-items-center gap-2">
                          <span className="fw-semibold">
                            {flexRender(header.column.columnDef.header, header.getContext())}
                          </span>
                          <div className="problem-table-filter-action">
                            <Button
                              ref={(node) => {
                                controlRefs.current[header.id] = node
                              }}
                              size="sm"
                              variant={hasState ? 'primary' : 'outline-secondary'}
                              className="problem-table-filter-btn"
                              onClick={() => {
                                setConditionDrafts((current) => {
                                  if (current[header.column.id] != null) return current
                                  return {
                                    ...current,
                                    [header.column.id]: normalizeConditions(
                                      header.column.getFilterValue()
                                    )
                                  }
                                })
                                setActiveControl((current) =>
                                  current === header.id ? null : header.id
                                )
                              }}
                              aria-label={`Show sort and filter for ${header.column.columnDef.header}`}
                            >
                              <BsFilter />
                            </Button>
                            <Overlay
                              show={isActive}
                              target={controlRefs.current[header.id]}
                              placement="bottom"
                              rootClose
                              onHide={() => setActiveControl(null)}
                            >
                              <Popover
                                id={`popover-${header.id}`}
                                className="problem-table-popover"
                              >
                                <Popover.Body>
                                  <div className="d-flex flex-column gap-3">
                                    <div className="d-flex align-items-center gap-2">
                                      <ButtonGroup size="sm">
                                        <Button
                                          variant={
                                            sortedState === 'asc' ? 'primary' : 'outline-secondary'
                                          }
                                          onClick={() => header.column.toggleSorting(false)}
                                          aria-label={`Sort ${header.column.columnDef.header} ascending`}
                                        >
                                          <BsSortUp />
                                        </Button>
                                        <Button
                                          variant={
                                            sortedState === 'desc' ? 'primary' : 'outline-secondary'
                                          }
                                          onClick={() => header.column.toggleSorting(true)}
                                          aria-label={`Sort ${header.column.columnDef.header} descending`}
                                        >
                                          <BsSortDown />
                                        </Button>
                                      </ButtonGroup>
                                      <Button
                                        size="sm"
                                        variant="outline-secondary"
                                        onClick={() => header.column.clearSorting()}
                                        disabled={!sortedState}
                                        aria-label={`Clear sort for ${header.column.columnDef.header}`}
                                      >
                                        <BsX />
                                        <span className="ms-1">Clear sort</span>
                                      </Button>
                                    </div>
                                    {header.column.getCanFilter() && (
                                      <div className="d-flex flex-column gap-2">
                                        {conditions.map((condition, index) => {
                                          const requiresValue = !['isEmpty', 'isNotEmpty'].includes(
                                            condition.op
                                          )
                                          return (
                                            <div
                                              key={condition.id}
                                              className="d-flex align-items-center gap-2 problem-table-filter-row"
                                            >
                                              <Form.Select
                                                size="sm"
                                                value={condition.op}
                                                onChange={(e) => {
                                                  const nextOp = e.currentTarget
                                                    .value as FilterOperator
                                                  const nextRequiresValue = ![
                                                    'isEmpty',
                                                    'isNotEmpty'
                                                  ].includes(nextOp)
                                                  handleUpdateCondition(condition.id, {
                                                    op: nextOp,
                                                    value: nextRequiresValue ? condition.value : ''
                                                  })
                                                }}
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
                                                onChange={(e) =>
                                                  handleUpdateCondition(condition.id, {
                                                    value: e.currentTarget.value ?? ''
                                                  })
                                                }
                                                placeholder="Filter"
                                                className="problem-table-filter-input"
                                                aria-label={`Filter ${header.column.columnDef.header}`}
                                                autoFocus={index === 0}
                                                disabled={!requiresValue}
                                              />
                                              <Button
                                                size="sm"
                                                variant="outline-secondary"
                                                onClick={() => handleRemoveCondition(condition.id)}
                                                disabled={conditions.length === 1}
                                                aria-label={`Remove filter condition for ${header.column.columnDef.header}`}
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
                                            onClick={handleAddCondition}
                                            aria-label={`Add filter condition for ${header.column.columnDef.header}`}
                                          >
                                            Add condition
                                          </Button>
                                          <Button
                                            size="sm"
                                            variant="outline-secondary"
                                            onClick={handleClearConditions}
                                            disabled={!filterActive}
                                            aria-label={`Clear all filters for ${header.column.columnDef.header}`}
                                          >
                                            Clear filters
                                          </Button>
                                        </div>
                                      </div>
                                    )}
                                  </div>
                                </Popover.Body>
                              </Popover>
                            </Overlay>
                          </div>
                        </div>
                      </div>
                    </th>
                  )
                })}
              </tr>
            ))}
          </thead>
          <tbody>
            {rowModel.rows.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="text-center text-muted py-3">
                  No matching rows
                </td>
              </tr>
            ) : (
              <>
                {paddingTop > 0 && (
                  <tr>
                    <td colSpan={columns.length} style={{ height: paddingTop }} />
                  </tr>
                )}
                {virtualRows.map((virtualRow) => {
                  const row = rowModel.rows[virtualRow.index]
                  return (
                    <tr
                      key={row.id}
                      data-index={virtualRow.index}
                      ref={(node) => {
                        if (node != null) {
                          rowVirtualizer.measureElement(node)
                        }
                      }}
                    >
                      {row.getVisibleCells().map((cell) => {
                        const alignment = cell.column.columnDef.meta?.alignment ?? 'left'
                        return (
                          <td key={cell.id} className={alignmentClass[alignment]}>
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                          </td>
                        )
                      })}
                    </tr>
                  )
                })}
                {paddingBottom > 0 && (
                  <tr>
                    <td colSpan={columns.length} style={{ height: paddingBottom }} />
                  </tr>
                )}
              </>
            )}
          </tbody>
        </Table>
        <div className="problem-table-footer text-center text-secondary">
          <span>
            Showing {filteredCount.toLocaleString()} / {totalCount.toLocaleString()} rows
          </span>
        </div>
      </div>
    </div>
  )
}

export default ProblemTable
