# Baseline FullStackHero preservada

Objetivo confirmado pelo solicitante: guardar uma versão do template funcional localmente como ponto de retorno; adaptar às necessidades Recovery aos poucos. Implantação de produção adiada. Push somente pelo solicitante, via console.

## Origem e alterações

- Origem: `fullstackhero/dotnet-starter-kit`, commit `a967537526f3b769c7d268da3ed2a6920af65ce9`.
- Cópia gerada: `Recovery.TemplateEvaluation`, com backend, Aspire e dois clientes React.
- Geração renomeia projetos da aplicação; namespaces FSH de módulos e BuildingBlocks permanecem conforme template.
- Licença MIT original preservada.
- Alterações funcionais de negócio Recovery: nenhuma aplicada.
- Alterações técnicas: atualização de dependências centrais .NET para corrigir bloqueios de auditoria; portas auxiliares de desenvolvimento para evitar conflitos locais. Detalhes em README e relatório de avaliação.
- SMTP de demonstração externo substituído por localhost e placeholders antes do commit; email não foi validado e exige serviço/configuração local. Nenhuma credencial SMTP externa deve ser preservada na cópia versionada. Padrões públicos locais de bootstrap/S3 são somente demonstração, não credenciais de produção.
- Pipeline/Sonar adaptados fora da cópia como preparação de CI; não validados remotamente e sem deploy automático.

Portanto, trata-se de baseline derivada e rastreável, não cópia byte a byte do upstream. O commit upstream permite recuperar o original exato.

## Estado verificado

- Build backend: sucesso após ajuste de dependências, auditoria ativa.
- Identity.Tests: 340 aprovados; Architecture.Tests: 55 aprovados.
- Builds de admin e dashboard: sucesso.
- Execução local: frontends e health da API respondem 200; login demo acme verificado.
- Frontends iniciados separadamente com npm.cmd devido a falha do lançador Aspire neste Windows.
- Login root com senha padrão não validado (401); nenhuma credencial preexistente redefinida.
- Nenhuma garantia de atendimento às regras Recovery, de implantação produtiva ou de aprovação de segurança completa.

## O que fica fora do Git

Download upstream, registro local de templates, bin/obj, node_modules, dist, logs de execução e artefatos. Serviços/volumes Docker e bancos locais não são preservados pelo commit; dados de demonstração devem ser reproduzidos pelo seeder. Credenciais corporativas não devem ser adicionadas.

## Evolução

Usar esta branch/commit como ponto de comparação. Separar próximas mudanças em commits por objetivo: primeiro corrigir limitações locais remanescentes; depois adaptar identidade, autorização e portais. Não continuar implantação produtiva nesta etapa.
