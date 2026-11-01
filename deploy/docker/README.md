# Docker local — Recovery BackOffice

A pilha usa SQL Server 2022 Developer, cache Valkey, armazenamento S3 compatível, Mailpit, migrador e clientes. SQL Server Developer é destinado a desenvolvimento e testes; a homologação gerenciada usa Azure SQL Database.

1. Copie `.env.example` para `.env` e configure os valores exigidos, incluindo `MSSQL_SA_PASSWORD` com senha forte.
2. Execute `docker compose up -d --build` nesta pasta.
3. O migrador prepara os schemas antes da API. Acompanhe com `docker compose logs migrator api`.

O volume do banco se chama `sqlserver_data`. Não execute exclusão de volumes para corrigir configurações: isso remove dados. O script de migração consegue criar o banco de aplicação no SQL Server local; no Azure SQL, o banco deve ser provisionado previamente.

Para usar banco externo, remova a dependência do serviço local sqlserver e configure DatabaseOptions__Provider=MSSQL, DatabaseOptions__MigrationsAssembly=Recovery.BackOffice.Migrations.SqlServer e DatabaseOptions__ConnectionString por segredo de ambiente. No Azure, use Encrypt=True e TrustServerCertificate=False.

Imagens/clientes e configuração de email precisam ser ajustados ao ambiente. Este compose não é a configuração de deployment da homologação Azure.
