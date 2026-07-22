// Identity API client — authentication and account management.
import { API_CONFIG, USE_MOCKS, apiFetch, delay, ApiError } from "./http";
import type {
  AuthResponse,
  LoginPayload,
  RegisterPayload,
  User,
} from "../types";

// In-memory mock user store (persisted to localStorage for the demo).
const MOCK_DB_KEY = "fastbite.mockUsers";

interface StoredUser extends User {
  password: string;
}

function readMockUsers(): StoredUser[] {
  try {
    return JSON.parse(localStorage.getItem(MOCK_DB_KEY) || "[]");
  } catch {
    return [];
  }
}

function writeMockUsers(users: StoredUser[]) {
  localStorage.setItem(MOCK_DB_KEY, JSON.stringify(users));
}

// Seed a demo account so users can log in immediately.
function ensureSeed() {
  const users = readMockUsers();
  if (!users.some((u) => u.email === "demo@fastbite.com")) {
    users.push({
      id: "u-demo",
      name: "Demo Foodie",
      email: "demo@fastbite.com",
      password: "password",
      phone: "+1 555 0100",
      address: "221B Baker Street, London",
    });
    writeMockUsers(users);
  }
}

function fakeToken(userId: string) {
  return `mock.${userId}.${Date.now()}`;
}

export const identityService = {
  async login(payload: LoginPayload): Promise<AuthResponse> {
    if (USE_MOCKS) {
      ensureSeed();
      const users = readMockUsers();
      const found = users.find((u) => u.email === payload.email);
      if (!found || found.password !== payload.password) {
        throw new ApiError(401, "Invalid email or password.");
      }
      const { password, ...user } = found;
      return delay({ token: fakeToken(user.id), user }, 600);
    }
    return apiFetch<AuthResponse>(API_CONFIG.identityBaseUrl, "/auth/login", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  async register(payload: RegisterPayload): Promise<AuthResponse> {
    if (USE_MOCKS) {
      ensureSeed();
      const users = readMockUsers();
      if (users.some((u) => u.email === payload.email)) {
        throw new ApiError(409, "An account with this email already exists.");
      }
      const newUser: StoredUser = {
        id: `u-${Date.now()}`,
        name: payload.name,
        email: payload.email,
        phone: payload.phone,
        password: payload.password,
      };
      users.push(newUser);
      writeMockUsers(users);
      const { password, ...user } = newUser;
      return delay({ token: fakeToken(user.id), user }, 700);
    }
    return apiFetch<AuthResponse>(API_CONFIG.identityBaseUrl, "/auth/register", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  async me(): Promise<User> {
    // With a real API this reads the current user from the bearer token.
    return apiFetch<User>(API_CONFIG.identityBaseUrl, "/auth/me");
  },
};
