## Teste Técnico - Sistema de Pedidos WPF (Benner)
Este projeto é uma aplicação desktop em C# e WPF para cadastro de Pessoas, Produtos e Pedidos, desenvolvido como parte do teste técnico.

A aplicação utiliza o padrão MVVM, persistência de dados em arquivos JSON e manipulação de dados com LINQ.

### Dependências
.NET 8 (O projeto foi desenvolvido na plataforma .NET 8)

WPF (Windows Presentation Foundation)

Visual Studio 2022 (Recomendado para abrir o projeto WpfApp.sln)

Não há pacotes NuGet externos ou dependências adicionais necessárias além do SDK do .NET 8.

### Instruções de Execução
- Clonar o Repositório
- Abrir a Solução

### Restaurar Dependências:

O Visual Studio deve restaurar automaticamente as dependências do .NET 8 ao carregar a solução. Se isso não ocorrer, clique com o botão direito na solução no "Solution Explorer" e escolha "Restore NuGet Packages".

Executar o Projeto:

Pressione F5 ou clique no botão "Iniciar" (com o ícone de play verde) na barra de ferramentas do Visual Studio para compilar e executar a aplicação.

### Sobre o Armazenamento de Dados
Os dados são salvos em arquivos JSON. Na primeira execução, uma pasta WpfApp/Data/ será criada automaticamente na raiz do projeto contendo os arquivos:
- pessoas.json
- produtos.json
- pedidos.json
