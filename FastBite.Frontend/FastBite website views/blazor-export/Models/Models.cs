namespace FastBite.Models;

// ── Identity API ──────────────────────────────────────────────────────────────

public class User
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = "";
    public User User { get; set; } = new();
}

public class LoginPayload
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterPayload
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Phone { get; set; }
}

// ── Catalog API ───────────────────────────────────────────────────────────────

public class Category
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
}

public class MenuItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public double Rating { get; set; }
    public int PrepMinutes { get; set; }
    public bool Popular { get; set; }
    public List<string> Tags { get; set; } = [];
}

// ── Ordering API ──────────────────────────────────────────────────────────────

public enum OrderStatus
{
    PENDING,
    CONFIRMED,
    PREPARING,
    OUT_FOR_DELIVERY,
    DELIVERED,
    CANCELLED
}

public class OrderLine
{
    public string ItemId { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    public string Id { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }
    public string DeliveryAddress { get; set; } = "";
    public int EstimatedMinutes { get; set; }
}

public class CreateOrderPayload
{
    public List<OrderLine> Lines { get; set; } = [];
    public string DeliveryAddress { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
}

public class CartItem
{
    public MenuItem Item { get; set; } = new();
    public int Quantity { get; set; }
}
