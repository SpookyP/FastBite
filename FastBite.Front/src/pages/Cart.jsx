import React from 'react';
import { Link } from 'react-router-dom';
// Importamos o nosso "Armazém" para ler a lista e a função de remover
import { useCart } from '../context/CartContext'; 

const Cart = () => {
    // Extraímos o que precisamos do Context
    const { cart, removerDoCarrinho, diminuirQuantidade, adicionarAoCarrinho } = useCart();

    // Calculamr o Subtotal
    const subtotal = cart.reduce((soma, item) => soma + (item.precoBase * item.quantidade), 0);

    return (
        <div className="container py-5">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2 className="fw-bold m-0">O teu Carrinho 🛒</h2>
                {/* Botão para voltar à loja */}
                <Link to="/" className="text-decoration-none text-muted">
                    Continuar a comprar
                </Link>
            </div>

            {/* Se o carrinho estiver vazio */}
            {cart.length === 0 ? (
                <div className="text-center py-5 bg-white rounded-4 shadow-sm border-0">
                    <h4 className="text-muted mb-3">O teu carrinho está vazio...</h4>
                    <Link to="/" className="btn text-white fw-bold px-4 py-2" style={{ backgroundColor: '#ff6b00' }}>
                        Ir para o Menu
                    </Link>
                </div>
            ) : (
                <div className="row g-4">
                    {/* COLUNA ESQUERDA: LISTA DE PRODUTOS */}
                    <div className="col-lg-8">
                        <div className="card border-0 shadow-sm rounded-4 overflow-hidden">
                            <div className="card-header bg-white border-bottom py-3">
                                <h6 className="fw-bold m-0 text-muted">Itens Selecionados ({cart.length})</h6>
                            </div>
                            <div className="card-body p-0">
                                <ul className="list-group list-group-flush">
                                    {/* 'foreach' para desenhar cada produto */}
                                    {cart.map((item) => (
                                        <li key={item.id || item.codigo} className="list-group-item d-flex justify-content-between align-items-center p-4">
                                            
                                            {/* Info do Produto */}
                                            <div className="d-flex flex-column">
                                                <h5 className="fw-bold mb-1">{item.nome}</h5>
                                                <small className="text-muted mb-2">{item.descricao}</small>
                                                {/* Controlador de Quantidade */}
                                                <div className="d-flex align-items-center gap-2 mt-2">
                                                    {/* Botão de Diminuir (-) */}
                                                    <button 
                                                        className="btn btn-sm btn-outline-secondary rounded-circle d-flex justify-content-center align-items-center"
                                                        style={{ width: '30px', height: '30px' }}
                                                        onClick={() => diminuirQuantidade(item.id)}
                                                    >
                                                        <b>-</b>
                                                    </button>
                                                    
                                                    {/* Número da Quantidade atual */}
                                                    <span className="fw-bold px-2">{item.quantidade}</span>
                                                    
                                                    {/* Botão de Aumentar (+) */}
                                                    <button 
                                                        className="btn btn-sm btn-outline-secondary rounded-circle d-flex justify-content-center align-items-center"
                                                        style={{ width: '30px', height: '30px' }}
                                                        onClick={() => adicionarAoCarrinho(item)}
                                                    >
                                                        <b>+</b>
                                                    </button>
                                                </div>
                                            </div>

                                            {/* Preço e Botão de Eliminar */}
                                            <div className="d-flex flex-column align-items-end gap-2">
                                                <h5 className="fw-bold m-0" style={{ color: '#ff6b00' }}>
                                                    {(item.precoBase * item.quantidade).toFixed(2)} €
                                                </h5>
                                                <button 
                                                    className="btn btn-sm btn-outline-danger border-0"
                                                    onClick={() => removerDoCarrinho(item.id || item.Id || item.codigo)}
                                                >
                                                    🗑️ Remover
                                                </button>
                                            </div>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        </div>
                    </div>

                    {/* COLUNA DIREITA: RESUMO E BOTÃO DE CHECKOUT */}
                    <div className="col-lg-4">
                        <div className="card border-0 shadow-sm rounded-4 p-4 sticky-top" style={{ top: '20px' }}>
                            <h5 className="fw-bold mb-4">Resumo</h5>
                            
                            <div className="d-flex justify-content-between mb-3 text-muted">
                                <span>Subtotal</span>
                                <span>{subtotal.toFixed(2)}€</span>
                            </div>
                            
                            <hr className="text-muted" />

                            <div className="d-flex justify-content-between align-items-center mb-4 mt-2">
                                <h5 className="m-0 fw-bold">Total</h5>
                                <h4 className="m-0 fw-bold" style={{ color: '#ff6b00' }}>
                                    {subtotal.toFixed(2)}€
                                </h4>
                            </div>

                            <p className="text-muted small text-center mb-3">
                                Os custos de entrega serão calculados no próximo passo.
                            </p>

                            {/* Botão que reencaminha para o CHECKOUT */}
                            <Link 
                                to="/checkout" 
                                className="btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0 d-flex justify-content-center align-items-center gap-2" 
                                style={{ backgroundColor: '#ff6b00' }}
                            >
                                Avançar para Checkout ➔
                            </Link>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Cart;