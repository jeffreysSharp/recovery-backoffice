# Regras vigentes — MVP Recovery Backoffice

Consolidação funcional em 2026-10-31. Responsável pelas confirmações: solicitante na entrevista, evidências E02–E38. Não equivale à homologação de políticas corporativas. Esta é a referência de estado atual; [evidências](referencias/inventario-de-evidencias.md) preservam origem e mudanças. Propostas técnicas dos documentos de trabalho não são aprovadas por esta consolidação.

## Escopo e públicos

- Primeiro MVP: Backoffice, Identidade, Administração do Backoffice e do portal do devedor, microfrontends React e cadastros básicos; backend .NET (E05/E06).
- Workflow, hierarquias, IP e escopos fazem parte do refinamento acordado. Execução de Workflow será nova, mas processos e recorte estão adiados (WFL-003, E35).
- Internos, parceiros/escritórios e devedores são públicos da plataforma. Jornadas operacionais do devedor ficam fora da entrega inicial; consulta de dívidas, boletos e acordos permanecem contexto futuro.
- Cadastros identificados: cidade, estado, país, sexo, gênero, tipo de documento, marcas/veículos. Campos e prioridades individuais adiados.

## Identidade e administração

| Regras | Estado atual aprovado | Evidências |
|---|---|---|
| USR-004/005/009/010 | Nome completo, email, CPF e tipo de usuário obrigatórios; país dispensado; login por CPF; uma conta por pessoa para todos os escritórios | E11/E12 |
| IAM-001/002/007 | Recovery administra centralmente; administrador de escritório cadastra usuários, administra somente seu escritório e pode nomear outros administradores dele | E07/E08 |
| IAM-003; USR-013 | Sem segunda aprovação geral; exceção: novo vínculo de conta existente a outro escritório exige autorização Recovery | E07/E13 |
| USR-007/008/022 | Desativação local separada da global Recovery; responsabilidades não impedem retirar acesso e ficam pendentes de transferência | E11/E34 |
| USR-016/019/020/021 | Somente Recovery altera CPF; usuário e administrador do escritório alteram email, confirmado pelo destinatário antes de efetivar | E15–E17 |
| USR-018 | Perfil pessoal com foto, cores, layout e preferências; detalhes pendentes | E15 |
| USR-015 | Alteração efetivada de CPF/email encerra sessões e invalida senhas temporárias e recuperações pendentes | E14 |

## Autenticação e sessões

| Regras | Estado atual aprovado | Evidências |
|---|---|---|
| USR-006/011/012 | Primeiro acesso com senha temporária gerada pelo sistema, enviada ao email individual, válida por 24 horas e com troca obrigatória; reemissão invalida anterior | E11–E13 |
| AUTH-003/005 | MFA para todos no Backoffice; recuperação do fator somente Recovery após verificar identidade; método adiado | E18/E19 |
| PWD-002/003 | Recuperação por autosserviço e administrador; redefinição administrativa por link, distinta da senha temporária do primeiro acesso | E18/E19 |
| SES-003 | Uma sessão por conta; novo login bem-sucedido encerra anterior | E16 |

Tempos de sessão, bloqueio e propagação estão na seção de decisões provisórias abaixo. CPF válido não comprova identidade. Provedor corporativo/Entra e implementação de credenciais ainda não estão decididos.

## Escopo, grupos e permissões

| Regras | Estado atual aprovado | Evidências |
|---|---|---|
| SCP-002–007 | Usuário pode atuar em vários escritórios; escritório pode atender várias carteiras e carteira vários escritórios. Todos os usuários do escritório recebem todas as suas carteiras e veem todos os casos/devedores delas, com ações limitadas pelas permissões | E03–E05 |
| IAM-004/005/008/015 | Múltiplos grupos/perfis; permissões variam por escritório; escritório ativo obrigatório também para Recovery | E07/E08/E23 |
| IAM-011/012 | Limite delegável único definido pela Recovery; escritório personaliza grupos/perfis dentro dele | E09/E10 |
| IAM-006/009/010/013/014 | Cópia somente no próprio escritório, acrescentando ou substituindo por escolha do administrador; somente permissões comuns, sem administração; origem/destino independentes após cópia | E08–E10 |
| HIE-002–007 | Grupos admitem vários superiores; herdam ações comuns de todos os descendentes; sem ciclos nem relações entre grupos personalizados de escritórios distintos; administração explícita, sem herança | E20/E21/E24/E25 |
| IAM-016–019 | Perfil global somente Recovery concede/revoga; permite qualquer escritório e todas as ações comuns, inclusive novas automaticamente; administração não é implícita | E24–E26 |
| ACL-003/004 | Base de ações: consultar, criar, editar, inativar/reativar, exportar. Gestão de usuários/acessos/segurança é administrativa; exceções a refinar | E26 |

