# DbMigrator

Recovery.BackOffice.DbMigrator é um console separado da API, seguindo a organização do FullStackHero. Ele aplica primeiro as migrations do catálogo, garante o tenant root e depois aplica as migrations do Identity para cada empresa cadastrada. As empresas com ConnectionString própria usam seu banco; as demais usam a conexão principal.

O seed inicial preserva os dados do modelo: identificador root, nome Root, e-mail administrativo e issuer constantes, ativação e validade inicial de um ano. Um root já existente não é alterado. Isso cria somente a empresa; nenhum usuário administrador ou senha é criado nesta etapa.

O comando disponível é apply. Sem argumentos ou com --help, o console imprime ajuda e termina sem acessar banco. Essa escolha explícita difere do default apply do clone: o futuro AppHost fornecerá o argumento apply ao executar o recurso. Não foram importados seed-demo, list-pending ou opções de módulos ainda ausentes.

DatabaseOptions:ConnectionString é obrigatória, o provider aceito é MSSQL e o assembly de migrations é o do Recovery. A configuração vem do host .NET, incluindo variáveis como DatabaseOptions__ConnectionString e argumentos de configuração posteriores a apply. Não há senha ou conexão padrão embutida no migrator.

MigrationRunner mantém a ordem da execução e interrompe o processo quando uma etapa falha ou recebe cancelamento. EfMigrationOperations executa o I/O do EF; IMigrationOperations é uma fronteira restrita ao console para testar o fluxo sem banco. Os contextos são criados e descartados por operação. O migrator não é registrado na API, que continua sem aplicar migrations no startup.

O console retorna 0 no sucesso e 1 na falha. As migrations são progressivas e usam o histórico do EF. Se uma etapa falhar depois de outras concluírem, o processo pode ser executado novamente após corrigir a causa; não há rollback automático dos bancos já migrados. A coordenação do processo como recurso único do ambiente será feita pelo AppHost.

Os testes verificam ordem, interrupção em falhas, cancelamento, propagação do token, configuração obrigatória e seleção de bancos. A factory também é comparada aos snapshots existentes. Os testes de fluxo substituem as operações de banco: ainda não comprovam execução real de DDL ou seed. Nenhum banco foi modificado para preparar esta etapa.

O AppHost ainda será adicionado para iniciar SQL Server, passar a conexão ao migrator e aguardar seu término antes de iniciar a API, como no modelo. A pipeline atual valida o novo projeto e seus testes, mas continua publicando somente a API.
