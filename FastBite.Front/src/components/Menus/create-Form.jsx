import React, { useState } from 'react';
import { useAuth } from 'react-oidc-context';
import { menuService } from '../services/menuService';

const AddMenu = ({ onMenuAdded }) => {
    const auth = useAuth();
    
    // 1. Estado Controlado
    const [nome, setNome] = useState('');
    const [preco, setPreco] = useState('');
    const [erro, setErro] = useState('');
    const [aSubmeter, setASubmeter] = useState(false);

    // 2. Função de Submissão Segura
    const handleSubmit = async (e) => {
        e.preventDefault(); // Impede que a página faça refresh
        setErro('');

        // Validação básica no cliente
        if (!nome.trim() || !preco || parseFloat(preco) <= 0) {
            setErro('Por favor, preenche um nome válido e um preço maior que zero.');
            return;
        }

        try {
            setASubmeter(true);
            
            const novoItem = {
                nome: nome.trim(),
                preco: parseFloat(preco)
            };

            // 3. Chamada segura à API passando o token da sessão
            await menuService.criar(novoItem, auth.user.access_token);
            
            // Limpa o formulário após sucesso
            setNome('');
            setPreco('');
            
            // Avisa a tabela (componente pai) para atualizar a lista
            if (onMenuAdded) onMenuAdded();
            
            alert('Item adicionado com sucesso!');

        } catch (error) {
            console.error("Erro:", error);
            setErro('Ocorreu um erro ao comunicar com o servidor.');
        } finally {
            setASubmeter(false);
        }
    };

    return (
        <div className="card p-4 shadow-sm">
            <h4>Adicionar Novo Item</h4>
            
            {erro && <div className="alert alert-danger">{erro}</div>}
            
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Nome do Menu</label>
                    <input 
                        type="text" 
                        className="form-control" 
                        value={nome}
                        onChange={(e) => setNome(e.target.value)} // Atualiza o estado controlado
                        disabled={aSubmeter}
                    />
                </div>
                
                <div className="mb-3">
                    <label className="form-label">Preço (€)</label>
                    <input 
                        type="number" 
                        step="0.01" 
                        className="form-control" 
                        value={preco}
                        onChange={(e) => setPreco(e.target.value)}
                        disabled={aSubmeter}
                    />
                </div>
                
                <button 
                    type="submit" 
                    className="btn btn-success"
                    disabled={aSubmeter}
                >
                    {aSubmeter ? 'A Guardar...' : 'Guardar Item'}
                </button>
            </form>
        </div>
    );
};

export default AddMenu;