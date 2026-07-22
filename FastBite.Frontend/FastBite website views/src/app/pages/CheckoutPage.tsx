import { useState } from "react";
import { useNavigate } from "react-router";
import {
  MapPin,
  CreditCard,
  Loader2,
  ChevronRight,
  ShoppingBag,
} from "lucide-react";
import { toast } from "sonner";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import { orderingService } from "../services/ordering";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Card, CardContent } from "../components/ui/card";
import { Separator } from "../components/ui/separator";
import { RadioGroup, RadioGroupItem } from "../components/ui/radio-group";

const DELIVERY_FEE = 2.99;
const FREE_DELIVERY_THRESHOLD = 25;

const PAYMENT_METHODS = [
  { value: "card", label: "Credit / Debit card", icon: "💳" },
  { value: "cash", label: "Cash on delivery", icon: "💵" },
  { value: "mbway", label: "MB WAY", icon: "📱" },
];

export default function CheckoutPage() {
  const { items, subtotal, clear } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();

  const [address, setAddress] = useState(user?.address || "");
  const [payment, setPayment] = useState("card");
  const [cardNumber, setCardNumber] = useState("");
  const [loading, setLoading] = useState(false);

  const deliveryFee = subtotal >= FREE_DELIVERY_THRESHOLD ? 0 : DELIVERY_FEE;
  const total = subtotal + deliveryFee;

  async function handlePlaceOrder(e: React.FormEvent) {
    e.preventDefault();
    if (!address.trim()) {
      toast.error("Please enter a delivery address.");
      return;
    }
    setLoading(true);
    try {
      const order = await orderingService.createOrder({
        lines: items.map(({ item, quantity }) => ({
          itemId: item.id,
          name: item.name,
          price: item.price,
          quantity,
        })),
        deliveryAddress: address,
        paymentMethod: payment,
      });
      clear();
      navigate(`/orders/${order.id}`);
      toast.success("Order placed! Sit tight 🍔");
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Could not place order.");
    } finally {
      setLoading(false);
    }
  }

  if (items.length === 0) {
    navigate("/cart", { replace: true });
    return null;
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-8">
      <div className="mb-2 flex items-center gap-2 text-sm text-muted-foreground">
        <span>Cart</span>
        <ChevronRight className="size-3.5" />
        <span className="font-medium text-foreground">Checkout</span>
      </div>
      <h1 className="mb-6 text-3xl font-bold">Checkout</h1>

      <form onSubmit={handlePlaceOrder}>
        <div className="flex flex-col gap-6 lg:flex-row">
          {/* Left — delivery + payment */}
          <div className="flex flex-1 flex-col gap-5">
            {/* Delivery address */}
            <Card>
              <CardContent className="flex flex-col gap-4 p-5">
                <div className="flex items-center gap-2">
                  <div className="flex size-8 items-center justify-center rounded-full bg-primary text-primary-foreground">
                    <MapPin className="size-4" />
                  </div>
                  <h2 className="text-lg font-semibold">Delivery address</h2>
                </div>
                <div className="flex flex-col gap-1.5">
                  <Label htmlFor="address">Street address</Label>
                  <Input
                    id="address"
                    placeholder="123 Main Street, City"
                    value={address}
                    onChange={(e) => setAddress(e.target.value)}
                    required
                  />
                </div>
              </CardContent>
            </Card>

            {/* Payment */}
            <Card>
              <CardContent className="flex flex-col gap-4 p-5">
                <div className="flex items-center gap-2">
                  <div className="flex size-8 items-center justify-center rounded-full bg-primary text-primary-foreground">
                    <CreditCard className="size-4" />
                  </div>
                  <h2 className="text-lg font-semibold">Payment method</h2>
                </div>

                <RadioGroup value={payment} onValueChange={setPayment} className="gap-3">
                  {PAYMENT_METHODS.map((m) => (
                    <Label
                      key={m.value}
                      htmlFor={m.value}
                      className={`flex cursor-pointer items-center gap-3 rounded-xl border p-4 transition-colors ${
                        payment === m.value
                          ? "border-primary bg-accent"
                          : "border-border hover:bg-muted"
                      }`}
                    >
                      <RadioGroupItem id={m.value} value={m.value} />
                      <span className="text-lg">{m.icon}</span>
                      <span>{m.label}</span>
                    </Label>
                  ))}
                </RadioGroup>

                {payment === "card" && (
                  <div className="flex flex-col gap-1.5">
                    <Label htmlFor="card">Card number</Label>
                    <Input
                      id="card"
                      placeholder="•••• •••• •••• ••••"
                      value={cardNumber}
                      onChange={(e) => setCardNumber(e.target.value)}
                      maxLength={19}
                    />
                    <p className="text-xs text-muted-foreground">
                      This is a demo — no real payment is processed.
                    </p>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Right — summary */}
          <div className="w-full lg:w-80">
            <Card className="sticky top-20">
              <CardContent className="flex flex-col gap-3 p-5">
                <div className="flex items-center gap-2">
                  <ShoppingBag className="size-4 text-primary" />
                  <h2 className="text-lg font-semibold">
                    {items.reduce((s, c) => s + c.quantity, 0)} items
                  </h2>
                </div>
                <Separator />
                <div className="flex flex-col gap-2 text-sm">
                  {items.map(({ item, quantity }) => (
                    <div key={item.id} className="flex justify-between">
                      <span className="text-muted-foreground">
                        {item.name} × {quantity}
                      </span>
                      <span>${(item.price * quantity).toFixed(2)}</span>
                    </div>
                  ))}
                </div>
                <Separator />
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Subtotal</span>
                  <span>${subtotal.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Delivery</span>
                  {deliveryFee === 0 ? (
                    <span className="font-medium text-primary">Free</span>
                  ) : (
                    <span>${deliveryFee.toFixed(2)}</span>
                  )}
                </div>
                <Separator />
                <div className="flex justify-between font-semibold">
                  <span>Total</span>
                  <span className="text-primary">${total.toFixed(2)}</span>
                </div>
                <Button type="submit" size="lg" className="mt-1 w-full" disabled={loading}>
                  {loading ? (
                    <>
                      <Loader2 className="size-4 animate-spin" /> Placing order…
                    </>
                  ) : (
                    "Place order"
                  )}
                </Button>
              </CardContent>
            </Card>
          </div>
        </div>
      </form>
    </div>
  );
}
