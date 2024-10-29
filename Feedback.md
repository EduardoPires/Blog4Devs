# Feedback do Instrutor

#### 28/10/24 - Revisão Inicial - Eduardo Pires

## Pontos Positivos:

- Boa separação de responsabilidades.
- Arquitetura enxuta de acordo com a complexidade do projeto
- Mostrou entendimento do ecossistema de desenvolvimento em .NET com Minimal APIs

## Pontos Negativos:

- Aparentemente o projeto está incompleto.
- As controllers acessam o contexto do EF sem nenhum tipo de lógica de negócios.
- Não existe controle de usuário associado a um autor
- Na parte das aplicações Web, as controllers não fazem controle de usuário, é possível excluir um post de outro usuário.
- O projeto está utilizando uma arquitetura coerente com o desafio mas está muito enxuto basicamente padrão scaffolding.

## Sugestões:

- Evoluir o projeto para as necessidades solicitadas no escopo.

## Problemas:

- Não consegui executar a aplicação de imediato na máquina. É necessário que o Seed esteja configurado corretamente, com uma connection string apontando para o SQLite.

  **P.S.** As migrations precisam ser geradas com uma conexão apontando para o SQLite; caso contrário, a aplicação não roda.
