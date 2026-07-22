import { useEffect, useState } from "react";
import { Link, useParams, useNavigate } from "react-router";
import { ArrowLeft, MapPin, Clock, Loader2, RefreshCw } from "lucide-react";
import { orderingService } from "../services/ordering";
import type { Order } from "../types";
import { OrderTracker } from "../components/OrderTracker";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Separator } from "../components/ui/separator";
import { Badge } from "../components/ui/badge";

const STATUS_LABELS: Record<string, { label: string; color: string }> = {
  PENDING: { label: "Pending", color: "bg-yellow-100 text-yellow-800" },
  CONFIRMED: { label: "Confirmed", color: "bg-blue-100 text-blue-800" },
  PREPARING: { label: "Preparing", color: "bg-orange-100 text-orange-800" },
  OUT_FOR_DELIVERY: { label: "On the way", color: "bg-indigo-100 text-indigo-800" },
  DELIVERED: { label: "Delivered", color: "bg-green-100 text-green-800" },
  CANCELLED: { label: "Cancelled", color: "bg-red-100 text-red-800" },
};

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);

  function loadOrder() {
    if (!id) return;
    orderingService.getOrder(id).then((data) => {
      if (!data) { navigate("/orders", { replace: true }); return; }
      setOrder(data);
      setLoading(false);
    });
  }

  useEffect(() => {
    loadOrder();
    const interval = setInterval(loadOrder, 10000);
    return () => clearInterval(interval);
  }, [id]);

  async function handleCancel() {
    if (!order) return;
    setCancelling(true);
    try {
      const updated = await orderingService.cancelOrder(order.id);
      setOrder(updated);
    } finally {
      setCancelling(false);
    }
  }

  if (loading) {
    return (
      <div className="flex h-[60vh] items-center justify-center">
        <Loader2 className="size-6 animate-spin text-primary" />
      </div>
    );
  }

  if (!order) return null;

  const meta = STATUS_LABELS[order.status] ?? { label: order.status, color: "" };
  const active = order.status !== "DELIVERED" && order.status !== "CANCELLED";

  return (
    <div className="mx-auto max-w-2xl px-4 py-8">
      <Button variant="ghost" size="sm" asChild className="mb-4 -ml-2">
        <Link to="/orders">
          <ArrowLeft className="size-4" /> Back to orders
        </Link>
      </Button>

      <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold">Order {order.id}</h1>
          <p className="mt-0.5 text-sm text-muted-foreground">
            Placed {new Date(order.createdAt).toLocaleString()}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Badge className={`${meta.color} border-0`}>{meta.label}</Badge>
          <Button variant="ghost" size="icon" onClick={loadOrder} title="Refresh">
            <RefreshCw className="size-4" />
          </Button>
        </div>
      </div>

      {/* Tracker */}
      <Card className="mb-4">
        <CardContent className="p-5">
          <OrderTracker status={order.status} />
          {active && (
            <p className="mt-4 flex items-center justify-center gap-1.5 text-sm text-muted-foreground">
              <Clock className="size-4" />
              Estimated delivery in ~{order.estimatedMinutes} minutes. Updates every 10 s.
            </p>
          )}
        </CardContent>
      </Card>

      {/* Delivery address */}
      <Card className="mb-4">
        <CardContent className="flex items-start gap-3 p-5">
          <MapPin className="mt-0.5 size-5 shrink-0 text-primary" />
          <div>
            <p className="font-medium">Delivery address</p>
            <p className="text-sm text-muted-foreground">{order.deliveryAddress}</p>
          </div>
        </CardContent>
      </Card>

      {/* Line items + totals */}
      <Card>
        <CardContent className="flex flex-col gap-3 p-5">
          <h2 className="font-semibold">Items ordered</h2>
          <div className="flex flex-col gap-2 text-sm">
            {order.lines.map((line) => (
              <div key={line.itemId} className="flex justify-between">
                <span className="text-muted-foreground">
                  {line.name} × {line.quantity}
                </span>
                <span>${(line.price * line.quantity).toFixed(2)}</span>
              </div>
            ))}
          </div>
          <Separator />
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">Subtotal</span>
            <span>${order.subtotal.toFixed(2)}</span>
          </div>
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">Delivery</span>
            {order.deliveryFee === 0 ? (
              <span className="font-medium text-primary">Free</span>
            ) : (
              <span>${order.deliveryFee.toFixed(2)}</span>
            )}
          </div>
          <Separator />
          <div className="flex justify-between font-semibold">
            <span>Total</span>
            <span className="text-primary">${order.total.toFixed(2)}</span>
          </div>

          {active && (
            <Button
              variant="outline"
              className="mt-2 text-destructive hover:text-destructive"
              onClick={handleCancel}
              disabled={cancelling}
            >
              {cancelling ? (
                <><Loader2 className="size-4 animate-spin" /> Cancelling…</>
              ) : (
                "Cancel order"
              )}
            </Button>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
