# Pendências e decisões provisórias

Responsável pelas respostas de negócio: a indicar pelo solicitante/time. Itens permanecem abertos salvo resolução explicitamente registrada. Nenhuma premissa deve ser considerada aprovada por ausência de resposta.

## Respostas recebidas em 2026-10-31

- E37: lote integralmente rejeitado com linha inválida; importação somente de novos registros e exclusiva Recovery com permissão específica. Detalhar duplicidade, formatos e limites sem presumir atualização ou importação parcial.

- E36: cadastros com inativação/reativação por padrão; importação por planilha/CSV com validação e relatório; conflito de edição exige revisão. Especificar lotes, permissões e tratamento de erros, sem reabrir campos específicos adiados.

- E35: execução do Workflow será construída na aplicação nova; detalhamento do processo adiado pelo solicitante. Não insistir nessa rodada; manter estados/transições e critérios pendentes, sem limitar implicitamente a entrega à administração.

- E34/P12: volumes ainda desconhecidos; retenção e capacidade não podem ser dimensionadas como dados confirmados. Levantar depois ou usar cenários explicitamente hipotéticos.
- E34/P06: recorte de Workflow aberto para detalhamento; não presumir apenas administração. Responsabilidades não impedem desativação; retirar acesso e manter transferência pendente (USR-022), substituindo hipótese de bloqueio da operação herdada do legado.

- E34/P12: volumes ainda desconhecidos; retenção e capacidade não podem ser dimensionadas como dados confirmados. Levantar depois ou usar cenários explicitamente hipotéticos.
- E34/P06: recorte de Workflow aberto para detalhamento; não presumir apenas administração. Responsabilidades não impedem desativação; retirar acesso e manter transferência pendente (USR-022), substituindo hipótese de bloqueio da operação herdada do legado.

- E33: auditoria agregada Recovery com permissão específica, exceção limitada de consulta; retenção será proposta após estimar volume. Alterações sensíveis só prosseguem com registro durável quando envio estiver indisponível; consistência e reentrega serão projetadas.

- E32: auditoria Recovery com alcance de todos os contextos; escritório somente próprio, com permissão específica. Edição/exclusão manual proibidas; retenção controlada com prazo pendente. Conciliar consulta agregada Recovery com escritório ativo obrigatório.
- PT-009 / AUD-005: eventos de login/falhas/logout/recuperação de senha e MFA/negativas incluídos provisoriamente; responsável a indicar; revisar volume e proteção de dados com o time antes da liberação operacional e ao dimensionar armazenamento.

- E31: remotos desenvolvidos somente pela Recovery; falha de carregamento fica no módulo com nova tentativa e preserva o restante do portal. Auditoria de alterações de módulos/menus/URLs/versões/permissões aprovada com ator, data, contexto e diferenças; retenção e leitores pendentes.

- E30: qualquer origem HTTPS permitida; endereço remoto alterável mediante permissão administrativa específica; versão/histórico e retorno explícito aprovados. Lista prévia de origens não adotada. Resolver isolamento e compartilhamento de sessão antes de executar remotos; versão não implica imutabilidade.

- E29 resolve disponibilidade: módulo inativo bloqueia execução sem alterar estado das funcionalidades. Menus hierárquicos com ícones/ordenação por portal. Cadastro inclui URL remota de microfrontend; detalhar confiança, autorização de alteração e versionamento.

- E28: estados de módulo/funcionalidade independentes; menus distintos dos módulos, com múltiplas referências. Desativação bloqueia backend na próxima requisição após salvar e navegação em até 1 minuto. Definir disponibilidade efetiva de funcionalidade ativa em módulo inativo; não presumir cascata nem liberação automática.

- E27: configurações de módulos/menus/ativação exclusivas Recovery; desativação bloqueia frontend e novas operações backend; salvar efetiva sem publicação separada. Prazo de propagação e operações em andamento ainda pendentes.

- E26: base de ações de cadastro e classificação administrativa aprovada para refinamento de exceções. Novas ações comuns entram automaticamente no perfil global. Cadastro de módulos/menus, ordenação e ativação por portal incluídos; definir responsáveis e efeito operacional.

- E25: concessão/revogação de perfil global exclusiva Recovery; inclui todas as ações comuns, com administração explícita separada. Hierarquias sem ciclos nem relações entre grupos personalizados de escritórios distintos. Substitui hipótese de perfil global apenas seletor.

- E24: perfil global Recovery habilita seleção de qualquer escritório; grupos admitem múltiplos superiores. Confirmar concessão do perfil, suas ações e prevenção de ciclos.
- PT-008 / SES-006: mudanças IP e retirada de permissões devem valer nas sessões existentes em até 1 minuto, provisoriamente. Impacta atualização das políticas e testes de revogação; responsável a indicar; revisar com time antes da liberação operacional.

- E23: IP obrigatório no escritório; regras opcionais de grupos/perfis só restringem mais. Etapa pré-contexto restrita permitida se IP aceito em algum escritório autorizado, sem dados operacionais. Recovery também escolhe escritório ativo; representação de alcance amplo e provisionamento inicial pendentes.

