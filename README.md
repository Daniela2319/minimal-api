# 🚗 Minimal API de Gestão de Administradores e Veículos – Projeto Final de Curso (.NET 9.0)

## 📚 Sobre o Projeto

Esta é uma API desenvolvida com **ASP.NET Core (.NET 9.0)** que simula um sistema de administração com controle de acesso e gerenciamento de veículos. O projeto foi documentado com **Scalar**, empacotado com **Docker** através de um `Dockerfile`, e executado com o orquestrador definido em `pipeline.yml`.



## 🛠️ Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core Web API
- Scalar (Documentação interativa)
- Docker e Dockerfile
- Pipeline (orquestração)
- Autenticação via Email e Senha (sem identidade externa)



## 📦 Funcionalidades

### 🔐 Login
- Login via **email e senha**
- Autenticação protegida com tokens
- Permite acesso apenas a usuários autenticados

### 🧑‍💼 CRUD de Administradores
- Criar administrador
- Listar todos os administradores
- Atualizar dados (perfil com permissão de edição)
- Deletar administrador

### 🚘 CRUD de Veículos
- Cadastrar novo veículo
- Listar veículos
- Editar veículo existente
- Excluir veículo



## 📄 Documentação com Scalar

A documentação da API foi gerada usando o Scalar, oferecendo:
- Listagem de todos os endpoints disponíveis
- Exemplos de requests/responses
- Testes manuais diretamente pela interface

Para visualizar a documentação, acesse o endpoint gerado após subir a API.



## 🐳 Docker

- O projeto possui um `Dockerfile` configurado para empacotar a API em uma imagem leve e eficiente
- A execução é orquestrada via `pipeline.yml`, definindo volumes, porta e ambiente de execução

### 🧪 Para rodar o projeto:

```bash
# Build da imagem
docker build -t veiculos-admin-api .

# Execução via papile
pipeline up
```
## Finalização
* Projeto implementado com sucesso

* Funcionalidades exigidas no curso atendidas

* Docker e documentação funcionando


  
## 👥 Participantes
- [Daniela](https://github.com/Daniela2319)
- [Emanuela](https://github.com/emanoelados1)


 
