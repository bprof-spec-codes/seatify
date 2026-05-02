export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
  confirmPassword: string
}

export interface AuthResponse {
  token: string
  expiresAtUtc: string
  organizerId: string
  email: string
  name: string
}

export interface CurrentUser {
  organizerId: string
  email: string
  name: string
}