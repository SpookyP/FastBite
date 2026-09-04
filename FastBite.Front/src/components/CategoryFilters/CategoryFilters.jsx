import React from 'react';

// Agora recebemos a categoria escolhida e a função para a mudar como parâmetros (props)
const CategoryFilters = ({ categoriaAtiva, onCategoriaChange }) => {
    
    const categorias = ['Tudo', 'Pratos', 'Acompanhamentos', 'Bebidas'];

    return (
            <div className="d-flex gap-2 mb-4 overflow-auto py-1" style={{ whiteSpace: 'nowrap' }}>
                {categorias.map((categoria) => (
                    <button 
                        key={categoria}
                        // Quando clicamos, avisamos a página Home!
                        onClick={() => onCategoriaChange(categoria)}
                        className={`btn btn-sm px-4 py-2 rounded-pill fw-semibold shadow-sm transition-all ${
                            categoriaAtiva === categoria 
                                ? 'text-white border-0' 
                                : 'btn-light text-secondary border' 
                        }`}
                        style={{ backgroundColor: categoriaAtiva === categoria ? '#ff6b00' : 'white' }}
                    >
                        {categoria}
                    </button>
                ))}
            </div>
    );
};

export default CategoryFilters;