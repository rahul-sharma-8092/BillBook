import clsx from 'clsx'
import type { ButtonHTMLAttributes } from 'react'

export function Button({
  className,
  ...props
}: ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <button
      className={clsx(
        'inline-flex items-center justify-center rounded-xl px-3 py-2 text-sm font-semibold',
        'bg-ink-950 text-white shadow-[0_10px_22px_rgba(11,12,19,0.20)]',
        'hover:bg-ink-900 active:bg-ink-950 disabled:opacity-50 disabled:cursor-not-allowed',
        className,
      )}
      {...props}
    />
  )
}

