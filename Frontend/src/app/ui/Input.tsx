import clsx from 'clsx'
import type { InputHTMLAttributes } from 'react'

export function Input({ className, ...props }: InputHTMLAttributes<HTMLInputElement>) {
  return (
    <input
      className={clsx(
        'w-full rounded-xl border border-black/10 bg-white/70 px-3 py-2 text-sm outline-none',
        'focus:border-black/20 focus:ring-4 focus:ring-black/5',
        className,
      )}
      {...props}
    />
  )
}

