import { createBrowserRouter } from 'react-router-dom'
import { Shell } from './layout/Shell'
import { DashboardPage } from './views/DashboardPage'
import { CategoriesPage } from './views/CategoriesPage'
import { ProductsPage } from './views/ProductsPage'
import { CustomersPage } from './views/CustomersPage'
import { InvoicesPage } from './views/InvoicesPage'
import { InvoiceNewPage } from './views/InvoiceNewPage'
import { InvoiceViewPage } from './views/InvoiceViewPage'
import { SettingsPage } from './views/SettingsPage'

export const router = createBrowserRouter([
  {
    element: <Shell />,
    children: [
      { path: '/', element: <DashboardPage /> },
      { path: '/categories', element: <CategoriesPage /> },
      { path: '/products', element: <ProductsPage /> },
      { path: '/customers', element: <CustomersPage /> },
      { path: '/invoices', element: <InvoicesPage /> },
      { path: '/invoices/new', element: <InvoiceNewPage /> },
      { path: '/invoices/:id', element: <InvoiceViewPage /> },
      { path: '/settings', element: <SettingsPage /> },
    ],
  },
])