Perfil global não dispensa MFA, IP ou escritório ativo. Independência da cópia não congela definições de perfis compartilhados. Catálogo de ações e regras para perfis mistos continuam pendentes.

## IP

- IP-002/003/005/007: configuração obrigatória por escritório, restrições opcionais por grupo/perfil; administração exclusiva Recovery. Sem política obrigatória, não acessar (E20/E22/E23).
- IP-006: verificar no login, troca de escritório e requisições protegidas (E22).
- IP-008: etapa restrita pré-contexto se IP elegível em ao menos um escritório autorizado, sem dados operacionais; aplicar regras completas ao selecionar (E23).
- Combinação de regras aplicáveis permanece provisória: todas devem permitir (IP-004).

## Portais e microfrontends

| Regras | Estado atual aprovado | Evidências |
|---|---|---|
| PRT-001–004/009/011 | Recovery autorizada cadastra módulos e menus distintos, com múltiplas entradas por módulo, submenus, ícones, ordenação e disponibilidade por portal | E26–E29 |
| PRT-005–008/010 | Salvar efetiva sem publicação separada; módulo inativo bloqueia funcionalidades sem reescrever estados; backend bloqueia próxima requisição, navegação reflete em até 1 minuto | E27–E29 |
| MFE-001–004 | URL remota de qualquer origem HTTPS; alteração exige permissão administrativa específica; versão/histórico e retorno explícito | E29/E30 |
| MFE-005/006 | Desenvolvimento somente Recovery; falha de carregamento mostra erro/repetição apenas no módulo e preserva restante do portal | E31 |

Falha isolada na experiência não comprova isolamento de segurança. Tecnologia, contrato de sessão e confiança no código remoto ainda precisam de arquitetura. Não há retorno automático de versão aprovado.

## Auditoria

- AUD-002: alterações de módulos, menus, URLs, versões e permissões com ator, data, contexto e diferenças antes/depois (E31).
- AUD-003/006: Recovery consulta todos os contextos e pode pesquisar agregadamente com permissão específica; escritório somente próprio contexto com permissão específica. Exceção agregada limitada à auditoria (E32/E33).
- AUD-004: sem edição/exclusão manual; descarte por retenção controlada (E32).
- AUD-007: operação sensível só prossegue com registro durável, permitindo entrega posterior ao destino de auditoria (E33).
- Retenção será proposta após estimar volume; volume desconhecido, nenhum prazo definido (E33/E34).

## Cadastros e importação

| Regras | Estado atual aprovado | Evidências |
|---|---|---|
| CAD-001/002 | Inativar/reativar por padrão; inativos consultáveis administrativamente por filtro, indisponíveis para novos vínculos | E36/E38 |
| CAD-003 | Manutenção dos cadastros básicos globais somente Recovery, com permissões específicas | E38 |
| CON-001 | Conflito entre edições exige revisão; sem sobrescrita silenciosa | E36 |
| IMP-001–004 | Planilha/CSV com validação/relatório; somente criar; qualquer linha inválida rejeita lote inteiro; somente Recovery com permissão específica | E36/E37 |

Histórico e referências existentes são preservados na inativação. Formatos, duplicidade e garantias de persistência de lotes ainda serão especificados.

## Decisões provisórias — não confundir com aprovadas definitivas

| Regra | Premissa provisória | Origem |
|---|---|---|
| SES-002 | Até 1 minuto para revogação após bloqueio/desativação/alteração de CPF ou email | E15 |
| SES-004/005 | 15 minutos sem atividade e duração absoluta de 8 horas | E17 |
| AUTH-004 | 5 senhas inválidas consecutivas bloqueiam login por 15 minutos | E18 |
| IP-004 | Todas as restrições IP aplicáveis devem permitir | E21 |
| ACL-002 | União das concessões comuns no contexto, sem negativas explícitas | E22 |
| SES-006 | Até 1 minuto para mudanças IP/retirada de permissões afetarem sessões existentes | E24 |
| AUD-005 | Incluir login, falhas, logout, recuperação de senha/MFA e negativas; revisar volume | E32 |

Responsáveis pela validação pelo time ainda a indicar; revisar antes da liberação operacional. Bloqueio por senhas inválidas não foi definido como causa de encerramento de sessão já válida. Prazo para substituir sessão por novo login ainda aberto. Disponibilidade de funcionalidades possui regra própria mais estrita no backend (PRT-008).

## Uso desta consolidação

Implementar por funcionalidades com critérios de aceite detalhados. Regras aprovadas em capacidade ainda exigem contratos, fluxos e validações. Consultar [pendências atuais](referencias/pendencias-atuais.md) e [roteiro de arquitetura](04-arquitetura/roteiro-de-decisoes.md). Não reproduzir hipóteses antigas dos documentos de entrevista quando substituídas aqui.
