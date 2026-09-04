import React, { createContext, useState, useContext } from 'react';

//Criar o Context
const CartContext = createContext();

//Criar o Provider
export const CartProvider = ({ children }) => {
    const [cart, setCart] = useState([]);

    // Adicionar um item ao carrinho
    const adicionarAoCarrinho = (produto) => {
        setCart((carrinhoAtual) => {
            // Verificar se o produto já existe no carrinho
            const itemExiste = carrinhoAtual.find(item => item.id === produto.id);

            if (itemExiste) {
                // Se já existir, aumenta a quantidade
                return carrinhoAtual.map(item =>
                    item.id === produto.id 
                        ? { ...item, quantidade: item.quantidade + 1 } 
                        : item
                );
            }

            // Se não existir, adiciona o produto com quantidade 1
            return [...carrinhoAtual, { ...produto, quantidade: 1 }];
        });
    };

    // Remover ou limpar o carrinho (podemos implementar depois)
    const limparCarrinho = () => {
        setCart([]);
    };

    // Expor a lista e os métodos para o resto da aplicação
    return (
        <CartContext.Provider value={{ cart, adicionarAoCarrinho, limparCarrinho }}>
            {children}
        </CartContext.Provider>
    );
};

// Hook personalizado
export const useCart = () => {
    return useContext(CartContext);
};