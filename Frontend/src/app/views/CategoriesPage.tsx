import { useEffect, useState } from 'react'
import { api } from '../api/client'
import type { Category } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { Select } from '../ui/Select'

export function CategoriesPage() {
  const [items, setItems] = useState<Category[]>([])
  const [name, setName] = useState('')
  const [isActive, setIsActive] = useState(true)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState<string | null>(null)

  async function load() {
    const res = await api.get<Category[]>('/api/categories')
    setItems(res.data)
  }

  useEffect(() => {
    load().catch(() => setErr('Failed to load categories.'))
  }, [])

  async function create() {
    setErr(null)
    if (!name.trim()) return
    setBusy(true)
    try {
      await api.post('/api/categories', { name: name.trim(), isActive })
      setName('')
      setIsActive(true)
      await load()
    } catch {
      setErr('Create failed.')
    } finally {
      setBusy(false)
    }
  }

  async function update(row: Category) {
    const nextName = window.prompt('Category name', row.name)
    if (!nextName?.trim()) return
    try {
      await api.put(`/api/categories/${row.id}`, { name: nextName.trim(), isActive: row.isActive ?? true })
      await load()
    } catch {
      setErr('Update failed.')
    }
  }

  async function toggle(row: Category) {
    try {
      await api.put(`/api/categories/${row.id}`, { name: row.name, isActive: !(row.isActive ?? true) })
      await load()
    } catch {
      setErr('Update failed.')
    }
  }

  async function del(row: Category) {
    if (!window.confirm(`Delete category "${row.name}"?`)) return
    try {
      await api.delete(`/api/categories/${row.id}`)
      await load()
    } catch {
      setErr('Delete failed. If products use this category, delete will be blocked by SQL.')
    }
  }

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="text-sm font-extrabold text-ink-950">Category</div>
        <div className="mt-3 grid gap-3 md:grid-cols-4">
          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Name</div>
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="e.g. Accessories" />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Active</div>
            <Select value={isActive ? '1' : '0'} onChange={(e) => setIsActive(e.target.value === '1')}>
              <option value="1">Yes</option>
              <option value="0">No</option>
            </Select>
          </div>
          <div className="flex items-end">
            <Button disabled={busy || !name.trim()} onClick={create} className="w-full">
              Add
            </Button>
          </div>
        </div>
        {err ? <div className="mt-3 text-sm text-red-700">{err}</div> : null}
      </Card>

      <Card className="overflow-hidden">
        <div className="px-4 py-3 text-sm font-extrabold text-ink-950">All Categories</div>
        <div className="overflow-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-black/5 text-xs font-semibold text-ink-700">
              <tr>
                <th className="px-4 py-2">Name</th>
                <th className="px-4 py-2">Active</th>
                <th className="px-4 py-2 w-56">Actions</th>
              </tr>
            </thead>
            <tbody>
              {items.map((c) => (
                <tr key={c.id} className="border-t border-black/5">
                  <td className="px-4 py-2 font-semibold text-ink-950">{c.name}</td>
                  <td className="px-4 py-2">{(c.isActive ?? true) ? 'Yes' : 'No'}</td>
                  <td className="px-4 py-2">
                    <div className="flex gap-2">
                      <Button className="bg-black/10 text-ink-950 hover:bg-black/15" onClick={() => update(c)}>
                        Edit
                      </Button>
                      <Button className="bg-black/10 text-ink-950 hover:bg-black/15" onClick={() => toggle(c)}>
                        Toggle
                      </Button>
                      <Button className="bg-red-600 hover:bg-red-700" onClick={() => del(c)}>
                        Delete
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
              {items.length === 0 ? (
                <tr>
                  <td className="px-4 py-8 text-ink-700" colSpan={3}>
                    No categories yet.
                  </td>
                </tr>
              ) : null}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  )
}

