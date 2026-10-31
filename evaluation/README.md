# Avaliação FullStackHero

Projeto gerado em `Recovery.TemplateEvaluation`, separado da aplicação principal e sem adoção definitiva.

Origem: https://github.com/fullstackhero/dotnet-starter-kit

Commit upstream: `a967537526f3b769c7d268da3ed2a6920af65ce9`.

Geração a partir do template local dessa revisão, com Aspire e frontends, sem guias de agentes e sem restore automático. Namespace de aplicação: `Recovery.TemplateEvaluation`; bibliotecas FSH mantêm os namespaces originais, conforme o template. Licença MIT preservada em `Recovery.TemplateEvaluation/LICENSE` (o gerador exclui o arquivo LICENSE por padrão).

## Reprodução da geração

Os comandos abaixo foram executados a partir da raiz do repositório. `.evaluation/` está ignorado no Git e contém download upstream e registro isolado de templates.

```powershell
git clone --depth 1 https://github.com/fullstackhero/dotnet-starter-kit.git .evaluation/fullstackhero-upstream
git -C .evaluation/fullstackhero-upstream rev-parse HEAD
dotnet new install .evaluation/fullstackhero-upstream --debug:custom-hive .evaluation/template-hive
dotnet new fsh -n Recovery.TemplateEvaluation -o evaluation/Recovery.TemplateEvaluation --aspire true --frontend true --agents false --skipRestore true --debug:custom-hive .evaluation/template-hive
```

`main` remoto pode mudar: para reprodução exata, usar o commit registrado em vez de assumir equivalência de uma nova clonagem superficial. Não executar novamente a geração sobre arquivos existentes.

## Correção local de dependências

O build original falhou em NuGetAudit (NU1902/NU1903). Na cópia gerada, `src/Directory.Packages.props` foi ajustado: referências `10.0.12` passaram para `10.0.14`; pin transitivo de `System.Security.Cryptography.Pkcs` e `Microsoft.AspNetCore.SignalR.Common` em `10.0.14`; pin de `System.Security.Cryptography.Xml` atualizado para `10.0.14`. Não foi desativado NuGetAudit nem o tratamento de alertas como erros. Não houve alteração dos BuildingBlocks.

## Comandos locais de avaliação

```powershell
Set-Location C:\Projects\recovery-backoffice\evaluation\Recovery.TemplateEvaluation
dotnet build src/Recovery.TemplateEvaluation.slnx
dotnet test src/Tests/Architecture.Tests --no-build
dotnet test src/Tests/Identity.Tests --no-build

Set-Location clients/admin
npm.cmd ci
npm.cmd run build
```

Para a pilha completa, requer Docker ativo, portas livres, dependências dos dois frontends e certificados de desenvolvimento configurados. O AppHost provisiona dados e contas de demonstração, usa serviços locais e imagens de terceiros; não é configuração aprovada para produção Recovery.

```powershell
Set-Location C:\Projects\recovery-backoffice\evaluation\Recovery.TemplateEvaluation\clients\dashboard
npm.cmd ci
Set-Location ..\..
dotnet run --project src/Host/Recovery.TemplateEvaluation.AppHost
```

As portas e serviços estão no README gerado. Não reutilizar as credenciais de demonstração em ambiente real. Nenhum push é executado por este processo.

## Execução local verificada

Em 2026-10-31, admin em `http://localhost:5173`, dashboard em `http://localhost:5174` e API pronta em `http://localhost:5030/health/ready` responderam 200. Login demo do tenant `acme` com `admin@acme.com` e senha demo `Password123!` funcionou pelo proxy do dashboard; tokens não foram exibidos. Login root com o padrão do AppHost retornou 401; não foram redefinidas credenciais preexistentes.

Aspire iniciou migrador, seeder e API, mas falhou ao lançar os processos React neste Windows. Frontends foram iniciados separadamente com `npm.cmd run dev`, usando `VITE_API_BASE_URL=http://localhost:5030`. Para reproduzir, abrir dois consoles nas pastas correspondentes e executar esses comandos.

Portas auxiliares foram ajustadas somente na cópia: pgAdmin 15050, RedisInsight 15540, RustFS 19000/19001, para evitar conflitos com serviços já existentes. Nada foi removido dos containers antigos.
