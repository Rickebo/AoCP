/* eslint-disable @typescript-eslint/no-explicit-any */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
import { useState } from 'react'

export type SaveFunction<T extends object> = (mutator: (state: T) => T) => void
export type MergingSaveFunction<T extends object> = (mutator: (state: T) => object) => void
export type UpdatingSaveFunction<T extends object> = (mutator: (state: T) => void) => void

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function setAny(source: object, value: any, ...path: string[]): void {
  let current = source
  for (let i = 0; i < path.length - 1; i++) {
    if (current == null) throw new Error('Invalid path')
    current = source[path[i]]
  }

  current[path[path.length - 1]] = value
}

export function set<T extends object>(saver: SaveFunction<T>, value: any, ...path: string[]): void {
  saver((settings: T) => {
    setAny(settings, value, ...path)
    return settings
  })
}

export function getSetter<T extends object>(
  saver: SaveFunction<T>
): (value: any, ...path: string[]) => void {
  return (value: any, ...path: string[]) => set(saver, value, ...path)
}

export function getMerger<T extends object>(saver: SaveFunction<T>): MergingSaveFunction<T> {
  return (merger: (state: T) => object) => {
    saver((state: T) => {
      return {
        ...state,
        ...merger(state)
      }
    })
  }
}

export function getUpdater<T extends object>(saver: SaveFunction<T>): UpdatingSaveFunction<T> {
  return (updater: (state: T) => void) => {
    saver((state: T) => {
      const clone = structuredClone(state)
      updater(clone)
      return clone
    })
  }
}

export interface SavableState<T extends object> {
  state: T
  set: (newState: T | ((state: T) => T)) => void
  update: (mutator: (state: T) => void) => void
  save: (state: T) => Promise<void>
}

/**
 * Create a savable state from an object.
 * @param initialState The initial state of the savable state.
 * @param saver A function to save the current state
 * @param preProcessor A processor to process the state before it is mutated
 * @param postProcessor A processor to process the state after it is mutated
 */
export function useObjectState<T extends object>(
  initialState: T,
  saver: (state: T) => Promise<void>,
  preProcessor?: ((state: T) => T) | undefined,
  postProcessor?: ((state: T) => T) | undefined
): SavableState<T> {
  const [state, setState] = useState<T>(initialState)

  const set = (mutator: T | ((state: T) => T)): void => {
    if (typeof mutator === 'function') {
      setState((current: T) => {
        let newState = structuredClone(current)
        if (preProcessor != null) newState = preProcessor(newState)

        newState = mutator(newState)

        if (postProcessor != null) newState = postProcessor(newState)

        return newState
      })
    } else {
      let newState = mutator
      if (postProcessor != null) newState = postProcessor(newState)

      setState(newState)
    }
  }

  const update = (mutator: (state: T) => void): void => {
    setState((current: T) => {
      let result = structuredClone(current)

      if (preProcessor != null) result = preProcessor(result)

      mutator(result)

      if (postProcessor != null) result = postProcessor(result)

      return result
    })
  }

  return {
    state: state,
    set: set,
    update: update,
    save: saver
  }
}

/**
 * Create a savable state from a property of another savable state.
 * @param parent The parent savable state to create a new savable state from.
 * @param selector A function selecting a property of the savable state to use to
 * create a child state.
 * @param writer A function to write the child state to the parent state.
 * @returns A savable state of the child property of the specified state. Modifying
 * the child state results in the specified property of parent state being modified,
 * and saving it saves the parent state.
 */
export function subState<TParent extends object, TSub extends object>(
  parent: SavableState<TParent>,
  selector: (state: TParent) => TSub,
  writer: (state: TParent, subState: TSub) => void
): SavableState<TSub> {
  const setSubState = (mutator: TSub | ((subState: TSub) => TSub)): void => {
    parent.set((current: TParent): TParent => {
      writer(current, typeof mutator === 'function' ? mutator(selector(current)) : mutator)
      return current
    })
  }

  const updateSubState = (mutator: (subState: TSub) => void): void => {
    parent.update((current: TParent): void => {
      mutator(selector(current))
    })
  }

  const saver = (state: TSub): Promise<void> => {
    const toSave = structuredClone(parent.state)
    writer(toSave, state)
    return parent.save(toSave)
  }

  return {
    state: selector(parent.state),
    set: setSubState,
    update: updateSubState,
    save: saver
  }
}

/**
 * Convert a savable object state from one type to another type. The source type must
 * be assignable to the target type.
 * @param state The state to convert from
 * @returns An aliased SavableState of the specified type.
 */
export function convertObjectState<TTo extends object, TFrom extends TTo>(
  state: SavableState<TFrom>
): SavableState<TTo> {
  return useObjectState<TTo>(state.state as TTo, (toState: TTo): Promise<void> => {
    const saveObject =
      state.state === toState
        ? state.state
        : {
            ...state.state,
            ...toState
          }

    return state.save(saveObject)
  })
}

/**
 * Clone a savable state.
 * @param state The savable state to clone
 * @returns A clone of the specified state that shares the specified states save
 * function.
 */
export function useObjectStateClone<T extends object>(state: SavableState<T>): SavableState<T> {
  return useObjectState(structuredClone(state.state), state.save)
}

/**
 * Create a savable state that saves to localstorage at a specified key
 * @param storageKey The key to store the state at. If undefined, the state will not
 * be stored or read from storage.
 * @param defaultValue The default value of the state
 */
export function usePersistentState<T extends object>(
  storageKey: string | undefined,
  defaultValue: T
): SavableState<T> {
  const [state, setState] = useState<T>(() => {
    const existingItem = storageKey != null ? localStorage.getItem(storageKey) : undefined
    if (existingItem == null) return defaultValue

    try {
      return {
        ...defaultValue,
        ...JSON.parse(existingItem)
      }
    } catch (ex) {
      console.error(ex)
      return {
        ...defaultValue
      }
    }
  })

  const save = async (newState: T): Promise<void> => {
    if (storageKey != null) localStorage.setItem(storageKey, JSON.stringify(newState))

    setState(newState)
  }

  const set = (mutator: T | ((state: T) => T)): void => {
    if (typeof mutator === 'function') {
      setState((current: T) => mutator(structuredClone(current)))
    } else {
      setState(mutator)
    }
  }

  const update = (mutator: (state: T) => void): void => {
    setState((current: T) => {
      const result = structuredClone(current)
      mutator(result)
      return result
    })
  }

  return {
    state: state,
    set: set,
    update: update,
    save: save
  }
}
