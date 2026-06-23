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
          setEvents(all)
        } else {
          // משתמש רגיל רואה רק אירועים שהוא משתתף בהם
          // username שווה ל-email כי כך נוצר החשבון (username = email)
          const mine = all.filter(ev =>
            ev.participants?.some(p =>
              p.email?.toLowerCase() === username?.toLowerCase()
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
          <p className="text-gray-500 mb-1">
            {isAdmin ? 'אין אירועים עדיין' : 'אין אירועים שאת/ה משתתף/ת בהם'}
          </p>
          {!isAdmin && (
            <p className="text-xs text-gray-400">
              כשמנהל יוסיף אותך לאירוע, הוא יופיע כאן
            </p>
          )}
          {isAdmin && (
            <Button className="mt-4" onClick={() => navigate('/events/new')}>
              צור אירוע ראשון
            </Button>
          )}
        </Card>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {events.map(event => (
            <Card
              key={event.id}
              className="p-5 cursor-pointer hover:shadow-lg hover:-translate-y-0.5 transition-all"
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
              <div className="mt-4 pt-3 border-t border-gray-100 flex justify-between items-center">
                <span className="text-xs text-indigo-500 font-medium">
                  {isAdmin ? 'לחץ לניהול →' : 'לחץ לפרטים →'}
                </span>
                {!isAdmin && event.participants && (() => {
                  const me = event.participants.find(
                    p => p.email?.toLowerCase() === username?.toLowerCase()
                  )
                  if (!me) return null
                  return (
                    <span className={`text-xs px-2 py-0.5 rounded-full ${
                      me.isAttending === true
                        ? 'bg-green-100 text-green-700'
                        : me.isAttending === false
                          ? 'bg-red-100 text-red-600'
                          : 'bg-yellow-100 text-yellow-700'
                    }`}>
                      {me.isAttending === true ? '✓ אישרתי הגעה' : me.isAttending === false ? '✗ דחיתי' : '? טרם הגבתי'}
                    </span>
                  )
                })()}
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
