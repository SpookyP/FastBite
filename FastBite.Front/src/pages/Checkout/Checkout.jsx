import React, { useState } from 'react';
import { useCart } from '../../context/CartContext.jsx';
import { useNavigate } from 'react-router-dom'; // 1. Importamos a ferramenta de navegação

const Checkout = () => {
    const { cart, limparCarrinho } = useCart();
    const navigate = useNavigate(); // 2. Inicializamos o navegador

    const [formData, setFormData] = useState({
        nome: '',
        telefone: '',
        morada: '',
        codigoPostal: '',
        cidade: ''
    });

    const [metodoPagamento, setMetodoPagamento] = useState('mbway');
    const [dadosMbway, setDadosMbway] = useState('');
    const [dadosCartao, setDadosCartao] = useState({ numero: '', validade: '', cvv: '', nomeCartao: '' });
    const [erros, setErros] = useState({});

    // 3. NOVO ESTADO: Controla se o nosso pop-up bonito está visível ou escondido
    const [mostrarPopup, setMostrarPopup] = useState(false);

    const subtotal = cart?.reduce((soma, item) => soma + (item.precoBase * item.quantidade), 0) || 0;
    const taxaEntrega = cart?.length > 0 ? 2.90 : 0;
    const total = subtotal + taxaEntrega;

    const handleFormChange = (e) => {
        const { name, value } = e.target;
        let newValue = value;

        if (name === 'nome' || name === 'cidade') {
            newValue = value.replace(/[^a-zA-ZÀ-ÿ\s]/g, '');
        } else if (name === 'telefone') {
            newValue = value.replace(/[^0-9+\s]/g, '');
        } else if (name === 'codigoPostal') {
            let nums = value.replace(/\D/g, '');
            nums = nums.substring(0, 7);
            newValue = nums.length > 4 ? `${nums.substring(0, 4)}-${nums.substring(4)}` : nums;
        }

        setFormData({ ...formData, [name]: newValue });
        setErros({ ...erros, [name]: null });
    };

    const handleMbwayChange = (e) => {
        const value = e.target.value.replace(/\D/g, '').substring(0, 9);
        setDadosMbway(value);
        setErros({ ...erros, mbway: null });
    };

    const handleCartaoChange = (e) => {
        const { name, value } = e.target;
        let newValue = value;

        if (name === 'numero') {
            newValue = value.replace(/\D/g, '').substring(0, 16);
        } else if (name === 'cvv') {
            newValue = value.replace(/\D/g, '').substring(0, 4);
        } else if (name === 'nomeCartao') {
            newValue = value.replace(/[^a-zA-ZÀ-ÿ\s]/g, '');
        } else if (name === 'validade') {
            let nums = value.replace(/\D/g, '').substring(0, 4);
            newValue = nums.length > 2 ? `${nums.substring(0, 2)}/${nums.substring(2)}` : nums;
        }

        setDadosCartao({ ...dadosCartao, [name]: newValue });
        const errorKey = name === 'numero' ? 'cartaoNumero' : name === 'validade' ? 'cartaoValidade' : name === 'cvv' ? 'cartaoCvv' : 'cartaoNome';
        setErros({ ...erros, [errorKey]: null });
    };

    const handleFinalizarPedido = (e) => {
        e.preventDefault();

        if (cart?.length === 0) {
            alert("O teu carrinho está vazio!");
            return;
        }

        const novosErros = {};

        if (!formData.nome) novosErros.nome = 'Campo obrigatório';
        if (!formData.telefone) novosErros.telefone = 'Campo obrigatório';
        if (!formData.morada) novosErros.morada = 'Campo obrigatório';
        if (!formData.cidade) novosErros.cidade = 'Campo obrigatório';

        if (!formData.codigoPostal) {
            novosErros.codigoPostal = 'Campo obrigatório';
        } else if (formData.codigoPostal.length !== 8) {
            novosErros.codigoPostal = 'Formato inválido (ex: 1000-123)';
        }

        if (metodoPagamento === 'mbway') {
            if (!dadosMbway) novosErros.mbway = 'Campo obrigatório';
            else if (dadosMbway.length < 9) novosErros.mbway = 'Número incompleto';
        }

        if (metodoPagamento === 'cartao') {
            if (!dadosCartao.numero) novosErros.cartaoNumero = 'Campo obrigatório';
            else if (dadosCartao.numero.length < 16) novosErros.cartaoNumero = 'Faltam dígitos';

            if (!dadosCartao.validade) novosErros.cartaoValidade = 'Campo obrigatório';
            else if (dadosCartao.validade.length !== 5) novosErros.cartaoValidade = 'Formato inválido';

            if (!dadosCartao.cvv) novosErros.cartaoCvv = 'Campo obrigatório';
            if (!dadosCartao.nomeCartao) novosErros.cartaoNome = 'Campo obrigatório';
        }

        if (Object.keys(novosErros).length > 0) {
            setErros(novosErros);
            return;
        }

        const pedidoFinal = {
            cliente: formData,
            pagamento: {
                metodo: metodoPagamento,
                detalhesMbway: metodoPagamento === 'mbway' ? dadosMbway : null,
                detalhesCartao: metodoPagamento === 'cartao' ? dadosCartao : null
            },
            itens: cart,
            totalCobrado: total
        };

        console.log("PRONTO PARA ENVIAR PARA A API:", pedidoFinal);

        // 4. Em vez do alert() feio, ativamos o nosso pop-up!
        setMostrarPopup(true);
    };

    // 5. Função acionada quando o utilizador clica no botão "OK" do nosso novo pop-up
    const fecharPopupERedirecionar = () => {
        limparCarrinho(); // Limpa o carrinho global
        setMostrarPopup(false); // Esconde o pop-up
        navigate('/'); // Redireciona para a página inicial (Home)
    };

    return (
        <div className="container py-5 min-vh-100 position-relative">
            <h2 className="fw-bold mb-4">Finalizar Pedido</h2>

            <div className="row g-4">
                {/* Lado Esquerdo: Formulário de Entrega e Pagamento */}
                <div className="col-lg-8">

                    <div className="card border-0 shadow-sm rounded-4 mb-4">
                        <div className="card-body p-4">
                            <h5 className="fw-bold mb-3">📍 Detalhes de Entrega</h5>
                            <form>
                                <div className="row g-3">
                                    <div className="col-md-6">
                                        <label className="form-label text-muted small fw-semibold">Nome Completo</label>
                                        <input type="text" className={`form-control ${erros.nome ? 'is-invalid' : ''}`} name="nome" value={formData.nome} onChange={handleFormChange} placeholder="Ex: Ana Silva" />
                                        {erros.nome && <small className="text-danger">{erros.nome}</small>}
                                    </div>
                                    <div className="col-md-6">
                                        <label className="form-label text-muted small fw-semibold">Contacto Telefónico</label>
                                        <input type="text" className={`form-control ${erros.telefone ? 'is-invalid' : ''}`} name="telefone" value={formData.telefone} onChange={handleFormChange} placeholder="+351 900 000 000" />
                                        {erros.telefone && <small className="text-danger">{erros.telefone}</small>}
                                    </div>
                                    <div className="col-12">
                                        <label className="form-label text-muted small fw-semibold">Morada de Entrega</label>
                                        <input type="text" className={`form-control ${erros.morada ? 'is-invalid' : ''}`} name="morada" value={formData.morada} onChange={handleFormChange} placeholder="Rua, Número, Andar..." />
                                        {erros.morada && <small className="text-danger">{erros.morada}</small>}
                                    </div>
                                    <div className="col-md-4">
                                        <label className="form-label text-muted small fw-semibold">Código Postal</label>
                                        <input type="text" className={`form-control ${erros.codigoPostal ? 'is-invalid' : ''}`} name="codigoPostal" value={formData.codigoPostal} onChange={handleFormChange} placeholder="0000-000" />
                                        {erros.codigoPostal && <small className="text-danger">{erros.codigoPostal}</small>}
                                    </div>
                                    <div className="col-md-8">
                                        <label className="form-label text-muted small fw-semibold">Cidade</label>
                                        <input type="text" className={`form-control ${erros.cidade ? 'is-invalid' : ''}`} name="cidade" value={formData.cidade} onChange={handleFormChange} placeholder="Ex: Lisboa" />
                                        {erros.cidade && <small className="text-danger">{erros.cidade}</small>}
                                    </div>
                                </div>
                            </form>
                        </div>
                    </div>

                    <div className="card border-0 shadow-sm rounded-4">
                        <div className="card-body p-4">
                            <h5 className="fw-bold mb-4">💳 Método de Pagamento</h5>

                            <div className="mb-3">
                                <div className="form-check mb-2">
                                    <input className="form-check-input" type="radio" name="pagamento" id="mbway" checked={metodoPagamento === 'mbway'} onChange={() => { setMetodoPagamento('mbway'); setErros({}); }} />
                                    <label className="form-check-label fw-semibold" htmlFor="mbway">MB Way</label>
                                </div>
                                {metodoPagamento === 'mbway' && (
                                    <div className="ms-4 ps-3 border-start border-2 mt-2" style={{ borderColor: '#ff6b00' }}>
                                        <label className="form-label text-muted small fw-semibold">Nº de Telemóvel associado</label>
                                        <input type="text" className={`form-control form-control-sm w-50 ${erros.mbway ? 'is-invalid' : ''}`} value={dadosMbway} onChange={handleMbwayChange} placeholder="Ex: 912 345 678" />
                                        {erros.mbway && <small className="text-danger">{erros.mbway}</small>}
                                    </div>
                                )}
                            </div>

                            <div className="mb-3">
                                <div className="form-check mb-2">
                                    <input className="form-check-input" type="radio" name="pagamento" id="cartao" checked={metodoPagamento === 'cartao'} onChange={() => { setMetodoPagamento('cartao'); setErros({}); }} />
                                    <label className="form-check-label fw-semibold" htmlFor="cartao">Cartão de Crédito / Débito</label>
                                </div>
                                {metodoPagamento === 'cartao' && (
                                    <div className="ms-4 ps-3 border-start border-2 mt-2 row g-3" style={{ borderColor: '#ff6b00' }}>
                                        <div className="col-12">
                                            <label className="form-label text-muted small fw-semibold">Número do Cartão</label>
                                            <input type="text" className={`form-control form-control-sm ${erros.cartaoNumero ? 'is-invalid' : ''}`} name="numero" value={dadosCartao.numero} onChange={handleCartaoChange} placeholder="0000 0000 0000 0000" />
                                            {erros.cartaoNumero && <small className="text-danger">{erros.cartaoNumero}</small>}
                                        </div>
                                        <div className="col-6">
                                            <label className="form-label text-muted small fw-semibold">Validade</label>
                                            <input type="text" className={`form-control form-control-sm ${erros.cartaoValidade ? 'is-invalid' : ''}`} name="validade" value={dadosCartao.validade} onChange={handleCartaoChange} placeholder="MM/AA" />
                                            {erros.cartaoValidade && <small className="text-danger">{erros.cartaoValidade}</small>}
                                        </div>
                                        <div className="col-6">
                                            <label className="form-label text-muted small fw-semibold">CVV</label>
                                            <input type="text" className={`form-control form-control-sm ${erros.cartaoCvv ? 'is-invalid' : ''}`} name="cvv" value={dadosCartao.cvv} onChange={handleCartaoChange} placeholder="123" />
                                            {erros.cartaoCvv && <small className="text-danger">{erros.cartaoCvv}</small>}
                                        </div>
                                        <div className="col-12">
                                            <label className="form-label text-muted small fw-semibold">Nome impresso no Cartão</label>
                                            <input type="text" className={`form-control form-control-sm ${erros.cartaoNome ? 'is-invalid' : ''}`} name="nomeCartao" value={dadosCartao.nomeCartao} onChange={handleCartaoChange} placeholder="Nome Completo" />
                                            {erros.cartaoNome && <small className="text-danger">{erros.cartaoNome}</small>}
                                        </div>
                                    </div>
                                )}
                            </div>

                            <div className="mb-2">
                                <div className="form-check">
                                    <input className="form-check-input" type="radio" name="pagamento" id="dinheiro" checked={metodoPagamento === 'dinheiro'} onChange={() => { setMetodoPagamento('dinheiro'); setErros({}); }} />
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

                            {cart?.length === 0 ? (
                                <p className="text-muted text-center py-3">O teu carrinho está vazio.</p>
                            ) : (
                                <ul className="list-group list-group-flush mb-3">
                                    {cart?.map((item) => (
                                        <li key={item.id} className="list-group-item px-0 d-flex justify-content-between align-items-center border-0 pb-2">
                                            <div>
                                                <span className="fw-bold me-2">{item.quantidade}x</span>
                                                <span className="text-muted">{item.nome}</span>
                                            </div>
                                            <span className="fw-semibold">{(item.precoBase * item.quantidade).toFixed(2)}€</span>
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
                                disabled={cart?.length === 0}
                                className={`btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0 ${cart?.length === 0 ? 'opacity-50' : ''}`}
                                style={{ backgroundColor: '#ff6b00', fontSize: '1.1rem' }}>
                                Confirmar e Pagar
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* 6. POP-UP DE SUCESSO (Renderizado por cima de tudo usando Bootstrap) */}
            {mostrarPopup && (
                <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.6)', zIndex: 1050 }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content rounded-4 border-0 shadow-lg text-center p-4">
                            <div className="modal-body">
                                <div className="mb-4">
                                    <span style={{ fontSize: '4rem' }}>✅</span>
                                </div>
                                <h4 className="fw-bold mb-3">Pedido Confirmado!</h4>
                                <p className="text-muted mb-4">A tua encomenda foi registada com sucesso e está a ser preparada. O teu carrinho foi limpo.</p>

                                <button
                                    className="btn w-100 text-white fw-bold rounded-3 py-3 shadow-sm border-0"
                                    style={{ backgroundColor: '#ff6b00' }}
                                    onClick={fecharPopupERedirecionar}
                                >
                                    Voltar à Página Principal
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Checkout;