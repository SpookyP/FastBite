import { Star, Clock, Plus } from "lucide-react";
import { toast } from "sonner";
import type { MenuItem } from "../types";
import { useCart } from "../context/CartContext";
import { Card, CardContent } from "./ui/card";
import { Button } from "./ui/button";
import { Badge } from "./ui/badge";
import { ImageWithFallback } from "./figma/ImageWithFallback";

export function ItemCard({ item }: { item: MenuItem }) {
  const { addItem } = useCart();

  return (
    <Card className="group overflow-hidden pt-0 transition-shadow hover:shadow-lg">
      <div className="relative h-44 w-full overflow-hidden">
        <ImageWithFallback
          src={item.imageUrl}
          alt={item.name}
          className="size-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
        {item.popular && (
          <Badge className="absolute left-3 top-3 bg-primary text-primary-foreground">
            🔥 Popular
          </Badge>
        )}
      </div>
      <CardContent className="flex flex-col gap-2">
        <div className="flex items-start justify-between gap-2">
          <h3 className="leading-tight">{item.name}</h3>
          <span className="whitespace-nowrap text-primary">${item.price.toFixed(2)}</span>
        </div>
        <p className="line-clamp-2 text-sm text-muted-foreground">{item.description}</p>
        <div className="flex items-center gap-3 text-sm text-muted-foreground">
          <span className="flex items-center gap-1">
            <Star className="size-3.5 fill-amber-400 text-amber-400" />
            {item.rating.toFixed(1)}
          </span>
          <span className="flex items-center gap-1">
            <Clock className="size-3.5" />
            {item.prepMinutes} min
          </span>
        </div>
        <Button
          className="mt-2 w-full"
          onClick={() => {
            addItem(item);
            toast.success(`${item.name} added to cart`);
          }}
        >
          <Plus className="size-4" /> Add to cart
        </Button>
      </CardContent>
    </Card>
  );
}
