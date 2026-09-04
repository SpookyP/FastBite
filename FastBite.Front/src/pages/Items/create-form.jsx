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
    const [erro, setErro] = useState(null);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setErro(null);

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
            
            // Redireciona de volta para o dashboard de admin após sucesso
            navigate('/items');
        } catch (err) {
            console.error(err);
            setErro('Erro ao criar o item do menu. Verifica os dados ou permissões.');
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

                            {erro && <div className="alert alert-danger">{erro}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label className="form-label fw-bold">Nome</label>
                                    <input 
                                        type="text" 
                                        className="form-control" 
                                        name="nome" 
                                        value={formData.nome} 
                                        onChange={handleChange} 
                                        required 
                                    />
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Descrição</label>
                                    <textarea 
                                        className="form-control" 
                                        name="descricao" 
                                        rows="3"
                                        value={formData.descricao} 
                                        onChange={handleChange} 
                                        required 
                                    />
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Categoria</label>
                                    <input 
                                        type="text" 
                                        className="form-control" 
                                        name="categoria" 
                                        value={formData.categoria} 
                                        onChange={handleChange} 
                                        required 
                                    />
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Alergénios</label>
                                    <input 
                                        type="text" 
                                        className="form-control" 
                                        name="alergenios" 
                                        value={formData.alergenios} 
                                        onChange={handleChange} 
                                        placeholder="Ex: Glúten, Lactose"
                                    />
                                </div>

                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Preço Base (€)</label>
                                        <input 
                                            type="number" 
                                            step="0.01"
                                            className="form-control" 
                                            name="precoBase" 
                                            value={formData.precoBase} 
                                            onChange={handleChange} 
                                            required 
                                        />
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Limite Diário</label>
                                        <input 
                                            type="number" 
                                            className="form-control" 
                                            name="limiteDiario" 
                                            value={formData.limiteDiario} 
                                            onChange={handleChange} 
                                            required 
                                        />
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