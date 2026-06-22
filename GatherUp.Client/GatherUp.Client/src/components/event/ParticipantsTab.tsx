import { useState } from 'react'
import { eventsApi } from '../../api/events'
import type { GatherEvent, Participant } from '../../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { Modal } from '../ui/Modal'
import { useAuth } from '../../context/AuthContext'

interface Props { event: GatherEvent; onReload: () => void }

export function ParticipantsTab({ event, onReload }: Props) {
  const { isAdmin } = useAuth()
  const [showAdd, setShowAdd] = useState(false)
  const [addLoading, setAddLoading] = useState(false)
  const [error, setError] = useState('')
  const [form, setForm] = useState({ name: '', email: '', idNumber: '' })

  const participants: Participant[] = event.participants ?? []

  async function handleAdd() {
    setError('')
    setAddLoading(true)
    try {
      await eventsApi.addParticipant(event.id, form)
      setShowAdd(false)
      setForm({ name: '', email: '', idNumber: '' })
      onReload()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'שגיאה')
    } finally {
      setAddLoading(false)
    }
  }

  async function updateRsvp(participantId: string, isAttending: boolean) {
    await eventsApi.updateRsvp(event.id, participantId, isAttending)
    onReload()
  }

  async function sendReminders() {
    await eventsApi.sendPaymentReminders(event.id)
    alert('תזכורות תשלום נשלחו!')
  }

  const attending = participants.filter(p => p.isAttending === true).length
  const notAnswered = participants.filter(p => p.isAttending === null || p.isAttending === undefined).length
  const paid = participants.filter(p => p.hasPaid).length

  return (
    <div>
      <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
        <div className="flex gap-4 text-sm text-gray-600">
          <span className="bg-green-50 text-green-700 px-3 py-1 rounded-full">✓ מגיעים: {attending}</span>
          <span className="bg-yellow-50 text-yellow-700 px-3 py-1 rounded-full">? ממתין: {notAnswered}</span>
          <span className="bg-blue-50 text-blue-700 px-3 py-1 rounded-full">💳 שילמו: {paid}</span>
        </div>
        <div className="flex gap-2">
          {isAdmin && (
            <Button variant="secondary" onClick={sendReminders}>שלח תזכורות תשלום</Button>
          )}
          <Button onClick={() => setShowAdd(true)}>+ הוסף משתתף</Button>
        </div>
      </div>

      {participants.length === 0 ? (
        <Card className="p-10 text-center text-gray-400">אין משתתפים עדיין</Card>
      ) : (
        <Card>
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50 text-gray-500 text-right">
                <th className="px-4 py-3 font-medium">שם</th>
                <th className="px-4 py-3 font-medium">אימייל</th>
                <th className="px-4 py-3 font-medium">הגעה</th>
                <th className="px-4 py-3 font-medium">תשלום</th>
                {isAdmin && <th className="px-4 py-3 font-medium">פעולות</th>}
              </tr>
            </thead>
            <tbody>
              {participants.map(p => (
                <tr key={p.id} className="border-b last:border-0 hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{p.name}</td>
                  <td className="px-4 py-3 text-gray-500">{p.email}</td>
                  <td className="px-4 py-3">
                    {p.isAttending === true && <span className="text-green-600 font-medium">✓ מגיע</span>}
                    {p.isAttending === false && <span className="text-red-500">✗ לא מגיע</span>}
                    {(p.isAttending === null || p.isAttending === undefined) && <span className="text-gray-400">?</span>}
                  </td>
                  <td className="px-4 py-3">
                    {p.hasPaid
                      ? <span className="text-green-600">✓ שילם {p.amountPaid > 0 ? `₪${p.amountPaid}` : ''}</span>
                      : <span className="text-gray-400">טרם שילם</span>
                    }
                  </td>
                  {isAdmin && (
                    <td className="px-4 py-3">
                      <div className="flex gap-2">
                        {p.isAttending !== true && (
                          <button onClick={() => updateRsvp(p.id, true)}
                            className="text-xs text-green-600 hover:underline">מגיע</button>
                        )}
                        {p.isAttending !== false && (
                          <button onClick={() => updateRsvp(p.id, false)}
                            className="text-xs text-red-500 hover:underline">לא מגיע</button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}

      {showAdd && (
        <Modal title="הוספת משתתף" onClose={() => setShowAdd(false)}>
          <div className="space-y-4">
            <Input label="שם *" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required autoFocus />
            <Input label="אימייל *" type="email" value={form.email} onChange={e => setForm(f => ({ ...f, email: e.target.value }))} required />
            <Input label="תעודת זהות" value={form.idNumber} onChange={e => setForm(f => ({ ...f, idNumber: e.target.value }))} />
            {error && <p className="text-sm text-red-500">{error}</p>}
            <div className="flex gap-3 justify-end pt-2">
              <Button variant="secondary" onClick={() => setShowAdd(false)}>ביטול</Button>
              <Button onClick={handleAdd} loading={addLoading}>הוסף</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  )
}
