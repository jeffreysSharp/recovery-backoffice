# Auditoria e revogação

## Regra confirmada

AUD-002 (E31), aprovada pelo solicitante em 2026-10-31: registrar alterações de módulos, menus, URLs remotas, versões e permissões com quem alterou, data, contexto e diferenças antes/depois.

O escopo inclui mudança de endereço/versão e retorno explícito de microfrontend. Representação técnica dos eventos, identificação do ator e do contexto ainda serão detalhadas.

## Decisões E32 — 2026-10-31

| ID | Regra | Estado |
|---|---|---|
| AUD-003 | Recovery pode consultar todos os contextos; escritório consulta somente seu contexto, mediante permissão específica | Aprovada |
| AUD-004 | Impedir edição e exclusão manual de eventos; descarte somente por política de retenção controlada | Aprovada; prazo de retenção pendente |
| AUD-005 | Incluir logins, falhas de autenticação, logout, recuperação de senha/MFA e negativas de acesso | Provisória; revisar volume com o time |

Consulta de auditoria é capacidade administrativa explícita; não concedê-la ao perfil global somente por conter todas as ações comuns. O alcance de leitura Recovery não aprova automaticamente uma consulta agregada sem escritório ativo: conciliar com IAM-015 antes de implementar pesquisa entre contextos.

Eventos de conta global ou sem contexto (por exemplo, login inválido anterior à seleção) precisam de política de visibilidade própria. Não replicar automaticamente dados pessoais ou vínculos de outros escritórios no histórico apresentado a um escritório.

Critérios adicionais: API de consulta impede leitura de outro contexto por administrador de escritório, mesmo alterando filtros; não disponibilizar edição/exclusão manual; remoção por retenção segue processo controlado a definir. Proibição funcional de edição/exclusão não comprova imutabilidade do armazenamento: definir controles operacionais para contas técnicas e administradores de infraestrutura.

## Propostas de proteção dos registros

## Decisões E33 — 2026-10-31

- AUD-006: permitir pesquisa agregada de vários escritórios pela Recovery, mediante permissão específica. Aprovada. Exceção limitada à consulta de auditoria; não libera outras operações sem contexto ativo.
- AUD-007: alterações sensíveis podem prosseguir com envio posterior da auditoria somente se houver registro durável garantido. Aprovada. Sem conseguir persistir esse registro, não concluir a alteração como bem-sucedida.
- Retenção: estimar volume antes de propor prazo provisório; nenhum prazo aprovado nesta etapa.

AUD-006 resolve a questão de consulta agregada mencionada acima. Visibilidade de eventos globais/sem contexto para escritórios continua pendente; acesso agregado exige autorização administrativa explícita e não decorre do perfil global comum.

Critérios AUD-007: indisponibilidade do destino final não impede alteração se o evento estiver duravelmente persistido; falha de persistência do evento impede conclusão da alteração. Definir mecanismo para consistência entre alteração e registro, repetição sem duplicação e recuperação após falha. Fila apenas em memória não satisfaz durabilidade. Não foi escolhida tecnologia de fila, banco ou padrão de implementação.

## Estimativa de retenção

Levantar usuários ativos, sessões/dia, alterações administrativas, negativas e falhas por dia, tamanho médio de evento, picos, índices, réplicas e cópias de segurança. Estimar volume diário e crescimento; separar classes de eventos se necessário. Só então propor prazo e custo ao time, sujeito também às políticas corporativas de dados. Ausência de prazo não autoriza retenção indefinida.

- Manter identificador estável do ator e recurso, horário consistente, escritório/portal quando aplicável, resultado e correlação da operação.
- Não gravar senhas, tokens, códigos MFA ou conteúdo secreto em diferenças antes/depois.
- Tratar valores pessoais com proteção e acesso limitado; registrar referência ao recurso quando uma cópia completa for desnecessária.
- Impedir edição de eventos pela interface administrativa comum; definir retenção, descarte e controles contra alteração no armazenamento.
- Tratar exportação da auditoria como ação administrativa própria, sujeita a escopo e auditoria.

As propostas de impedir edição/exclusão manual e limitar leitores foram refinadas por AUD-003/004. Demais controles permanecem propostas. Prazo de retenção, exportação, armazenamento, eventos globais e comportamento em falha do registro continuam abertos.

## Critérios iniciais

- Após uma alteração bem-sucedida dos recursos cobertos, existir evento com ator, data, contexto e diferenças relevantes.
- Retorno de versão também registrar estado anterior e novo.
- Alteração recusada não deve aparecer como mudança bem-sucedida; cobertura de tentativas negadas ainda a aprovar.
- Testar proteção das credenciais antes de disponibilizar consultas ou exportações de eventos.

## Revogação — referências atuais

- CPF/email: invalidar sessões, senhas temporárias e recuperações pendentes após efetivação (USR-015); até 1 minuto, provisoriamente (SES-002).
- Mudança IP e retirada de permissões: até 1 minuto nas sessões existentes, provisoriamente (SES-006).
- Disponibilidade de funcionalidade/módulo: backend bloqueia próxima requisição após salvar; navegação reflete em até 1 minuto (PRT-008/010).

Revogação não desfaz operações concluídas. Processamento já iniciado e falha de auditoria ainda precisam de tratamento específico.
