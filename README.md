# Recovery BackOffice

Backend modular .NET 10 e aplicações React para a plataforma Recovery. As regras de negócio serão incorporadas progressivamente; a personalização não representa conclusão funcional do MVP.

## Estrutura

- `src/BuildingBlocks`: infraestrutura compartilhada.
- `src/Modules`: módulos e contratos públicos.
- `src/Host`: API, AppHost, migrador e migrações PostgreSQL.
- `src/Tests`: testes unitários, integração e arquitetura.
- `clients/admin` e `clients/dashboard`: aplicações React.
- `docs`: regras, decisões e referência técnica.

## Execução local

Requer .NET 10 SDK, Node.js e Docker ativo.

```powershell
dotnet build src/Recovery.BackOffice.slnx
dotnet run --project src/Host/Recovery.BackOffice.AppHost
```

Se o Aspire não iniciar os clientes no Windows, execute `npm.cmd ci` e `npm.cmd run dev` nas pastas `clients/admin` e `clients/dashboard`. Admin usa 5173, dashboard 5174 e API 5030. As configurações e dados de demonstração precisam de revisão antes de produção.

Veja [documentação](docs/README.md), [licença de origem](LICENSE). Push e publicação são feitos pelo responsável pelo repositório após revisão.
