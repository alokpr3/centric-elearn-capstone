export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

export async function getJson<T>(path: string, params?: Record<string, string | number | undefined>): Promise<T> {
  const query = new URLSearchParams()
  Object.entries(params ?? {}).forEach(([key, value]) => {
    if (value !== undefined && value !== '') query.set(key, String(value))
  })
  const qs = query.toString()
  const response = await fetch(qs ? `${path}?${qs}` : path)
  if (!response.ok) {
    let message = response.statusText
    try {
      const problem = await response.json()
      message = problem.detail ?? problem.title ?? message
    } catch {
      /* body was not JSON */
    }
    throw new ApiError(response.status, message)
  }
  return response.json() as Promise<T>
}
