import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar/Navbar';
import { useCart } from '../context/CartContext';
import { orderService } from '../services/orderService';

const TAXA_ENTREGA = 2.50;

const Cart = () => {
    const { cart, removerDoCarrinho, diminuirQuantidade, adicionarAoCarrinho } = useCart();
    const navigate = useNavigate();

    const [preview, setPreview] = useState(null);
    const [loadingPreview, setLoadingPreview] = useState(false);
    const [previewError, setPreviewError] = useState(null);
    const [proceeding, setProceeding] = useState(false);

    // Subtotal local (mostrado enquanto o preview ainda não chegou)
    const subtotalLocal = cart.reduce((soma, item) => soma + (item.precoBase * item.quantidade), 0);

    // Sempre que o carrinho muda, recalcular o preview no backend (combos + totais fiéis)
    useEffect(() => {
        if (cart.length === 0) {
            setPreview(null);
            setPreviewError(null);
            return;
        }

        let cancelled = false;

        const fetchPreview = async () => {
            setLoadingPreview(true);
            setPreviewError(null);
            try {
                const result = await orderService.previewOrder(cart, TAXA_ENTREGA);
                if (!cancelled) setPreview(result);
            } catch (err) {
                if (!cancelled) {
                    setPreview(null);
                    setPreviewError(err.message || 'Não foi possível calcular o preview.');
                }
            } finally {
                if (!cancelled) setLoadingPreview(false);
            }
        };

        // debounce para não spamar a API a cada clique rápido nos +/-
        const t = setTimeout(fetchPreview, 300);
        return () => {
            cancelled = true;
            clearTimeout(t);
        };
    }, [cart]);

    const handleProceedToCheckout = async () => {
        setProceeding(true);
        setPreviewError(null);

        try {
            // Garante que temos o preview mais recente antes de avançar
            let currentPreview = preview;
            if (!currentPreview) {
                currentPreview = await orderService.previewOrder(cart, TAXA_ENTREGA);
                setPreview(currentPreview);
            }

            if (!currentPreview.podeFinalizar) {
                setPreviewError('Alguns itens já não estão disponíveis em stock.');
                return;
            }

            navigate('/checkout', { state: { orderData: currentPreview } });
        } catch (err) {
            setPreviewError(`Erro ao validar o carrinho: ${err.message}`);
        } finally {
            setProceeding(false);
        }
    };

    const totalDescontos = preview?.totalDescontos ?? 0;
    const subtotal = preview?.subtotal ?? subtotalLocal;
    const taxaEntrega = preview?.taxaEntrega ?? (cart.length > 0 ? TAXA_ENTREGA : 0);
    const total = preview?.total ?? (subtotal + taxaEntrega);

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

                {cart.length === 0 ? (
                    <div className="text-center py-5 bg-white rounded-4 shadow-sm border-0">
                        <h4 className="text-muted mb-3">O teu carrinho está vazio...</h4>
                        <Link to="/" className="btn text-white fw-bold px-4 py-2" style={{ backgroundColor: '#ff6b00' }}>
                            Ir para o Menu
                        </Link>
                    </div>
                ) : (
                    <div className="row g-4">
                        {/* COLUNA ESQUERDA: LISTA DE PRODUTOS EDITÁVEL */}
                        <div className="col-lg-8">
                            <div className="card border-0 shadow-sm rounded-4 overflow-hidden">
                                <div className="card-header bg-white border-bottom py-3">
                                    <h6 className="fw-bold m-0 text-muted">Itens Selecionados ({cart.length})</h6>
                                </div>
                                <div className="card-body p-0">
                                    <ul className="list-group list-group-flush">
                                        {cart.map((item) => (
                                            <li key={item.id || item.codigo} className="list-group-item d-flex justify-content-between align-items-center p-4">
                                                <div className="d-flex flex-column">
                                                    <h5 className="fw-bold mb-1">{item.nome}</h5>
                                                    <small className="text-muted mb-2">{item.descricao}</small>
                                                    <div className="d-flex align-items-center gap-2 mt-2">
                                                        <button className="btn btn-sm btn-outline-secondary rounded-circle d-flex justify-content-center align-items-center"
                                                            style={{ width: '30px', height: '30px' }}
                                                            onClick={() => diminuirQuantidade(item.id)}>
                                                            <b>-</b>
                                                        </button>
                                                        <span className="fw-bold px-2">{item.quantidade}</span>
                                                        <button className="btn btn-sm btn-outline-secondary rounded-circle d-flex justify-content-center align-items-center"
                                                            style={{ width: '30px', height: '30px' }}
                                                            onClick={() => adicionarAoCarrinho(item)}>
                                                            <b>+</b>
                                                        </button>
                                                    </div>
                                                </div>
                                                <div className="d-flex flex-column align-items-end gap-2">
                                                    <h5 className="fw-bold m-0" style={{ color: '#ff6b00' }}>
                                                        {(item.precoBase * item.quantidade).toFixed(2)} €
                                                    </h5>
                                                    <button className="btn btn-sm btn-outline-danger border-0"
                                                        onClick={() => removerDoCarrinho(item.id || item.codigo)}>
                                                        🗑️ Remover
                                                    </button>
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            </div>

                            {/* PREVIEW DE COMBOS APLICADOS PELO BACKEND */}
                            <div className="card border-0 shadow-sm rounded-4 mt-4 p-4">
                                <div className="d-flex justify-content-between align-items-center mb-3">
                                    <h5 className="fw-bold m-0">Combos & Descontos</h5>
                                    {loadingPreview && (
                                        <span className="spinner-border spinner-border-sm text-muted" role="status"></span>
                                    )}
                                </div>

                                {previewError && (
                                    <div className="alert alert-danger p-2 small mb-3">{previewError}</div>
                                )}

                                {!preview && !loadingPreview && !previewError && (
                                    <p className="text-muted small mb-0">A calcular os melhores combos...</p>
                                )}

                                {preview && (
                                    <>
                                        {preview.combos?.length > 0 && (
                                            <ul className="list-group list-group-flush mb-2">
                                                {preview.combos.map((c, idx) => (
                                                    <li key={`combo-${idx}`} className="list-group-item px-0 d-flex justify-content-between align-items-center border-0 pb-2">
                                                        <div>
                                                            <span className="fw-bold me-2">{c.quantity}x</span>
                                                            <span className="text-muted">{c.descricao}</span>
                                                            <span className="badge bg-success ms-2">Combo -10%</span>
                                                        </div>
                                                        <span className="fw-semibold">€{Number(c.totalLinha).toFixed(2)}</span>
                                                    </li>
                                                ))}
                                            </ul>
                                        )}

                                        {preview.avulsos?.length > 0 && (
                                            <ul className="list-group list-group-flush">
                                                {preview.avulsos.map((a, idx) => (
                                                    <li key={`avulso-${idx}`} className="list-group-item px-0 d-flex justify-content-between align-items-center border-0 pb-2">
                                                        <div>
                                                            <span className="fw-bold me-2">{a.quantity}x</span>
                                                            <span className="text-muted">{a.nome}</span>
                                                        </div>
                                                        <span className="fw-semibold">€{Number(a.totalLinha).toFixed(2)}</span>
                                                    </li>
                                                ))}
                                            </ul>
                                        )}

                                        {totalDescontos > 0 && (
                                            <div className="alert alert-success mt-3 mb-0 p-2 small">
                                                🎉 Poupaste <strong>€{Number(totalDescontos).toFixed(2)}</strong> com combos!
                                            </div>
                                        )}

                                        {preview.indisponiveis?.length > 0 && (
                                            <div className="alert alert-warning mt-3 mb-0 p-2 small">
                                                {preview.indisponiveis.map((i, idx) => (
                                                    <div key={idx}>⚠️ {i.nome} — sem stock suficiente.</div>
                                                ))}
                                            </div>
                                        )}
                                    </>
                                )}
                            </div>
                        </div>

                        {/* COLUNA DIREITA: RESUMO E BOTÃO DE CHECKOUT */}
                        <div className="col-lg-4">
                            <div className="card border-0 shadow-sm rounded-4 p-4 sticky-top" style={{ top: '20px' }}>
                                <h5 className="fw-bold mb-4">Resumo</h5>

                                <div className="d-flex justify-content-between mb-2">
                                    <span className="text-muted">Subtotal</span>
                                    <span>€{Number(subtotal).toFixed(2)}</span>
                                </div>

                                {totalDescontos > 0 && (
                                    <div className="d-flex justify-content-between mb-2 text-success">
                                        <span>Descontos</span>
                                        <span>-€{Number(totalDescontos).toFixed(2)}</span>
                                    </div>
                                )}

                                <div className="d-flex justify-content-between mb-3 text-muted">
                                    <span>Taxa de Entrega</span>
                                    <span>€{Number(taxaEntrega).toFixed(2)}</span>
                                </div>

                                <hr className="text-muted" />

                                <div className="d-flex justify-content-between align-items-center mb-4 mt-2">
                                    <h5 className="m-0 fw-bold">Total</h5>
                                    <h4 className="m-0 fw-bold" style={{ color: '#ff6b00' }}>
                                        €{Number(total).toFixed(2)}
                                    </h4>
                                </div>

                                {previewError && (
                                    <div className="alert alert-danger p-2 small text-center mb-3" role="alert">
                                        {previewError}
                                    </div>
                                )}

                                <button
                                    onClick={handleProceedToCheckout}
                                    disabled={proceeding || loadingPreview}
                                    className="btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0 d-flex justify-content-center align-items-center gap-2"
                                    style={{
                                        backgroundColor: '#ff6b00',
                                        opacity: (proceeding || loadingPreview) ? 0.8 : 1,
                                        cursor: (proceeding || loadingPreview) ? 'not-allowed' : 'pointer'
                                    }}
                                >
                                    {proceeding ? (
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