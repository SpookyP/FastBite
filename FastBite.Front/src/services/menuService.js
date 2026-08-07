const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const menuService = {
    // Recebe o token como parâmetro
    async obterTodos(accessToken = null) {
        const headers = {
            'Content-Type': 'application/json',
        };

        // Se o utilizador estiver logado, envia o token de segurança
        if (accessToken) {
            headers['Authorization'] = `Bearer ${accessToken}`;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/menus/ObterTodos`, {
                method: 'GET',
                headers: headers
            });

            if (!response.ok) {
                throw new Error(`Erro da API: ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error("Falha ao obter menus:", error);
            throw error;
        }
    }
};