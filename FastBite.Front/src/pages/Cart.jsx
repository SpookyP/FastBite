import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar/Navbar';
import { useCart } from '../context/CartContext'; 

const Cart = () => {
    const { cart, removerDoCarrinho, diminuirQuantidade, adicionarAoCarrinho } = useCart();
    const navigate = useNavigate();

    // Estados para gerir a validação com o backend
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    // Calcular o Subtotal
    const subtotal = cart.reduce((soma, item) => soma + (item.precoBase * item.quantidade), 0);

    // Função de validação no servidor antes de ir para o Checkout
    const handleProceedToCheckout = async () => {
        setLoading(true);
        setError(null);

        try {
            // Chamada à API .NET para pré-validação do pedido e cálculo fidedigno
            const response = await fetch('http://localhost:5000/api/orders/preview', {
                method: 'POST',
                headers: { 
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${token}` // Descomentar caso a tua API requeira JWT
                },
                body: JSON.stringify({ items: cart })
            });

            if (!response.ok) {
                throw new Error('Falha ao validar os dados no servidor.');
            }

            const validatedOrder = await response.json();

            // Navegação para o Checkout com os dados validados injetados no estado do router
            navigate('/checkout', { state: { orderData: validatedOrder } });
        } catch (err) {
            setError('Não foi possível validar o carrinho. Tenta novamente.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-vh-100 bg-light">
            <Navbar />
            <div className="container py-5">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h2 className="fw-bold m-0">O teu Carrinho 🛒</h2>
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
                                        {cart.map((item) => (
                                            <li key={item.id || item.codigo} className="list-group-item d-flex justify-content-between align-items-center p-4">
                                                
                                                {/* Info do Produto */}
                                                <div className="d-flex flex-column">
                                                    <h5 className="fw-bold mb-1">{item.nome}</h5>
                                                    <small className="text-muted mb-2">{item.descricao}</small>
                                                    {/* Controlador de Quantidade */}
                                                    <div className="d-flex align-items-center gap-2 mt-2">
                                                        <button 
                                                            className="btn btn-sm btn-outline-secondary rounded-circle d-flex justify-content-center align-items-center"
                                                            style={{ width: '30px', height: '30px' }}
                                                            onClick={() => diminuirQuantidade(item.id)}
                                                        >
                                                            <b>-</b>
                                                        </button>
                                                        
                                                        <span className="fw-bold px-2">{item.quantidade}</span>
                                                        
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

                                {error && (
                                    <div className="alert alert-danger p-2 small text-center mb-3" role="alert">
                                        {error}
                                    </div>
                                )}

                                {/* Botão de acção com feedback de carregamento mantendo o design Bootstrap */}
                                <button 
                                    onClick={handleProceedToCheckout}
                                    disabled={loading}
                                    className="btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0 d-flex justify-content-center align-items-center gap-2" 
                                    style={{ 
                                        backgroundColor: '#ff6b00', 
                                        opacity: loading ? 0.8 : 1,
                                        cursor: loading ? 'not-allowed' : 'pointer' 
                                    }}
                                >
                                    {loading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                                            <span>A validar...</span>
                                        </>
                                    ) : (
                                        <>Avançar para Checkout ➔</>
                                    )}
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Cart;