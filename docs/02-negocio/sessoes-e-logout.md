# Sessões e logout

## Decisões registradas

- SES-006 (E24): alterações de política IP e retirada de permissões devem afetar sessões já abertas em até 1 minuto, provisoriamente. O prazo conta da efetivação; responsável pela validação a indicar. Não exige encerrar a sessão inteira quando apenas uma ação ou contexto deixa de ser autorizado.

- SES-003 (E16): uma sessão por conta; novo login bem-sucedido encerra a anterior, independentemente do escritório.
- IAM-008 (E08): usuários com vários escritórios escolhem contexto ativo. Troca de contexto não é, por si só, um novo login.
- SES-002 (E15): prazo provisório de até 1 minuto para revogação nas requisições protegidas seguintes. Validar prazo do encerramento por substituição de sessão com a mesma política; ainda não aprovado especificamente para esse evento.
- USR-015 (E14): alteração efetivada de CPF/email encerra sessões e invalida senhas temporárias/recuperações pendentes.
- SES-004 (E17): encerrar sessão após 15 minutos sem atividade. Decisão provisória, sujeita à validação do time.
- SES-005 (E17): exigir novo login após duração absoluta de 8 horas, mesmo com atividade. Decisão provisória, sujeita à validação do time.

## Semântica e critérios dos prazos

Os dois limites são independentes: atividade não permite ultrapassar a duração absoluta. Proposta: medir 8 horas desde a autenticação bem-sucedida e medir inatividade desde a última ação qualificada no servidor; não reiniciar o limite absoluto na renovação de credenciais de sessão ou troca de escritório. Definir quais eventos qualificam atividade antes de implementar; polling e atualização automática não devem ser presumidos como ação humana.

Critérios: sessão sem atividade por 15 minutos exige nova autenticação; sessão ativa por 8 horas também exige nova autenticação. Precisão nos limites, relógio autoritativo e comunicação ao frontend serão detalhados. Esses valores permanecem provisórios, não requisitos corporativos confirmados.

## Critérios propostos

- Login inválido não encerra a sessão válida já existente.
- Dois logins concorrentes não mantêm duas sessões autorizadas após o prazo definido; mecanismo de concorrência ainda será projetado.
- Sessão revogada não volta a funcionar por troca de escritório ou reutilização de credencial de sessão.
- Logout deve retirar a capacidade de usar a sessão no backend, além de limpar o estado de autenticação do cliente.
- Mensagens ao usuário distinguem expiração, logout e substituição de sessão sem expor dados de outros dispositivos.

## Pendências

- Definição de atividade e renovação do limite de inatividade; valores provisórios já definidos em SES-004/005.
- Prazo de substituição da sessão antiga por novo login.
- Definição de sessão: navegador/dispositivo, abas e compartilhamento de contexto.
- Comportamento de logout local e eventual logout do provedor corporativo.
- MFA, autenticação reforçada e efeitos do primeiro acesso antes da troca obrigatória.
