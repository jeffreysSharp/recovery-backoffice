# Personalização Recovery BackOffice

Data: 2026-10-31. Branch: `codex/recovery-branding`. Baseline anterior preservada no commit `74167fd`.

## Identidade atual

Projetos, assemblies e namespaces usam `Recovery.BackOffice`, `Recovery.BackOffice.BuildingBlocks` e `Recovery.BackOffice.Modules`. A solução é `src/Recovery.BackOffice.slnx`. Os testes também usam o prefixo Recovery.BackOffice, com os respectivos vínculos InternalsVisibleTo atualizados.

Foram substituídos nomes de classes e métodos associados ao gerador, metadados dos projetos, marcas nos dois clientes, títulos, favicon e logotipo. O monograma R é provisório, não uma reprodução do logotipo corporativo. A CLI de geração foi retirada do manifesto de ferramentas; o template não é necessário para compilar ou executar a aplicação.

## Compatibilidade local

- Header de identificação da aplicação: `X-Recovery-App`; clientes e backend foram atualizados juntos.
- Chaves de armazenamento do navegador, cache e identificadores de autenticação foram renomeados. Sessões anteriores devem ser descartadas; autenticar novamente.
- O prefixo de volumes Docker deriva do novo AppHost. Na próxima execução, o Aspire usará novos nomes e poderá inicializar um novo ambiente de demonstração. Volumes anteriores não foram apagados e seus dados não foram migrados.
- PostgreSQL e os módulos funcionais existentes foram mantidos. A mudança não implementa as regras de negócio refinadas nem converte o banco para SQL Server.
- A pasta temporária de download e geração foi removida. A origem continua recuperável pelo commit upstream registrado nos avisos legais e no histórico.

## Rastreabilidade

A licença MIT original e os avisos de autoria permanecem em `LICENSE`. Referências de origem nos documentos históricos foram preservadas; não são dependências de execução nem identidade do produto.

Build da solução e dos dois clientes foi validado. Testes de arquitetura continuam verificando projetos e assemblies pelos novos nomes, incluindo a detecção de referências proibidas entre módulos. A execução completa com serviços Docker e os testes E2E não fazem parte desta validação.

Validação final: solução compilada com zero avisos/erros; admin e dashboard compilados; 901 testes aprovados (identidade 340, arquitetura 55, auditoria 68, infraestrutura 346, multitenancy 92). Frontends mantêm aviso de tamanho de chunks do Vite. Nenhum commit ou push foi executado nesta personalização.
