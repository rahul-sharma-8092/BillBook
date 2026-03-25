import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/client'
import type { InvoiceListResponse } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { useDebouncedValue } from '../hooks/useDebouncedValue'

export function InvoicesPage() {
  const [invoiceNo, setInvoiceNo] = useState('')
  const [customer, setCustomer] = useState('')
  const [mobile, setMobile] = useState('')
  const dinv = useDebouncedValue(invoiceNo, 350)
  const dcus = useDebouncedValue(customer, 350)
  const dmob = useDebouncedValue(mobile, 350)

  const [data, setData] = useState<InvoiceListResponse | null>(null)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [err, setErr] = useState<string | null>(null)

  async function load(p = page) {
    const qs = new URLSearchParams()
    if (dinv.trim()) qs.set('invoiceNo', dinv.trim())
    if (dcus.trim()) qs.set('customer', dcus.trim())
    if (dmob.trim()) qs.set('mobile', dmob.trim())
    qs.set('page', String(p))
    qs.set('pageSize', String(pageSize))
    const res = await api.get<InvoiceListResponse>(`/api/invoices?${qs.toString()}`)
    setData(res.data)
  }

  useEffect(() => {
    setPage(1)
    load(1).catch(() => setErr('Failed to load invoices.'))
  }, [dinv, dcus, dmob])

  const total = data?.totalCount ?? 0
  const pages = Math.max(1, Math.ceil(total / pageSize))

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <div className="text-sm font-extrabold text-ink-950">Invoices</div>
            <div className="text-xs text-ink-700">Search by invoice no / customer / mobile</div>
          </div>
          <Link to="/invoices/new">
            <Button>New Invoice</Button>
          </Link>
        </div>

        <div className="mt-3 grid gap-3 md:grid-cols-3">
          <div>
            <div className="text-xs font-semibold text-ink-700">Invoice No</div>
            <Input value={invoiceNo} onChange={(e) => setInvoiceNo(e.target.value)} placeholder="RK-YYYY-0001" />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Customer</div>
            <Input value={customer} onChange={(e) => setCustomer(e.target.value)} placeholder="Name" />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Mobile</div>
            <Input value={mobile} onChange={(e) => setMobile(e.target.value)} placeholder="Mobile" />
          </div>
        </div>
        {err ? <div className="mt-3 text-sm text-red-700">{err}</div> : null}
      </Card>

      <Card className="overflow-hidden">
        <div className="flex items-center justify-between px-4 py-3">
          <div className="text-sm font-extrabold text-ink-950">
            List ({total})
          </div>
          <div className="flex items-center gap-2">
            <Button
              className="bg-black/10 text-ink-950 hover:bg-black/15"
              disabled={page <= 1}
              onClick={() => {
                const p = Math.max(1, page - 1)
                setPage(p)
                load(p).catch(() => setErr('Failed to load invoices.'))
              }}
            >
              Prev
            </Button>
            <div className="text-sm text-ink-700">
              Page {page} / {pages}
            </div>
            <Button
              className="bg-black/10 text-ink-950 hover:bg-black/15"
              disabled={page >= pages}
              onClick={() => {
                const p = Math.min(pages, page + 1)
                setPage(p)
                load(p).catch(() => setErr('Failed to load invoices.'))
              }}
            >
              Next
            </Button>
          </div>
        </div>
        <div className="overflow-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-black/5 text-xs font-semibold text-ink-700">
              <tr>
                <th className="px-4 py-2">Invoice No</th>
                <th className="px-4 py-2">Customer</th>
                <th className="px-4 py-2">Mobile</th>
                <th className="px-4 py-2 text-right">Total</th>
                <th className="px-4 py-2 text-right">Paid</th>
                <th className="px-4 py-2 text-right">Balance</th>
                <th className="px-4 py-2 w-56">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data?.items?.map((i) => (
                <tr key={i.id} className="border-t border-black/5">
                  <td className="px-4 py-2 font-semibold text-ink-950">{i.invoiceNo}</td>
                  <td className="px-4 py-2">{i.customerName ?? ''}</td>
                  <td className="px-4 py-2">{i.mobile ?? ''}</td>
                  <td className="px-4 py-2 text-right">{Number(i.totalAmount).toFixed(2)}</td>
                  <td className="px-4 py-2 text-right">{Number(i.paidAmount ?? 0).toFixed(2)}</td>
                  <td className="px-4 py-2 text-right">{Number(i.balanceAmount ?? 0).toFixed(2)}</td>
                  <td className="px-4 py-2">
                    <div className="flex gap-2">
                      <Link to={`/invoices/${i.id}`}>
                        <Button className="bg-black/10 text-ink-950 hover:bg-black/15">View</Button>
                      </Link>
                      <a href={`/api/invoices/${i.id}/pdf`} target="_blank" rel="noreferrer">
                        <Button className="bg-black/10 text-ink-950 hover:bg-black/15">PDF</Button>
                      </a>
                    </div>
                  </td>
                </tr>
              ))}
              {data?.items?.length ? null : (
                <tr>
                  <td className="px-4 py-8 text-ink-700" colSpan={7}>
                    No invoices found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  )
}

