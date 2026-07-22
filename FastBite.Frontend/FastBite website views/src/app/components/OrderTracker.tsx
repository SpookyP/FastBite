import { Check, Clock, ChefHat, Bike, PackageCheck, XCircle } from "lucide-react";
import type { OrderStatus } from "../types";

const STEPS: { status: OrderStatus; label: string; icon: typeof Check }[] = [
  { status: "PENDING", label: "Order placed", icon: Clock },
  { status: "CONFIRMED", label: "Confirmed", icon: Check },
  { status: "PREPARING", label: "Preparing", icon: ChefHat },
  { status: "OUT_FOR_DELIVERY", label: "On the way", icon: Bike },
  { status: "DELIVERED", label: "Delivered", icon: PackageCheck },
];

export function OrderTracker({ status }: { status: OrderStatus }) {
  if (status === "CANCELLED") {
    return (
      <div className="flex items-center gap-2 rounded-lg bg-destructive/10 p-4 text-destructive">
        <XCircle className="size-5" />
        <span>This order was cancelled.</span>
      </div>
    );
  }

  const currentIndex = STEPS.findIndex((s) => s.status === status);

  return (
    <div className="flex items-start justify-between">
      {STEPS.map((step, i) => {
        const done = i <= currentIndex;
        const active = i === currentIndex;
        const Icon = step.icon;
        return (
          <div key={step.status} className="flex flex-1 flex-col items-center text-center">
            <div className="flex w-full items-center">
              <div
                className={`h-1 flex-1 rounded ${i === 0 ? "bg-transparent" : done ? "bg-primary" : "bg-muted"}`}
              />
              <div
                className={`flex size-9 shrink-0 items-center justify-center rounded-full transition-colors ${
                  done ? "bg-primary text-primary-foreground" : "bg-muted text-muted-foreground"
                } ${active ? "ring-4 ring-primary/20" : ""}`}
              >
                <Icon className="size-4" />
              </div>
              <div
                className={`h-1 flex-1 rounded ${i === STEPS.length - 1 ? "bg-transparent" : i < currentIndex ? "bg-primary" : "bg-muted"}`}
              />
            </div>
            <span className={`mt-2 text-xs ${done ? "text-foreground" : "text-muted-foreground"}`}>
              {step.label}
            </span>
          </div>
        );
      })}
    </div>
  );
}
