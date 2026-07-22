using FastBite.Models;

namespace FastBite.Services;

/// <summary>
/// Scoped service that manages the shopping cart.
/// Inject this wherever you need cart access.
/// </summary>
public class CartState
{
    private readonly List<CartItem> _items = [];

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public int Count => _items.Sum(i => i.Quantity);
    public decimal Subtotal => _items.Sum(i => i.Item.Price * i.Quantity);

    public event Action? OnChange;

    public void AddItem(MenuItem item, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(c => c.Item.Id == item.Id);
        if (existing is not null)
            existing.Quantity += quantity;
        else
            _items.Add(new CartItem { Item = item, Quantity = quantity });

        NotifyStateChanged();
    }

    public void RemoveItem(string itemId)
    {
        _items.RemoveAll(c => c.Item.Id == itemId);
        NotifyStateChanged();
    }

    public void SetQuantity(string itemId, int quantity)
    {
        if (quantity <= 0) { RemoveItem(itemId); return; }
        var item = _items.FirstOrDefault(c => c.Item.Id == itemId);
        if (item is not null) item.Quantity = quantity;
        NotifyStateChanged();
    }

    public void Clear()
    {
        _items.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
