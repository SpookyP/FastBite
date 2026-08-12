import React from 'react';
import { useAuth } from 'react-oidc-context';

const Navbar = () => {
    // 1. A Navbar acede diretamente ao contexto de autenticação
    const auth = useAuth();

    // 2. Extrair o perfil do utilizador (claims do Token OIDC)
    const userProfile = auth.user?.profile;

    // 3. Verificar o papel de "Admin"
    const userRoles = userProfile?.role || userProfile?.roles || [];
    const isAdmin = Array.isArray(userRoles) 
        ? userRoles.includes('Admin') 
        : userRoles === 'Admin';

    // 4. Obter o Nome (Tenta várias propriedades dependendo de como o .NET envia)
    const userName = userProfile?.name || userProfile?.preferred_username || userProfile?.email || 'Utilizador';

    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-white shadow-sm py-3 mb-4">
            <div className="container">
                <a className="navbar-brand d-flex align-items-center fw-bold text-danger fs-3" href="/">
                    <span className="me-2">🍴</span> FastBite
                </a>

                <div className="d-flex align-items-center ms-auto gap-4">
                    <a href="#menu" className="text-decoration-none text-dark fw-semibold">Menu</a>
                    <a href="#orders" className="text-decoration-none text-dark fw-semibold">Orders</a>
                    <a href="#cart" className="text-decoration-none text-dark fw-semibold">Cart</a>

                    {isAdmin && (
                        <a 
                            href="/items" 
                            className="btn btn-outline-primary btn-sm rounded-pill px-3 fw-bold"
                        >
                            ⚙️ Gestão Admin
                        </a>
                    )}

                    {/* Mostra o nome do utilizador e o Logout se estiver autenticado */}
                    {auth.isAuthenticated && (
                        <div className="d-flex align-items-center gap-3 ms-3 border-start ps-3">
                            <span className="text-secondary small fw-bold">
                                👤 {userName}
                            </span>
                            <button 
                                onClick={() => auth.signoutRedirect()} 
                                className="btn btn-outline-danger btn-sm rounded-pill px-3"
                            >
                                Sair
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </nav>
    );
};

export default Navbar;