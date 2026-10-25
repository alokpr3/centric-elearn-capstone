import type { ReactNode } from 'react'
import type { ApiState } from '../hooks/useApi'

interface Props<T> {
  state: ApiState<T>
  isEmpty?: (data: T) => boolean
  emptyText?: string
  children: (data: T) => ReactNode
}

export default function AsyncView<T>({ state, isEmpty, emptyText = 'Nothing to show.', children }: Props<T>) {
  if (state.loading) return <p className="muted">Loading...</p>
  if (state.error) return <p className="error">{state.error}</p>
  if (state.data === null) return null
  if (isEmpty?.(state.data)) return <p className="muted">{emptyText}</p>
  return <>{children(state.data)}</>
}
