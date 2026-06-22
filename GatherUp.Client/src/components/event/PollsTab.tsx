import { useState } from 'react'
import { eventsApi } from '../../api/events'
import type { GatherEvent, Poll, PollQuestion } from '../../types'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import { Modal } from '../ui/Modal'
import { useAuth } from '../../context/AuthContext'

interface Props { event: GatherEvent; onReload: () => void }

export function PollsTab({ event, onReload }: Props) {
  const { isAdmin, userId } = useAuth()
  const [showCreate, setShowCreate] = useState(false)
  const [createLoading, setCreateLoading] = useState(false)
  const [error, setError] = useState('')
  const [results, setResults] = useState<Record<string, Record<string, number>>>({})

  const [pollForm, setPollForm] = useState({
    title: '',
    description: '',
    questions: [{ questionText: '', options: ['', ''] }],
  })

  const polls: Poll[] = event.polls ?? []

  function addQuestion() {
    setPollForm(f => ({ ...f, questions: [...f.questions, { questionText: '', options: ['', ''] }] }))
  }

  function updateQuestion(qi: number, field: string, value: string) {
    setPollForm(f => {
      const questions = [...f.questions]
      questions[qi] = { ...questions[qi], [field]: value }
      return { ...f, questions }
    })
  }

  function addOption(qi: number) {
    setPollForm(f => {
      const questions = [...f.questions]
      questions[qi] = { ...questions[qi], options: [...questions[qi].options, ''] }
      return { ...f, questions }
    })
  }

  function updateOption(qi: number, oi: number, value: string) {
    setPollForm(f => {
      const questions = [...f.questions]
      const options = [...questions[qi].options]
      options[oi] = value
      questions[qi] = { ...questions[qi], options }
      return { ...f, questions }
    })
  }

  async function handleCreate() {
    setError('')
    setCreateLoading(true)
    try {
      await eventsApi.createPoll(event.id, {
        ...pollForm,
        questions: pollForm.questions.map(q => ({
          ...q,
          options: q.options.filter(o => o.trim()),
        })),
      })
      setShowCreate(false)
      setPollForm({ title: '', description: '', questions: [{ questionText: '', options: ['', ''] }] })
      onReload()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'שגיאה')
    } finally {
      setCreateLoading(false)
    }
  }

  async function vote(pollId: string, questionId: string, answer: string) {
    const participant = event.participants?.find(p => p.email)
    if (!participant) return alert('לא ניתן להצביע — אינך משתתף באירוע')
    await eventsApi.vote(event.id, pollId, questionId, { participantId: participant.id, answer })
    onReload()
  }

  async function loadResults(pollId: string, questionId: string) {
    const res = await eventsApi.getResults(event.id, pollId, questionId)
    setResults(prev => ({ ...prev, [`${pollId}_${questionId}`]: res }))
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold text-gray-700">סקרים</h2>
        {isAdmin && <Button onClick={() => setShowCreate(true)}>+ סקר חדש</Button>}
      </div>

      {polls.length === 0 ? (
        <Card className="p-10 text-center text-gray-400">אין סקרים עדיין</Card>
      ) : (
        <div className="space-y-4">
          {polls.map(poll => (
            <Card key={poll.id} className="p-5">
              <div className="flex items-center justify-between mb-3">
                <h3 className="font-semibold text-gray-800">{poll.title}</h3>
                {poll.isClosed && (
                  <span className="text-xs bg-gray-100 text-gray-500 px-2 py-1 rounded-full">סגור</span>
                )}
              </div>
              {poll.description && <p className="text-sm text-gray-500 mb-4">{poll.description}</p>}

              <div className="space-y-4">
                {poll.questions?.map((q: PollQuestion) => (
                  <div key={q.id} className="bg-gray-50 rounded-xl p-4">
                    <p className="font-medium text-gray-700 mb-3">{q.questionText}</p>
                    <div className="flex flex-wrap gap-2 mb-3">
                      {q.options.map(opt => (
                        <button key={opt}
                          onClick={() => !poll.isClosed && vote(poll.id, q.id, opt)}
                          disabled={poll.isClosed}
                          className={`px-3 py-1.5 rounded-lg text-sm border transition
                            ${poll.isClosed
                              ? 'bg-gray-100 text-gray-400 cursor-not-allowed border-gray-200'
                              : 'bg-white hover:bg-indigo-50 hover:border-indigo-400 border-gray-300 cursor-pointer'}`}
                        >
                          {opt}
                        </button>
                      ))}
                    </div>
                    {isAdmin && (
                      <button
                        onClick={() => loadResults(poll.id, q.id)}
                        className="text-xs text-indigo-500 hover:underline"
                      >
                        הצג תוצאות
                      </button>
                    )}
                    {results[`${poll.id}_${q.id}`] && (
                      <div className="mt-2 space-y-1">
                        {Object.entries(results[`${poll.id}_${q.id}`]).map(([ans, count]) => (
                          <div key={ans} className="flex items-center gap-2 text-sm">
                            <span className="w-24 text-gray-600">{ans}</span>
                            <div className="flex-1 bg-gray-200 rounded-full h-2">
                              <div className="bg-indigo-500 h-2 rounded-full"
                                style={{ width: `${(count / Math.max(...Object.values(results[`${poll.id}_${q.id}`]))) * 100}%` }} />
                            </div>
                            <span className="text-gray-500 w-6 text-right">{count}</span>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </Card>
          ))}
        </div>
      )}

      {showCreate && (
        <Modal title="סקר חדש" onClose={() => setShowCreate(false)}>
          <div className="space-y-4">
            <Input label="כותרת הסקר *" value={pollForm.title}
              onChange={e => setPollForm(f => ({ ...f, title: e.target.value }))} autoFocus />
            <Input label="תיאור" value={pollForm.description}
              onChange={e => setPollForm(f => ({ ...f, description: e.target.value }))} />

            <div className="space-y-4">
              {pollForm.questions.map((q, qi) => (
                <div key={qi} className="bg-gray-50 rounded-xl p-4 space-y-3">
                  <Input label={`שאלה ${qi + 1}`} value={q.questionText}
                    onChange={e => updateQuestion(qi, 'questionText', e.target.value)} />
                  <div className="space-y-2">
                    {q.options.map((opt, oi) => (
                      <Input key={oi} placeholder={`אפשרות ${oi + 1}`} value={opt}
                        onChange={e => updateOption(qi, oi, e.target.value)} />
                    ))}
                  </div>
                  <button onClick={() => addOption(qi)} className="text-xs text-indigo-500 hover:underline">
                    + הוסף אפשרות
                  </button>
                </div>
              ))}
            </div>

            <button onClick={addQuestion} className="text-sm text-indigo-500 hover:underline">
              + הוסף שאלה
            </button>

            {error && <p className="text-sm text-red-500">{error}</p>}
            <div className="flex gap-3 justify-end pt-2">
              <Button variant="secondary" onClick={() => setShowCreate(false)}>ביטול</Button>
              <Button onClick={handleCreate} loading={createLoading}>צור סקר</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  )
}
