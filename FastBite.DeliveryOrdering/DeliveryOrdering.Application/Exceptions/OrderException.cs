
using DeliveryOrdering.Application.DTOs;

namespace DeliveryOrdering.Application.Exceptions
{
    public abstract class OrderException : Exception
    {
        protected OrderException(string message, Exception? inner = null) : base(message, inner) { }
    }
    public sealed class PedidoInvalidoException : OrderException { public PedidoInvalidoException(string m) : base(m) { } }        // 400
    public sealed class ProdutoNaoEncontradoException : OrderException { public int ProductId { get; } public ProdutoNaoEncontradoException(int id) : base($"Produto {id} não existe no catálogo.") => ProductId = id; }  // 404
    public sealed class StockInsuficienteException : OrderException { public IReadOnlyList<ItemIndisponivelDto> Itens { get; } public StockInsuficienteException(IReadOnlyList<ItemIndisponivelDto> i) : base("Um ou mais itens sem stock.") => Itens = i; }  // 409
    public sealed class PrecoAlteradoException : OrderException { public decimal SubtotalEsperado { get; } public decimal SubtotalActual { get; } public PrecoAlteradoException(decimal e, decimal a) : base($"Preços alterados (esperado {e:F2}, actual {a:F2}).") { SubtotalEsperado = e; SubtotalActual = a; } }  // 409
    public sealed class DescontoStockFalhouException : OrderException { public Guid OrderId { get; } public DescontoStockFalhouException(Guid id, Exception inner) : base($"Pedido {id} não confirmado: falha ao descontar stock.", inner) => OrderId = id; }  // 502
}
