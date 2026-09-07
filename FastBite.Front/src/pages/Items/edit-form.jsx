import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { menuService } from '../../services/menuService';

const EditItem = () => {
  const location = useLocation();
  const navigate = useNavigate();

  // Recebe o id de forma oculta a partir do location.state
  const id = location.state?.id;

  const [formData, setFormData] = useState({
    nome: '',
    descricao: '',
    categoria: '',
    alergenios: '',
    precoBase: '',
    limiteDiario: ''
  });

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [erro, setErro] = useState(null); // Erros de API
  const [erros, setErros] = useState({}); // Erros de validação dos campos

  useEffect(() => {
    if (!id) {
      navigate('/items');
      return;
    }

    const fetchItem = async () => {
      try {
        setLoading(true);
        // Garante que o método do service está correto
        const data = await menuService.obterPorId(id);
        
        setFormData({
          nome: data.nome || '',
          descricao: data.descricao || '',
          categoria: data.categoria || '',
          alergenios: data.alergenios || '',
          precoBase: data.precoBase || '',
          limiteDiario: data.limiteDiario || ''
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

  // Handler para atualizar os campos do formulário ao digitar
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    // Limpa o erro do campo assim que o utilizador começa a escrever
    setErros((prev) => ({ ...prev, [name]: null }));
  };

  // Handler para submeter as alterações
  const handleSubmit = async (e) => {
    e.preventDefault();

    // 1. Validação Manual
    const novosErros = {};
    
    // Usamos ?.trim() para prevenir erros caso o valor venha undefined da API inicialmente
    if (!formData.nome?.toString().trim()) novosErros.nome = 'Por favor, preenche este campo.';
    if (!formData.descricao?.toString().trim()) novosErros.descricao = 'Por favor, preenche este campo.';
    if (!formData.categoria) novosErros.categoria = 'Por favor, seleciona uma categoria.';
    if (!formData.precoBase || formData.precoBase <= 0 || formData.precoBase > 1000) novosErros.precoBase = 'Insere um preço válido.';
    if (!formData.limiteDiario || formData.limiteDiario <= 0 || formData.limiteDiario > 100) novosErros.limiteDiario = 'Insere um limite válido.';

    // Se houver erros, guardamos no estado e paramos a execução
    if (Object.keys(novosErros).length > 0) {
        setErros(novosErros);
        return;
    }

    // 2. Se passar a validação, avança para a API
    setSubmitting(true);
    setErro(null);

    try {
      const payload = {
        nome: formData.nome,
        descricao: formData.descricao,
        categoria: formData.categoria,
        alergenios: formData.alergenios || '',
        precoBase: parseFloat(formData.precoBase) || 0,
        limiteDiario: parseInt(formData.limiteDiario, 10) || 0
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

  if (!id) return null;

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

            <div className="col-md-6 mb-3">
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
                min="1"
                max="100"
                className={`form-control ${erros.limiteDiario ? 'is-invalid' : ''}`}
                name="limiteDiario"
                value={formData.limiteDiario}
                onChange={handleChange}
              />
              {erros.limiteDiario && <small className="text-danger">{erros.limiteDiario}</small>}
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