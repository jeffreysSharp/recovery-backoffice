# Usuários — cadastro e ciclo de vida

Fonte E11: respostas do solicitante em 2026-10-31. Complementa [administração de identidade](administracao-de-identidade.md).

## Regras confirmadas

| ID | Regra | Estado |
|---|---|---|
| USR-004 | Cadastro inicial exige nome completo, email, documento e tipo de usuário | Aprovada como lista inicial, sujeita a refinamento |
| USR-005 | País não é necessário no cadastro de usuário | Aprovada; não elimina o cadastro básico de países nem resolve políticas geográficas de outros recursos |
| USR-006 | Primeiro acesso usa senha temporária com troca obrigatória | Aprovada; entrega, validade e política de senha pendentes |
| USR-007 | Desativação local retira somente o vínculo de acesso ao escritório administrado | Aprovada |
| USR-008 | Desativação global da conta é separada e reservada à administração autorizada Recovery | Aprovada |
| USR-009 | Login do Backoffice usa CPF; somente CPF é aceito como documento de usuários internos e de escritórios | Aprovada em E12 |
| USR-010 | Cada pessoa possui uma única conta, utilizada em todos os seus escritórios | Aprovada em E12; CPF identifica a conta |
| USR-011 | Sistema gera senha temporária e a envia ao email individual do usuário | Aprovada em E12; validade, reemissão e tratamento de falhas pendentes |
| USR-012 | Senha temporária vale por 24 horas; reemissão invalida imediatamente a anterior | Aprovada em E13 |
| USR-013 | Vínculo de uma conta já existente a outro escritório exige autorização da Recovery | Aprovada em E13; fluxo operacional pendente |
| USR-014 | Administrador do escritório também pode alterar CPF e email da conta única de seus usuários | Parcialmente substituída em E15: CPF exclusivo Recovery; edição administrativa de email a esclarecer |
| USR-015 | Ao efetivar alteração de CPF ou email, encerrar sessões da conta e invalidar senhas temporárias e solicitações de recuperação pendentes | Aprovada em E14; prazo e implementação a definir |
| USR-016 | Somente administração autorizada Recovery pode alterar CPF | Aprovada em E15; substitui autorização de CPF em USR-014 |
| USR-017 | Usuário pode alterar seu próprio email no perfil pessoal | Aprovada em E15; confirmação e recuperação pendentes |
| USR-018 | Usuário dispõe de perfil pessoal com foto e configurações visuais do portal, incluindo cores e layout | Aprovada em escopo em E15; campos e opções a refinar |
| SES-002 | Revogação em bloqueios, desativações e alterações de CPF/email produz efeito nas requisições protegidas seguintes em até 1 minuto da efetivação | Provisória em E15; sujeita a validação do time |
| USR-019 | Próprio usuário e administrador do escritório podem editar email | Aprovada em E16; respeitar vínculo e alcance administrativo |
| USR-020 | Alteração de email pelo próprio usuário exige confirmação do novo endereço antes de efetivar | Aprovada em E16; fluxo administrativo ainda a confirmar |
| SES-003 | Uma única sessão por conta; novo login bem-sucedido encerra a sessão anterior | Aprovada em E16; definição de sessão e mecanismo a detalhar |
| USR-021 | Alteração de email pelo administrador exige confirmação do novo endereço pelo destinatário antes de efetivar | Aprovada em E17 |

Atualização E17: USR-020/021 aplicam a confirmação ao autosserviço e à edição administrativa. Até confirmar, manter email efetivo anterior; a efetivação dispara USR-015. Aviso ao endereço antigo permanece sem aprovação específica.

Fonte E16: respostas do solicitante em 2026-10-31. A confirmação de novo email no autosserviço não aprova automaticamente aviso ao endereço antigo nem resolve o processo administrativo recusado em E14. Não revogar sessão anterior por simples tentativa de login inválida; este é um critério proposto para SES-003.

## Email — fluxo de autosserviço confirmado

1. Usuário solicita troca do próprio email.
2. Novo endereço permanece pendente até confirmação; formato, unicidade, prazo e mecanismo a definir.
3. Antes de confirmar, novo endereço não substitui email efetivo nem destino de recuperação.
4. Ao confirmar, efetivar email e aplicar USR-015/SES-002: encerrar sessões e invalidar senhas temporárias/recuperações pendentes no prazo provisório de até 1 minuto.

