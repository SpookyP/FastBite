import React, { useState } from 'react';

interface OrderData {
  customerName: string;
  email: string;
  address: string;
  nif: string;
  paymentMethod: 'mbway' | 'multibanco' | 'credit_card';
}

export const Checkout: React.FC = () => {
  const [formData, setFormData] = useState<OrderData>({
    customerName: '',
    email: '',
    address: '',
    nif: '',
    paymentMethod: 'mbway',
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const validateForm = (): boolean => {
    if (!formData.customerName || !formData.email || !formData.address) {
      setErrorMessage('Por favor, preencha todos os campos obrigatórios.');
      return false;
    }
    return true;
  };

  const handleFinalizarPedido = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMessage(null);

    if (!validateForm()) return;

    setIsSubmitting(true);

    try {
      const response = await fetch('/api/checkout', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        throw new Error('Erro ao processar o pedido. Tente novamente.');
      }

      setIsSuccess(true);
    } catch (err: any) {
      setErrorMessage(err.message || 'Ocorreu um erro inesperado.');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isSuccess) {
    return (
      <div className="checkout-success">
        <h2>Pedido Confirmado!</h2>
        <p>Obrigado pela sua compra. Enviámos a confirmação para {formData.email}.</p>
      </div>
    );
  }

  return (
    <form onSubmit={handleFinalizarPedido} className="checkout-form">
      <h2>Finalizar Encomenda</h2>

      {errorMessage && <div className="error-banner">{errorMessage}</div>}

      <div className="form-group">
        <label htmlFor="customerName">Nome Completo *</label>
        <input
          id="customerName"
          type="text"
          name="customerName"
          value={formData.customerName}
          onChange={handleChange}
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="email">E-mail *</label>
        <input
          id="email"
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="address">Morada *</label>
        <input
          id="address"
          type="text"
          name="address"
          value={formData.address}
          onChange={handleChange}
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="nif">NIF (Opcional)</label>
        <input
          id="nif"
          type="text"
          name="nif"
          value={formData.nif}
          onChange={handleChange}
        />
      </div>

      <div className="form-group">
        <label htmlFor="paymentMethod">Método de Pagamento</label>
        <select
          id="paymentMethod"
          name="paymentMethod"
          value={formData.paymentMethod}
          onChange={handleChange}
        >
          <option value="mbway">MB WAY</option>
          <option value="multibanco">Multibanco</option>
          <option value="credit_card">Cartão de Crédito</option>
        </select>
      </div>

      <button type="submit" disabled={isSubmitting} className="btn-submit">
        {isSubmitting ? 'A processar...' : 'Finalizar Pedido'}
      </button>
    </form>
  );
};