# Continuidade do pipeline e Sonar

Configuração anterior consultada em `91f101a:azure-pipelines.yml`; pipeline adaptado nesta etapa para a cópia em raiz do repositório.

Preservados como referências (existência atual e autorização não verificadas no serviço): conexão Sonar `sc-recovery-backoffice-sonar`, organização `gauss-platform`, projeto `gauss-platform_recovery-backoffice`. A execução continua em main/develop; Sonar somente em main fora de PR, como antes. Branch policies de PR continuam configuradas no Azure DevOps, fora do YAML.

## Fluxo de estágios

`BackendQuality → FrontendBuild → Package`, com dependências sequenciais explícitas. Isso fornece o grafo de estágios conectados na interface do Azure DevOps; a disposição visual exata depende da tela disponível.

1. **Backend e testes**: instala .NET 10; prepara o Sonar quando aplicável; compila a solução; executa arquitetura primeiro, seguida por identidade, auditoria, infraestrutura e multitenancy, com cobertura; publica os relatórios; executa análise/Quality Gate e disponibiliza o pacote da API. Preparação, build e análise Sonar permanecem no mesmo job/workspace.
2. **Frontends**: após aprovação do backend, instala Node 24 e dependências com npm ci, verifica TypeScript e compila Admin/Dashboard. Não inclui E2E nem cobertura JavaScript.
3. **Artefatos**: após aprovação dos frontends, baixa somente os pacotes da execução atual e publica `recovery-backoffice`, contendo `api`, `admin` e `dashboard`. Os artefatos intermediários se chamam `backend-package` e `frontend-package`. Não há implantação automática.

As cinco suítes selecionadas somam 901 testes na versão atual. Não representam a suíte completa de integração/E2E. Resultados e cobertura são publicados mesmo se testes falharem; etapas seguintes exigem sucesso.

`scripts/verify-template.ps1` aceita `-Phase Backend`, `-Phase Frontend` ou `-Phase All` (padrão), com `-Configuration Release` por padrão. O backend é executado com build antes de testar e gerar o pacote. A fase frontend exige Windows/npm.cmd, consistente com o agente windows-latest.

## Versões fixadas das tarefas

Todas as referências task usam major.minor.patch para atender ao aviso de versão incompleta. SonarCloudPrepare, Analyze e Publish usam `4.2.6`, confirmado nos manifests publicados da [release SonarSource sc-4.2.6.2781](https://github.com/SonarSource/sonar-scanner-azdo/tree/sc-4.2.6.2781/src/extensions/sonarcloud/tasks). A sintaxe de versão completa é [suportada pelo Azure Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/tasks?view=azure-devops#task-versions).

As tarefas Microsoft foram conferidas na [release oficial v281](https://github.com/microsoft/azure-pipelines-tasks/releases/tag/v281): UseDotNet 2.281.2; NodeTool 0.281.0; PowerShell 2.281.0; PublishTestResults 2.279.0; PublishCodeCoverageResults 2.281.1; PublishPipelineArtifact 1.279.0; DownloadPipelineArtifact 2.279.0.

Pins exigem atualização explícita para receber correções futuras. O Azure permite executar somente versões disponíveis na organização; disponibilidade da extensão instalada, autorização da conexão Sonar, validação do YAML pelo serviço e execução remota permanecem pendentes. Nenhum pipeline remoto foi executado nesta etapa.

## Implantação pendente de adequação

Destino anterior: App Service `recovery-backoffice-api`, conexão `sc-recovery-backoffice-prod`. Não verificados como ativos. O template precisa de PostgreSQL, cache, armazenamento, email, segredos/chaves e hospedagem dos clientes. Aspire serve à orquestração local; publicar somente a DLL da API não reproduz a pilha.

Por isso, esta versão produz artefatos, mas não restaura a antiga tarefa automática de produção. A verificação antiga esperava literalmente Hello World e não valida esta aplicação. Preparar deployment concreto após confirmar recursos/configuração existentes e se o alvo é demonstração ou operação real. Não implantar seeds, contas ou credenciais demo em produção real.

Push permanece manual pelo solicitante. Nenhum segredo deve ser incluído no Git ou em parâmetros de console compartilhados na conversa.

Validação local desta revisão: YAML analisado sem erros, versões completas e vínculos entre artefatos conferidos; fase Backend em Release executada com sucesso, 901 testes aprovados, cinco relatórios TRX e cobertura Cobertura/OpenCover gerados, pacote da API confirmado. A fase Frontend não foi reexecutada nesta revisão; seus comandos npm ci/build foram preservados. Execução das tarefas Azure/Sonar e da consolidação no serviço continua pendente.
