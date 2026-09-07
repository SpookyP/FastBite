import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { menuService } from '../../services/menuService';

const CreateItem = () => {
    const navigate = useNavigate();
    
    const [formData, setFormData] = useState({
        nome: '',
        descricao: '',
        categoria: '',
        alergenios: '',
        precoBase: '',
        limiteDiario: ''
    });

    const [loading, setLoading] = useState(false);
    const [erroGlobal, setErroGlobal] = useState(null); // Erros de API
    const [erros, setErros] = useState({}); // Erros de validação dos campos

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
        // Limpa o erro do campo assim que o utilizador começa a escrever
        setErros({ ...erros, [name]: null });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        // 1. Validação Manual
        const novosErros = {};
        
        if (!formData.nome.trim()) novosErros.nome = 'Por favor, preenche este campo.';
        if (!formData.descricao.trim()) novosErros.descricao = 'Por favor, preenche este campo.';
        if (!formData.categoria) novosErros.categoria = 'Por favor, seleciona uma categoria.';
        if (!formData.precoBase || formData.precoBase <= 0 || formData.precoBase > 1000) novosErros.precoBase = 'Insere um preço válido.';
        if (!formData.limiteDiario || formData.limiteDiario <= 0 || formData.limiteDiario > 100) novosErros.limiteDiario = 'Insere um limite válido.';

        // Se houver erros, guardamos no estado e paramos a execução
        if (Object.keys(novosErros).length > 0) {
            setErros(novosErros);
            return;
        }

        // 2. Se passar a validação, avança para a API
        setLoading(true);
        setErroGlobal(null);

        try {
            const preco = parseFloat(formData.precoBase) || 0;
            const limite = parseInt(formData.limiteDiario, 10) || 0;

            const payload = {
                nome: formData.nome,
                Nome: formData.nome,
                descricao: formData.descricao,
                Descricao: formData.descricao,
                categoria: formData.categoria,
                Categoria: formData.categoria,
                alergenios: formData.alergenios || "",
                Alergenios: formData.alergenios || "",
                precoBase: preco,
                PrecoBase: preco,
                limiteDiario: limite,
                LimiteDiario: limite
            };

            await menuService.create(payload);
            navigate('/items');
        } catch (err) {
            console.error(err);
            setErroGlobal('Erro ao criar o item do menu. Verifica os dados ou permissões.');
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
                            <h2 className="mb-4">Adicionar Novo Item ao Catálogo</h2>

                            {/* Alerta para erros de servidor/API */}
                            {erroGlobal && <div className="alert alert-danger">{erroGlobal}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label className="form-label fw-bold">Nome</label>
                                    <input 
                                        type="text" 
                                        className={`form-control ${erros.nome ? 'is-invalid' : ''}`}
                                        name="nome" 
                                        value={formData.nome} 
                                        onChange={handleChange} 
                                    />
                                    {erros.nome && <small className="text-danger">{erros.nome}</small>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Descrição</label>
                                    <textarea 
                                        className={`form-control ${erros.descricao ? 'is-invalid' : ''}`}
                                        name="descricao" 
                                        rows="3"
                                        value={formData.descricao} 
                                        onChange={handleChange}
                                    />
                                    {erros.descricao && <small className="text-danger">{erros.descricao}</small>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Categoria</label>
                                    <select
                                        className={`form-select ${erros.categoria ? 'is-invalid' : ''}`}
                                        name="categoria"
                                        value={formData.categoria}
                                        onChange={handleChange}
                                    >
                                        <option value="">Select one</option>
                                        <option value="Prato">Prato</option>
                                        <option value="Bebida">Bebida</option>
                                        <option value="Acompanhamento">Acompanhamento</option>
                                    </select>
                                    {erros.categoria && <small className="text-danger">{erros.categoria}</small>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Alergénios</label>
                                    <input 
                                        type="text" 
                                        className="form-control" 
                                        name="alergenios" 
                                        value={formData.alergenios} 
                                        onChange={handleChange}
                                        placeholder="Ex: Glúten,Lactose"
                                    />
                                </div>

                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Preço Base (€)</label>
                                        <input 
                                            type="number" 
                                            step="0.01"
                                            min="0.01"
                                            max="1000"
                                            className={`form-control ${erros.precoBase ? 'is-invalid' : ''}`}
                                            name="precoBase" 
                                            value={formData.precoBase} 
                                            onChange={handleChange}
                                        />
                                        {erros.precoBase && <small className="text-danger">{erros.precoBase}</small>}
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Limite Diário</label>
                                        <input 
                                            type="number" 
                                            className={`form-control ${erros.limiteDiario ? 'is-invalid' : ''}`}
                                            name="limiteDiario" 
                                            min="1"
                                            max="100"
                                            value={formData.limiteDiario} 
                                            onChange={handleChange}
                                        />
                                        {erros.limiteDiario && <small className="text-danger">{erros.limiteDiario}</small>}
                                    </div>
                                </div>

                                <div className="d-flex justify-content-between mt-4">
                                    <button 
                                        type="button" 
                                        className="btn btn-secondary px-4" 
                                        onClick={() => navigate('/items')}
                                    >
                                        Cancelar
                                    </button>
                                    <button 
                                        type="submit" 
                                        className="btn btn-success px-4" 
                                        disabled={loading}
                                    >
                                        {loading ? 'A guardar...' : 'Criar Item'}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CreateItem;