Tratamento de múltiplas solicitações, confirmação expirada e falha de entrega precisa de refinamento. E17 confirma que edição pelo administrador também exige confirmação do destinatário antes da efetivação.

Fonte E15: respostas do solicitante em 2026-10-31. Perfil pessoal é distinto de perfil de permissões. Preferência visual não concede acesso. Foto ou mudança visual isolada não exige encerrar sessões pela regra USR-015. SES-002 cobre a retirada do acesso afetado: desativação local preserva outros escritórios; alteração global alcança todos. Operações concluídas não são desfeitas.

Estado atual prevalece sobre observações históricas abaixo: CPF exclusivo da Recovery, email editável pelo próprio usuário e prazo provisório de até 1 minuto. Processo de confirmação do email e participação administrativa na edição continuam abertos.

Fonte E14: respostas do solicitante em 2026-10-31. O fluxo proposto de confirmar novo email e avisar endereço anterior não foi aprovado. A proposta de correção de CPF com justificativa e reautenticação também não foi aprovada; ambos exigem outro processo, ainda a descrever. Isso não constitui aprovação de alteração irrestrita ou dispensa de qualquer controle.

Fonte E13: respostas do solicitante em 2026-10-31. USR-014 não cria cópia local de CPF/email: a alteração afeta a identidade compartilhada entre escritórios. Limites de administração permanecem; autorização para alterar não equivale a comprovação de titularidade.

USR-013 é uma exceção específica à regra geral IAM-003 (sem segunda aprovação): cadastrar conta nova e alterar acessos no escritório não exigem aprovação adicional; vincular conta existente a outro escritório requer autorização Recovery. Não introduzir aprovação geral por interpretação dessa exceção.

Fonte E12: respostas do solicitante em 2026-10-31. IDs USR deste projeto são independentes dos identificadores do documento legado.

USR-004/005 refinam o inventário USR-001: não importar obrigatoriedade de país do legado. E12 fixa CPF e conta única. A confirmação refere-se a usuários internos e de escritórios; não estende o fluxo de cadastro às jornadas futuras do devedor. O cadastro básico de tipos de documento pode existir para outras entidades, sem ampliar os documentos aceitos neste fluxo.

Propostas técnicas a detalhar: normalizar CPF para comparação, validar formato e dígitos verificadores e impor unicidade persistida. Validade matemática de CPF não comprova identidade ou titularidade. CPF não é segredo nem substitui senha.

## Estado da identidade versus estado do vínculo

Manter conceitos separados: identidade pode estar ativa/inativa globalmente e possuir vínculos ativos/inativos em escritórios. Nomes finais dos estados e transições ainda serão detalhados.

- Desativar vínculo em A não desativa vínculo em B.
- Desativar globalmente deve impedir acesso em todos os contextos; prazo para encerrar sessões existentes a definir.
- Conta sem vínculo ativo não deve receber escopo global por ausência de escritório. Diretriz proposta; tratamento de administradores centrais precisa de fluxo próprio.
- Responsabilidades operacionais podem exigir transferência ou outras validações antes da desativação; regra ainda aberta.

## Primeiro acesso — fluxo a detalhar

1. Administrador autorizado cadastra identidade e vínculo, respeitando seu alcance.
2. Sistema gera senha temporária e envia ao email individual cadastrado (USR-011). Validade: 24 horas; reemissão invalida imediatamente a anterior (USR-012). Falha de entrega e confirmação do destinatário ainda serão definidas.
3. Usuário comprova a senha temporária antes de receber autorização restrita para trocá-la. Diretriz proposta para impedir troca apenas por nome de usuário.
4. Antes da troca bem-sucedida, não permitir operação normal do Backoffice. Critério decorrente da troca obrigatória.
5. Após a troca, aplicar permissões e seleção do contexto autorizado.

Geração, proteção do envio, expiração, armazenamento, reemissão e auditoria serão especificados antes da implementação. Senha temporária não deve ser registrada em logs nem permanecer recuperável no cadastro; diretrizes de segurança propostas. Entra ID continua pendente e precisará conciliar CPF como login e o fluxo aprovado, sem presumir suporte automático do provedor.

## Critérios de aceite iniciais

