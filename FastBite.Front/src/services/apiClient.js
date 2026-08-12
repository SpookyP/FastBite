// src/services/apiClient.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const fetchComToken = async (endpoint, options = {}) => {
   
    let token = null;

    const oidcKey = Object.keys(localStorage).find(key => key.startsWith('oidc.user:'));
    
    if (oidcKey) {
        const oidcUserString = localStorage.getItem(oidcKey);
        if (oidcUserString) {
            try {
                const oidcUser = JSON.parse(oidcUserString);
                token = oidcUser?.access_token;
            } catch (e) {
                console.error('Erro ao fazer parse do objeto OIDC da localStorage:', e);
            }
        }
    }

    const headers = {
        'Content-Type': 'application/json',
        ...(options.headers || {}),
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    } else {
        console.warn('Aviso: Nenhum token na localStorage.');        
    }

    const config = {
        ...options,
        headers
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

    if (!response.ok) {
        if (response.status === 401) {
            console.error('401 Unauthorized: O token pode ser inválido, estar expirado, ou faltam permissões de Admin.');
        }
            throw new Error(`Erro na API: ${response.status}`);
    }

    if (response.status !== 204) {
        return await response.json();
    }
    return null;
};