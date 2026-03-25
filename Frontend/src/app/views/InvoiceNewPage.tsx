import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../api/client'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import type { Customer, Product } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { Select } from '../ui/Select'

type DiscountType = '' | 'RS' | '%'

type ItemDraft = {
  productId?: number | null
  productName: string
  productType?: string | null
  modelNo?: string | null
  serialNo?: string | null
  warrantyMonths?: number | null
  warrantyNote?: string | null
  qty: number
  rate: number
  discountType: DiscountType
  discountValue: number
  invoiceNote?: string | null
}

function discountAmount(base: number, type: DiscountType, value: number) {
  if (!base || base <= 0) return 0
  if (!type || !value || value <= 0) return 0
  if (type === 'RS') return Math.min(base, round2(value))
  if (type === '%') return Math.min(base, round2((base * value) / 100))
  return 0
}

function round2(n: number) {
  return Math.round(n * 100) / 100
}

function newItem(): ItemDraft {
  return {
    productId: null,
    productName: '',
    productType: 'Product',
    modelNo: '',
    serialNo: '',
    warrantyMonths: null,
    warrantyNote: '',
    qty: 1,
    rate: 0,
    discountType: '',
    discountValue: 0,
    invoiceNote: '',
  }
}

export function InvoiceNewPage() {
  const nav = useNavigate()
  const [products, setProducts] = useState<Product[]>([])
  const [prodQ, setProdQ] = useState('')
  const dProdQ = useDebouncedValue(prodQ, 250)

  const [custQ, setCustQ] = useState('')
  const dCustQ = useDebouncedValue(custQ, 300)
  const [custResults, setCustResults] = useState<Customer[]>([])
  const [customerId, setCustomerId] = useState<number | null>(null)
  const [customerName, setCustomerName] = useState('')
  const [mobile, setMobile] = useState('')

  const [items, setItems] = useState<ItemDraft[]>([newItem()])
  const [overallDiscountType, setOverallDiscountType] = useState<DiscountType>('')
  const [overallDiscountValue, setOverallDiscountValue] = useState(0)
  const [paymentMode, setPaymentMode] = useState<'Cash' | 'UPI'>('Cash')
  const [paidAmount, setPaidAmount] = useState(0)
  const [notes, setNotes] = useState('')

  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState<string | null>(null)

  useEffect(() => {
    api
      .get<Product[]>('/api/products?onlyActive=true')
      .then((res) => setProducts(res.data))
      .catch(() => setErr('Failed to load products.'))
  }, [])

  useEffect(() => {
    let cancelled = false
    async function search() {
      if (!dCustQ.trim()) {
        setCustResults([])
        return
      }
      const res = await api.get<Customer[]>(`/api/customers?q=${encodeURIComponent(dCustQ.trim())}`)
      if (!cancelled) setCustResults(res.data)
    }
    search().catch(() => {})
    return () => {
      cancelled = true
    }
  }, [dCustQ])

  const filteredProducts = useMemo(() => {
    const q = dProdQ.trim().toLowerCase()
    if (!q) return products.slice(0, 50)
    return products
      .filter((p) => (p.name ?? '').toLowerCase().includes(q) || (p.modelNo ?? '').toLowerCase().includes(q))
      .slice(0, 50)
  }, [products, dProdQ])

  const computed = useMemo(() => {
    const lines = items.map((it) => {
      const base = round2((it.qty || 0) * (it.rate || 0))
      const disc = discountAmount(base, it.discountType, it.discountValue || 0)
      const amount = round2(base - disc)
      return { base, disc, amount }
    })
    const subTotal = round2(lines.reduce((a, b) => a + b.amount, 0))
    const overallDisc = discountAmount(subTotal, overallDiscountType, overallDiscountValue || 0)
    const total = round2(subTotal - overallDisc)
    const balance = round2(total - (paidAmount || 0))
    return { lines, subTotal, overallDisc, total, balance }
  }, [items, overallDiscountType, overallDiscountValue, paidAmount])

  function selectCustomer(c: Customer) {
    setCustomerId(c.id)
    setCustomerName(c.name)
    setMobile(c.mobile ?? '')
    setCustQ(`${c.name}${c.mobile ? ` (${c.mobile})` : ''}`)
    setCustResults([])
  }

  function applyProduct(idx: number, p: Product) {
    setItems((prev) =>
      prev.map((it, i) => {
        if (i !== idx) return it
        return {
          ...it,
          productId: p.id,
          productName: p.name,
          productType: p.productType,
          modelNo: p.modelNo ?? '',
          serialNo: p.serialNo ?? '',
          warrantyMonths: p.warrantyMonths ?? null,
          warrantyNote: p.warrantyNote ?? '',
          rate: Number(p.sellPrice ?? 0),
          discountType: (p.discountType as DiscountType) ?? '',
          discountValue: Number(p.discountValue ?? 0),
          invoiceNote: p.invoiceNote ?? '',
        }
      }),
    )
  }

  async function submit() {
    setErr(null)
    const cleanedItems = items
      .map((it) => ({ ...it, productName: it.productName.trim() }))
      .filter((it) => it.productName)
    if (!cleanedItems.length) {
      setErr('At least one item is required.')
      return
    }
    if (!customerName.trim() && !customerId) {
      setErr('Customer name is required (or select a customer).')
      return
    }
    setBusy(true)
    try {
      const payload = {
        customerId,
        customerName: customerName.trim() || null,
        mobile: mobile.trim() || null,
        discountType: overallDiscountType || null,
        discountValue: overallDiscountType ? Number(overallDiscountValue || 0) : null,
        paidAmount: Number(paidAmount || 0),
        paymentMode,
        notes: notes.trim() || null,
        items: cleanedItems.map((it) => ({
          productId: it.productId ?? null,
          productName: it.productName,
          productType: it.productType ?? null,
          modelNo: it.modelNo || null,
          serialNo: it.serialNo || null,
          warrantyMonths: it.warrantyMonths ?? null,
          warrantyNote: it.warrantyNote || null,
          qty: Number(it.qty || 0),
          rate: Number(it.rate || 0),
          discountType: it.discountType || null,
          discountValue: it.discountType ? Number(it.discountValue || 0) : null,
          invoiceNote: it.invoiceNote || null,
        })),
      }

      const res = await api.post<{ invoiceId: number; invoiceNo: string }>('/api/invoices', payload)
      nav(`/invoices/${res.data.invoiceId}`)
    } catch {
      setErr('Create invoice failed.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="text-sm font-extrabold text-ink-950">Create Invoice</div>
        <div className="text-xs text-ink-700">Cash memo billing</div>

        <div className="mt-4 grid gap-3 md:grid-cols-3">
          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Customer Search</div>
            <Input value={custQ} onChange={(e) => setCustQ(e.target.value)} placeholder="Search by name / mobile..." />
            {custResults.length ? (
              <div className="mt-2 overflow-hidden rounded-2xl border border-black/10 bg-white/80">
                {custResults.slice(0, 8).map((c) => (
                  <button
                    key={c.id}
                    type="button"
                    className="block w-full px-3 py-2 text-left text-sm hover:bg-black/5"
                    onClick={() => selectCustomer(c)}
                  >
                    <div className="font-semibold text-ink-950">{c.name}</div>
                    <div className="text-xs text-ink-700">{c.mobile ?? ''}</div>
                  </button>
                ))}
              </div>
            ) : null}
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Payment Mode</div>
            <Select value={paymentMode} onChange={(e) => setPaymentMode(e.target.value as 'Cash' | 'UPI')}>
              <option value="Cash">Cash</option>
              <option value="UPI">UPI</option>
            </Select>
          </div>

          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Customer Name</div>
            <Input value={customerName} onChange={(e) => setCustomerName(e.target.value)} placeholder="Customer name" />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Mobile</div>
            <Input value={mobile} onChange={(e) => setMobile(e.target.value)} placeholder="Mobile" />
          </div>
        </div>
      </Card>

      <Card className="p-4">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <div className="text-sm font-extrabold text-ink-950">Items</div>
            <div className="text-xs text-ink-700">Select product or enter manually</div>
          </div>
          <div className="w-64">
            <Input value={prodQ} onChange={(e) => setProdQ(e.target.value)} placeholder="Filter products..." />
          </div>
        </div>

        <div className="mt-3 overflow-auto rounded-2xl border border-black/10 bg-white/60">
          <table className="w-full text-left text-sm">
            <thead className="bg-black/5 text-xs font-semibold text-ink-700">
              <tr>
                <th className="px-3 py-2 w-72">Item</th>
                <th className="px-3 py-2 w-40">Model</th>
                <th className="px-3 py-2 w-40">Serial</th>
                <th className="px-3 py-2 w-24 text-right">Qty</th>
                <th className="px-3 py-2 w-28 text-right">Rate</th>
                <th className="px-3 py-2 w-40 text-right">Discount</th>
                <th className="px-3 py-2 w-28 text-right">Amount</th>
                <th className="px-3 py-2 w-20"></th>
              </tr>
            </thead>
            <tbody>
              {items.map((it, idx) => (
                <tr key={idx} className="border-t border-black/5 align-top">
                  <td className="px-3 py-2">
                    <Select
                      value={it.productId ?? ''}
                      onChange={(e) => {
                        const id = e.target.value ? Number(e.target.value) : null
                        const p = id ? products.find((x) => x.id === id) : null
                        if (p) applyProduct(idx, p)
                        else {
                          setItems((prev) =>
                            prev.map((x, i) => (i === idx ? { ...x, productId: null } : x)),
                          )
                        }
                      }}
                    >
                      <option value="">(Manual)</option>
                      {filteredProducts.map((p) => (
                        <option key={p.id} value={p.id}>
                          {p.name}
                        </option>
                      ))}
                    </Select>
                    <div className="mt-2">
                      <Input
                        value={it.productName}
                        onChange={(e) =>
                          setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, productName: e.target.value } : x)))
                        }
                        placeholder="Item name"
                      />
                    </div>
                    <div className="mt-2">
                      <Input
                        value={it.invoiceNote ?? ''}
                        onChange={(e) =>
                          setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, invoiceNote: e.target.value } : x)))
                        }
                        placeholder="Invoice note (optional)"
                      />
                    </div>
                  </td>
                  <td className="px-3 py-2">
                    <Input
                      value={it.modelNo ?? ''}
                      onChange={(e) =>
                        setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, modelNo: e.target.value } : x)))
                      }
                    />
                  </td>
                  <td className="px-3 py-2">
                    <Input
                      value={it.serialNo ?? ''}
                      onChange={(e) =>
                        setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, serialNo: e.target.value } : x)))
                      }
                    />
                  </td>
                  <td className="px-3 py-2">
                    <Input
                      type="number"
                      className="text-right"
                      value={it.qty}
                      onChange={(e) =>
                        setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, qty: Number(e.target.value) } : x)))
                      }
                    />
                  </td>
                  <td className="px-3 py-2">
                    <Input
                      type="number"
                      className="text-right"
                      value={it.rate}
                      onChange={(e) =>
                        setItems((prev) => prev.map((x, i) => (i === idx ? { ...x, rate: Number(e.target.value) } : x)))
                      }
                    />
                  </td>
                  <td className="px-3 py-2">
                    <div className="grid grid-cols-2 gap-2">
                      <Select
                        value={it.discountType}
                        onChange={(e) =>
                          setItems((prev) =>
                            prev.map((x, i) =>
                              i === idx ? { ...x, discountType: e.target.value as DiscountType } : x,
                            ),
                          )
                        }
                      >
                        <option value="">None</option>
                        <option value="RS">RS</option>
                        <option value="%">%</option>
                      </Select>
                      <Input
                        type="number"
                        className="text-right"
                        value={it.discountValue}
                        onChange={(e) =>
                          setItems((prev) =>
                            prev.map((x, i) => (i === idx ? { ...x, discountValue: Number(e.target.value) } : x)),
                          )
                        }
                      />
                    </div>
                  </td>
                  <td className="px-3 py-2 text-right font-semibold text-ink-950">
                    {computed.lines[idx]?.amount.toFixed(2)}
                  </td>
                  <td className="px-3 py-2">
                    <Button
                      className="bg-red-600 hover:bg-red-700"
                      onClick={() => setItems((prev) => prev.filter((_, i) => i !== idx))}
                      disabled={items.length <= 1}
                    >
                      Remove
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-3 flex gap-2">
          <Button className="bg-black/10 text-ink-950 hover:bg-black/15" onClick={() => setItems((p) => [...p, newItem()])}>
            Add Item
          </Button>
        </div>
      </Card>

      <Card className="p-4">
        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <div className="text-sm font-extrabold text-ink-950">Notes</div>
            <div className="mt-2">
              <Input value={notes} onChange={(e) => setNotes(e.target.value)} placeholder="Invoice notes (optional)" />
            </div>
          </div>
          <div className="rounded-2xl border border-black/10 bg-white/60 p-4">
            <div className="text-sm font-extrabold text-ink-950">Summary</div>
            <div className="mt-3 flex items-center justify-between text-sm">
              <div className="text-ink-700">Subtotal</div>
              <div className="font-semibold text-ink-950">{computed.subTotal.toFixed(2)}</div>
            </div>
            <div className="mt-2 grid grid-cols-3 gap-2">
              <div className="col-span-1">
                <div className="text-xs font-semibold text-ink-700">Discount Type</div>
                <Select value={overallDiscountType} onChange={(e) => setOverallDiscountType(e.target.value as DiscountType)}>
                  <option value="">None</option>
                  <option value="RS">RS</option>
                  <option value="%">%</option>
                </Select>
              </div>
              <div className="col-span-2">
                <div className="text-xs font-semibold text-ink-700">Discount Value</div>
                <Input
                  type="number"
                  value={overallDiscountValue}
                  onChange={(e) => setOverallDiscountValue(Number(e.target.value))}
                  className="text-right"
                />
              </div>
            </div>
            <div className="mt-2 flex items-center justify-between text-sm">
              <div className="text-ink-700">Discount Amount</div>
              <div className="font-semibold text-ink-950">{computed.overallDisc.toFixed(2)}</div>
            </div>
            <div className="mt-3 flex items-center justify-between border-t border-black/10 pt-3 text-base">
              <div className="font-extrabold text-ink-950">Total</div>
              <div className="font-extrabold text-ink-950">{computed.total.toFixed(2)}</div>
            </div>
            <div className="mt-3">
              <div className="text-xs font-semibold text-ink-700">Paid Amount</div>
              <Input
                type="number"
                className="text-right"
                value={paidAmount}
                onChange={(e) => setPaidAmount(Number(e.target.value))}
              />
            </div>
            <div className="mt-2 flex items-center justify-between text-sm">
              <div className="text-ink-700">Balance</div>
              <div className="font-semibold text-ink-950">{computed.balance.toFixed(2)}</div>
            </div>

            <div className="mt-4 flex gap-2">
              <Button disabled={busy} onClick={submit}>
                Create
              </Button>
              <Button className="bg-black/10 text-ink-950 hover:bg-black/15" disabled={busy} onClick={() => nav('/invoices')}>
                Cancel
              </Button>
            </div>
            {err ? <div className="mt-3 text-sm text-red-700">{err}</div> : null}
          </div>
        </div>
      </Card>
    </div>
  )
}