- E22: configuração IP obrigatória; validar login, troca de escritório e requisições protegidas. Definir política pré-contexto, níveis obrigatórios e provisionamento inicial Recovery.
- PT-007 / ACL-002: união de concessões comuns no escritório ativo, sem negativas explícitas, provisoriamente. Impacta composição de acesso; responsável a indicar; revisar com o time ao definir catálogo de ações e antes da liberação operacional. Não elimina IP, limites Recovery ou escopo.

- E21: herança alcança todos os descendentes, excluindo permissões administrativas, que exigem atribuição explícita.
- PT-006 / IP-004: todas as restrições de IP aplicáveis devem permitir, provisoriamente. Impacta autorização em múltiplos níveis; responsável pela validação a indicar; revisar com o time antes da liberação operacional e ao definir aplicabilidade/ausência de regras.

- E20: superior herda permissões dos inferiores. IP por escritório e por grupo/perfil; manutenção exclusiva Recovery. Composição, profundidade e reavaliação ainda abertas.

- E19: método de MFA adiado; recuperação autorizada exclusivamente pela Recovery com verificação de identidade. Redefinição administrativa por link, preservando senha temporária para primeiro acesso. Validade do link e procedimento do fator continuam abertos; avançar outros módulos.

- E18: MFA obrigatório para todo Backoffice; recuperação de senha por autosserviço e administração. Método do fator, recuperação e redefinição ainda a detalhar.
- PT-005 / AUTH-004: 5 tentativas consecutivas de senha inválida bloqueiam login por 15 minutos. Provisório; responsável pela validação a indicar; revisar com política corporativa antes de produção. Definir contagem e proteção contra abuso, sem presumir efeito sobre sessão existente.

- E17: confirmação do novo endereço também na edição administrativa de email; resolve essa pendência de E16.
- PT-003 / SES-004: 15 minutos de inatividade, provisoriamente. PT-004 / SES-005: duração absoluta de 8 horas, provisoriamente. Impacto: expiração e experiência de uso; responsável pela validação a indicar; revisar com time antes da liberação operacional e ao definir atividade/renovação.

- E16: email editável por usuário e administrador; autosserviço exige confirmar novo endereço antes da efetivação. Não pressupor a mesma etapa para edição administrativa sem confirmação do fluxo.
- E16/P07: sessão única por conta, substituída por novo login. Refinar tempos, concorrência e prazo de substituição.

- E15 prevalece sobre E13 para CPF: somente Recovery pode alterá-lo. Usuário pode alterar próprio email e terá perfil com foto e preferências visuais. Edição de email pelo administrador e verificação do endereço ainda a esclarecer.
- PT-002 / SES-002: prazo provisório de revogação de até 1 minuto, escolhido pelo solicitante. Impacta sessão/cache e futuras verificações; validar com time responsável antes de produção. Não desfaz operações concluídas. Responsável pela validação: a indicar; revisão quando requisitos de operação e arquitetura de sessão forem definidos.

- E14: processos propostos de alteração de email/CPF não aprovados; solicitar alternativa antes de detalhar implementação. Aprovada revogação de sessões, senhas temporárias e recuperações pendentes após efetivação. Não pressupor confirmação de email como etapa do novo fluxo.

- E13: senha temporária de 24 horas, anterior invalidada na reemissão. Novo vínculo de conta existente a outro escritório exige autorização Recovery, exceção específica a IAM-003. Administrador pode alterar CPF/email globais; controles e efeitos ainda abertos.

- E12/P04: CPF como único documento e login de internos/escritórios; conta única por pessoa para todos os escritórios. Sistema gera senha temporária e envia ao email individual. Ainda definir edição global, vínculo a conta existente, prazo e reemissão.

- E11/P04: obrigatórios nome completo, email, documento e tipo de usuário; país dispensado no cadastro. Primeiro acesso com senha temporária e troca obrigatória. Formatos, unicidade e entrega pendentes.
- E11: desativação local do vínculo por escritório e desativação global Recovery separadas; reativação, sessões existentes e responsabilidades operacionais ainda a detalhar.

- E10: limite delegável único para todos os escritórios; cópia somente de permissões comuns, sem administração; atribuições de origem e destino independentes após a cópia. Não confundir independência com imutabilidade dos perfis compartilhados ou do limite global.

- E09: cópia restrita ao próprio escritório, com modo de acréscimo ou substituição escolhido pelo administrador. Recovery estabelece limites e escritório personaliza grupos/perfis dentro deles. Detalhar quais concessões são copiadas e o efeito de mudanças nos limites.

- E08 resolve o cadastro delegado: administrador do escritório pode cadastrar usuários, copiar permissões e nomear administradores do próprio escritório. Escopo e semântica da cópia permanecem abertos.
- E08 resolve o contexto operacional: usuário com vários escritórios escolhe escritório ativo, que determina as permissões aplicáveis. Administração central e troca de contexto ainda precisam de fluxo detalhado.

