import { fetchComToken, API_BASES } from './apiClient';

// item do CartContext ({ id, quantidade, ... }) -> DTO do backend ({ ProductId, Quantity })
const toOrderItem = (item) => ({
  ProductId: item.id,
  Quantity: item.quantidade,
});

export const orderService = {
  // POST /api/Order/agrupar — preview: agrupa combos/avulsos, calcula totais.
  // Não grava nem desconta stock.
  previewOrder: async (cartItems, taxaEntrega) =>
    fetchComToken('/Order/agrupar', {
      baseUrl: API_BASES.order,
      method: 'POST',
      body: JSON.stringify({
        Items: cartItems.map(toOrderItem),
        ...(taxaEntrega != null && { TaxaEntrega: taxaEntrega }),
      }),
    }),

  // POST /api/Order — cria o pedido.
  // items = orderData.items (lista normalizada devolvida pelo preview, pronta a reenviar)
  createOrder: async ({ items, entrega, pagamento, subtotalEsperado }) =>
    fetchComToken('/Order', {
      baseUrl: API_BASES.order,
      method: 'POST',
      body: JSON.stringify({
        Items: items,
        Entrega: entrega,
        Pagamento: pagamento,
        ...(subtotalEsperado != null && { SubtotalEsperado: subtotalEsperado }),
      }),
    }),

  // GET /api/Order/my-orders — histórico de encomendas do utilizador
  getMyOrders: () =>
    fetchComToken('/Order/my-orders', { baseUrl: API_BASES.order }),
};