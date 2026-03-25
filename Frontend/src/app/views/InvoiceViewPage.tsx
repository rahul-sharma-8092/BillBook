import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { api } from '../api/client'
import type { InvoiceDetails } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'

export function InvoiceViewPage() {
  const { id } = useParams()
  const invoiceId = Number(id)
  const [data, setData] = useState<InvoiceDetails | null>(null)
  const [err, setErr] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    async function load() {
      const res = await api.get<InvoiceDetails>(`/api/invoices/${invoiceId}`)
      if (cancelled) return
      setData(res.data)
    }
    if (!Number.isFinite(invoiceId)) return
    load().catch(() => setErr('Failed to load invoice.'))
    return () => {
      cancelled = true
    }
  }, [invoiceId])

  if (!Number.isFinite(invoiceId)) return <div className="text-red-700">Invalid invoice id.</div>

  const inv = data?.invoice
  const items = data?.items ?? []

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <div className="text-sm font-extrabold text-ink-950">Invoice</div>
          <div className="text-xs text-ink-700">{inv?.invoiceNo ?? ''}</div>
        </div>
        <div className="flex gap-2">
          <Link to="/invoices">
            <Button className="bg-black/10 text-ink-950 hover:bg-black/15">Back</Button>
          </Link>
          <a href={`/api/invoices/${invoiceId}/html`} target="_blank" rel="noreferrer">
            <Button className="bg-black/10 text-ink-950 hover:bg-black/15">Print</Button>
          </a>
          <a href={`/api/invoices/${invoiceId}/pdf`} target="_blank" rel="noreferrer">
            <Button>Download PDF</Button>
          </a>
        </div>
      </div>

      {err ? <div className="text-sm text-red-700">{err}</div> : null}

      <Card className="p-4">
        {!inv ? (
          <div className="text-ink-700">Loading...</div>
        ) : (
          <div className="space-y-3">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <div className="text-xs font-semibold tracking-[0.22em] text-ink-600">CASH MEMO</div>
                <div className="mt-1 text-lg font-extrabold text-ink-950">{inv.invoiceNo}</div>
                <div className="mt-1 text-sm text-ink-700">
                  Date: {inv.createdAt ? new Date(inv.createdAt).toLocaleString() : ''}
                </div>
              </div>
              <div className="rounded-2xl border border-black/10 bg-white/60 px-4 py-3">
                <div className="text-xs text-ink-700">Customer</div>
                <div className="text-sm font-bold text-ink-950">{inv.customerName ?? ''}</div>
                <div className="text-sm text-ink-700">{inv.mobile ?? ''}</div>
              </div>
            </div>

            <div className="overflow-auto rounded-2xl border border-black/10 bg-white/60">
              <table className="w-full text-left text-sm">
                <thead className="bg-black/5 text-xs font-semibold text-ink-700">
                  <tr>
                    <th className="px-3 py-2">Item</th>
                    <th className="px-3 py-2">Model</th>
                    <th className="px-3 py-2">Serial</th>
                    <th className="px-3 py-2 text-right">Qty</th>
                    <th className="px-3 py-2 text-right">Rate</th>
                    <th className="px-3 py-2 text-right">Discount</th>
                    <th className="px-3 py-2 text-right">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((it) => (
                    <tr key={it.id} className="border-t border-black/5">
                      <td className="px-3 py-2">
                        <div className="font-semibold text-ink-950">{it.productName}</div>
                        {it.invoiceNote ? <div className="text-xs text-ink-700">{it.invoiceNote}</div> : null}
                      </td>
                      <td className="px-3 py-2">{it.modelNo ?? ''}</td>
                      <td className="px-3 py-2">{it.serialNo ?? ''}</td>
                      <td className="px-3 py-2 text-right">{Number(it.qty).toFixed(2)}</td>
                      <td className="px-3 py-2 text-right">{Number(it.rate).toFixed(2)}</td>
                      <td className="px-3 py-2 text-right">{Number(it.discountAmount ?? 0).toFixed(2)}</td>
                      <td className="px-3 py-2 text-right">{Number(it.amount).toFixed(2)}</td>
                    </tr>
                  ))}
                  {items.length === 0 ? (
                    <tr>
                      <td className="px-3 py-8 text-ink-700" colSpan={7}>
                        No items.
                      </td>
                    </tr>
                  ) : null}
                </tbody>
              </table>
            </div>

            <div className="grid gap-3 md:grid-cols-2">
              <div className="rounded-2xl border border-black/10 bg-white/60 p-4">
                <div className="text-xs font-semibold text-ink-700">Payment</div>
                <div className="mt-1 text-sm text-ink-700">Mode: {inv.paymentMode ?? ''}</div>
                <div className="text-sm text-ink-700">Notes: {inv.notes ?? ''}</div>
              </div>
              <div className="rounded-2xl border border-black/10 bg-white/60 p-4">
                <div className="flex justify-between text-sm">
                  <div className="text-ink-700">Subtotal</div>
                  <div className="font-semibold text-ink-950">{Number(inv.subTotal).toFixed(2)}</div>
                </div>
                <div className="mt-2 flex justify-between text-sm">
                  <div className="text-ink-700">Discount</div>
                  <div className="font-semibold text-ink-950">{Number(inv.discountAmount ?? 0).toFixed(2)}</div>
                </div>
                <div className="mt-3 flex justify-between border-t border-black/10 pt-3 text-base">
                  <div className="font-extrabold text-ink-950">Total</div>
                  <div className="font-extrabold text-ink-950">{Number(inv.totalAmount).toFixed(2)}</div>
                </div>
                <div className="mt-2 flex justify-between text-sm">
                  <div className="text-ink-700">Paid</div>
                  <div className="font-semibold text-ink-950">{Number(inv.paidAmount ?? 0).toFixed(2)}</div>
                </div>
                <div className="mt-2 flex justify-between text-sm">
                  <div className="text-ink-700">Balance</div>
                  <div className="font-semibold text-ink-950">{Number(inv.balanceAmount ?? 0).toFixed(2)}</div>
                </div>
              </div>
            </div>
          </div>
        )}
      </Card>
    </div>
  )
}

