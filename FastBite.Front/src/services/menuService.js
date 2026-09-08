import { fetchComToken, API_BASES } from './apiClient';

export const menuService = {
    obterTodos: () => fetchComToken('/Menus/ObterTodos'),
    
    obterPorId: async (id) => fetchComToken(`/Menus/ObterPorId?id=${id}`),

    edit: async (id, data) => fetchComToken(`/Menus/Atualizar?id=${id}`, {
        baseUrl: API_BASES.menu,
        method: 'PUT',
        headers:{
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data)
    }),

    create: (novoItem) => fetchComToken('/Menus', {
        baseUrl: API_BASES.menu,
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(novoItem)
    }),
    delete: (id) => fetchComToken(`/Menus/Eliminar?id=${id}`, {
        method: 'DELETE',
    })
};