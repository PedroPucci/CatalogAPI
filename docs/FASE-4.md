# Fase 4 - CatalogAPI

## Objetivo

Evoluir o microsservico CatalogAPI com persistencia NoSQL,
cache distribuido, busca avancada e preparacao para CI/CD
e implantacao em Kubernetes gerenciado.

## Tecnologias

- .NET 8
- MongoDB
- Redis
- OpenSearch
- Docker
- Kubernetes
- GitHub Actions
- AWS ECR e Amazon EKS (implantacao pendente)

## Funcionalidades implementadas

### Persistencia NoSQL
Integracao com MongoDB para armazenamento de dados do catalogo.

### Cache distribuido
Integracao com Redis para reduzir consultas repetidas e
melhorar o desempenho.

### Busca avancada
Integracao com OpenSearch, incluindo busca aproximada
(Fuzzy Search) e ordenacao por relevancia.

Endpoint:
GET /api/game-catalog/search

### Testes automatizados
15 testes aprovados localmente em configuracao Release.

### Docker
Build da imagem Docker validado localmente.

### CI/CD
Workflow GitHub Actions preparado para build, testes
e construcao da imagem Docker.

Jobs de publicacao no Amazon ECR e implantacao no
Amazon EKS preparados, mas desabilitados.

## Pendencias

- Validar pipeline no GitHub Actions.
- Configurar recursos AWS.
- Publicar imagem no Amazon ECR.
- Implantar no Amazon EKS.
- Demonstrar Rolling Update automatico.
- Validar OpenSearch gerenciado na nuvem.
- Registrar evidencias de desempenho do Redis.
- Produzir video de demonstracao.
