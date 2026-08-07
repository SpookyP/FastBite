import React from 'react';

const Navbar = ({ user, onLogout }) => {
    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-white shadow-sm py-3 mb-4">
            <div className="container">
                {/* Logo / Marca */}
                <a className="navbar-brand d-flex align-items-center fw-bold text-danger fs-3" href="#">
                    <span className="me-2">🍴</span> FastBite
                </a>

                {/* Links da Navbar */}
                <div className="d-flex align-items-center ms-auto gap-4">
                    <a href="#menu" className="text-decoration-none text-dark fw-semibold">Catalog</a>
                    <a href="#orders" className="text-decoration-none text-dark fw-semibold">Orders</a>
                    <a href="#cart" className="text-decoration-none text-dark fw-semibold">Cart</a>

                    {/* LÓGICA DO BOTÃO: Se houver user mostra Logout, caso contrário (impossível neste fluxo) mostra Login */}
                    {user ? (
                        <div className="d-flex align-items-center gap-3">
                            <span className="text-secondary small fw-bold">
                                👤 {user.name ?? 'Utilizador'}
                            </span>
                            <button 
                                onClick={onLogout} 
                                className="btn btn-outline-danger btn-sm rounded-pill px-3"
                            >
                                Terminar Sessão
                            </button>
                        </div>
                    ) : (
                        <button className="btn btn-success rounded-pill px-4">
                            Login
                        </button>
                    )}
                </div>
            </div>
        </nav>
    );
};

export default Navbar;