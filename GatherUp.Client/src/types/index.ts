// ── Enums ──────────────────────────────────────────────────────────────────

export enum EventStatus {
  Draft = 0,
  PollOpen = 1,
  PollClosed = 2,
  InvitationsSent = 3,
  Active = 4,
  Completed = 5,
  Cancelled = 6,
}

export const EventStatusLabels: Record<EventStatus, string> = {
  [EventStatus.Draft]: 'טיוטה',
  [EventStatus.PollOpen]: 'סקר פתוח',
  [EventStatus.PollClosed]: 'סקר סגור',
  [EventStatus.InvitationsSent]: 'הזמנות נשלחו',
  [EventStatus.Active]: 'פעיל',
  [EventStatus.Completed]: 'הסתיים',
  [EventStatus.Cancelled]: 'בוטל',
}

export enum PaymentMethod {
  BankTransfer = 0,
  Cash = 1,
}

export const PaymentMethodLabels: Record<PaymentMethod, string> = {
  [PaymentMethod.BankTransfer]: 'העברה בנקאית',
  [PaymentMethod.Cash]: 'מזומן',
}

export enum NotificationPreference {
  None = 0,
  EventChanges = 1,
  DirectMessages = 2,
  NewPolls = 4,
}

// ── Models ─────────────────────────────────────────────────────────────────

export interface AppUser {
  id: string
  username: string
  role: string
  email?: string
}

export interface Person {
  id: string
  name: string
  email: string
  idNumber?: string
}

export interface EventHost extends Person {}
export interface EventManager extends Person {}

export interface Participant extends Person {
  isAttending?: boolean
  hasPaid: boolean
  amountPaid: number
  notificationPreferences: NotificationPreference
}

export interface PollQuestion {
  id: string
  questionText: string
  options: string[]
  votesByParticipant?: Record<string, string>
}

export interface Poll {
  id: string
  title: string
  description: string
  closesAt?: string
  isClosed?: boolean
  questions: PollQuestion[]
}

export interface VendorAllocation {
  id: string
  name: string
  amountOwed: number
  hasReceipt?: boolean
}

export interface ReceiptDetails {
  id: string
  amount: number
  issuedAt: string
  filePath: string
}

export interface GatherEvent {
  id: string
  title: string
  eventDate?: string
  location?: string
  pricePerParticipant?: number
  customMessage?: string
  status: EventStatus
  paymentMethod: PaymentMethod
  bankDetails?: string
  cashContactName?: string
  invitationScheduledAt?: string
  invitationContent: string
  host?: EventHost
  managers: EventManager[]
  participants: Participant[]
  vendors: VendorAllocation[]
  polls: Poll[]
  totalCollected?: number
  totalOwedToVendors?: number
  budget?: number
}

// ── Auth ───────────────────────────────────────────────────────────────────

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
}

// ── Misc ───────────────────────────────────────────────────────────────────

export interface PaymentRequest {
  amount: number
  method: PaymentMethod
}

export interface VoteRequest {
  participantId: string
  answer: string
}
