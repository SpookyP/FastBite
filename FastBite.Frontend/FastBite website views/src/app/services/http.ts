// Lightweight HTTP helper shared by all three API clients.
//
// Each API has its own base URL. Replace these placeholder values with your
// real deployed endpoints (e.g. via environment variables) when connecting the
// live backends. While `USE_MOCKS` is true the service modules serve local
// mock data so the whole UI is fully navigable without a running backend.

export const API_CONFIG = {
  identityBaseUrl: "https://YOUR_IDENTITY_API_URL/api",
  catalogBaseUrl: "https://YOUR_CATALOG_API_URL/api",
  orderingBaseUrl: "https://YOUR_ORDERING_API_URL/api",
};

// Flip to false once the real APIs above are wired up.
export const USE_MOCKS = true;

const TOKEN_KEY = "fastbite.token";

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN_KEY, token);
  else localStorage.removeItem(TOKEN_KEY);
}

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

// Simulate network latency for a nicer, more realistic mock experience.
export function delay<T>(value: T, ms = 500): Promise<T> {
  return new Promise((resolve) => setTimeout(() => resolve(value), ms));
}

export async function apiFetch<T>(
  baseUrl: string,
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const token = getToken();
  const res = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {}),
    },
  });

  if (!res.ok) {
    let message = res.statusText;
    try {
      const body = await res.json();
      message = body.message || message;
    } catch {
      // ignore
    }
    throw new ApiError(res.status, message);
  }

  return res.json() as Promise<T>;
}
