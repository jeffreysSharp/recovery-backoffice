# Identidade e ambiente de testes — investigação inicial

Estado: opções avaliadas; nenhuma infraestrutura provisionada. Consulta às fontes Microsoft em 2026-10-31.

## Contexto

Há públicos interno, parceiro e devedor. Entra ID corporativo é uma hipótese. A conta Azure gratuita do desenvolvedor não comprova acesso ao tenant corporativo nem licença de recursos avançados.

## Opções a validar

- Entra ID Free pode atender testes de autenticação de colaboradores e disponibiliza security defaults. Políticas específicas de Conditional Access exigem licenciamento correspondente.
- Entra External ID possui oferta básica com os primeiros 50.000 usuários ativos mensais sem custo. Recursos adicionais e autenticação por máquina podem ter cobrança. É candidato para identidade de clientes; desenho para parceiros exige avaliação própria.
- Autenticação e autorização de negócio são distintas: o provedor identifica a pessoa; a aplicação valida ações e escopos.
- Hospedagem, banco, email, logs e rede possuem limites e cobranças próprios. Gratuidade de identidade não implica ambiente inteiro gratuito.
- App Service Free/Shared é voltado a desenvolvimento/testes; a Microsoft informa que cargas de produção não são suportadas nesses planos.

## Direção proposta

Começar com homologação publicada, isolada do ambiente corporativo e com dados sintéticos. Validar os fluxos antes de escolher a composição final de hospedagem e licenças. Operação real exige definição de disponibilidade, dados, integrações e capacidade; não é equivalente a uma demonstração acessível pela internet.

## Fontes oficiais

- [Licenciamento Entra](https://learn.microsoft.com/en-us/entra/fundamentals/licensing).
- [Preços External ID](https://www.microsoft.com/en-us/security/pricing/microsoft-entra-external-id/).
- [Modelo de cobrança External ID](https://learn.microsoft.com/en-us/entra/external-id/external-identities-pricing).
- [Planos App Service](https://azure.microsoft.com/en-us/pricing/details/app-service/windows/).
