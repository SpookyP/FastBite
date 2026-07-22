// Ordering API client — create orders and track their status.
import { API_CONFIG, USE_MOCKS, apiFetch, delay } from "./http";
import type { CreateOrderPayload, Order, OrderStatus } from "../types";

const ORDERS_KEY = "fastbite.mockOrders";

function readOrders(): Order[] {
  try {
    return JSON.parse(localStorage.getItem(ORDERS_KEY) || "[]");
  } catch {
    return [];
  }
}

function writeOrders(orders: Order[]) {
  localStorage.setItem(ORDERS_KEY, JSON.stringify(orders));
}

const STATUS_FLOW: OrderStatus[] = [
  "PENDING",
  "CONFIRMED",
  "PREPARING",
  "OUT_FOR_DELIVERY",
  "DELIVERED",
];

// Advance a mock order's status based on how long ago it was created, so the
// tracking screen feels alive without a real backend pushing updates.
function progressStatus(order: Order): Order {
  if (order.status === "CANCELLED" || order.status === "DELIVERED") return order;
  const elapsedMin = (Date.now() - new Date(order.createdAt).getTime()) / 60000;
  const step = Math.min(Math.floor(elapsedMin / 0.5), STATUS_FLOW.length - 1);
  return { ...order, status: STATUS_FLOW[step] };
}

export const orderingService = {
  async createOrder(payload: CreateOrderPayload): Promise<Order> {
    if (USE_MOCKS) {
      const subtotal = payload.lines.reduce(
        (sum, l) => sum + l.price * l.quantity,
        0,
      );
      const deliveryFee = subtotal > 25 ? 0 : 2.99;
      const order: Order = {
        id: `ORD-${Math.floor(1000 + Math.random() * 9000)}`,
        createdAt: new Date().toISOString(),
        status: "PENDING",
        lines: payload.lines,
        subtotal,
        deliveryFee,
        total: subtotal + deliveryFee,
        deliveryAddress: payload.deliveryAddress,
        estimatedMinutes: 30,
      };
      const orders = readOrders();
      orders.unshift(order);
      writeOrders(orders);
      return delay(order, 800);
    }
    return apiFetch<Order>(API_CONFIG.orderingBaseUrl, "/orders", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  async getOrders(): Promise<Order[]> {
    if (USE_MOCKS) {
      const orders = readOrders().map(progressStatus);
      writeOrders(orders);
      return delay(orders, 400);
    }
    return apiFetch<Order[]>(API_CONFIG.orderingBaseUrl, "/orders");
  },

  async getOrder(id: string): Promise<Order | undefined> {
    if (USE_MOCKS) {
      const order = readOrders().find((o) => o.id === id);
      return delay(order ? progressStatus(order) : undefined, 300);
    }
    return apiFetch<Order>(API_CONFIG.orderingBaseUrl, `/orders/${id}`);
  },

  async cancelOrder(id: string): Promise<Order> {
    if (USE_MOCKS) {
      const orders = readOrders();
      const idx = orders.findIndex((o) => o.id === id);
      if (idx >= 0) {
        orders[idx] = { ...orders[idx], status: "CANCELLED" };
        writeOrders(orders);
      }
      return delay(orders[idx], 400);
    }
    return apiFetch<Order>(API_CONFIG.orderingBaseUrl, `/orders/${id}/cancel`, {
      method: "POST",
    });
  },
};
