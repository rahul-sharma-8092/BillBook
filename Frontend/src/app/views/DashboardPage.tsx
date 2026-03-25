import { useEffect, useMemo, useState } from 'react'
import { ResponsiveContainer, AreaChart, Area, CartesianGrid, Tooltip, XAxis, YAxis, BarChart, Bar } from 'recharts'
import { api } from '../api/client'
import { Card } from '../ui/Card'

type Summary = { todaySales: number; totalInvoices: number }
type DailyPoint = { date: string; total: number }
type MonthlyPoint = { yearMonth: string; total: number }

export function DashboardPage() {
  const [summary, setSummary] = useState<Summary | null>(null)
  const [daily, setDaily] = useState<DailyPoint[]>([])
  const [monthly, setMonthly] = useState<MonthlyPoint[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    async function load() {
      setLoading(true)
      try {
        const [s, d, m] = await Promise.all([
          api.get('/api/dashboard/summary'),
          api.get('/api/dashboard/daily-sales?days=14'),
          api.get('/api/dashboard/monthly-trend?months=12'),
        ])
        if (cancelled) return
        setSummary(s.data)
        setDaily(d.data)
        setMonthly(m.data)
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => {
      cancelled = true
    }
  }, [])

  const dailyData = useMemo(
    () =>
      daily.map((p) => ({
        ...p,
        label: new Date(p.date).toLocaleDateString(undefined, { month: 'short', day: '2-digit' }),
      })),
    [daily],
  )

  return (
    <div className="space-y-4">
      <div className="grid gap-3 md:grid-cols-2">
        <Card className="p-4">
          <div className="text-xs font-semibold tracking-[0.22em] text-ink-600">
            TODAY SALES
          </div>
          <div className="mt-2 text-3xl font-extrabold text-ink-950">
            {summary ? summary.todaySales.toFixed(2) : loading ? '...' : '0.00'}
          </div>
          <div className="mt-1 text-sm text-ink-700">Total for today</div>
        </Card>

        <Card className="p-4">
          <div className="text-xs font-semibold tracking-[0.22em] text-ink-600">
            TOTAL INVOICES
          </div>
          <div className="mt-2 text-3xl font-extrabold text-ink-950">
            {summary ? summary.totalInvoices : loading ? '...' : 0}
          </div>
          <div className="mt-1 text-sm text-ink-700">All time count</div>
        </Card>
      </div>

      <div className="grid gap-3 lg:grid-cols-2">
        <Card className="p-4">
          <div className="flex items-end justify-between">
            <div>
              <div className="text-sm font-extrabold text-ink-950">Daily Sales</div>
              <div className="text-xs text-ink-700">Last 14 days</div>
            </div>
          </div>
          <div className="mt-3 h-72">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={dailyData}>
                <defs>
                  <linearGradient id="salesFill" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor="#10b981" stopOpacity={0.28} />
                    <stop offset="100%" stopColor="#10b981" stopOpacity={0.02} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(11,12,19,0.08)" />
                <XAxis dataKey="label" tick={{ fontSize: 12 }} />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip />
                <Area type="monotone" dataKey="total" stroke="#059669" strokeWidth={2} fill="url(#salesFill)" />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </Card>

        <Card className="p-4">
          <div className="text-sm font-extrabold text-ink-950">Monthly Trend</div>
          <div className="text-xs text-ink-700">Last 12 months</div>
          <div className="mt-3 h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={monthly}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(11,12,19,0.08)" />
                <XAxis dataKey="yearMonth" tick={{ fontSize: 11 }} />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip />
                <Bar dataKey="total" fill="#2563eb" radius={[10, 10, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>
      </div>
    </div>
  )
}

