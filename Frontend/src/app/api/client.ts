import axios from 'axios'

export const api = axios.create({
  baseURL: '',
})

export type ApiError = {
  error: string
  code?: string
  details?: unknown
}

