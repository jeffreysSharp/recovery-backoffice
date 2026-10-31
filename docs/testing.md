# Padrão de testes

## Organização

```text
tests/
  Directory.Build.props
  coverage.runsettings
  Unit/
    Modules/
      Identity/
        Recovery.BackOffice.Identity.UnitTests/
  Integration/
    Host/
      Recovery.BackOffice.Api.IntegrationTests/
```

Novos testes unitários acompanham o módulo em `Unit/Modules/<Module>`; bibliotecas compartilhadas usarão `Unit/BuildingBlocks/<Library>`. Testes que combinam componentes ficam em `Integration`, separados por host ou módulo. Criar projetos apenas quando houver comportamento a testar. Testes arquiteturais e E2E terão categorias próprias quando necessários.

## Convenções

- xUnit com assertions nativas; adicionar mocks ou bibliotecas de assertions somente quando necessários.
- Arquivo/classe `<Componente>Tests`; métodos `Metodo_Should_Resultado_When_Condicao`.
- Organizar Arrange, Act e Assert; validar resultados observáveis e falhas relevantes.
- Usar Fact para um cenário e Theory para o mesmo comportamento com diferentes entradas.
- Cada teste deve ser independente, determinístico e liberar seus recursos.
- Testes unitários não acessam banco, rede ou serviços externos.
- Integração HTTP usa WebApplicationFactory em Production; não substitui o smoke test do artefato publicado.
- Integração futura de persistência usará SQL Server real e dados isolados, sem usar EF InMemory como prova de comportamento SQL.
- Não testar a existência de interfaces/classes ou reproduzir a implementação apenas para aumentar cobertura.
- Os testes iniciais de Identity verificam somente suas proteções contra null; ainda não existe autenticação para testar.

## Execução e critérios

Executar `./scripts/verify.ps1` na raiz. O script restaura, verifica formatação, compila, executa testes com cobertura, publica e executa smoke test. Warnings são erros em todos os projetos pelo Directory.Build.props da raiz.

O comando de testes também pode ser executado separadamente:

```powershell
dotnet test Recovery.BackOffice.slnx --configuration Release --logger trx --collect "XPlat Code Coverage" --settings tests/coverage.runsettings
```

Uma execução válida exige testes descobertos, todos aprovados, nenhum ignorado e relatórios presentes. A pipeline publica TRX e cobertura Cobertura, inclusive após falhas para diagnóstico. Os arquivos OpenCover são preparados para integração futura com Sonar. Relatórios ficam em artifacts e não entram no Git.

Não há limite percentual de cobertura implementado neste incremento. Cobertura é evidência complementar; não comprova ausência de bugs. O quality gate do Sonar será configurado depois de identificar organização, projeto e plano, sem excluir código de produção para melhorar métricas.

## SonarQube Cloud

Organização: `gauss-platform`. Projeto: `gauss-platform_recovery-backoffice`. Service connection: `sc-recovery-backoffice-sonar`, do tipo SonarQube Cloud. Tokens são armazenados na service connection, nunca no repositório ou no chat. A extensão oficial SonarQube Cloud deve fornecer as tarefas de versão 4.

A preparação do scanner ocorre antes do build; análise e validação do quality gate ocorrem antes de publicar o artefato/deploy. O scanner C# consome `artifacts/test-results/**/coverage.opencover.xml` e os TRX. `sonar.qualitygate.wait=true` exige o resultado do gate e falha quando ele é reprovado ou não chega dentro de 300 segundos. A tarefa Publish apenas acrescenta o resultado ao resumo. Falha no gate impede o deploy.

A análise foi limitada à `main` para compatibilidade com a restrição de branches do plano gratuito; não existe análise Sonar das features, develop ou PRs neste incremento. Testes continuam obrigatórios em todas essas execuções. Não fingir que o gate da main protege um PR antes do merge. Confirmar o plano antes de habilitar análise de branches/PRs.

O projeto usa Previous version como definição de código novo. `projectVersion: 0.2.0` representa a próxima release e deve mudar somente a cada release, não a cada build. A primeira análise estabelece a base. A tag Git não define esse parâmetro.

Antes da primeira execução na main, conferir no Sonar: projeto vinculado ao repositório Azure Repos correto, análise configurada via CI e quality gate atribuído. Se Automatic Analysis estiver habilitada, desabilitá-la para usar esta integração de CI. Autorizar somente esta pipeline para acessar a service connection. A integração remota ainda requer a primeira execução para comprovar conexão, importação de cobertura e aprovação do quality gate.

Referências oficiais:

- https://docs.sonarsource.com/sonarqube-cloud/analyzing-source-code/ci-based-analysis/azure-pipelines/adding-analysis-to-build-pipeline/dotnet-project
- https://docs.sonarsource.com/sonarqube-cloud/administering-sonarcloud/managing-subscription/subscription-plans
