import { useEffect, useState } from 'react'
import { usersApi, type AppUserDto, type CreateUserResponse } from '../api/users'
import { Card } from '../components/ui/Card'
import { Button } from '../components/ui/Button'
import { Input } from '../components/ui/Input'
import { Modal } from '../components/ui/Modal'

export function UsersPage() {
  const [users, setUsers] = useState<AppUserDto[]>([])
  const [loading, setLoading] = useState(true)
  const [showCreate, setShowCreate] = useState(false)
  const [createLoading, setCreateLoading] = useState(false)
  const [newUser, setNewUser] = useState<CreateUserResponse | null>(null)
  const [error, setError] = useState('')
  const [form, setForm] = useState({ username: '', role: 'User', email: '' })

  function load() {
    usersApi.getAll()
      .then(setUsers)
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  async function handleCreate() {
    setError('')
    setCreateLoading(true)
    try {
      const res = await usersApi.create(form)
      setNewUser(res)
      setShowCreate(false)
      setForm({ username: '', role: 'User', email: '' })
      load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'שגיאה')
    } finally {
      setCreateLoading(false)
    }
  }

  async function handleDelete(id: string, name: string) {
    if (!confirm(`למחוק את ${name}?`)) return
    await usersApi.delete(id)
    load()
  }

  if (loading) return (
    <div className="flex justify-center items-center h-64">
      <div className="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
    </div>
  )

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">ניהול משתמשים</h1>
        <Button onClick={() => setShowCreate(true)}>+ משתמש חדש</Button>
      </div>

      <Card>
        {users.length === 0 ? (
          <p className="text-center text-gray-400 py-10">אין משתמשים רשומים</p>
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50 text-gray-500 text-right">
                <th className="px-4 py-3 font-medium">שם משתמש</th>
                <th className="px-4 py-3 font-medium">אימייל</th>
                <th className="px-4 py-3 font-medium">תפקיד</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id} className="border-b last:border-0 hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{u.username}</td>
                  <td className="px-4 py-3 text-gray-500">{u.email || '—'}</td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-0.5 rounded-full text-xs font-medium
                      ${u.role === 'Admin' ? 'bg-purple-100 text-purple-700' : 'bg-gray-100 text-gray-600'}`}>
                      {u.role === 'Admin' ? 'מנהל' : 'משתמש'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-left">
                    <button onClick={() => handleDelete(u.id, u.username)}
                      className="text-xs text-red-400 hover:text-red-600">מחק</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>

      {showCreate && (
        <Modal title="משתמש חדש" onClose={() => { setShowCreate(false); setError('') }}>
          <div className="space-y-4">
            <Input label="שם משתמש *" value={form.username} autoFocus
              onChange={e => setForm(f => ({ ...f, username: e.target.value }))} />
            <Input label="אימייל *" type="email" value={form.email}
              placeholder="הסיסמה תישלח לכתובת זו"
              onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
            <div className="flex flex-col gap-1">
              <label className="text-sm font-medium text-gray-700">תפקיד</label>
              <select value={form.role}
                onChange={e => setForm(f => ({ ...f, role: e.target.value }))}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-indigo-500">
                <option value="User">משתמש רגיל</option>
                <option value="Admin">מנהל</option>
              </select>
            </div>
            {error && <p className="text-sm text-red-500">{error}</p>}
            <div className="flex gap-3 justify-end">
              <Button variant="secondary" onClick={() => { setShowCreate(false); setError('') }}>ביטול</Button>
              <Button onClick={handleCreate} loading={createLoading}
                disabled={!form.username.trim() || !form.email.trim()}>
                צור משתמש
              </Button>
            </div>
          </div>
        </Modal>
      )}

      {/* הסיסמה הזמנית — מוצגת פעם אחת בלבד */}
      {newUser && (
        <Modal title="✅ משתמש נוצר" onClose={() => setNewUser(null)}>
          <div className="space-y-4">
            <div className="bg-green-50 border border-green-200 rounded-xl p-4 text-sm text-gray-700 space-y-1">
              <p><span className="font-medium">שם משתמש:</span> {newUser.username}</p>
              <p><span className="font-medium">תפקיד:</span> {newUser.role === 'Admin' ? 'מנהל' : 'משתמש'}</p>
              {newUser.email && <p><span className="font-medium">מייל:</span> {newUser.email}</p>}
            </div>
            <div className="bg-yellow-50 border border-yellow-300 rounded-xl p-4">
              <p className="text-xs text-yellow-700 font-medium mb-2">
                ⚠️ הסיסמה מוצגת פעם אחת בלבד — העתיקי אותה עכשיו
                {newUser.email && ' (נשלח גם למייל)'}
              </p>
              <div className="flex items-center gap-3">
                <code className="text-xl font-bold tracking-widest text-gray-800 flex-1 select-all">
                  {newUser.temporaryPassword}
                </code>
                <button
                  onClick={() => navigator.clipboard.writeText(newUser.temporaryPassword)}
                  className="text-xs bg-white border border-gray-300 rounded px-2 py-1 hover:bg-gray-50 shrink-0">
                  העתק
                </button>
              </div>
            </div>
            <Button className="w-full justify-center" onClick={() => setNewUser(null)}>
              הבנתי, סגור
            </Button>
          </div>
        </Modal>
      )}
    </div>
  )
}
