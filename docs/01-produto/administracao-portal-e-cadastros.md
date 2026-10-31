# Administração do Portal e cadastros básicos

Fonte E05: áreas incluídas no foco inicial pelo solicitante. O detalhamento abaixo é proposta de refinamento, sem aprovação automática de funcionalidades.

Complemento E06: Administração do Portal abrange tanto o próprio Backoffice quanto o portal do devedor. Essa administração não antecipa a entrega das jornadas operacionais do devedor, adiadas em E05.

## Administração do Portal — catálogo candidato

### Capacidades confirmadas em E26

| ID | Regra | Estado |
|---|---|---|
| PRT-001 | Cadastrar módulos | Aprovada em escopo |
| PRT-002 | Cadastrar menus e ordenar navegação | Aprovada em escopo |
| PRT-003 | Ativar/desativar funcionalidades separadamente para Backoffice e portal do devedor | Aprovada em escopo |
| PRT-004 | Somente administradores autorizados Recovery cadastram módulos/menus e ativam/desativam funcionalidades | Aprovada, E27 |
| PRT-005 | Desativação impede uso no frontend e novas operações correspondentes no backend | Aprovada, E27 |
| PRT-006 | Configuração passa a valer ao salvar, sem rascunho/publicação | Aprovada, E27; prazo de propagação a definir |
| PRT-007 | Módulo e funcionalidades possuem estados independentes; desativar módulo não altera automaticamente estados das funcionalidades | Aprovada, E28; disponibilidade efetiva a esclarecer |
| PRT-008 | Após salvar desativação, backend bloqueia na próxima requisição; navegação pode refletir alteração em até 1 minuto | Aprovada, E28; sem prazo permissivo de 1 minuto no backend |
| PRT-009 | Menu e módulo são cadastros distintos; um módulo pode aparecer em vários menus apontando para rotas autorizadas | Aprovada, E28 |
| PRT-010 | Módulo inativo bloqueia execução de suas funcionalidades, sem alterar seus estados individuais | Aprovada, E29 |
| PRT-011 | Menus incluem hierarquia/submenus, ícones e ordenação por portal | Aprovada em escopo, E29 |
| MFE-001 | Cadastro de microfrontend inclui endereço remoto de carregamento | Aprovada em capacidade, E29; controles e contrato técnico pendentes |

E30 detalha MFE-001: permitir qualquer origem HTTPS; alteração do endereço exige permissão administrativa específica de microfrontends; registrar versão e histórico com retorno explícito. Ver [microfrontends](../04-arquitetura/microfrontends.md). Não há aprovação de lista restritiva de origens ou retorno automático.

E29: respostas do solicitante em 2026-10-31. Resolve a ambiguidade de PRT-007: estados persistidos independentes; disponibilidade efetiva depende do módulo e da funcionalidade estarem ativos no portal. Permissões e demais controles continuam necessários. Reativar módulo não ativa funcionalidade que estava individualmente inativa.

PRT-011 exige definir profundidade, tipos de item (agrupador/rota), referência de ícones e regra de apresentação quando não houver filhos acessíveis. MFE-001 não autoriza automaticamente qualquer origem externa nem determina framework de composição. Implantação, publicação de artefatos e mudança do endereço no catálogo são operações distintas, com processo a detalhar.

E28: respostas do solicitante em 2026-10-31. PRT-008 especifica a propagação antes pendente e é mais estrita que prazos provisórios de revogação de permissões/IP; não confundir essas políticas.

PRT-007 preserva estados individuais, mas ainda não determina se um módulo desativado bloqueia a execução de suas funcionalidades ativas sem alterar seus estados, ou se afeta apenas o próprio módulo. Essa distinção precisa ser resolvida antes do cálculo de disponibilidade efetiva e não deve ser inferida da interface.

Critérios: salvar estado de módulo não reescreve estados das funcionalidades; múltiplos menus podem referenciar o mesmo módulo; após desativar uma funcionalidade, nova requisição correspondente ao portal afetado é negada no backend, sem aguardar atualização visual de menus. Requisições iniciadas antes do salvamento precisam de política própria.

E27: respostas do solicitante em 2026-10-31. Salvar é a efetivação da configuração; cache e clientes devem refletir o estado dentro de prazo ainda a definir. Não interpretar “ao salvar” como aprovação de cache indefinido nem como garantia técnica de atualização simultânea em todos os clientes.

Critérios iniciais: administrador de escritório não altera essas configurações pela API; após desativação efetiva, esconder navegação e negar novas operações da funcionalidade no portal correspondente, inclusive por acesso direto. Não desfazer operações já concluídas. Operações em andamento e endpoints compartilhados entre portais precisam de regra própria para não desativar indevidamente o outro portal.

Confirmação do solicitante em 2026-10-31. Itens acima deixam de ser somente candidatos. Autoridades, estados, campos, efeito sobre APIs/sessões, propagação e vínculo com microfrontends ainda precisam de detalhamento. Cadastro de módulo não significa autorização para executar código arbitrário ou publicar uma aplicação.

- Cadastro de aplicações/módulos e seu estado de disponibilidade.
- Navegação e menus associados às permissões do usuário.
- Configurações administrativas com responsáveis e histórico.
- Associação de módulos às funcionalidades protegidas.

Os dois portais estão incluídos; funcionalidades específicas, configurações permitidas e responsáveis ainda serão refinados. Menu visível não substitui autorização de API. Alterações destinadas a um portal devem ter seu destino explícito, evitando aplicação acidental ao outro.

E15 inclui perfil pessoal com foto e preferências de cores/layout. Preferências individuais são distintas das configurações administrativas globais; alcance entre portais, opções e persistência ainda precisam de refinamento.

## Cadastros básicos — inventário candidato

| Cadastro | Situação |
|---|---|
| Usuários, grupos, perfis e permissões | Já identificados no refinamento de Identidade |
| Escritórios e vínculos com carteiras | Regras de escopo parcialmente confirmadas |
| Carteiras e países | Referenciados; definir fonte mestre e possibilidade de edição |
| Outros cadastros operacionais | Solicitar lista priorizada ao time |

### Lista inicial informada pelo solicitante (E06)

- Cidade.
- Estado.
- País.
- Sexo.
- Gênero.
- Tipo de documento.
- Marcas e veículos — relação exata e eventual cadastro de modelos ainda não definidos.

Esses itens foram identificados para o catálogo inicial; prioridade individual, campos, relacionamentos e fonte dos dados serão tratados depois, conforme orientação do solicitante. A lista é aberta e não implica implementação imediata de todos os itens. Sexo e gênero permanecem termos distintos; finalidade de uso e necessidade de coleta em cadastros de pessoas não são presumidas pela existência das tabelas de referência.

Para cada cadastro registrar proprietário do dado, campos, identificador, unicidade, estados, vínculos, exclusão/inativação, escopo, ações autorizadas, auditoria e critérios de aceite. Não presumir que a aplicação será a fonte mestre de todos os cadastros.

E36 define a base transversal: inativar/reativar, importar planilha/CSV com validação e erros, e detectar edição concorrente. Ver [regras transversais](../02-negocio/cadastros-regras-transversais.md).

## Microfrontends — direcionamento

Microfrontends fazem parte do foco inicial. Ainda definir limites dos módulos, shell, carregamento, navegação, sessão compartilhada, tratamento de falhas e publicação. Escolha de framework de composição e independência de implantação exigem decisão arquitetural posterior.

Proposta de segurança: o shell orienta navegação e contexto; APIs verificam identidade, ação e escopo. Um microfrontend carregado não adquire privilégios adicionais por estar no portal.
