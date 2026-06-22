import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { eventsApi } from '../api/events'
import type { GatherEvent } from '../types'
import { EventStatus, EventStatusLabels } from '../types'
import { StatusBadge } from '../components/ui/Badge'
import { Button } from '../components/ui/Button'
import { useAuth } from '../context/AuthContext'
import { ParticipantsTab } from '../components/event/ParticipantsTab'
import { PollsTab } from '../components/event/PollsTab'
import { FinanceTab } from '../components/event/FinanceTab'
import { SettingsTab } from '../components/event/SettingsTab'

type Tab = 'participants' | 'polls' | 'finance' | 'settings'

const tabs: { id: Tab; label: string; icon: string }[] = [
  { id: 'participants', label: 'משתתפים', icon: '👥' },
  { id: 'polls',        label: 'סקרים',   icon: '📊' },
  { id: 'finance',      label: 'פיננסים', icon: '💰' },
  { id: 'settings',     label: 'הגדרות',  icon: '⚙️' },
]

export function EventDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { isAdmin } = useAuth()
  const [event, setEvent] = useState<GatherEvent | null>(null)
  const [loading, setLoading] = useState(true)
  const [activeTab, setActiveTab] = useState<Tab>('participants')
  const [statusLoading, setStatusLoading] = useState(false)

  function reload() {
    if (!id) return
    eventsApi.getById(id).then(setEvent).catch(() => navigate('/events'))
  }

  useEffect(() => {
    if (!id) return
    eventsApi.getById(id)
      .then(setEvent)
      .catch(() => navigate('/events'))
      .finally(() => setLoading(false))
  }, [id])

  async function changeStatus(status: EventStatus) {
    if (!event) return
    setStatusLoading(true)
    try {
      await eventsApi.updateStatus(event.id, status)
      setEvent(prev => prev ? { ...prev, status } : prev)
    } finally {
      setStatusLoading(false)
    }
  }

  async function sendInvitations() {
    if (!event) return
    await eventsApi.sendInvitations(event.id)
    alert('הזמנות נשלחו!')
  }

  if (loading) return (
    <div className="flex justify-center items-center h-64">
      <div className="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
    </div>
  )

  if (!event) return null

  return (
    <div dir="rtl">
      {/* Header */}
      <div className="mb-6">
        <button onClick={() => navigate('/events')} className="text-sm text-gray-400 hover:text-indigo-600 mb-2 flex items-center gap-1">
          ← חזרה לאירועים
        </button>
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <div className="flex items-center gap-3 mb-1">
              <h1 className="text-2xl font-bold text-gray-800">{event.title}</h1>
              <StatusBadge status={event.status} />
            </div>
            <div className="flex flex-wrap gap-4 text-sm text-gray-500">
              {event.eventDate && <span>📅 {new Date(event.eventDate).toLocaleDateString('he-IL')}</span>}
              {event.location && <span>📍 {event.location}</span>}
              <span>👥 {event.participants?.length ?? 0} משתתפים</span>
            </div>
          </div>

          {isAdmin && (
            <div className="flex flex-wrap gap-2">
              {event.status === EventStatus.Draft && (
                <Button variant="secondary" onClick={() => changeStatus(EventStatus.PollOpen)} loading={statusLoading}>
                  פתח סקר
                </Button>
              )}
              {event.status === EventStatus.PollOpen && (
                <Button variant="secondary" onClick={() => changeStatus(EventStatus.PollClosed)} loading={statusLoading}>
                  סגור סקר
                </Button>
              )}
              {(event.status === EventStatus.PollClosed || event.status === EventStatus.Draft) && (
                <Button onClick={sendInvitations}>שלח הזמנות</Button>
              )}
              {event.status === EventStatus.InvitationsSent && (
                <Button onClick={() => changeStatus(EventStatus.Active)} loading={statusLoading}>
                  הפעל אירוע
                </Button>
              )}
              {event.status === EventStatus.Active && (
                <Button variant="secondary" onClick={() => changeStatus(EventStatus.Completed)} loading={statusLoading}>
                  סיים אירוע
                </Button>
              )}
              {event.status !== EventStatus.Cancelled && event.status !== EventStatus.Completed && (
                <Button variant="danger" onClick={() => changeStatus(EventStatus.Cancelled)} loading={statusLoading}>
                  בטל
                </Button>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-1 bg-gray-100 rounded-xl p-1 mb-6 w-fit">
        {tabs.map(tab => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`flex items-center gap-1.5 px-4 py-2 rounded-lg text-sm font-medium transition-all
              ${activeTab === tab.id
                ? 'bg-white text-indigo-600 shadow-sm'
                : 'text-gray-500 hover:text-gray-700'}`}
          >
            <span>{tab.icon}</span>
            {tab.label}
          </button>
        ))}
      </div>

      {/* Tab content */}
      {activeTab === 'participants' && <ParticipantsTab event={event} onReload={reload} />}
      {activeTab === 'polls'        && <PollsTab event={event} onReload={reload} />}
      {activeTab === 'finance'      && <FinanceTab event={event} onReload={reload} />}
      {activeTab === 'settings'     && <SettingsTab event={event} onReload={reload} />}
    </div>
  )
}
