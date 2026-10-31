# Cadastros — regras transversais

Fonte E36: respostas do solicitante em 2026-10-31. Aplicam-se como base aos cadastros básicos; não substituem campos e validações específicos, cujo detalhamento foi adiado.

| ID | Regra | Estado |
|---|---|---|
| CAD-001 | Usar inativação/reativação como padrão, preservando referências e histórico | Aprovada; exceções de descarte exigem decisão própria |
| IMP-001 | Disponibilizar importação em lote por planilha/CSV, com validação e relatório de erros | Aprovada em capacidade; cadastros elegíveis e formatos exatos pendentes |
| CON-001 | Detectar edição concorrente e solicitar revisão da versão atual, sem sobrescrita silenciosa | Aprovada; mecanismo técnico pendente |
| IMP-002 | Rejeitar todo o lote se houver qualquer linha inválida | Aprovada, E37 |
| IMP-003 | Importação inicialmente somente cria novos registros; não atualiza existentes | Aprovada, E37 |
| IMP-004 | Somente Recovery pode importar, mediante permissão específica | Aprovada, E37; não concedida automaticamente ao perfil global comum |
| CAD-002 | Inativos permanecem consultáveis administrativamente por filtro e não podem ser usados em novos vínculos | Aprovada, E38 |
| CAD-003 | Somente Recovery mantém cadastros básicos globais, mediante permissões específicas | Aprovada, E38 |

E37: respostas do solicitante em 2026-10-31. Essas decisões resolvem comportamento parcial, atualização e autoridade mencionados como pendentes no registro inicial. Formatos, cadastros elegíveis, chave de duplicidade e limites continuam abertos.

Critérios adicionais: lote com uma linha inválida não cria nenhuma das demais; importação não altera registros já existentes; usuário de escritório não importa mesmo que possa cadastrar manualmente. Proposta: falha durante persistência também deve impedir gravação parcial, mantendo consistência do lote. Estratégia transacional e tratamento de concorrência ainda serão definidos.

Duplicatas no arquivo ou contra o cadastro devem ser identificadas conforme chave específica do cadastro, ainda a definir. Reenvio após falha ou interrupção não deve causar criação duplicada; mecanismo de identificação do lote pendente. Não tratar importação de existentes como atualização implícita.

## Critérios iniciais

- Inativar preserva o identificador e referências existentes. Uso de registros inativos em novos vínculos e consultas será definido por cadastro.
- Importação identifica registros inválidos e informa erros que permitam correção. Colunas, regras de atualização, limites e comportamento parcial/atômico ainda serão especificados.
- Se duas pessoas editarem a mesma versão de um registro, a segunda gravação baseada em versão desatualizada não sobrescreve silenciosamente a primeira. Apresentar conflito para revisão, preservando proposta de edição quando viável.

## Propostas de segurança e operação

- Importação respeita ação autorizada, escritório ativo e limites de dados; arquivo não concede poderes adicionais.
- Validar cada registro no backend, não apenas o formato do arquivo.
- Aplicar controle de concorrência também em atualizações por importação, quando suportadas.
- Relatórios não devem revelar dados de outros escopos nem segredos. Formato de exportação precisa impedir execução involuntária de conteúdo como fórmulas.
- Registrar autor e resultado das operações em lote; cobertura exata da auditoria de cadastros ainda a decidir.

Esses controles são propostas para refinamento. Formato XLSX, biblioteca, processamento assíncrono, limites de tamanho/linhas e ferramenta de armazenamento não foram escolhidos.
