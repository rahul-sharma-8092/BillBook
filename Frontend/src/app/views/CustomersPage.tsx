import { useEffect, useState } from 'react'
import { api } from '../api/client'
import type { Customer } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { useDebouncedValue } from '../hooks/useDebouncedValue'

type Draft = Omit<Customer, 'id' | 'createdAt'> & { id?: number }

const empty: Draft = {
  name: '',
  companyName: '',
  mobile: '',
  email: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  district: '',
  state: '',
  country: 'India',
  notes: '',
}

export function CustomersPage() {
  const [q, setQ] = useState('')
  const dq = useDebouncedValue(q, 350)
  const [items, setItems] = useState<Customer[]>([])
  const [draft, setDraft] = useState<Draft>(empty)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState<string | null>(null)

  async function load() {
    const qs = new URLSearchParams()
    if (dq.trim()) qs.set('q', dq.trim())
    const res = await api.get<Customer[]>(`/api/customers?${qs.toString()}`)
    setItems(res.data)
  }

  useEffect(() => {
    load().catch(() => setErr('Failed to load customers.'))
  }, [dq])

  function reset() {
    setDraft(empty)
  }

  async function save() {
    setErr(null)
    if (!draft.name.trim()) {
      setErr('Name is required.')
      return
    }
    setBusy(true)
    try {
      const payload = {
        name: draft.name.trim(),
        companyName: draft.companyName || null,
        mobile: draft.mobile || null,
        email: draft.email || null,
        addressLine1: draft.addressLine1 || null,
        addressLine2: draft.addressLine2 || null,
        city: draft.city || null,
        district: draft.district || null,
        state: draft.state || null,
        country: draft.country || null,
        notes: draft.notes || null,
      }

      if (draft.id) await api.put(`/api/customers/${draft.id}`, payload)
      else await api.post('/api/customers', payload)

      reset()
      await load()
    } catch {
      setErr('Save failed.')
    } finally {
      setBusy(false)
    }
  }

  async function del(row: Customer) {
    if (!window.confirm(`Delete "${row.name}"?`)) return
    try {
      await api.delete(`/api/customers/${row.id}`)
      await load()
    } catch {
      setErr('Delete failed. If invoices are linked, delete will be blocked by SQL.')
    }
  }

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="flex items-end justify-between gap-3">
          <div>
            <div className="text-sm font-extrabold text-ink-950">
              {draft.id ? `Edit Customer #${draft.id}` : 'Add Customer'}
            </div>
            <div className="text-xs text-ink-700">Search by name or mobile</div>
          </div>
          {draft.id ? (
            <Button className="bg-black/10 text-ink-950 hover:bg-black/15" onClick={reset}>
              Cancel Edit
            </Button>
          ) : null}
        </div>

        <div className="mt-3 grid gap-3 md:grid-cols-3">
          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Name</div>
            <Input value={draft.name} onChange={(e) => setDraft((d) => ({ ...d, name: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Mobile</div>
            <Input value={draft.mobile ?? ''} onChange={(e) => setDraft((d) => ({ ...d, mobile: e.target.value }))} />
          </div>
          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Company</div>
            <Input value={draft.companyName ?? ''} onChange={(e) => setDraft((d) => ({ ...d, companyName: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Email</div>
            <Input value={draft.email ?? ''} onChange={(e) => setDraft((d) => ({ ...d, email: e.target.value }))} />
          </div>
          <div className="md:col-span-3">
            <div className="text-xs font-semibold text-ink-700">Address Line 1</div>
            <Input value={draft.addressLine1 ?? ''} onChange={(e) => setDraft((d) => ({ ...d, addressLine1: e.target.value }))} />
          </div>
          <div className="md:col-span-3">
            <div className="text-xs font-semibold text-ink-700">Address Line 2</div>
            <Input value={draft.addressLine2 ?? ''} onChange={(e) => setDraft((d) => ({ ...d, addressLine2: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">City</div>
            <Input value={draft.city ?? ''} onChange={(e) => setDraft((d) => ({ ...d, city: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">District</div>
            <Input value={draft.district ?? ''} onChange={(e) => setDraft((d) => ({ ...d, district: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">State</div>
            <Input value={draft.state ?? ''} onChange={(e) => setDraft((d) => ({ ...d, state: e.target.value }))} />
          </div>
          <div className="md:col-span-3">
            <div className="text-xs font-semibold text-ink-700">Notes</div>
            <Input value={draft.notes ?? ''} onChange={(e) => setDraft((d) => ({ ...d, notes: e.target.value }))} />
          </div>
        </div>

        <div className="mt-4 flex gap-2">
          <Button disabled={busy} onClick={save}>
            {draft.id ? 'Update' : 'Create'}
          </Button>
          {!draft.id ? (
            <Button className="bg-black/10 text-ink-950 hover:bg-black/15" disabled={busy} onClick={reset}>
              Reset
            </Button>
          ) : null}
        </div>
        {err ? <div className="mt-3 text-sm text-red-700">{err}</div> : null}
      </Card>

      <Card className="p-4">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div className="text-sm font-extrabold text-ink-950">Customers</div>
          <div className="w-72">
            <Input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search name / mobile..." />
          </div>
        </div>

        <div className="mt-3 overflow-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-black/5 text-xs font-semibold text-ink-700">
              <tr>
                <th className="px-3 py-2">Name</th>
                <th className="px-3 py-2">Mobile</th>
                <th className="px-3 py-2">Company</th>
                <th className="px-3 py-2 w-52">Actions</th>
              </tr>
            </thead>
            <tbody>
              {items.map((c) => (
                <tr key={c.id} className="border-t border-black/5">
                  <td className="px-3 py-2 font-semibold text-ink-950">{c.name}</td>
                  <td className="px-3 py-2">{c.mobile ?? ''}</td>
                  <td className="px-3 py-2">{c.companyName ?? ''}</td>
                  <td className="px-3 py-2">
                    <div className="flex gap-2">
                      <Button
                        className="bg-black/10 text-ink-950 hover:bg-black/15"
                        onClick={() =>
                          setDraft({
                            id: c.id,
                            name: c.name,
                            companyName: c.companyName ?? '',
                            mobile: c.mobile ?? '',
                            email: c.email ?? '',
                            addressLine1: c.addressLine1 ?? '',
                            addressLine2: c.addressLine2 ?? '',
                            city: c.city ?? '',
                            district: c.district ?? '',
                            state: c.state ?? '',
                            country: c.country ?? 'India',
                            notes: c.notes ?? '',
                          })
                        }
                      >
                        Edit
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
                  <td className="px-3 py-8 text-ink-700" colSpan={4}>
                    No customers found.
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

