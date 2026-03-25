export type Category = {
  id: number
  name: string
  isActive?: boolean
  createdAt?: string
}

export type Product = {
  id: number
  categoryId?: number | null
  categoryName?: string | null
  productType: string
  name: string
  modelNo?: string | null
  serialNo?: string | null
  warrantyMonths?: number | null
  warrantyNote?: string | null
  invoiceNote?: string | null
  purchasePrice?: number | null
  sellPrice: number
  stockQty?: number | null
  discountType?: string | null
  discountValue?: number | null
  isActive?: boolean
  createdAt?: string
}

export type Customer = {
  id: number
  name: string
  companyName?: string | null
  mobile?: string | null
  email?: string | null
  addressLine1?: string | null
  addressLine2?: string | null
  city?: string | null
  district?: string | null
  state?: string | null
  country?: string | null
  notes?: string | null
  createdAt?: string
}

export type Invoice = {
  id: number
  invoiceNo: string
  customerId?: number | null
  customerName?: string | null
  mobile?: string | null
  subTotal: number
  discountType?: string | null
  discountValue?: number | null
  discountAmount?: number | null
  totalAmount: number
  paidAmount?: number | null
  balanceAmount?: number | null
  paymentMode?: string | null
  notes?: string | null
  createdAt?: string
}

export type InvoiceItem = {
  id: number
  invoiceId: number
  productId?: number | null
  productName: string
  productType?: string | null
  modelNo?: string | null
  serialNo?: string | null
  warrantyMonths?: number | null
  warrantyNote?: string | null
  qty: number
  rate: number
  discountType?: string | null
  discountValue?: number | null
  discountAmount?: number | null
  amount: number
  invoiceNote?: string | null
}

export type InvoiceDetails = {
  invoice: Invoice
  items: InvoiceItem[]
}

export type InvoiceListResponse = {
  totalCount: number
  items: Invoice[]
}

export type Setting = {
  id: number
  shopName?: string | null
  address?: string | null
  phone?: string | null
  email?: string | null
  bankName?: string | null
  accountName?: string | null
  accountNumber?: string | null
  ifsc?: string | null
  upi?: string | null
  terms?: string | null
  footerMessage?: string | null
}

