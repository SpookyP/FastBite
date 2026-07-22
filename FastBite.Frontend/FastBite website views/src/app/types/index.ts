// Shared domain types for FastBite across the three integrated APIs.

// ---- Identity API ----
export interface User {
  id: string;
  name: string;
  email: string;
  phone?: string;
  address?: string;
  avatarUrl?: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  name: string;
  email: string;
  password: string;
  phone?: string;
}

// ---- Catalog API ----
export interface Category {
  id: string;
  name: string;
  icon: string; // emoji or icon key
}

export interface MenuItem {
  id: string;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  categoryId: string;
  rating: number;
  prepMinutes: number;
  popular?: boolean;
  tags?: string[];
}

// ---- Ordering API ----
export interface CartItem {
  item: MenuItem;
  quantity: number;
}

export type OrderStatus =
  | "PENDING"
  | "CONFIRMED"
  | "PREPARING"
  | "OUT_FOR_DELIVERY"
  | "DELIVERED"
  | "CANCELLED";

export interface OrderLine {
  itemId: string;
  name: string;
  price: number;
  quantity: number;
}

export interface Order {
  id: string;
  createdAt: string;
  status: OrderStatus;
  lines: OrderLine[];
  subtotal: number;
  deliveryFee: number;
  total: number;
  deliveryAddress: string;
  estimatedMinutes: number;
}

export interface CreateOrderPayload {
  lines: OrderLine[];
  deliveryAddress: string;
  paymentMethod: string;
}
