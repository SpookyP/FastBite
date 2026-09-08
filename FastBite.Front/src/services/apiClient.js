// src/services/apiClient.js
const API_MENU_URL = import.meta.env.VITE_API_MENU_BASE_URL;
const API_ORDER_URL = import.meta.env.VITE_API_ORDER_BASE_URL;

export const API_BASES = {
  menu: API_MENU_URL,
  order: API_ORDER_URL,
};

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

    // Retira baseUrl das options para não ir parar ao fetch
    const { baseUrl: _ignored, ...rest } = options;
    const base = options.baseUrl || API_MENU_URL;

    const headers = {
        'Content-Type': 'application/json',
        ...(rest.headers || {}),
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    } else {
        console.warn('Aviso: Nenhum token na localStorage.');
    }

    const config = { ...rest, headers };

    const response = await fetch(`${base}${endpoint}`, config);

    if (!response.ok) {
        if (response.status === 401) {
            console.error('401 Unauthorized: O token pode ser inválido, estar expirado, ou faltam permissões.');
        }
        throw new Error(`Erro na API: ${response.status}`);
    }

    if (response.status !== 204) {
        return await response.json();
    }
    return null;
};