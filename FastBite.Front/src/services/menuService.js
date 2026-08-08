import { fetchComToken } from './apiClient';

export const menuService = {
    obterTodos: () => fetchComToken('/Menus'),
    
    // Adiciona esta função para o POST:
    create: (novoItem) => fetchComToken('/Menus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(novoItem)
    })
};