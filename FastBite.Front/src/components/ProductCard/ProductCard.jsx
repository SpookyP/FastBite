import React, { useState } from 'react';
import { useCart } from '../../context/CartContext';

const ProductCard = ({ item }) => {
    const { adicionarAoCarrinho } = useCart();
    const [foiAdicionado, setFoiAdicionado] = useState(false);

    if (!item) {
        return null; 
    }

    const handleAdicionar = () => {
        adicionarAoCarrinho(item);
        setFoiAdicionado(true);
        setTimeout(() => {
            setFoiAdicionado(false);
        }, 2000);
    };

    return (
        <div className="col">
            <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden d-flex flex-column justify-content-between">
                
                <div className="card-body p-4 d-flex flex-column justify-content-between flex-grow-1">
                    <div>
                        <div className="d-flex justify-content-between align-items-start mb-2">
                            <div>
                                <h6 className="card-title fw-bold m-0 fs-5">{item.nome}</h6>
                            </div>
                            <span className="fw-bold fs-5" style={{ color: '#ff6b00' }}>
                                {item.precoBase?.toFixed(2) || '0.00'}€
                            </span>
                        </div>
                        
                        <p className="card-text text-muted small mb-2" style={{ display: '-webkit-box', WebkitLineClamp: 3, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                            {item.descricao}
                        </p>
                        {item.alergenios && item.alergenios.length > 0 && (
                            <p className="card-text text-muted mb-3" style={{ fontSize: '0.75rem' }}>
                                <strong>Alergénios: </strong> 
                                {
                                    Array.isArray(item.alergenios) 
                                        ? item.alergenios.join(', ') 
                                        : item.alergenios
                                }
                            </p>
                        )}
                    </div>
                    <div className="mt-auto pt-3">
                        <button 
                            className={`btn w-100 text-white fw-bold rounded-3 py-2 shadow-sm border-0 transition-all ${foiAdicionado ? 'bg-success' : ''}`} 
                            style={{ backgroundColor: foiAdicionado ? '' : '#ff6b00' }}
                            onClick={handleAdicionar}
                            disabled={foiAdicionado} 
                        >
                            {foiAdicionado ? '✅ Adicionado!' : '+ Adicionar'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;