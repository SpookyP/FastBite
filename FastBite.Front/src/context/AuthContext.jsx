import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Verifica o estado da sessão atual junto do backend/IdentityServer
        const checkUserSession = async () => {
            try {
                const response = await fetch('https://localhost:7281/api/account/user-info', {
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const userData = await response.json();
                    setIsAuthenticated(true);
                    setUser(userData);
                } else {
                    setIsAuthenticated(false);
                    setUser(null);
                }
            } catch (error) {
                console.error("Erro ao validar sessão:", error);
                setIsAuthenticated(false);
            } finally {
                setLoading(false);
            }
        };

        checkUserSession();
    }, []);

    // Redireciona para o endpoint de login gerido pelo Duende IdentityServer no .NET 8
    const login = () => {
        window.location.href = 'https://localhost:7281/authentication/login';
    };

    // Redireciona para o endpoint de logout
    const logout = () => {
        window.location.href = 'https://localhost:7281/authentication/logout';
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, user, loading, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);