- E07: administração central Recovery e administração restrita ao próprio escritório; dúvida inicial sobre criação externa esclarecida em E08.
- E07/P03: múltiplos grupos/perfis permitidos, com permissões variáveis por escritório. Herança e composição de concessões continuam abertas.
- E07: criação/alteração de acessos sem aprovação por segunda pessoa. Não introduzir dupla aprovação obrigatória nesses fluxos.

- E06: Administração cobre Backoffice e portal do devedor. Cadastros iniciais identificados: cidade, estado, país, sexo, gênero, tipo de documento e marcas/veículos. Detalhamento adiado por orientação do solicitante; não bloquear o refinamento de Identidade por essas questões.

- E05 redefine o foco inicial: Backoffice, Identidade, Administração do Portal, microfrontends e cadastros básicos. Perguntas de boletos/devedor ficam adiadas, sem bloquear o MVP.
- P05: todos os usuários de um escritório recebem todas as carteiras dele; não há subconjunto individual dentro do escritório na regra aprovada. Ações, estados dos vínculos e contexto de múltiplos escritórios ainda precisam de definição.

- P01 parcialmente resolvida: internos, parceiros/assessorias e devedores incluídos. Identidades técnicas e jornadas do devedor continuam abertas.
- P02 aberta: Entra ID é uma possibilidade, sem confirmação corporativa. Solicitante dispõe de conta gratuita do Azure para desenvolvimento/testes.
- P03/P05 parcialmente resolvidas: usuários podem atuar em um ou vários escritórios; alguns têm alcance amplo. Escritórios podem acessar várias carteiras. Isso não define cardinalidade de grupos, herança, países, empresas ou poderes administrativos.
- P12 aberta: investigar implantação de homologação de baixo custo; gratuidade integral e capacidade de produção não estão aprovadas.
- Complemento P05 (E04): uma carteira pode estar em vários escritórios e todos os casos/devedores da carteira são visíveis aos escritórios autorizados. Permissões por ação, países e concessões individuais permanecem pendentes.
- Complemento P01/P06/P11 (E04): devedor consulta dívidas, emite boleto e acompanha acordos. Negociação não incluída na seleção atual; autenticação, titularidade e integração financeira ainda abertas.

| ID | Pergunta ou evidência necessária | Impacto e condição de revisão |
|---|---|---|
| P01 | Quem acessa: internos, parceiros, assessorias, devedores e identidades técnicas? | Define fronteiras e atores; revisar antes de fechar autenticação |
| P02 | Existe IdP corporativo/SSO? Atende externos? Quais exigências de MFA e recuperação? | Define gestão de credenciais; revisar antes de implementar login |
| P03 | Usuário pode ter vários grupos? Hierarquia delega administração, dados ou permissões? Como resolver conflitos? | Define composição de autorização; revisar antes de fechar vínculos e permissões |
| P04 | Campos, unicidade, estados e regras de reativação; contas genéricas serão permitidas? | Define cadastro; validar com operação e política corporativa |
| P05 | Relações entre empresas, países, carteiras e agências; quais fronteiras são obrigatórias? | Define isolamento de dados; resolver antes de disponibilizar consultas reais |
| P06 | Quais processos, etapas, ações e papéis entram no Workflow? Há alçadas, exclusividade e dupla aprovação? | Define motor e autorização de transições; buscar fluxos concretos no legado |
| P07 | Duração/simultaneidade das sessões e prazo máximo de revogação? | Define sessão e cache; quantificar antes da implementação |
| P08 | IP por usuário, grupo ou organização? IPv4/IPv6, VPN, proxies e exceções? | Define origem confiável e aplicação das regras |
| P09 | O que conta como atividade? Limites globais ou específicos? Quem fica isento? | Define processamento de inatividade; eliminar divergências do legado |
| P10 | Eventos auditáveis, retenção, acesso, exportação e proteção contra alterações? | Define auditoria e custos; validar com responsáveis corporativos |
| P11 | Migração/coexistência com SIR, integrações, responsáveis e fontes confiáveis? | Define transição e dependências externas |
| P12 | Infraestrutura, banco, versões suportadas, volumes e metas operacionais? | Define decisões técnicas e validação de capacidade |

## Premissa de trabalho provisória PT-001

- Usar “usuário”, “grupo”, “perfil”, “papel” e “escopo” como conceitos separados na documentação.
- Justificativa: evitar equivalência prematura entre estruturas legadas com funções distintas.
- Impacto: organização do refinamento; não fixa tabelas nem cardinalidades.
- Responsável pela validação: a definir com o time.
- Revisão: após respostas P03, P05 e P06.
- Estado: provisória, adotada somente para estruturar a documentação.

## Busca no legado

Para cada evidência futura, registrar repositório e commit, arquivo/método ou definição SQL, ambiente, data e limitações. Priorizar trechos completos e fluxos consumidores; nome de procedure ou presença de atributo isolados não comprovam comportamento efetivo.
