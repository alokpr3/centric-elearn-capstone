import { useState } from 'react'
import { importManifest } from '../api/reports'
import type { ImportResult } from '../api/types'

export default function ImportPage() {
  const [file, setFile] = useState<File | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<ImportResult | null>(null)

  async function submit() {
    if (!file) return
    setBusy(true)
    setError(null)
    try {
      setResult(await importManifest(file))
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setBusy(false)
    }
  }

  return (
    <section>
      <h1>Booking manifest import</h1>
      <input type="file" accept=".csv" onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
      <button disabled={!file || busy} onClick={submit}>{busy ? 'Importing...' : 'Import'}</button>
      {error && <p className="error">{error}</p>}
      {result && (
        <div>
          <p>{result.importedRows} of {result.totalRows} rows imported, {result.skippedRows} skipped.</p>
          <ul>{result.messages.map((m) => <li key={m}>{m}</li>)}</ul>
        </div>
      )}
    </section>
  )
}
