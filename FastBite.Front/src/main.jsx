import React from 'react';
import ReactDOM from 'react-dom/client';
import { AuthProvider } from 'react-oidc-context';
import App from './App.jsx';
import './index.css';

const oidcConfig = {
    authority: import.meta.env.VITE_IDENTITY_AUTHORITY,
    client_id: import.meta.env.VITE_IDENTITY_CLIENT_ID,
    redirect_uri: import.meta.env.VITE_FRONTEND_URL,
    response_type: 'code',
    scope: 'openid profile roles MenuCatalog.api.full DeliveryOrdering.api.full',
    post_logout_redirect_uri: import.meta.env.VITE_FRONTEND_URL,
    // Garante que guarda os tokens no sessionStorage do browser de forma segura
    automaticSilentRenew: true,
};

ReactDOM.createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <AuthProvider {...oidcConfig}>
            <App />
        </AuthProvider>
    </React.StrictMode>
);