import React, { createContext, useState, useContext } from 'react';

//Criar o Context
const CartContext = createContext();

//Criar o Provider
export const CartProvider = ({ children }) => {
    //'cart' - como um List<Item>
    //'setCart' é o método para atualizar essa lista
    const [cart, setCart] = useState([]);

    // Adicionar um item ao carrinho
    const adicionarAoCarrinho = (produto) => {
        const stockDisponivel = produto.limiteDiario - (produto.quantidadeVendidaHoje || 0);

        if (stockDisponivel <= 0) {
            alert(`O item ${produto.nome} encontra-se esgotado!`);
            return;
        }
        setCart((carrinhoAtual) => {
            // Verificar se o produto já existe no carrinho
            const itemExiste = carrinhoAtual.find(item => item.id === produto.id);

            if (itemExiste) {
                // 3. Valida contra o stock calculado em vez do limiteDiario puro
                if (itemExiste.quantidade >= stockDisponivel) {
                    alert(`Atenção: Só existem ${stockDisponivel} unidades disponíveis de ${produto.nome}!`);
                    return carrinhoAtual;
                }
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

    //Remover produtos do carrinho
    const removerDoCarrinho = (produtoId) => {
        setCart((carrinhoAtual) => {
            // Filtramos a lista, mantendo apenas os itens cujo id seja diferente do id que queremos remover
            return carrinhoAtual.filter(item => item.id !== produtoId);
        });
    };

    const diminuirQuantidade = (produtoId) => {
        setCart((carrinhoAtual) => {
            //Procurar o item na lista
            const itemExistente = carrinhoAtual.find(item => item.id === produtoId);

            //Se a quantidade for 1, remover o item completamente
            if (itemExistente?.quantidade === 1) {
                return carrinhoAtual.filter(item => item.id !== produtoId);
            }

            //Caso contrário, apenas diminuímos 1 à quantidade atual
            return carrinhoAtual.map(item =>
                item.id === produtoId 
                    ? { ...item, quantidade: item.quantidade - 1 } 
                    : item
            );
        });
    };

    // Limpar o carrinho
    const limparCarrinho = () => {
        setCart([]);
    };

    // Expor a lista e os métodos para o resto da aplicação
    return (
        <CartContext.Provider value={{ cart, adicionarAoCarrinho, removerDoCarrinho, diminuirQuantidade, limparCarrinho }}>
            {children}
        </CartContext.Provider>
    );
};

// Hook personalizado
export const useCart = () => {
    return useContext(CartContext);
};