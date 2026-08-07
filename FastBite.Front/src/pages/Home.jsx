import React, { useState, useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import Navbar from '../components/Navbar/Navbar';
import HeroBanner from '../components/HeroBanner/HeroBanner';
import CategoryFilters from '../components/CategoryFilters/CategoryFilters';
import ProductCard from '../components/ProductCard/ProductCard';
import { menuService } from '../services/menuService';

const Home = () => {
    const auth = useAuth();
    const [menuItems, setMenuItems] = useState([]);
    const [loadingMenus, setLoadingMenus] = useState(true);
    const [errorMenus, setErrorMenus] = useState(null);

    useEffect(() => {
        // Se já terminou de carregar, não tem erros e não está autenticado, força o login imediato
        if (!auth.isLoading && !auth.error && !auth.isAuthenticated && !auth.activeNavigator) {
            auth.signinRedirect();
        }
    }, [auth.isLoading, auth.isAuthenticated, auth.error, auth.activeNavigator]);

    // Buscar os dados da API protegida assim que o utilizador estiver autenticado
    useEffect(() => {
        const fetchMenus = async () => {
            if (auth.isAuthenticated && auth.user?.access_token) {
                try {
                    setLoadingMenus(true);
                    const data = await menuService.obterTodos(auth.user.access_token);
                    setMenuItems(data || []);
                } catch (err) {
                    console.error("Erro ao carregar menus:", err);
                    setErrorMenus("Não foi possível carregar os dados do catálogo.");
                } finally {
                    setLoadingMenus(false);
                }
            }
        };

        fetchMenus();
    }, [auth.isAuthenticated, auth.user]);

    if (auth.isLoading) {
        return <div className="text-center py-5">A verificar credenciais de segurança...</div>;
    }

    if (auth.error) {
        return <div className="alert alert-danger m-4">Erro de autenticação: {auth.error.message}</div>;
    }

    if (!auth.isAuthenticated) {
        return null; // Evita mostrar qualquer HTML antes do redirecionamento
    }

    return (
        <div className="min-vh-100 bg-light">
            {/* Navbar com dados do utilizador e logout integrado */}
            <Navbar 
                user={auth.user?.profile} 
                onLogout={() => auth.signoutRedirect()} 
            />

            {/* Banner de Destaque */}
            <HeroBanner />

            <main className="container-fluid py-4 px-4">
                {/* Filtros por Categoria */}
                <CategoryFilters />

                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h3 className="fw-bold">Menu em Destaque</h3>
                </div>

                {/* Tratamento de estados de carregamento da API de Menus */}
                {loadingMenus && (
                    <div className="text-center py-5">
                        <div className="spinner-border text-secondary" role="status"></div>
                        <p className="text-muted mt-2">A carregar produtos do servidor...</p>
                    </div>
                )}

                {errorMenus && (
                    <div className="alert alert-warning" role="alert">
                        {errorMenus}
                    </div>
                )}

                {/* Grelha de Produtos */}
                {!loadingMenus && !errorMenus && (
                    <div className="row g-4">
                        {menuItems.length > 0 ? (
                            menuItems.map((item) => (
                                <div className="col-12 col-sm-6 col-md-4 col-lg-3" key={item.id || item.codigo}>
                                    <ProductCard product={item} />
                                </div>
                            ))
                        ) : (
                            <div className="col-12 text-center py-5 text-muted">
                                <p>Nenhum item encontrado no catálogo.</p>
                            </div>
                        )}
                    </div>
                )}
            </main>
        </div>
    );
};

export default Home;