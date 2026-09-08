import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { menuService } from '../../services/menuService';

const ShowItem = () => {
    const navigate = useNavigate();
    const location = useLocation();
    
    // Recupera o ID enviado pelo componente anterior (ListItems)
    const id = location.state?.id;

    // Estado para guardar os dados recebidos da API
    const [item, setItem] = useState(null);
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState(null);

    useEffect(() => {
        if (!id) {
            setErro('Nenhum item selecionado. Regressa à lista.');
            setLoading(false);
            return;
        }

        carregarItemDetalhes(id);
    }, [id]);

    const carregarItemDetalhes = async (itemId) => {
        try {
            setLoading(true);
            const dados = await menuService.obterPorId(itemId);
            setItem(dados);
        } catch (err) {
            console.error(err);
            setErro('Erro ao carregar os detalhes do item. O item pode já não existir.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-vh-100 bg-light">
            <Navbar />

            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-8">
                        <div className="card shadow-sm p-4">
                            <h2 className="mb-4">Detalhes do Item</h2>

                            {erro && <div className="alert alert-danger">{erro}</div>}

                            {loading ? (
                                <div className="text-center py-4">
                                    <div className="spinner-border text-primary" role="status">
                                        <span className="visually-hidden">A carregar...</span>
                                    </div>
                                </div>
                            ) : item ? (
                                <div>
                                    {/* Apresentação dos dados em formato de grelha de leitura */}
                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">ID</div>
                                        <div className="col-sm-8">{item.id || item.Id}</div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Nome</div>
                                        <div className="col-sm-8">{item.nome || item.Nome}</div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Descrição</div>
                                        <div className="col-sm-8">{item.descricao || item.Descricao}</div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Categoria</div>
                                        <div className="col-sm-8">
                                            <span className="badge bg-secondary">
                                                {item.categoria || item.Categoria}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Alergénios</div>
                                        <div className="col-sm-8">
                                            {item.alergenios || item.Alergenios ? (
                                                <div className="d-flex flex-wrap gap-1">
                                                    {(item.alergenios || item.Alergenios)
                                                        .split(/[\s,]+/) // Separa por vírgulas ou espaços
                                                        .filter(Boolean) // Remove vazios
                                                        .map((tag, index) => (
                                                            <span key={index} className="badge bg-warning text-dark px-2 py-1">
                                                                {tag}
                                                            </span>
                                                        ))}
                                                </div>
                                            ) : (
                                                <span className="text-muted">Nenhum alergénio registado</span>
                                            )}
                                        </div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Preço Base</div>
                                        <div className="col-sm-8">{Number(item.precoBase || item.PrecoBase).toFixed(2)} €</div>
                                    </div>

                                    <div className="row mb-3 border-bottom pb-2">
                                        <div className="col-sm-4 fw-bold text-muted">Limite Diário</div>
                                        <div className="col-sm-8">{item.limiteDiario || item.LimiteDiario} unidades</div>
                                    </div>

                                    <div className="row mb-4">
                                        <div className="col-sm-4 fw-bold text-muted">Vendidos Hoje</div>
                                        <div className="col-sm-8">
                                            {item.quantidadeVendidaHoje ?? item.QuantidadeVendidaHoje ?? 0} unidades
                                        </div>
                                    </div>

                                    <div className="d-flex mt-4">
                                        <button 
                                            type="button" 
                                            className="btn btn-secondary px-4 me-2" 
                                            onClick={() => navigate('/items')} 
                                        >
                                            Voltar à Lista
                                        </button>
                                        <button 
                                            type="button" 
                                            className="btn btn-primary px-4" 
                                            onClick={() => navigate('/items/edit', { state: { id: item.id || item.Id } })}
                                        >
                                            Editar Item
                                        </button>
                                    </div>
                                </div>
                            ) : null}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ShowItem;