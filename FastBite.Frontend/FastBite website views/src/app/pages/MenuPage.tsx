import { useEffect, useState } from "react";
import { Search, SlidersHorizontal } from "lucide-react";
import { catalogService } from "../services/catalog";
import type { Category, MenuItem } from "../types";
import { ItemCard } from "../components/ItemCard";
import { Input } from "../components/ui/input";
import { Skeleton } from "../components/ui/skeleton";
import { Badge } from "../components/ui/badge";

function ItemCardSkeleton() {
  return (
    <div className="overflow-hidden rounded-xl border bg-card">
      <Skeleton className="h-44 w-full rounded-none" />
      <div className="flex flex-col gap-2 p-4">
        <Skeleton className="h-5 w-3/4" />
        <Skeleton className="h-4 w-full" />
        <Skeleton className="h-4 w-2/3" />
        <Skeleton className="mt-2 h-9 w-full" />
      </div>
    </div>
  );
}

export default function MenuPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [items, setItems] = useState<MenuItem[]>([]);
  const [activeCat, setActiveCat] = useState("all");
  const [search, setSearch] = useState("");
  const [loadingCats, setLoadingCats] = useState(true);
  const [loadingItems, setLoadingItems] = useState(true);

  useEffect(() => {
    catalogService.getCategories().then((cats) => {
      setCategories(cats);
      setLoadingCats(false);
    });
  }, []);

  useEffect(() => {
    setLoadingItems(true);
    const timeout = setTimeout(() => {
      catalogService.getItems(activeCat, search).then((data) => {
        setItems(data);
        setLoadingItems(false);
      });
    }, 200);
    return () => clearTimeout(timeout);
  }, [activeCat, search]);

  return (
    <div className="mx-auto max-w-6xl px-4 py-8">
      {/* Hero strip */}
      <div className="mb-8 overflow-hidden rounded-2xl bg-gradient-to-br from-primary via-orange-500 to-amber-400 px-8 py-10 text-primary-foreground shadow-lg">
        <h1 className="text-4xl font-bold leading-tight">
          What are you<br />craving today? 🍽️
        </h1>
        <p className="mt-2 text-primary-foreground/80">
          Fresh ingredients · Fast delivery · Happy eating
        </p>
      </div>

      {/* Search */}
      <div className="relative mb-6">
        <Search className="absolute left-4 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          className="h-12 pl-11 text-base"
          placeholder="Search burgers, pizza, sushi…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <SlidersHorizontal className="absolute right-4 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
      </div>

      {/* Category pills */}
      <div className="mb-6 flex gap-2 overflow-x-auto pb-2">
        {loadingCats
          ? Array.from({ length: 7 }).map((_, i) => (
              <Skeleton key={i} className="h-9 w-20 shrink-0 rounded-full" />
            ))
          : categories.map((cat) => (
              <button
                key={cat.id}
                onClick={() => setActiveCat(cat.id)}
                className={`flex shrink-0 items-center gap-1.5 rounded-full border px-4 py-1.5 text-sm font-medium transition-colors ${
                  activeCat === cat.id
                    ? "border-primary bg-primary text-primary-foreground"
                    : "border-border bg-card text-foreground hover:bg-accent"
                }`}
              >
                <span>{cat.icon}</span>
                {cat.name}
              </button>
            ))}
      </div>

      {/* Results heading */}
      {!loadingItems && (
        <div className="mb-4 flex items-center gap-2">
          <h2 className="text-lg font-semibold">
            {search
              ? `Results for "${search}"`
              : categories.find((c) => c.id === activeCat)?.name ?? "All"}
          </h2>
          <Badge variant="secondary">{items.length} items</Badge>
        </div>
      )}

      {/* Grid */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        {loadingItems
          ? Array.from({ length: 8 }).map((_, i) => <ItemCardSkeleton key={i} />)
          : items.length === 0
            ? (
                <div className="col-span-full flex flex-col items-center gap-3 py-20 text-muted-foreground">
                  <span className="text-5xl">🫙</span>
                  <p className="text-lg font-medium">No items found</p>
                  <p className="text-sm">Try a different category or search term.</p>
                </div>
              )
            : items.map((item) => <ItemCard key={item.id} item={item} />)}
      </div>
    </div>
  );
}