- Não exigir país para concluir cadastro válido com os quatro campos definidos.
- Rejeitar cadastro sem qualquer um dos campos obrigatórios; formatos e limites serão definidos separadamente.
- Usuário com senha temporária não acessa funcionalidades normais antes de trocá-la.
- Desativação em A não altera diretamente o vínculo ativo em B.
- Administrador restrito a escritório não pode executar desativação global pela API.
- Conta globalmente inativa não inicia nova sessão em outro escritório.
- A pessoa mantém a mesma identidade e credencial ao acessar diferentes escritórios autorizados; a escolha do contexto aplica as permissões correspondentes.
- Não criar segunda conta para o mesmo CPF. O fluxo seguro para associar conta existente a outro escritório ainda será refinado.
- Sistema envia a senha temporária ao email individual do destinatário; falha de envio não pode ser apresentada como entrega bem-sucedida.
- Rejeitar senha temporária após sua validade de 24 horas; proposta técnica: contar tempo decorrido desde a emissão, sem arredondar por dia de calendário.
- Após reemissão, rejeitar a senha temporária anterior mesmo que ainda estivesse dentro das 24 horas.
- Não efetivar vínculo com outro escritório para CPF existente sem autorização Recovery registrada.
- Alteração de CPF não cria segunda conta nem perde vínculos existentes; rejeitar CPF já associado a outra conta.

## Alteração de CPF e email — controles a decidir

O administrador pode afetar o login e o destino de recuperação de uma pessoa que atua em vários escritórios. Isso exige um processo explícito antes da implementação do fluxo. As propostas de confirmação do novo email com aviso ao antigo e de correção de CPF com justificativa/reautenticação foram recusadas como desenho do processo (E14). Aguardar descrição da alternativa; não implementá-las como regras aprovadas.

E14 define encerrar sessões e invalidar senhas temporárias e recuperações pendentes ao efetivar alteração de CPF/email. Ainda definir o instante de efetivação no processo alternativo, prazo de propagação e efeito em eventual identidade corporativa. Não presumir que alteração de CPF representa autorização para transferir conta a outra pessoa.

Critério de aceite USR-015: após efetivação e propagação dentro do prazo a definir, sessões anteriores não autorizam requisições protegidas e credenciais temporárias/recuperações anteriores não podem ser utilizadas, em nenhum escritório.

## Pendências

Atualização E34 — 2026-10-31: USR-022 determina retirar acesso imediatamente ao desativar usuário, mesmo com responsabilidades em casos/processos; transferência permanece pendente. Aprovada pelo solicitante. Substitui a hipótese de impedir desativação por responsabilidades (USR-002 no inventário inicial). Desativação local/global continua respeitando USR-007/008. Não apagar nem transferir responsabilidades automaticamente.

“Imediatamente” significa não aguardar transferência para efetivar retirada. O prazo técnico provisório de até 1 minuto para atingir sessões existentes permanece registrado em SES-002; confirmar se é suficiente para todos os cenários urgentes. Critério: existência de responsabilidade não impede efetivar desativação, e a responsabilidade permanece identificável para transferência posterior. Destinatário da pendência, alertas e prazo de transferência ainda a definir.

Atualização E34 — 2026-10-31: USR-022 determina retirar acesso imediatamente ao desativar usuário, mesmo com responsabilidades em casos/processos; transferência permanece pendente. Aprovada pelo solicitante. Substitui a hipótese de impedir desativação por responsabilidades (USR-002 no inventário inicial). Desativação local/global continua respeitando USR-007/008. Não apagar nem transferir responsabilidades automaticamente.

“Imediatamente” significa não aguardar transferência para efetivar retirada. O prazo técnico provisório de até 1 minuto para atingir sessões existentes permanece registrado em SES-002; confirmar se é suficiente para todos os cenários urgentes. Critério: existência de responsabilidade não impede efetivar desativação, e a responsabilidade permanece identificável para transferência posterior. Destinatário da pendência, alertas e prazo de transferência ainda a definir.

- Regras de unicidade e alteração do email; correção de CPF e proteção de dados na consulta de identidade existente.
- Identidade existente em outro escritório e proteção de seus dados globais.
- Instante inicial e limite exato das 24 horas, falha de envio/reemissão; política da senha definitiva e recuperação.
- Efeito imediato sobre sessões, reativação e último administrador do escritório.
