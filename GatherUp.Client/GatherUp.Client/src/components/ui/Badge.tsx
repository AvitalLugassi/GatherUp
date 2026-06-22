import { EventStatus } from '../../types'

const colors: Record<EventStatus, string> = {
  [EventStatus.Draft]:            'bg-gray-100 text-gray-600',
  [EventStatus.PollOpen]:         'bg-blue-100 text-blue-700',
  [EventStatus.PollClosed]:       'bg-yellow-100 text-yellow-700',
  [EventStatus.InvitationsSent]:  'bg-purple-100 text-purple-700',
  [EventStatus.Active]:           'bg-green-100 text-green-700',
  [EventStatus.Completed]:        'bg-teal-100 text-teal-700',
  [EventStatus.Cancelled]:        'bg-red-100 text-red-600',
}

const labels: Record<EventStatus, string> = {
  [EventStatus.Draft]:            'טיוטה',
  [EventStatus.PollOpen]:         'סקר פתוח',
  [EventStatus.PollClosed]:       'סקר סגור',
  [EventStatus.InvitationsSent]:  'הזמנות נשלחו',
  [EventStatus.Active]:           'פעיל',
  [EventStatus.Completed]:        'הסתיים',
  [EventStatus.Cancelled]:        'בוטל',
}

export function StatusBadge({ status }: { status: EventStatus }) {
  return (
    <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${colors[status]}`}>
      {labels[status]}
    </span>
  )
}

export function Badge({ children, color = 'gray' }: { children: React.ReactNode; color?: string }) {
  return (
    <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium bg-${color}-100 text-${color}-700`}>
      {children}
    </span>
  )
}
