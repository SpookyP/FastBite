import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { menuService } from '../../services/menuService';

const ListItems = () => {
    const navigate = useNavigate();
    const [menus, setMenus] = useState([]);
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState(null);

    useEffect(() => {
        carregarMenus();
    }, []);

    const carregarMenus = async () => {
        try {
            setLoading(true);
            const dados = await menuService.obterTodos();
            setMenus(dados);
        } catch (err) {
            console.error(err);
            setErro('Erro ao carregar os itens do menu da API.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-vh-100 bg-light">
            <Navbar />

            <div className="container py-5">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h2>Painel de Controlo - Gestão de Menus</h2>
                    <div className="d-flex gap-2">
                        {/* 3. Adicionar o onClick com o navigate */}
                        <button 
                            className="btn btn-success" 
                            onClick={() => navigate('/admin/menus/create')}
                        >
                            + Criar Novo Item
                        </button>
                    </div>
                </div>

                {erro && <div className="alert alert-danger">{erro}</div>}

                <div className="card shadow-sm p-4">
                    <h5 className="mb-4">Itens Registados no Catálogo</h5>
                    
                    {loading ? (
                        <div className="text-center py-4">
                            <div className="spinner-border text-primary" role="status">
                                <span className="visually-hidden">A carregar...</span>
                            </div>
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-striped table-bordered align-middle">
                                <thead className="table-dark">
                                    <tr>
                                        <th scope="col">ID</th>
                                        <th scope="col">Nome</th>
                                        <th scope="col">Categoria</th>
                                        <th scope="col">Preço Base</th>
                                        <th scope="col">Limite Diário</th>
                                        <th scope="col" className="text-center">Ações</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {menus && menus.length > 0 ? (
                                        menus.map((item) => (
                                            <tr key={item.id || item.Id}>
                                                <th scope="row">{item.id || item.Id}</th>
                                                <td>{item.nome || item.Nome}</td>
                                                <td>
                                                    <span className="badge bg-secondary">
                                                        {item.categoria || item.Categoria}
                                                    </span>
                                                </td>
                                                <td>{Number(item.precoBase || item.PrecoBase).toFixed(2)} €</td>
                                                <td>{item.limiteDiario || item.LimiteDiario}</td>
                                                <td>
                                                    <div className="d-flex justify-content-center gap-2">
                                                        <button type="button" className="btn btn-sm btn-success">Detalhes</button>
                                                            <Link to="/items/edit" state={{ id: item.id }} className="btn btn-primary">
                                                                Editar
                                                            </Link>
                                                        <button type="button" className="btn btn-sm btn-danger">Apagar</button>
                                                    </div>
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan="6" className="text-center text-muted py-3">
                                                Nenhum item encontrado na base de dados.
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ListItems;