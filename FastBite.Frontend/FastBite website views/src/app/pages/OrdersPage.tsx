import { useEffect, useState } from "react";
import { Link } from "react-router";
import { ChevronRight, Package, Loader2 } from "lucide-react";
import { orderingService } from "../services/ordering";
import type { Order } from "../types";
import { OrderTracker } from "../components/OrderTracker";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Button } from "../components/ui/button";
import { Separator } from "../components/ui/separator";

const STATUS_LABELS: Record<string, { label: string; color: string }> = {
  PENDING: { label: "Pending", color: "bg-yellow-100 text-yellow-800" },
  CONFIRMED: { label: "Confirmed", color: "bg-blue-100 text-blue-800" },
  PREPARING: { label: "Preparing", color: "bg-orange-100 text-orange-800" },
  OUT_FOR_DELIVERY: { label: "On the way", color: "bg-indigo-100 text-indigo-800" },
  DELIVERED: { label: "Delivered", color: "bg-green-100 text-green-800" },
  CANCELLED: { label: "Cancelled", color: "bg-red-100 text-red-800" },
};

function OrderCard({ order, onRefresh }: { order: Order; onRefresh: () => void }) {
  const [cancelling, setCancelling] = useState(false);
  const meta = STATUS_LABELS[order.status] ?? { label: order.status, color: "" };

  async function handleCancel() {
    setCancelling(true);
    try {
      await orderingService.cancelOrder(order.id);
      onRefresh();
    } finally {
      setCancelling(false);
    }
  }

  return (
    <Card>
      <CardContent className="flex flex-col gap-4 p-5">
        <div className="flex flex-wrap items-center justify-between gap-2">
          <div>
            <p className="text-xs text-muted-foreground">Order ID</p>
            <p className="font-semibold">{order.id}</p>
          </div>
          <span className={`rounded-full px-3 py-1 text-xs font-medium ${meta.color}`}>
            {meta.label}
          </span>
        </div>

        <OrderTracker status={order.status} />

        <Separator />

        <div className="flex flex-col gap-1 text-sm">
          {order.lines.map((line) => (
            <div key={line.itemId} className="flex justify-between text-muted-foreground">
              <span>{line.name} × {line.quantity}</span>
              <span>${(line.price * line.quantity).toFixed(2)}</span>
            </div>
          ))}
        </div>

        <Separator />

        <div className="flex items-center justify-between">
          <div className="text-sm text-muted-foreground">
            <p>{new Date(order.createdAt).toLocaleString()}</p>
            <p className="font-medium text-foreground">Total: ${order.total.toFixed(2)}</p>
          </div>
          <div className="flex gap-2">
            {order.status !== "DELIVERED" && order.status !== "CANCELLED" && (
              <Button
                variant="outline"
                size="sm"
                onClick={handleCancel}
                disabled={cancelling}
                className="text-destructive hover:text-destructive"
              >
                {cancelling ? <Loader2 className="size-3 animate-spin" /> : "Cancel"}
              </Button>
            )}
            <Button variant="outline" size="sm" asChild>
              <Link to={`/orders/${order.id}`}>
                Details <ChevronRight className="size-3" />
              </Link>
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  function loadOrders() {
    setLoading(true);
    orderingService.getOrders().then((data) => {
      setOrders(data);
      setLoading(false);
    });
  }

  useEffect(() => {
    loadOrders();
    const interval = setInterval(loadOrders, 15000);
    return () => clearInterval(interval);
  }, []);

  if (loading) {
    return (
      <div className="flex h-[60vh] items-center justify-center">
        <Loader2 className="size-6 animate-spin text-primary" />
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="flex min-h-[calc(100vh-4rem)] flex-col items-center justify-center gap-4 px-4 text-center">
        <Package className="size-16 text-muted-foreground/40" />
        <h2 className="text-2xl font-bold">No orders yet</h2>
        <p className="text-muted-foreground">{"You haven't placed any orders. Time to eat!"}</p>
        <Button asChild size="lg" className="mt-2">
          <Link to="/">Browse menu</Link>
        </Button>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-8">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-3xl font-bold">My orders</h1>
        <Badge variant="secondary">{orders.length} orders</Badge>
      </div>
      <div className="flex flex-col gap-4">
        {orders.map((order) => (
          <OrderCard key={order.id} order={order} onRefresh={loadOrders} />
        ))}
      </div>
    </div>
  );
}
