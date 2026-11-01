# Avaliação

Aplicação promovida para `C:\Projects\recovery-backoffice`: `src`, `clients` e `deploy` ficam diretamente na raiz. Identidade atual: `Recovery.BackOffice`, incluindo projetos, assemblies, namespaces e interfaces.

Commit upstream: `a967537526f3b769c7d268da3ed2a6920af65ce9`.

Geração a partir do template local dessa revisão, com Aspire e frontends, sem guias de agentes e sem restore automático. Namespaces atuais: `Recovery.BackOffice`, `Recovery.BackOffice.BuildingBlocks` e `Recovery.BackOffice.Modules`. Licença MIT preservada em `LICENSE` (o gerador exclui o arquivo LICENSE por padrão).

## Reprodução da geração

Os comandos abaixo são o histórico da geração inicial em subpasta, antes da promoção para a raiz. Não executá-los sobre a aplicação atual. O download e o registro isolado de templates foram temporários e foram removidos na personalização; `.evaluation/` continua ignorado.

```powershell

```

`main` remoto pode mudar: para reprodução exata, usar o commit registrado em vez de assumir equivalência de uma nova clonagem superficial. Não executar novamente a geração sobre arquivos existentes.

## Correção local de dependências

O build original falhou em NuGetAudit (NU1902/NU1903). Na cópia gerada, `src/Directory.Packages.props` foi ajustado: referências `10.0.12` passaram para `10.0.14`; pin transitivo de `System.Security.Cryptography.Pkcs` e `Microsoft.AspNetCore.SignalR.Common` em `10.0.14`; pin de `System.Security.Cryptography.Xml` atualizado para `10.0.14`. Não foi desativado NuGetAudit nem o tratamento de alertas como erros. Não houve alteração dos BuildingBlocks.

## Comandos locais de avaliação

```powershell
Set-Location C:\Projects\recovery-backoffice
dotnet build src/Recovery.BackOffice.slnx
dotnet test src/Tests/Recovery.BackOffice.Architecture.Tests --no-build
dotnet test src/Tests/Recovery.BackOffice.Identity.Tests --no-build

Set-Location clients/admin
npm.cmd ci
npm.cmd run build
```

Para a pilha completa, requer Docker ativo, portas livres, dependências dos dois frontends e certificados de desenvolvimento configurados. O AppHost provisiona dados e contas de demonstração, usa serviços locais e imagens de terceiros; não é configuração aprovada para produção Recovery.

```powershell
Set-Location C:\Projects\recovery-backoffice\clients\dashboard
npm.cmd ci
Set-Location ..\..
dotnet run --project src/Host/Recovery.BackOffice.AppHost
```

As portas e serviços estão no README gerado. Não reutilizar as credenciais de demonstração em ambiente real. Nenhum push é executado por este processo.

## Execução local verificada

Em 2026-10-31, admin em `http://localhost:5173`, dashboard em `http://localhost:5174` e API pronta em `http://localhost:5030/health/ready` responderam 200. Login demo do tenant `acme` com `admin@acme.com` e senha demo `Password123!` funcionou pelo proxy do dashboard; tokens não foram exibidos. Login root com o padrão do AppHost retornou 401; não foram redefinidas credenciais preexistentes.

Aspire iniciou migrador, seeder e API, mas falhou ao lançar os processos React neste Windows. Frontends foram iniciados separadamente com `npm.cmd run dev`, usando `VITE_API_BASE_URL=http://localhost:5030`. Para reproduzir, abrir dois consoles nas pastas correspondentes e executar esses comandos.
