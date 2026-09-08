import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';

const ProtectedRoute = ({ allowedRoles }) => {
    const auth = useAuth();

    // 1. Mostrar estado de loading enquanto o auth inicializa
    if (auth.isLoading) {
        return <div className="text-center py-5">A verificar permissões...</div>;
    }

    // 2. Se não estiver autenticado, manda para a Home (onde o teu código já força o login) ou direto para login
    if (!auth.isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    const profile = auth.user?.profile;
    const userRole = profile?.role || profile?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    // Como uma role pode vir como string ("Admin") ou array (["Admin", "User"]), normalizamos para array
    const userRolesArray = Array.isArray(userRole) ? userRole : [userRole];

    // 4. Verificar se o utilizador tem pelo menos uma das roles exigidas
    const hasRequiredRole = allowedRoles.some(role => userRolesArray.includes(role));

    if (!hasRequiredRole) {
        // Redireciona para uma página de Acesso Negado
        return <Navigate to="/unauthorized" replace />; 
    }

    // 5. Se tiver a role certa, renderiza a vista pretendida
    return <Outlet />;
};

export default ProtectedRoute;