import { useEffect, useMemo, useState } from 'react'
import { api } from '../api/client'
import type { Category, Product } from '../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { Select } from '../ui/Select'
import { useDebouncedValue } from '../hooks/useDebouncedValue'

type ProductDraft = Omit<Product, 'id' | 'createdAt' | 'categoryName'> & { id?: number }

const emptyDraft: ProductDraft = {
  categoryId: null,
  productType: 'Product',
  name: '',
  modelNo: '',
  serialNo: '',
  warrantyMonths: null,
  warrantyNote: '',
  invoiceNote: '',
  purchasePrice: null,
  sellPrice: 0,
  stockQty: 0,
  discountType: null,
  discountValue: null,
  isActive: true,
}

export function ProductsPage() {
  const [categories, setCategories] = useState<Category[]>([])
  const [items, setItems] = useState<Product[]>([])
  const [q, setQ] = useState('')
  const dq = useDebouncedValue(q, 350)
  const [onlyActive, setOnlyActive] = useState(false)
  const [draft, setDraft] = useState<ProductDraft>(emptyDraft)
  const [busy, setBusy] = useState(false)
  const [err, setErr] = useState<string | null>(null)

  async function loadCategories() {
    const res = await api.get<Category[]>('/api/categories?onlyActive=true')
    setCategories(res.data)
  }

  async function load() {
    const qs = new URLSearchParams()
    if (dq.trim()) qs.set('q', dq.trim())
    if (onlyActive) qs.set('onlyActive', 'true')
    const res = await api.get<Product[]>(`/api/products?${qs.toString()}`)
    setItems(res.data)
  }

  useEffect(() => {
    loadCategories().catch(() => setErr('Failed to load categories.'))
  }, [])

  useEffect(() => {
    load().catch(() => setErr('Failed to load products.'))
  }, [dq, onlyActive])

  const title = useMemo(() => (draft.id ? `Edit Product #${draft.id}` : 'Add Product'), [draft.id])

  function reset() {
    setDraft(emptyDraft)
  }

  async function save() {
    setErr(null)
    if (!draft.name?.trim()) {
      setErr('Name is required.')
      return
    }
    setBusy(true)
    try {
      const payload = {
        categoryId: draft.categoryId ?? null,
        productType: draft.productType,
        name: draft.name.trim(),
        modelNo: draft.modelNo || null,
        serialNo: draft.serialNo || null,
        warrantyMonths: draft.warrantyMonths ?? null,
        warrantyNote: draft.warrantyNote || null,
        invoiceNote: draft.invoiceNote || null,
        purchasePrice: draft.purchasePrice ?? null,
        sellPrice: Number(draft.sellPrice ?? 0),
        stockQty: Number(draft.stockQty ?? 0),
        discountType: draft.discountType || null,
        discountValue: draft.discountValue ?? null,
        isActive: !!draft.isActive,
      }

      if (draft.id) await api.put(`/api/products/${draft.id}`, payload)
      else await api.post('/api/products', payload)

      reset()
      await load()
    } catch {
      setErr('Save failed.')
    } finally {
      setBusy(false)
    }
  }

  async function del(row: Product) {
    if (!window.confirm(`Delete "${row.name}"?`)) return
    try {
      await api.delete(`/api/products/${row.id}`)
      await load()
    } catch {
      setErr('Delete failed.')
    }
  }

  return (
    <div className="space-y-4">
      <Card className="p-4">
        <div className="flex items-end justify-between gap-3">
          <div>
            <div className="text-sm font-extrabold text-ink-950">{title}</div>
            <div className="text-xs text-ink-700">Product / Service master</div>
          </div>
          {draft.id ? (
            <Button className="bg-black/10 text-ink-950 hover:bg-black/15" onClick={reset}>
              Cancel Edit
            </Button>
          ) : null}
        </div>

        <div className="mt-3 grid gap-3 md:grid-cols-3">
          <div>
            <div className="text-xs font-semibold text-ink-700">Type</div>
            <Select value={draft.productType} onChange={(e) => setDraft((d) => ({ ...d, productType: e.target.value }))}>
              <option value="Product">Product</option>
              <option value="Service">Service</option>
            </Select>
          </div>
          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Name</div>
            <Input value={draft.name ?? ''} onChange={(e) => setDraft((d) => ({ ...d, name: e.target.value }))} />
          </div>

          <div>
            <div className="text-xs font-semibold text-ink-700">Category</div>
            <Select
              value={draft.categoryId ?? ''}
              onChange={(e) => setDraft((d) => ({ ...d, categoryId: e.target.value ? Number(e.target.value) : null }))}
            >
              <option value="">(None)</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </Select>
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Sell Price</div>
            <Input type="number" value={draft.sellPrice ?? 0} onChange={(e) => setDraft((d) => ({ ...d, sellPrice: Number(e.target.value) }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Stock Qty</div>
            <Input type="number" value={draft.stockQty ?? 0} onChange={(e) => setDraft((d) => ({ ...d, stockQty: Number(e.target.value) }))} />
          </div>

          <div>
            <div className="text-xs font-semibold text-ink-700">Model No</div>
            <Input value={draft.modelNo ?? ''} onChange={(e) => setDraft((d) => ({ ...d, modelNo: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Serial No</div>
            <Input value={draft.serialNo ?? ''} onChange={(e) => setDraft((d) => ({ ...d, serialNo: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Warranty (Months)</div>
            <Input
              type="number"
              value={draft.warrantyMonths ?? ''}
              onChange={(e) => setDraft((d) => ({ ...d, warrantyMonths: e.target.value ? Number(e.target.value) : null }))}
            />
          </div>

          <div className="md:col-span-2">
            <div className="text-xs font-semibold text-ink-700">Warranty Note</div>
            <Input value={draft.warrantyNote ?? ''} onChange={(e) => setDraft((d) => ({ ...d, warrantyNote: e.target.value }))} />
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Active</div>
            <Select value={draft.isActive ? '1' : '0'} onChange={(e) => setDraft((d) => ({ ...d, isActive: e.target.value === '1' }))}>
              <option value="1">Yes</option>
              <option value="0">No</option>
            </Select>
          </div>

          <div>
            <div className="text-xs font-semibold text-ink-700">Discount Type</div>
            <Select value={draft.discountType ?? ''} onChange={(e) => setDraft((d) => ({ ...d, discountType: e.target.value || null }))}>
              <option value="">(None)</option>
              <option value="RS">RS</option>
              <option value="%">%</option>
            </Select>
          </div>
          <div>
            <div className="text-xs font-semibold text-ink-700">Discount Value</div>
            <Input
              type="number"
              value={draft.discountValue ?? ''}
              onChange={(e) => setDraft((d) => ({ ...d, discountValue: e.target.value ? Number(e.target.value) : null }))}
            />
          </div>
          <div className="md:col-span-3">
            <div className="text-xs font-semibold text-ink-700">Invoice Note (default)</div>
            <Input value={draft.invoiceNote ?? ''} onChange={(e) => setDraft((d) => ({ ...d, invoiceNote: e.target.value }))} />
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
          <div>
            <div className="text-sm font-extrabold text-ink-950">Products</div>
            <div className="text-xs text-ink-700">Search by name / model / serial</div>
          </div>
          <div className="flex gap-2">
            <div className="w-64">
              <Input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search..." />
            </div>
            <Select value={onlyActive ? '1' : '0'} onChange={(e) => setOnlyActive(e.target.value === '1')}>
              <option value="0">All</option>
              <option value="1">Active</option>
            </Select>
          </div>
        </div>

        <div className="mt-3 overflow-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-black/5 text-xs font-semibold text-ink-700">
              <tr>
                <th className="px-3 py-2">Name</th>
                <th className="px-3 py-2">Type</th>
                <th className="px-3 py-2">Category</th>
                <th className="px-3 py-2 text-right">Price</th>
                <th className="px-3 py-2 text-right">Stock</th>
                <th className="px-3 py-2 w-52">Actions</th>
              </tr>
            </thead>
            <tbody>
              {items.map((p) => (
                <tr key={p.id} className="border-t border-black/5">
                  <td className="px-3 py-2">
                    <div className="font-semibold text-ink-950">{p.name}</div>
                    <div className="text-xs text-ink-700">{[p.modelNo, p.serialNo].filter(Boolean).join(' | ')}</div>
                  </td>
                  <td className="px-3 py-2">{p.productType}</td>
                  <td className="px-3 py-2">{p.categoryName ?? ''}</td>
                  <td className="px-3 py-2 text-right">{Number(p.sellPrice).toFixed(2)}</td>
                  <td className="px-3 py-2 text-right">{Number(p.stockQty ?? 0).toFixed(2)}</td>
                  <td className="px-3 py-2">
                    <div className="flex gap-2">
                      <Button
                        className="bg-black/10 text-ink-950 hover:bg-black/15"
                        onClick={() =>
                          setDraft({
                            id: p.id,
                            categoryId: p.categoryId ?? null,
                            productType: p.productType,
                            name: p.name,
                            modelNo: p.modelNo ?? '',
                            serialNo: p.serialNo ?? '',
                            warrantyMonths: p.warrantyMonths ?? null,
                            warrantyNote: p.warrantyNote ?? '',
                            invoiceNote: p.invoiceNote ?? '',
                            purchasePrice: p.purchasePrice ?? null,
                            sellPrice: p.sellPrice,
                            stockQty: p.stockQty ?? 0,
                            discountType: p.discountType ?? null,
                            discountValue: p.discountValue ?? null,
                            isActive: p.isActive ?? true,
                          })
                        }
                      >
                        Edit
                      </Button>
                      <Button className="bg-red-600 hover:bg-red-700" onClick={() => del(p)}>
                        Delete
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
              {items.length === 0 ? (
                <tr>
                  <td className="px-3 py-8 text-ink-700" colSpan={6}>
                    No products found.
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

