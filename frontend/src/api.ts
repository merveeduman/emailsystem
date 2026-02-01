import type { InboxMessage } from './types';

const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5161';

async function handleResponse(res: Response) {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  if (res.status === 204) return null;
  return res.json();
}

export const authApi = {
  async register(username: string, password: string) {
    const res = await fetch(`${baseUrl}/api/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    });
    return handleResponse(res);
  },
  async login(username: string, password: string) {
    const res = await fetch(`${baseUrl}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    });
    return handleResponse(res) as Promise<{ token: string }>;
  },
};

export const messagesApi = {
  async send(token: string, payload: { recipientUsername: string; subject: string; body: string }) {
    const res = await fetch(`${baseUrl}/api/messages`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        // BURASI KRİTİK: Ters tırnak (backtick) kullanıldı
        'Authorization': `Bearer ${token}`, 
      },
      body: JSON.stringify(payload),
    });
    return handleResponse(res);
  },
};

export const inboxApi = {
  async list(token: string) {
    const res = await fetch(`${baseUrl}/api/messages/inbox`, {
      headers: { 
        'Authorization': `Bearer ${token}` 
      },
    });
    return handleResponse(res) as Promise<InboxMessage[]>;
  },
};