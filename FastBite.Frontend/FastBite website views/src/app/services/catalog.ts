// Catalog API client — browse categories and menu items.
import { API_CONFIG, USE_MOCKS, apiFetch, delay } from "./http";
import type { Category, MenuItem } from "../types";

const CATEGORIES: Category[] = [
  { id: "all", name: "All", icon: "🍽️" },
  { id: "burgers", name: "Burgers", icon: "🍔" },
  { id: "pizza", name: "Pizza", icon: "🍕" },
  { id: "sushi", name: "Sushi", icon: "🍣" },
  { id: "salads", name: "Salads", icon: "🥗" },
  { id: "desserts", name: "Desserts", icon: "🍰" },
  { id: "drinks", name: "Drinks", icon: "🥤" },
];

const ITEMS: MenuItem[] = [
  {
    id: "b1",
    name: "Classic Smash Burger",
    description: "Double beef patty, cheddar, pickles and house sauce on a brioche bun.",
    price: 9.9,
    imageUrl:
      "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800&q=80",
    categoryId: "burgers",
    rating: 4.8,
    prepMinutes: 15,
    popular: true,
    tags: ["Beef", "Bestseller"],
  },
  {
    id: "b2",
    name: "Crispy Chicken Burger",
    description: "Buttermilk fried chicken, slaw and spicy mayo.",
    price: 8.5,
    imageUrl:
      "https://images.unsplash.com/photo-1606755962773-d324e0a13086?w=800&q=80",
    categoryId: "burgers",
    rating: 4.6,
    prepMinutes: 14,
    tags: ["Chicken", "Spicy"],
  },
  {
    id: "p1",
    name: "Margherita Pizza",
    description: "San Marzano tomato, fresh mozzarella and basil.",
    price: 11.0,
    imageUrl:
      "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=800&q=80",
    categoryId: "pizza",
    rating: 4.9,
    prepMinutes: 20,
    popular: true,
    tags: ["Vegetarian"],
  },
  {
    id: "p2",
    name: "Pepperoni Pizza",
    description: "Loaded pepperoni with mozzarella and oregano.",
    price: 12.5,
    imageUrl:
      "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=800&q=80",
    categoryId: "pizza",
    rating: 4.7,
    prepMinutes: 20,
    tags: ["Meat"],
  },
  {
    id: "s1",
    name: "Salmon Nigiri Set",
    description: "Eight pieces of fresh salmon nigiri with wasabi and ginger.",
    price: 14.0,
    imageUrl:
      "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=800&q=80",
    categoryId: "sushi",
    rating: 4.8,
    prepMinutes: 18,
    tags: ["Fish"],
  },
  {
    id: "sa1",
    name: "Caesar Salad",
    description: "Romaine, parmesan, croutons and creamy caesar dressing.",
    price: 7.5,
    imageUrl:
      "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=800&q=80",
    categoryId: "salads",
    rating: 4.4,
    prepMinutes: 8,
    tags: ["Healthy"],
  },
  {
    id: "d1",
    name: "Molten Chocolate Cake",
    description: "Warm chocolate cake with a gooey center and vanilla ice cream.",
    price: 6.0,
    imageUrl:
      "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800&q=80",
    categoryId: "desserts",
    rating: 4.9,
    prepMinutes: 10,
    popular: true,
    tags: ["Sweet"],
  },
  {
    id: "dr1",
    name: "Fresh Orange Juice",
    description: "Cold-pressed 100% orange juice, no added sugar.",
    price: 4.0,
    imageUrl:
      "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=800&q=80",
    categoryId: "drinks",
    rating: 4.5,
    prepMinutes: 5,
    tags: ["Cold"],
  },
];

export const catalogService = {
  async getCategories(): Promise<Category[]> {
    if (USE_MOCKS) return delay(CATEGORIES, 300);
    return apiFetch<Category[]>(API_CONFIG.catalogBaseUrl, "/categories");
  },

  async getItems(categoryId?: string, search?: string): Promise<MenuItem[]> {
    if (USE_MOCKS) {
      let result = ITEMS;
      if (categoryId && categoryId !== "all") {
        result = result.filter((i) => i.categoryId === categoryId);
      }
      if (search) {
        const q = search.toLowerCase();
        result = result.filter(
          (i) =>
            i.name.toLowerCase().includes(q) ||
            i.description.toLowerCase().includes(q),
        );
      }
      return delay(result, 400);
    }
    const params = new URLSearchParams();
    if (categoryId && categoryId !== "all") params.set("category", categoryId);
    if (search) params.set("search", search);
    return apiFetch<MenuItem[]>(
      API_CONFIG.catalogBaseUrl,
      `/items?${params.toString()}`,
    );
  },

  async getItem(id: string): Promise<MenuItem | undefined> {
    if (USE_MOCKS) return delay(ITEMS.find((i) => i.id === id), 200);
    return apiFetch<MenuItem>(API_CONFIG.catalogBaseUrl, `/items/${id}`);
  },
};
