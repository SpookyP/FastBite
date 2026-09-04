import React, { useState, useEffect } from 'react';

const Checkout = () => {
    // 1. ESTADO DO CARRINHO (Pronto para receber dados reais)
    // Quando a tua colega terminar o Context do carrinho, vamos substituir esta linha 
    // por algo como: const { cartItems } = useCart();
    const [cartItems, setCartItems] = useState([]);

    // 2. ESTADO DO FORMULÁRIO DE ENTREGA (Guarda o que o utilizador digita)
    const [formData, setFormData] = useState({
        nome: '',
        telefone: '',
        morada: '',
        codigoPostal: '',
        cidade: ''
    });

    // 3. ESTADOS DE PAGAMENTO
    const [metodoPagamento, setMetodoPagamento] = useState('mbway');
    const [dadosMbway, setDadosMbway] = useState('');
    const [dadosCartao, setDadosCartao] = useState({ numero: '', validade: '', cvv: '', nomeCartao: '' });

    // 4. CÁLCULOS AUTOMÁTICOS (Matemática real baseada nos itens)
    // O reduce soma o (preço * quantidade) de todos os itens na lista
    const subtotal = cartItems?.reduce((soma, item) => soma + (item.preco * item.quantidade), 0) || 0;
    const taxaEntrega = cartItems?.length > 0 ? 2.90 : 0; // Só cobra entrega se houver itens
    const total = subtotal + taxaEntrega;

    // 5. FUNÇÃO PARA ATUALIZAR O TEXTO DOS INPUTS
    const handleFormChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    // 6. FUNÇÃO PARA ENVIAR O PEDIDO (Onde faremos a chamada à API mais tarde)
    const handleFinalizarPedido = (e) => {
        e.preventDefault(); // Evita que a página recarregue ao clicar no botão

        if (cartItems.length === 0) {
            alert("O teu carrinho está vazio!");
            return;
        }

        const pedidoFinal = {
            cliente: formData,
            pagamento: {
                metodo: metodoPagamento,
                detalhesMbway: metodoPagamento === 'mbway' ? dadosMbway : null,
                detalhesCartao: metodoPagamento === 'cartao' ? dadosCartao : null
            },
            itens: cartItems,
            totalCobrado: total
        };

        console.log("PRONTO PARA ENVIAR PARA A API:", pedidoFinal);
        alert("Pedido gerado com sucesso! Vê a consola (F12) para veres o JSON.");
    };

    // Apenas para simular a chegada de dados ao entrares na página (podes apagar isto depois)
    useEffect(() => {
        setCartItems([
            { id: 1, nome: "Hambúrguer da Casa", preco: 8.50, quantidade: 2 },
            { id: 2, nome: "Batata Frita M", preco: 2.50, quantidade: 1 }
        ]);
    }, []);

    return (
        <div className="container py-5 min-vh-100">
            <h2 className="fw-bold mb-4">Checkout</h2>

            <div className="row g-4">
                {/* Lado Esquerdo: Formulário de Entrega e Pagamento */}
                <div className="col-lg-8">

                    {/* Bloco de Morada */}
                    <div className="card border-0 shadow-sm rounded-4 mb-4">
                        <div className="card-body p-4">
                            <h5 className="fw-bold mb-3">📦 Detalhes de Entrega</h5>
                            <form>
                                <div className="row g-3">
                                    <div className="col-md-6">
                                        <label className="form-label text-muted small fw-semibold">Nome Completo</label>
                                        <input type="text" className="form-control" name="nome" value={formData.nome} onChange={handleFormChange} placeholder="O teu nome..." />
                                    </div>
                                    <div className="col-md-6">
                                        <label className="form-label text-muted small fw-semibold">Contacto Telefónico</label>
                                        <input type="text" className="form-control" name="telefone" value={formData.telefone} onChange={handleFormChange} placeholder="+351 900 000 000" />
                                    </div>
                                    <div className="col-12">
                                        <label className="form-label text-muted small fw-semibold">Morada de Entrega</label>
                                        <input type="text" className="form-control" name="morada" value={formData.morada} onChange={handleFormChange} placeholder="Rua, Número, Andar..." />
                                    </div>
                                    <div className="col-md-4">
                                        <label className="form-label text-muted small fw-semibold">Código Postal</label>
                                        <input type="text" className="form-control" name="codigoPostal" value={formData.codigoPostal} onChange={handleFormChange} placeholder="0000-000" />
                                    </div>
                                    <div className="col-md-8">
                                        <label className="form-label text-muted small fw-semibold">Cidade</label>
                                        <input type="text" className="form-control" name="cidade" value={formData.cidade} onChange={handleFormChange} placeholder="Lisboa, Porto..." />
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>

                    {/* Bloco de Pagamento */}
                    <div className="card border-0 shadow-sm rounded-4">
                        <div className="card-body p-4">
                            <h5 className="fw-bold mb-4">💳 Método de Pagamento</h5>

                            {/* Opção MB WAY */}
                            <div className="mb-3">
                                <div className="form-check mb-2">
                                    <input className="form-check-input" type="radio" name="pagamento" id="mbway" checked={metodoPagamento === 'mbway'} onChange={() => setMetodoPagamento('mbway')} />
                                    <label className="form-check-label fw-semibold" htmlFor="mbway">MB Way</label>
                                </div>
                                {metodoPagamento === 'mbway' && (
                                    <div className="ms-4 ps-3 border-start border-2 mt-2" style={{ borderColor: '#ff6b00' }}>
                                        <label className="form-label text-muted small fw-semibold">Nº de Telemóvel associado</label>
                                        <input type="text" className="form-control form-control-sm w-50" value={dadosMbway} onChange={(e) => setDadosMbway(e.target.value)} placeholder="Ex: 912 345 678" />
                                    </div>
                                )}
                            </div>

                            {/* Opção CARTÃO DE CRÉDITO */}
                            <div className="mb-3">
                                <div className="form-check mb-2">
                                    <input className="form-check-input" type="radio" name="pagamento" id="cartao" checked={metodoPagamento === 'cartao'} onChange={() => setMetodoPagamento('cartao')} />
                                    <label className="form-check-label fw-semibold" htmlFor="cartao">Cartão de Crédito / Débito</label>
                                </div>
                                {metodoPagamento === 'cartao' && (
                                    <div className="ms-4 ps-3 border-start border-2 mt-2 row g-3" style={{ borderColor: '#ff6b00' }}>
                                        <div className="col-12">
                                            <label className="form-label text-muted small fw-semibold">Número do Cartão</label>
                                            <input type="text" className="form-control form-control-sm" value={dadosCartao.numero} onChange={(e) => setDadosCartao({ ...dadosCartao, numero: e.target.value })} placeholder="0000 0000 0000 0000" />
                                        </div>
                                        <div className="col-6">
                                            <label className="form-label text-muted small fw-semibold">Validade</label>
                                            <input type="text" className="form-control form-control-sm" value={dadosCartao.validade} onChange={(e) => setDadosCartao({ ...dadosCartao, validade: e.target.value })} placeholder="MM/AA" />
                                        </div>
                                        <div className="col-6">
                                            <label className="form-label text-muted small fw-semibold">CVV</label>
                                            <input type="text" className="form-control form-control-sm" value={dadosCartao.cvv} onChange={(e) => setDadosCartao({ ...dadosCartao, cvv: e.target.value })} placeholder="123" />
                                        </div>
                                        <div className="col-12">
                                            <label className="form-label text-muted small fw-semibold">Nome impresso no Cartão</label>
                                            <input type="text" className="form-control form-control-sm" value={dadosCartao.nomeCartao} onChange={(e) => setDadosCartao({ ...dadosCartao, nomeCartao: e.target.value })} placeholder="Nome Completo" />
                                        </div>
                                    </div>
                                )}
                            </div>

                            {/* Opção DINHEIRO */}
                            <div className="mb-2">
                                <div className="form-check">
                                    <input className="form-check-input" type="radio" name="pagamento" id="dinheiro" checked={metodoPagamento === 'dinheiro'} onChange={() => setMetodoPagamento('dinheiro')} />
                                    <label className="form-check-label fw-semibold" htmlFor="dinheiro">Pagamento na Entrega (Dinheiro)</label>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                {/* Lado Direito: Resumo da Encomenda */}
                <div className="col-lg-4">
                    <div className="card border-0 shadow-sm rounded-4 sticky-top" style={{ top: '20px' }}>
                        <div className="card-body p-4">
                            <h5 className="fw-bold mb-4">🛒 Resumo da Encomenda</h5>

                            {cartItems?.length === 0 ? (
                                <p className="text-muted text-center py-3">O teu carrinho está vazio.</p>
                            ) : (
                                <ul className="list-group list-group-flush mb-3">
                                    {cartItems?.map((item) => (
                                        <li key={item.id} className="list-group-item px-0 d-flex justify-content-between align-items-center border-0 pb-2">
                                            <div>
                                                <span className="fw-bold me-2">{item.quantidade}x</span>
                                                <span className="text-muted">{item.nome}</span>
                                            </div>
                                            <span className="fw-semibold">{(item.preco * item.quantidade).toFixed(2)}€</span>
                                        </li>
                                    ))}
                                </ul>
                            )}

                            <hr className="my-3" />

                            <div className="d-flex justify-content-between mb-2">
                                <span className="text-muted">Subtotal</span>
                                <span>{subtotal.toFixed(2)}€</span>
                            </div>
                            <div className="d-flex justify-content-between mb-3">
                                <span className="text-muted">Taxa de Entrega</span>
                                <span>{taxaEntrega.toFixed(2)}€</span>
                            </div>

                            <div className="d-flex justify-content-between align-items-center mb-4 pt-2 border-top">
                                <span className="fw-bold fs-5">Total</span>
                                <span className="fw-bold fs-4" style={{ color: '#ff6b00' }}>{total.toFixed(2)}€</span>
                            </div>

                            <button
                                onClick={handleFinalizarPedido}
                                disabled={cartItems?.length === 0}
                                className={`btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0 ${cartItems?.length === 0 ? 'opacity-50' : ''}`}
                                style={{ backgroundColor: '#ff6b00', fontSize: '1.1rem' }}>
                                Confirmar e Pagar
                            </button>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Checkout;