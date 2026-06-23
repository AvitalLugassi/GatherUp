import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

export function AdminRoute() {
  const { isAdmin } = useAuth()
  return isAdmin ? <Outlet /> : <Navigate to="/events" replace />
}
