import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { eventsApi } from '../api/events'
import type { GatherEvent } from '../types'
import { Card } from '../components/ui/Card'
import { Button } from '../components/ui/Button'
import { StatusBadge } from '../components/ui/Badge'
import { useAuth } from '../context/AuthContext'

export function EventsPage() {
  const [events, setEvents] = useState<GatherEvent[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const navigate = useNavigate()
  const { isAdmin, username } = useAuth()

  useEffect(() => {
    eventsApi.getAll()
      .then(all => {
        if (isAdmin) {
          // Admin רואה הכל
          setEvents(all)
        } else {
          // משתמש רגיל רואה רק אירועים שהוא משתתף בהם (לפי שם משתמש / אימייל)
          const mine = all.filter(ev =>
            ev.participants?.some(p =>
              p.email?.toLowerCase() === username?.toLowerCase() ||
              p.name?.toLowerCase() === username?.toLowerCase()
            )
          )
          setEvents(mine)
        }
      })
      .catch(() => setError('שגיאה בטעינת האירועים'))
      .finally(() => setLoading(false))
  }, [isAdmin, username])

  if (loading) return (
    <div className="flex justify-center items-center h-64">
      <div className="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
    </div>
  )

  if (error) return (
    <div className="bg-red-50 border border-red-200 text-red-600 rounded-xl p-4">{error}</div>
  )

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">
          {isAdmin ? 'כל האירועים' : 'האירועים שלי'}
        </h1>
        {isAdmin && (
          <Button onClick={() => navigate('/events/new')}>+ אירוע חדש</Button>
        )}
      </div>

      {events.length === 0 ? (
        <Card className="p-12 text-center">
          <div className="text-5xl mb-4">📅</div>
          <p className="text-gray-500">
            {isAdmin ? 'אין אירועים עדיין' : 'אין אירועים שאת/ה משתתף/ת בהם'}
          </p>
          {isAdmin && (
            <Button className="mt-4" onClick={() => navigate('/events/new')}>צור אירוע ראשון</Button>
          )}
        </Card>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {events.map(event => (
            <Card
              key={event.id}
              className="p-5 cursor-pointer hover:shadow-md transition-shadow"
              onClick={() => navigate(`/events/${event.id}`)}
            >
              <div className="flex items-start justify-between mb-3">
                <h2 className="font-semibold text-gray-800 text-lg leading-tight">{event.title}</h2>
                <StatusBadge status={event.status} />
              </div>
              <div className="space-y-1 text-sm text-gray-500">
                {event.eventDate && (
                  <p>📅 {new Date(event.eventDate).toLocaleDateString('he-IL')}</p>
                )}
                {event.location && <p>📍 {event.location}</p>}
                <p>👥 {event.participants?.length ?? 0} משתתפים</p>
                {event.pricePerParticipant != null && event.pricePerParticipant > 0 && (
                  <p>💰 ₪{event.pricePerParticipant} לאדם</p>
                )}
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
