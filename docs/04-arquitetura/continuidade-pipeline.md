# Continuidade do pipeline e Sonar

Configuração anterior consultada em `91f101a:azure-pipelines.yml`; pipeline adaptado nesta etapa para a cópia em `evaluation/Recovery.TemplateEvaluation`.

Preservados como referências (existência atual e autorização não verificadas no serviço): conexão Sonar `sc-recovery-backoffice-sonar`, organização `gauss-platform`, projeto `gauss-platform_recovery-backoffice`. A execução continua em main/develop; Sonar somente em main fora de PR, como antes. Branch policies de PR continuam configuradas no Azure DevOps, fora do YAML.

CI prepara .NET 10/Node 24, compila solução, executa Identity.Tests e Architecture.Tests com cobertura e publica API/admin/dashboard. Não equivale à suíte completa. `scripts/verify-template.ps1` permite executar localmente; validação no agente Azure e no Sonar ainda pendente. Nenhum pipeline remoto foi executado nesta etapa.

## Implantação pendente de adequação

Destino anterior: App Service `recovery-backoffice-api`, conexão `sc-recovery-backoffice-prod`. Não verificados como ativos. O template precisa de PostgreSQL, cache, armazenamento, email, segredos/chaves e hospedagem dos clientes. Aspire serve à orquestração local; publicar somente a DLL da API não reproduz a pilha.

Por isso, esta versão produz artefatos, mas não restaura a antiga tarefa automática de produção. A verificação antiga esperava literalmente Hello World e não valida esta aplicação. Preparar deployment concreto após confirmar recursos/configuração existentes e se o alvo é demonstração ou operação real. Não implantar seeds, contas ou credenciais demo em produção real.

Push permanece manual pelo solicitante. Nenhum segredo deve ser incluído no Git ou em parâmetros de console compartilhados na conversa.
