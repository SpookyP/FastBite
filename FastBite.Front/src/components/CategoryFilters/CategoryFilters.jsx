import React from 'react';

const CategoryFilters = () => {
    return (
        <>
            {/* Barra de Pesquisa */}
            <div className="mb-4">
                <div className="input-group input-group-lg shadow-sm rounded-3 overflow-hidden">
                    <span className="input-group-text bg-light border-0 ps-4">🔍</span>
                    <input type="text" className="form-control bg-light border-0 fs-6 py-3" placeholder="Search burgers, pizza, sushi..." />
                </div>
            </div>

            {/* Filtros de Categoria */}
            <div className="d-flex gap-2 mb-4 overflow-auto py-1">
                <button className="btn btn-sm px-4 py-2 rounded-pill text-white fw-semibold shadow-sm" style={{ backgroundColor: '#ff6b00' }}>🍔 All</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🍔 Burgers</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🍕 Pizza</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🍣 Sushi</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🥗 Salads</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🍰 Desserts</button>
                <button className="btn btn-sm px-4 py-2 rounded-pill btn-light fw-semibold text-secondary border">🥤 Drinks</button>
            </div>
        </>
    );
};

export default CategoryFilters;