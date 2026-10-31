# Migrations SQL Server

Recovery.BackOffice.Migrations.SqlServer concentra as migrations por contexto, em Identity e Multitenancy, seguindo a separação de assemblies do FullStackHero. A API referencia esse projeto para que o EF consiga carregá-lo nas ferramentas. Program não aplica migrations nem ativa os contextos por causa dessa referência.

O manifesto .config/dotnet-tools.json fixa dotnet-ef 10.0.12, alinhado ao EF Core do projeto. Para trabalhar com migrations, execute na raiz:

```powershell
dotnet tool restore
dotnet build Recovery.BackOffice.slnx --configuration Release -warnaserror
```

As factories em src/Host/Recovery.BackOffice.Api/DesignTime permitem ao CLI construir os contextos sem iniciar a API. A configuração lê os appsettings opcionais do diretório atual, variáveis de ambiente e os argumentos posteriores a --, nessa ordem de precedência. O provider aceito é MSSQL e o assembly de migrations é fixado pelo marcador do projeto.

Sem conexão configurada, as factories usam SQL Server LocalDB com autenticação integrada e banco RecoveryBackOffice. Isso permite gerar modelos sem um servidor; a geração de migrations e scripts não abre a conexão. A factory do Identity fornece metadados de root apenas para construir o modelo. Não cria empresa, não configura tenant padrão em requisições e não insere credenciais.

As migrations iniciais são InitialTenantCatalog e InitialIdentity. Para gerar arquivos de SQL revisáveis, sem executá-los:

```powershell
dotnet ef migrations script --idempotent --project src/Host/Recovery.BackOffice.Migrations.SqlServer --startup-project src/Host/Recovery.BackOffice.Api --context Recovery.BackOffice.Multitenancy.Data.TenantDbContext --configuration Release --no-build --output artifacts/tenant-catalog.sql
dotnet ef migrations script --idempotent --project src/Host/Recovery.BackOffice.Migrations.SqlServer --startup-project src/Host/Recovery.BackOffice.Api --context Recovery.BackOffice.Identity.Data.IdentityDbContext --configuration Release --no-build --output artifacts/identity.sql
```

Os testes de migrations fazem parte da solução e são executados pelo verify.ps1 e pela pipeline existente. Verificam os snapshots, a composição real do Identity, geração de scripts idempotentes Up/Down e configuração das factories, sem abrir conexão. Alterar um modelo sem atualizar sua migration faz o teste de snapshot falhar.

Nenhum database update, Migrate ou execução de SQL faz parte desta etapa. A aplicação das migrations em banco local e os testes de leitura/gravação são o próximo passo. A implantação em produção também precisará configurar a conexão e aplicar migrations como etapa própria, seguindo o padrão do migrator do modelo; não haverá atualização de banco no startup da API.
