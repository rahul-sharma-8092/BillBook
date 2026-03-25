import { useEffect, useState } from 'react'
import { api } from '../api/client'
import type { Setting } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'

export function SettingsPage() {
  const [data, setData] = useState<Setting | null>(null)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState<string | null>(null)
  const [ok, setOk] = useState<string | null>(null)

  useEffect(() => {
    api
      .get<Setting>('/api/settings')
      .then((res) => setData(res.data))
      .catch(() => setErr('Failed to load settings.'))
  }, [])

  async function save() {
    if (!data) return
    setErr(null)
    setOk(null)
    setBusy(true)
    try {
      const res = await api.put<Setting>('/api/settings', {
        shopName: data.shopName ?? null,
        address: data.address ?? null,
        phone: data.phone ?? null,
        email: data.email ?? null,
        bankName: data.bankName ?? null,
        accountName: data.accountName ?? null,
        accountNumber: data.accountNumber ?? null,
        ifsc: data.ifsc ?? null,
        upi: data.upi ?? null,
        terms: data.terms ?? null,
        footerMessage: data.footerMessage ?? null,
      })
      setData(res.data)
      setOk('Saved.')
    } catch {
      setErr('Save failed.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="text-sm font-extrabold text-ink-950">Settings</div>
        <div className="text-xs text-ink-700">Shop header, payment info, terms</div>

        {!data ? (
          <div className="mt-3 text-ink-700">Loading...</div>
        ) : (
          <div className="mt-4 grid gap-3 md:grid-cols-2">
            <div>
              <div className="text-xs font-semibold text-ink-700">Shop Name</div>
              <Input value={data.shopName ?? ''} onChange={(e) => setData((d) => (d ? { ...d, shopName: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">Phone</div>
              <Input value={data.phone ?? ''} onChange={(e) => setData((d) => (d ? { ...d, phone: e.target.value } : d))} />
            </div>
            <div className="md:col-span-2">
              <div className="text-xs font-semibold text-ink-700">Address</div>
              <Input value={data.address ?? ''} onChange={(e) => setData((d) => (d ? { ...d, address: e.target.value } : d))} />
            </div>

            <div>
              <div className="text-xs font-semibold text-ink-700">Bank Name</div>
              <Input value={data.bankName ?? ''} onChange={(e) => setData((d) => (d ? { ...d, bankName: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">UPI</div>
              <Input value={data.upi ?? ''} onChange={(e) => setData((d) => (d ? { ...d, upi: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">Account Name</div>
              <Input value={data.accountName ?? ''} onChange={(e) => setData((d) => (d ? { ...d, accountName: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">Account Number</div>
              <Input value={data.accountNumber ?? ''} onChange={(e) => setData((d) => (d ? { ...d, accountNumber: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">IFSC</div>
              <Input value={data.ifsc ?? ''} onChange={(e) => setData((d) => (d ? { ...d, ifsc: e.target.value } : d))} />
            </div>
            <div>
              <div className="text-xs font-semibold text-ink-700">Email</div>
              <Input value={data.email ?? ''} onChange={(e) => setData((d) => (d ? { ...d, email: e.target.value } : d))} />
            </div>

            <div className="md:col-span-2">
              <div className="text-xs font-semibold text-ink-700">Terms & Conditions</div>
              <textarea
                className="w-full rounded-xl border border-black/10 bg-white/70 px-3 py-2 text-sm outline-none focus:border-black/20 focus:ring-4 focus:ring-black/5"
                rows={6}
                value={data.terms ?? ''}
                onChange={(e) => setData((d) => (d ? { ...d, terms: e.target.value } : d))}
              />
            </div>
            <div className="md:col-span-2">
              <div className="text-xs font-semibold text-ink-700">Footer Message</div>
              <Input value={data.footerMessage ?? ''} onChange={(e) => setData((d) => (d ? { ...d, footerMessage: e.target.value } : d))} />
            </div>

            <div className="md:col-span-2 flex items-center gap-2">
              <Button disabled={busy} onClick={save}>
                Save
              </Button>
              {ok ? <div className="text-sm text-emerald-700">{ok}</div> : null}
              {err ? <div className="text-sm text-red-700">{err}</div> : null}
            </div>
          </div>
        )}
      </Card>
    </div>
  )
}

