import React from 'react';
import ReactDOM from 'react-dom/client';
import { AuthProvider } from 'react-oidc-context';
import { WebStorageStateStore } from "oidc-client-ts";
import App from './App.jsx';
import './index.css';

const oidcConfig = {
    authority: import.meta.env.VITE_IDENTITY_AUTHORITY,
    client_id: import.meta.env.VITE_IDENTITY_CLIENT_ID,
    redirect_uri: import.meta.env.VITE_FRONTEND_URL,
    response_type: 'code',
    scope: 'openid profile roles MenuCatalog.api.full DeliveryOrdering.api.full',
    post_logout_redirect_uri: import.meta.env.VITE_FRONTEND_URL,
    
    // Integração perfeita com o LocalStorage
    userStore: new WebStorageStateStore({ store: window.localStorage }),
    
    loadUserInfo: true, // <-- VÍRGULA ADICIONADA AQUI
    automaticSilentRenew: true
};

ReactDOM.createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        {/* Este é o único AuthProvider que a aplicação deve ter */}
        <AuthProvider {...oidcConfig}>
            <App />
        </AuthProvider>
    </React.StrictMode>
);