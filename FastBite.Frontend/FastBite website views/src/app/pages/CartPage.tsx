import { Link, useNavigate } from "react-router";
import { Minus, Plus, Trash2, ShoppingBag, ArrowRight } from "lucide-react";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Separator } from "../components/ui/separator";
import { ImageWithFallback } from "../components/figma/ImageWithFallback";

const DELIVERY_FEE = 2.99;
const FREE_DELIVERY_THRESHOLD = 25;

export default function CartPage() {
  const { items, subtotal, setQuantity, removeItem } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();

  const deliveryFee = subtotal >= FREE_DELIVERY_THRESHOLD ? 0 : DELIVERY_FEE;
  const total = subtotal + deliveryFee;

  if (items.length === 0) {
    return (
      <div className="flex min-h-[calc(100vh-4rem)] flex-col items-center justify-center gap-4 px-4 text-center">
        <span className="text-7xl">🛒</span>
        <h2 className="text-2xl font-bold">Your cart is empty</h2>
        <p className="text-muted-foreground">Add some delicious items from our menu!</p>
        <Button asChild size="lg" className="mt-2">
          <Link to="/">Browse menu</Link>
        </Button>
      </div>
    );
  }

  function handleCheckout() {
    if (!user) {
      navigate("/login", { state: { from: "/checkout" } });
    } else {
      navigate("/checkout");
    }
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-8">
      <h1 className="mb-6 text-3xl font-bold">Your cart</h1>

      <div className="flex flex-col gap-6 lg:flex-row">
        {/* Line items */}
        <div className="flex flex-1 flex-col gap-3">
          {items.map(({ item, quantity }) => (
            <Card key={item.id} className="overflow-hidden">
              <CardContent className="flex gap-4 p-0">
                <div className="relative h-24 w-24 shrink-0">
                  <ImageWithFallback
                    src={item.imageUrl}
                    alt={item.name}
                    className="size-full object-cover"
                  />
                </div>
                <div className="flex flex-1 flex-col justify-center gap-1 py-3 pr-4">
                  <div className="flex items-start justify-between gap-2">
                    <span className="font-semibold leading-tight">{item.name}</span>
                    <button
                      onClick={() => removeItem(item.id)}
                      className="text-muted-foreground transition-colors hover:text-destructive"
                      aria-label="Remove"
                    >
                      <Trash2 className="size-4" />
                    </button>
                  </div>
                  <span className="text-sm text-muted-foreground">${item.price.toFixed(2)} each</span>
                  <div className="mt-1 flex items-center gap-3">
                    <button
                      onClick={() => setQuantity(item.id, quantity - 1)}
                      className="flex size-7 items-center justify-center rounded-full border bg-background text-foreground transition-colors hover:bg-accent"
                    >
                      <Minus className="size-3" />
                    </button>
                    <span className="w-4 text-center font-medium">{quantity}</span>
                    <button
                      onClick={() => setQuantity(item.id, quantity + 1)}
                      className="flex size-7 items-center justify-center rounded-full bg-primary text-primary-foreground transition-opacity hover:opacity-90"
                    >
                      <Plus className="size-3" />
                    </button>
                    <span className="ml-auto font-semibold text-primary">
                      ${(item.price * quantity).toFixed(2)}
                    </span>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {/* Summary */}
        <div className="w-full lg:w-80">
          <Card className="sticky top-20">
            <CardContent className="flex flex-col gap-3 p-5">
              <h2 className="text-lg font-semibold">Order summary</h2>
              <Separator />
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Subtotal</span>
                <span>${subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Delivery fee</span>
                {deliveryFee === 0 ? (
                  <span className="font-medium text-primary">Free 🎉</span>
                ) : (
                  <span>${deliveryFee.toFixed(2)}</span>
                )}
              </div>
              {deliveryFee > 0 && (
                <p className="rounded-lg bg-accent px-3 py-2 text-xs text-accent-foreground">
                  Add ${(FREE_DELIVERY_THRESHOLD - subtotal).toFixed(2)} more for free delivery!
                </p>
              )}
              <Separator />
              <div className="flex justify-between font-semibold">
                <span>Total</span>
                <span className="text-primary">${total.toFixed(2)}</span>
              </div>
              <Button size="lg" className="mt-1 w-full" onClick={handleCheckout}>
                {user ? (
                  <>
                    Proceed to checkout <ArrowRight className="size-4" />
                  </>
                ) : (
                  <>
                    <ShoppingBag className="size-4" /> Sign in to checkout
                  </>
                )}
              </Button>
              <Button variant="outline" asChild className="w-full">
                <Link to="/">Continue shopping</Link>
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
