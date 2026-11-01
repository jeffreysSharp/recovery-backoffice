# Exemplo opcional de hospedagem AWS

Estes módulos Terraform são exemplos de infraestrutura AWS e não são usados na homologação Azure atual. Não foram aplicados nesta alteração.

O banco foi desacoplado da infraestrutura de compute: provisione SQL Server separadamente e informe um segredo existente em Secrets Manager com a conexão SQL Server completa, usando `db_connection_secret_arn` e `db_use_connection_secret=true`. O stack não cria servidor de banco e mantém os demais módulos de rede, ECS, armazenamento e monitoramento.

Os exemplos de tfvars precisam ser preenchidos para a conta de destino. Qualquer estado existente deve ser revisado com terraform plan antes de uma atualização; a remoção do módulo antigo de banco exige tratar esse recurso separadamente para preservar os dados.

A configuração de Azure SQL e os comandos locais estão em docs/04-arquitetura/sql-server-homologacao.md na raiz do repositório.
