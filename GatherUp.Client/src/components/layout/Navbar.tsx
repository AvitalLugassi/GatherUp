import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { Button } from '../ui/Button'

export function Navbar() {
  const { isAuthenticated, username, isAdmin, logout } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate('/login')
  }

  return (
    <header className="sticky top-0 z-40 bg-white border-b border-gray-200 shadow-sm">
      <div className="max-w-6xl mx-auto px-4 h-14 flex items-center justify-between">
        <Link to="/events" className="text-xl font-bold text-indigo-600 tracking-tight">
          GatherUp 🎉
        </Link>
        <nav className="flex items-center gap-4">
          {isAuthenticated ? (
            <>
              <Link to="/events" className="text-sm text-gray-600 hover:text-indigo-600 transition-colors">
                אירועים
              </Link>
              {isAdmin && (
                <Link to="/users" className="text-sm text-gray-600 hover:text-indigo-600 transition-colors">
                  משתמשים
                </Link>
              )}
              <span className="text-sm text-gray-400">שלום, {username}</span>
              <Button variant="secondary" onClick={handleLogout}>יציאה</Button>
            </>
          ) : (
            <Button onClick={() => navigate('/login')}>התחברות</Button>
          )}
        </nav>
      </div>
    </header>
  )
}
