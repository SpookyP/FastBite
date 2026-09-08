import { fetchComToken } from './apiClient';

export const orderService = {
    createOrder: async (cartItems) => fetchComToken('/Order/CriarPedido', {
        method 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(cartItems)
    }),

    previewOrder: async (cartItems) => fetchComToken('/Order/ObterCombos', {
        method 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({items: cartItems})
    });
};