import { NavLink, Outlet } from "react-router-dom";
import clsx from "clsx";

const nav = [
    { to: "/", label: "Dashboard", end: true },
    { to: "/invoices/new", label: "New Invoice" },
    { to: "/invoices", label: "Invoices", end: true },
    { to: "/customers", label: "Customers" },
    { to: "/products", label: "Products" },
    { to: "/categories", label: "Categories" },
    { to: "/settings", label: "Settings" },
];

export function Shell() {
    return (
        <div className='min-h-full'>
            <div className='mx-auto flex min-h-screen w-full gap-4 px-4 py-4'>
                <aside className='hidden w-64 shrink-0 lg:block'>
                    <div className='rounded-3xl border border-black/10 bg-white/70 shadow-lift backdrop-blur'>
                        <div className='px-4 py-4'>
                            <div className='text-xs font-semibold tracking-[0.2em] text-ink-600'>
                                BILLBOOK
                            </div>
                            <div className='mt-1 text-lg font-extrabold text-ink-950'>
                                Cash Memo
                            </div>
                        </div>
                        <nav className='px-2 pb-3'>
                            {nav.map((n) => (
                                <NavLink
                                    key={n.to}
                                    to={n.to}
                                    end={n.end}
                                    className={({ isActive }) =>
                                        clsx(
                                            "block rounded-2xl px-3 py-2 text-sm font-semibold",
                                            isActive
                                                ? "bg-ink-950 text-white"
                                                : "text-ink-800 hover:bg-black/5",
                                        )
                                    }>
                                    {n.label}
                                </NavLink>
                            ))}
                        </nav>
                    </div>
                </aside>

                <main className='min-w-0 flex-1'>
                    <div className='mb-4 flex items-center justify-between'>
                        <div className='lg:hidden'>
                            <div className='text-xs font-semibold tracking-[0.2em] text-ink-600'>
                                BILLBOOK
                            </div>
                            <div className='text-lg font-extrabold text-ink-950'>
                                Cash Memo
                            </div>
                        </div>
                        <div className='hidden lg:block text-sm text-ink-700'>
                            R K Electronics billing app
                        </div>
                    </div>

                    <Outlet />
                </main>
            </div>
        </div>
    );
}
