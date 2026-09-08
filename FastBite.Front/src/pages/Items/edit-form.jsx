import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { menuService } from '../../services/menuService';

const EditItem = () => {
  const location = useLocation();
  const id = location.state?.id;
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    nome: '',
    descricao: '',
    categoria: '',
    alergenios: '',
    precoBase: '',
    limiteDiario: '',
    quantidadeVendidaHoje: 0
  });

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [erro, setErro] = useState(null);
  const [erros, setErros] = useState({});

  useEffect(() => {
    if (!id) {
      navigate('/items');
      return;
    }

    const fetchItem = async () => {
      try {
        setLoading(true);
        const data = await menuService.obterPorId(id);
        
        setFormData({
          nome: data.nome || '',
          descricao: data.descricao || '',
          categoria: data.categoria || '',
          alergenios: data.alergenios || '',
          precoBase: data.precoBase ?? '',
          limiteDiario: data.limiteDiario ?? '',
          quantidadeVendidaHoje: data.quantidadeVendidaHoje ?? 0
        });
      } catch (err) {
        console.error(err);
        setErro('Não foi possível carregar os dados do item.');
      } finally {
        setLoading(false);
      }
    };

    fetchItem();
  }, [id, navigate]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    setErros((prev) => ({ ...prev, [name]: null }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const novosErros = {};
    const preco = parseFloat(formData.precoBase);
    const limite = parseInt(formData.limiteDiario, 10);
    const qtdVendida = parseInt(formData.quantidadeVendidaHoje, 10);
    
    if (!formData.nome?.toString().trim()) novosErros.nome = 'Por favor, preenche este campo.';
    if (!formData.descricao?.toString().trim()) novosErros.descricao = 'Por favor, preenche este campo.';
    if (!formData.categoria) novosErros.categoria = 'Por favor, seleciona uma categoria.';
    if (isNaN(preco) || preco <= 0 || preco > 1000) novosErros.precoBase = 'Insere um preço válido (0.01 - 1000).';
    if (isNaN(limite) || limite <= 0 || limite > 100) novosErros.limiteDiario = 'Insere um limite válido (1 - 100).';
    if (isNaN(qtdVendida) || qtdVendida < 0) novosErros.quantidadeVendidaHoje = 'Insere uma quantidade válida (>= 0).';

    if (Object.keys(novosErros).length > 0) {
      setErros(novosErros);
      return;
    }

    setSubmitting(true);
    setErro(null);

    try {
      const payload = {
        id: parseInt(id, 10),
        nome: formData.nome.trim(),
        descricao: formData.descricao.trim(),
        categoria: formData.categoria,
        alergenios: formData.alergenios?.trim() || '',
        precoBase: preco,
        limiteDiario: limite,
        quantidadeVendidaHoje: qtdVendida
      };

      await menuService.edit(id, payload);
      navigate('/items');
    } catch (err) {
      console.error(err);
      setErro('Erro ao atualizar o item. Verifica os dados enviados.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="container mt-5 text-center">A carregar item...</div>;
  }

  return (
    <div className="min-vh-100 bg-light">
      <Navbar />
      <div className="container mt-4" style={{ maxWidth: '600px' }}>
        <h2>Editar Item</h2>

        {erro && <div className="alert alert-danger">{erro}</div>}

        <form onSubmit={handleSubmit} className="card p-4 shadow-sm mt-3">
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
            ></textarea>
            {erros.descricao && <small className="text-danger">{erros.descricao}</small>}
          </div>

          <div className="row">
            <div className="col-md-6 mb-3">
              <label className="form-label fw-bold">Categoria</label>
              <select
                className={`form-select ${erros.categoria ? 'is-invalid' : ''}`}
                name="categoria"
                value={formData.categoria}
                onChange={handleChange}
              >
                <option value="">Seleciona uma opção</option>
                <option value="Prato">Prato</option>
                <option value="Bebida">Bebida</option>
                <option value="Acompanhamento">Acompanhamento</option>
              </select>
              {erros.categoria && <small className="text-danger">{erros.categoria}</small>}
            </div>

            <div className="col-md-6 mb-3">
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
          </div>

          <div className="row">
            <div className="col-md-4 mb-3">
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

            <div className="col-md-4 mb-3">
              <label className="form-label fw-bold">Limite Diário</label>
              <input
                type="number"
                min="1"
                max="100"
                className={`form-control ${erros.limiteDiario ? 'is-invalid' : ''}`}
                name="limiteDiario"
                value={formData.limiteDiario}
                onChange={handleChange}
              />
              {erros.limiteDiario && <small className="text-danger">{erros.limiteDiario}</small>}
            </div>

            <div className="col-md-4 mb-3">
              <label className="form-label fw-bold">Vendidos Hoje</label>
              <input
                type="number"
                min="0"
                className={`form-control ${erros.quantidadeVendidaHoje ? 'is-invalid' : ''}`}
                name="quantidadeVendidaHoje"
                value={formData.quantidadeVendidaHoje}
                onChange={handleChange}
              />
              {erros.quantidadeVendidaHoje && (
                <small className="text-danger">{erros.quantidadeVendidaHoje}</small>
              )}
            </div>
          </div>

          <div className="d-flex gap-2 justify-content-end mt-3">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => navigate('/items')}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn btn-warning text-white"
              disabled={submitting}
            >
              {submitting ? 'A guardar...' : 'Guardar Alterações'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditItem;