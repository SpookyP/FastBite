# 🍕 FastBite

> Plataforma web para gestão de catálogos, combos automáticos e ponto de venda multiutilizador.  
> *Projeto académico desenvolvido na **ATEC - Academia de Formação**.*

---

## ℹ️ Sobre o Projeto

O **FastBite** é uma solução digital criada para otimizar a gestão de produtos e o processo de vendas em estabelecimentos de restauração. A plataforma permite gerir diferentes perfis de utilizador, centralizar a criação de menus e automatizar a aplicação de descontos em tempo real.

## 🌟 Destaques (Highlights)

- **Combos Automáticos**: Agrupamento inteligente de produtos com aplicação direta de 10% de desconto no valor final.
- **Painel Administrativo Completo**: Acesso restrito a administradores com funcionalidades completas de criação, leitura, atualização e eliminação (CRUD).
- **Integração Robusta de API**: Comunicação fluida entre o Frontend e o Backend, totalmente documentada e testada via Postman.

---

## 🛠️ Requisitos Mínimos

Antes de iniciar, garante que tens as seguintes ferramentas instaladas:

- **.NET SDK**: `8.0.0`
- **Node.js**: `16.16.0`

---

## 🚀 Como Iniciar

Escreve os seguintes comandos no teu terminal para colocar a aplicação a correr localmente:

```bash
# Entrar na pasta do Projeto
cd FastBite

# Instalar dotnet ef
dotnet tool install --global dotnet-ef

# Entrar na pasta do Frontend
cd FastBite.Front
# Certificar que o .env se encontra na pasta

# Instalar as dependências do projeto
npm install

# Voltar à pasta do Projeto
cd ..

# Instalar as dependências do projeto
npm install

# Configurar o certificado SSL do .NET (apenas na primeira execução)
dotnet dev-certs https --trust

# Criar Base de Dados locais
npm run predev

# Iniciar o servidor de desenvolvimento
npm run dev
