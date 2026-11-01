# Migrador SQL Server / Azure SQL

Processo independente da API que aplica as migrações de todos os módulos e, quando solicitado, dados iniciais. A sincronização entre instâncias usa sp_getapplock com lock de sessão no banco de catálogo.

Configuração por variáveis de ambiente:

- DatabaseOptions__Provider: MSSQL
- DatabaseOptions__MigrationsAssembly: Recovery.BackOffice.Migrations.SqlServer
- DatabaseOptions__ConnectionString: conexão com o banco de aplicação, nunca master
- Seed__DefaultAdminPassword: segredo necessário ao criar a conta inicial com --seed

Exemplo de invocação, após configurar os segredos no console local:

```powershell
dotnet run --project src/Host/Recovery.BackOffice.DbMigrator -c Release -- apply --seed
```

Comandos adicionais: `list-pending`, `apply`, `seed`, `seed-demo`; consulte `--help` para filtros de catálogo e tenant. `seed-demo` só funciona em Development e deve ser usado apenas com dados fictícios. No Azure, o banco deve existir antes de executar o migrador. No SQL Server local, o migrador pode criar o banco de aplicação quando o login possui permissão.

As migrações criam 11 schemas/contextos e preservam o modelo de isolamento por tenant. As conexões de administração/migração e de execução devem ser configuradas separadamente no ambiente de homologação. Nunca versionar senhas.
