import clsx from 'clsx'
import type { PropsWithChildren } from 'react'

export function Card({
  className,
  children,
}: PropsWithChildren<{ className?: string }>) {
  return (
    <div
      className={clsx(
        'rounded-2xl border border-black/10 bg-white/70 shadow-lift backdrop-blur',
        className,
      )}
    >
      {children}
    </div>
  )
}

