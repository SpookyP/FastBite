import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { menuService } from '../../services/menuService';

const EditItem = () => {
  const { id } = useParams();
  const navigate = useNavigate();

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
  const [erro, setErro] = useState(null);

  // 1. Carregar os dados atuais do item ao montar a página
  useEffect(() => {
    const fetchItem = async () => {
      try {
        setLoading(true);
        const data = await menuService.getById(id);
        
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

    if (id) fetchItem();
  }, [id]);

  // 2. Atualizar estado ao digitar nos inputs
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  // 3. Submeter formulário
  const handleSubmit = async (e) => {
    e.preventDefault();
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

      await menuService.update(id, payload);
      navigate('/items'); // Redireciona para a lista após atualizar
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
    <div className="container mt-4" style={{ maxWidth: '600px' }}>
      <h2>Editar Item #{id}</h2>

      {erro && <div className="alert alert-danger">{erro}</div>}

      <form onSubmit={handleSubmit} className="card p-4 shadow-sm mt-3">
        <div className="mb-3">
          <label className="form-label">Nome</label>
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
          <label className="form-label">Descrição</label>
          <textarea
            className="form-control"
            name="descricao"
            rows="3"
            value={formData.descricao}
            onChange={handleChange}
          ></textarea>
        </div>

        <div className="row">
          <div className="col-md-6 mb-3">
            <label className="form-label">Categoria</label>
            <input
              type="text"
              className="form-control"
              name="categoria"
              value={formData.categoria}
              onChange={handleChange}
              required
            />
          </div>

          <div className="col-md-6 mb-3">
            <label className="form-label">Alergénios</label>
            <input
              type="text"
              className="form-control"
              name="alergenios"
              value={formData.alergenios}
              onChange={handleChange}
            />
          </div>
        </div>

        <div className="row">
          <div className="col-md-6 mb-3">
            <label className="form-label">Preço Base (€)</label>
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
            <label className="form-label">Limite Diário</label>
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
  );
};

export default EditItem;