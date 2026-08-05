import React from 'react';

const obterImagemPorCategoria = (categoria) => {
    const cat = (categoria || "").toLowerCase();
    if (cat.includes('burger') || cat.includes('hamburguer')) return "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=500";
    if (cat.includes('pizza')) return "https://images.unsplash.com/photo-1604382355076-af4b0eb60143?w=500";
    if (cat.includes('sushi')) return "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=500";
    if (cat.includes('salada') || cat.includes('salad')) return "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500";
    return "https://images.unsplash.com/photo-1550547660-d9450f859349?w=500";
};

const ProductCard = ({ item }) => {
    return (
        <div className="col">
            <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden d-flex flex-column justify-content-between">
                <div className="position-relative" style={{ height: '180px', backgroundColor: '#f8f9fa' }}>
                    <img 
                        src={obterImagemPorCategoria(item.categoria)} 
                        className="w-100 h-100 object-fit-cover" 
                        alt={item.nome} 
                    />
                    <span className="position-absolute top-0 start-0 m-3 badge text-white px-3 py-2 rounded-pill shadow-sm" style={{ backgroundColor: '#ff6b00' }}>
                        🔥 Popular
                    </span>
                </div>
                <div className="card-body p-3 d-flex flex-column justify-content-between flex-grow-1">
                    <div>
                        <div className="d-flex justify-content-between align-items-start mb-1">
                            <h6 className="card-title fw-bold m-0 fs-6">{item.nome}</h6>
                            <span className="fw-bold fs-6" style={{ color: '#ff6b00' }}>
                                ${item.precoBase?.toFixed(2) || '0.00'}
                            </span>
                        </div>
                        <p className="card-text text-muted small mb-3" style={{ display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden', height: '38px' }}>
                            {item.descricao}
                        </p>
                    </div>
                    <div>
                        <div className="d-flex gap-3 text-muted small mb-3 fw-semibold">
                            <span>⭐ 4.8</span>
                            <span>🕒 15 min</span>
                        </div>
                        <button className="btn w-100 text-white fw-bold rounded-3 py-2 shadow-sm border-0" style={{ backgroundColor: '#ff6b00' }}>
                            + Add to cart
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;