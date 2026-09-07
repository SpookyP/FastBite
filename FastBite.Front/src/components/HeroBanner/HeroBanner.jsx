import React from 'react';

const HeroBanner = () => {
    return (
        <div className="p-5 mb-4 text-white rounded-4 shadow" style={{ background: 'linear-gradient(135deg, #ff8c00, #ff5500)' }}>
            <h1 className="display-5 fw-bold mb-2">O que te apetece hoje? 🍽️</h1>
            <p className="fs-5 mb-0 opacity-75">Ingredientes frescos · Entrega rápida · Bom apetite</p>
        </div>
    );
};

export default HeroBanner;