import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import type { CartItem, MenuItem } from "../types";

interface CartContextValue {
  items: CartItem[];
  count: number;
  subtotal: number;
  addItem: (item: MenuItem, quantity?: number) => void;
  removeItem: (itemId: string) => void;
  setQuantity: (itemId: string, quantity: number) => void;
  clear: () => void;
}

const CartContext = createContext<CartContextValue | undefined>(undefined);

const CART_KEY = "fastbite.cart";

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>(() => {
    try {
      return JSON.parse(localStorage.getItem(CART_KEY) || "[]");
    } catch {
      return [];
    }
  });

  useEffect(() => {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
  }, [items]);

  function addItem(item: MenuItem, quantity = 1) {
    setItems((prev) => {
      const existing = prev.find((c) => c.item.id === item.id);
      if (existing) {
        return prev.map((c) =>
          c.item.id === item.id
            ? { ...c, quantity: c.quantity + quantity }
            : c,
        );
      }
      return [...prev, { item, quantity }];
    });
  }

  function removeItem(itemId: string) {
    setItems((prev) => prev.filter((c) => c.item.id !== itemId));
  }

  function setQuantity(itemId: string, quantity: number) {
    if (quantity <= 0) {
      removeItem(itemId);
      return;
    }
    setItems((prev) =>
      prev.map((c) => (c.item.id === itemId ? { ...c, quantity } : c)),
    );
  }

  function clear() {
    setItems([]);
  }

  const count = useMemo(
    () => items.reduce((sum, c) => sum + c.quantity, 0),
    [items],
  );
  const subtotal = useMemo(
    () => items.reduce((sum, c) => sum + c.item.price * c.quantity, 0),
    [items],
  );

  return (
    <CartContext.Provider
      value={{ items, count, subtotal, addItem, removeItem, setQuantity, clear }}
    >
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error("useCart must be used within CartProvider");
  return ctx;
}
