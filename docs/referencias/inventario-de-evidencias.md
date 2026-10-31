# Inventário de evidências

## E01 — Transcrição do documento SIR

- Arquivo: `SIR_User_and_Access_Rules_transcricao.md`.
- Local consultado: `C:\Users\jeffr\Downloads\SIR_User_and_Access_Rules_transcricao.md`.
- SHA-256 verificado em 2026-10-31: `5D630E2CC151B4745CCCF2CD014EAA37D6228DDE63A164C4136F5D25148CDD56`.
- Origem declarada: OCR de 31 fotografias de `SIR_User_and_Access_Rules_v2.docx`, versão visível 2.0 de 2026-10-31.
- Limitações: OCR não revisado, trechos cortados, duplicações e identificadores ambíguos. Documento original, imagens, DLLs, fontes e SQL não estão disponíveis neste repositório.
- Uso: referência AS-IS e pistas de investigação. “Confirmado” no documento representa a classificação do autor original, sem verificação independente neste projeto.
- Preservação: original não alterado nem copiado para o Git nesta etapa. Reprodutibilidade depende da disponibilidade dessa cópia ou da obtenção das fontes.

## E02 — Direcionamento do produto

- Origem: conversa de refinamento com o solicitante em 2026-10-31.
- Direcionamento: aplicação nova para a plataforma Grupo Recovery, em .NET e React.
- Escopo: incluir Workflow, hierarquias, IP e carteiras/agências já no MVP.
- Processo: investigar lacunas no legado ou documentar decisões provisórias para validação pelo time, mantendo avanço das entregas independentes.
- Limite: não representa aprovação das políticas técnicas ou de todas as regras relatadas em E01.

## E03 — Respostas iniciais do solicitante

- Data: 2026-10-31, conversa de refinamento.
- Públicos: colaboradores internos, assessorias/parceiros e devedores.
- Acesso: usuários com alcance amplo, restritos a um escritório ou vinculados a vários; escritórios podem acessar mais de uma carteira.
- Identidade: possível Entra ID, ainda não confirmado; conta gratuita Azure disponível para testes.
- Limites: não confirma grupos múltiplos, equivalência agência/escritório, isolamento por empresa/país nem privilégios administrativos globais.

## E04 — Compartilhamento de carteiras e jornadas do devedor

- Data: 2026-10-31, respostas do solicitante na conversa.
- Uma carteira pode ser atendida por vários escritórios; cada escritório autorizado vê todos os devedores/casos da carteira.
- Devedor: consultar dívidas, emitir boleto e acompanhar acordos.
- Limites: visibilidade compartilhada não concede automaticamente edição, exportação ou ações financeiras; forma de emissão de boleto e fontes de dados ainda não definidas.

## E05 — Foco do primeiro MVP e carteiras por usuário

- Fonte: resposta do solicitante, 2026-10-31.
- Todos os usuários do escritório acessam todas as carteiras do escritório; permissões por ação continuam separadas.
- Primeiro MVP: Backoffice com foco inicial em Identidade, Administração do Portal, microfrontends e cadastros básicos.
- Não aprofundar jornadas do devedor ou emissão de boletos nesta etapa. E04 permanece como contexto da plataforma, com prioridade e entrega inicial substituídas por E05.

## E06 — Portais administrados e lista inicial de cadastros

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Administração cobre Backoffice e portal do devedor.
- Cadastros citados: cidade, estado, país, sexo, gênero, tipo de documento e “Marcas > Veículos”.
- Solicitante adiou o detalhamento dos cadastros. Ordem, campos e relação marca/veículo não estão aprovados.

## E07 — Administração e composição de acessos

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Cadastro: usuário interno Recovery; haverá administrador central e administrador de escritório, este limitado ao próprio escritório.
- Criação/alteração de acessos sem aprovação de outra pessoa.
- Usuários podem ter vários grupos/perfis; permissões podem variar por escritório.
- Limite: criação de identidades por administrador externo, privilégios delegáveis, campos globais e resolução de conflitos ainda não definidos.

## E08 — Cadastro delegado, cópia e contexto ativo

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Administrador de escritório pode cadastrar usuários e copiar permissões de outros.
- Pode nomear outros administradores do próprio escritório.
- Usuários com múltiplos escritórios escolhem um escritório ativo para aplicar as permissões desse contexto.
- Esclarece a dúvida de E07 sobre cadastro externo. Limites de cópia, composição, edição de identidade global e privilégios delegáveis continuam pendentes.

## E09 — Cópia e personalização com limites

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Cópia pelo administrador de escritório: origem e destino somente no próprio escritório.
- Administrador escolhe acrescentar ou substituir permissões.
- Recovery define limites; escritório personaliza grupos/perfis dentro deles.
- Pendências: conteúdo copiado, privilégio administrativo, sincronização posterior, representação e revogação dos limites.

## E10 — Limite global e independência da cópia

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Limite único de permissões delegáveis para todos os escritórios.
- Copiar somente permissões comuns, sem administração.
- Origem e destino independentes após a cópia; não sincronizar atribuições futuras da origem.
- Pendências: representação da cópia, tratamento de perfis mistos, administração preexistente no destino e efeito de mudanças em perfis compartilhados ou no limite global.

## E11 — Campos, primeiro acesso e desativação

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Obrigatórios iniciais: nome completo, email, documento e tipo de usuário; país não necessário.
- Primeiro acesso: senha temporária com troca obrigatória.
- Desativação local do escritório separada da global, esta reservada à Recovery.
- Limites: formato/unicidade, canal de entrega e expiração da senha, reativação e revogação de sessões ainda pendentes.

## E12 — CPF, conta única e senha por email

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Login por documento: somente CPF para internos e escritórios.
- Uma conta por pessoa, compartilhada entre seus vínculos com escritórios.
- Sistema gera senha temporária e envia ao email individual.
- Pendências: expiração/reemissão, confirmação do email, associação segura a conta existente e compatibilidade com eventual IdP corporativo.

## E13 — Validade, vínculo adicional e edição global

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Senha temporária válida por 24 horas; reemissão invalida imediatamente a anterior.
- Recovery autoriza novo vínculo de conta existente a outro escritório: exceção específica ao fluxo sem aprovação geral.
- Administrador de escritório também pode alterar CPF e email da identidade única.
- Pendências: confirmação/notificação, reautenticação, efeitos sobre sessões e recuperação e operacionalização da autorização Recovery.

## E14 — Processos alternativos de edição e revogação

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Não aprovado o fluxo proposto de confirmação do novo email e aviso ao antigo; outro fluxo será detalhado.
- Não aprovado o processo proposto de correção de CPF com justificativa e reautenticação; necessário outro processo.
- Aprovado encerrar sessões e invalidar senhas temporárias/recuperações pendentes após alteração de CPF ou email.
- Limites: recusa das propostas não aprova ausência de controles; processo alternativo e prazo de revogação continuam abertos.

## E15 — Perfil pessoal, CPF exclusivo Recovery e prazo provisório

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Usuário pode alterar próprio email; perfil completo com foto e configurações do portal (cores, layout etc.). Campos adicionais não definidos.
- Somente Recovery altera CPF; substitui a permissão administrativa de escritório antes relatada em E13.
- Prazo provisório de revogação: até 1 minuto após efetivar bloqueio, desativação ou alteração de CPF/email.
- Pendências: confirmação do email, manutenção de edição administrativa do email, opções de personalização e validação do prazo pelo time.

## E16 — Edição de email e sessão única

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Próprio usuário e administrador do escritório podem editar email.
- No fluxo do próprio usuário, confirmar novo endereço antes de efetivar.
- Uma sessão por conta; novo login encerra anterior.
- Limites: confirmação no fluxo administrativo, duração/definição da sessão e prazo de substituição ainda pendentes.

## E17 — Confirmação administrativa de email e prazos provisórios

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Novo email alterado pelo administrador também exige confirmação pelo destinatário antes de valer.
- Inatividade da sessão: provisoriamente 15 minutos.
- Duração absoluta: provisoriamente 8 horas, mesmo com atividade.
- Limites: valores aguardam validação do time; eventos de atividade e mecanismos não definidos.

## E18 — Bloqueio provisório, recuperação e MFA

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Provisoriamente 5 senhas inválidas consecutivas e bloqueio de login por 15 minutos.
- Recuperação de senha por autosserviço e por administrador autorizado.
- MFA obrigatório para todos os usuários do Backoffice.
- Pendências: método/recuperação do MFA, contadores, link e fluxo administrativo, efeitos em sessões e compatibilidade com provedor.

## E19 — Recuperação de MFA e redefinição administrativa

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Método de MFA permanece pendente; avançar com demais refinamentos.
- Recuperação de MFA autorizada somente pela Recovery após verificar identidade.
- Redefinição administrativa de senha por link para definir nova senha; não por senha temporária.
- Pendências: verificação de identidade, validade/consumo do link e reinscrição do fator.

## E20 — Herança e administração de IP

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Grupo superior herda permissões dos inferiores.
- Restrições de IP configuráveis por escritório e por grupo/perfil.
- Somente Recovery administra restrições de IP.
- Pendências: profundidade da herança, administração herdada, composição de restrições e momento de avaliação.

## E21 — Profundidade, administração explícita e composição provisória de IP

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Herança comum alcança todos os descendentes.
- Permissões administrativas exigem atribuição explícita, sem herança.
- Provisoriamente, todas as restrições de IP aplicáveis devem permitir o endereço.
- Pendências: definição das regras aplicáveis, ausência de configuração, reavaliação e catálogo de ações administrativas.

## E22 — IP obrigatório e união provisória de concessões

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Sem configuração de IP, não permitir acesso ao contexto.
- Validar IP no login, troca de escritório e requisições protegidas.
- Provisoriamente unir permissões comuns de múltiplos grupos/perfis, sem negativas explícitas; demais restrições mantidas.
- Pendências: política anterior à seleção de escritório, níveis obrigatórios e primeiro acesso administrativo.

## E23 — IP por escritório e contexto obrigatório Recovery

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Escritório exige política IP; grupos/perfis adicionam restrições opcionais.
- Etapa restrita de autenticação/seleção permitida quando IP aceito por ao menos um escritório autorizado; nenhum dado operacional antes da seleção e das verificações do contexto.
- Recovery também deve usar escritório ativo; contexto central separado proposto não foi adotado.
- Pendências: alcance amplo e vínculos Recovery, provisionamento inicial e detalhes da elegibilidade pré-contexto.

## E24 — Perfil global, múltiplos superiores e propagação

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Perfil global Recovery permite selecionar qualquer escritório; contexto ativo permanece obrigatório.
- Grupos podem possuir vários superiores.
- Mudanças de IP e retirada de permissões afetam sessões existentes em até 1 minuto, provisoriamente.
- Pendências: gestão/ações do perfil global, ciclos e fronteiras da hierarquia e validação operacional do prazo.

## E25 — Gestão e alcance global; hierarquia isolada

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Perfil global concedido/revogado exclusivamente por administradores centrais Recovery.
- Perfil global inclui todas as ações comuns, além da seleção de qualquer escritório; não concede administração implicitamente.
- Hierarquias sem ciclos nem relações entre grupos personalizados de escritórios diferentes.
- Pendências: catálogo de ações, novas ações comuns, modelos centrais e relação entre alcance global e limite delegável.

## E26 — Ações e Administração do Portal

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Base de ações: consultar, criar, editar, inativar/reativar e exportar. Gestão de usuários/acessos/configurações de segurança administrativa; exceções serão refinadas.
- Perfil global recebe automaticamente novas ações comuns.
- Administração inclui cadastrar módulos/menus, ordenar navegação e ativar/desativar funcionalidades separadamente nos dois portais.
- Pendências: autoridades, efeito da desativação, propagação e classificação detalhada das ações.

## E27 — Autoridade e efeito das configurações de portal

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Administração de módulos/menus e ativação de funcionalidades exclusiva de administradores autorizados Recovery.
- Desativação bloqueia frontend e backend.
- Alteração entra em vigor ao salvar, sem rascunho/publicação.
- Pendências: propagação, operações em andamento, hierarquia módulo/funcionalidade e isolamento por portal em APIs compartilhadas.

## E28 — Estados independentes, bloqueio e menus

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Módulos e funcionalidades têm estados independentes, sem desativação automática de todas as funcionalidades.
- Backend deve bloquear na próxima requisição após salvar; navegação atualiza em até 1 minuto.
- Menu e módulo são distintos; módulo pode aparecer em vários menus com rotas autorizadas.
- Pendências: efeito do módulo inativo sobre execução de funcionalidade ativa, requisições em andamento e estrutura dos menus.

## E29 — Disponibilidade efetiva, navegação e remotos

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Módulo inativo bloqueia execução de funcionalidades, preservando estados individuais.
- Menus com hierarquia/submenus, ícones e ordenação por portal.
- Cadastro de microfrontends inclui endereço remoto de carregamento.
- Pendências: controle de origens, autoridade específica sobre endereço, versionamento e contrato shell/remoto.

## E30 — Origem HTTPS, permissão específica e retorno

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Permitir qualquer origem HTTPS, substituindo proposta de lista prévia de origens.
- Alterar endereço exige permissão administrativa específica para microfrontends.
- Registrar versão e histórico com retorno explícito a endereço/versão anterior.
- Pendências: isolamento, sessão, integridade/imutabilidade de artefatos e contrato técnico; HTTPS não certifica confiabilidade do código.

## E31 — Autoria, falhas e auditoria

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Microfrontends desenvolvidos somente pela equipe Recovery.
- Falha de carregamento afeta somente a experiência do módulo, com erro e nova tentativa; restante do portal continua funcionando.
- Auditar alterações de módulos, menus, URLs, versões e permissões com ator, data, contexto e diferenças antes/depois.
- Pendências: leitores/retencão/proteção dos registros, falha de auditoria e parâmetros de carregamento.

## E32 — Consulta, proteção e eventos provisórios de auditoria

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Recovery consulta todos os contextos; escritório somente o próprio, com permissão específica.
- Impedir edição e exclusão manual; descarte por retenção controlada.
- Incluir provisoriamente logins, falhas, logout, recuperação de senha/MFA e negativas de acesso; revisar volume.
- Pendências: prazo de retenção, eventos sem contexto/global, consulta agregada Recovery e falha de auditoria.

## E33 — Consulta agregada, retenção estimada e durabilidade

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Recovery pesquisa auditoria de vários escritórios com permissão específica; exceção restrita à consulta.
- Propor retenção provisória após estimar volume, sem prazo atual aprovado.
- Alteração sensível pode prosseguir durante indisponibilidade de envio somente com registro durável para entrega posterior.
- Pendências: capacidade estimada, consistência alteração/evento e tratamento de reentrega.

## E34 — Volume desconhecido, Workflow aberto e retirada de acesso

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Quantidade de usuários cadastrados/simultâneos e escritórios ainda desconhecida.
- Recorte de Workflow precisa de mais detalhes; não aprovada limitação administrativa proposta.
- Desativação retira acesso sem aguardar transferência de responsabilidades, que fica pendente.
- Pendências: processo de transferência, alertas, recorte funcional e futura estimativa de carga.

## E35 — Execução nova e detalhamento adiado de Workflow

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Construir execução de Workflow na nova aplicação.
- Pular nesta etapa o exemplo e detalhamento do processo.
- Limites: não define motor, estados, transições ou integrações de dados. Avançar demais módulos sem assumir fluxo operacional.

## E36 — Cadastros, importação e concorrência

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Inativação/reativação como padrão, preservando referências e histórico.
- Importação por planilha/CSV com validação e relatório de erros incluída no MVP.
- Detectar conflitos entre edições e pedir revisão, sem sobrescrita silenciosa.
- Pendências: elegibilidade/formato dos lotes, atomicidade, permissões e limites.

## E37 — Importação restrita e rejeição integral

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Qualquer linha inválida rejeita todo o lote.
- Importação somente cria novos registros inicialmente.
- Somente Recovery importa, com permissão específica.
- Pendências: chaves de duplicidade, formatos, limites e garantias de persistência/reenvio.

## E38 — Inativos, cadastros globais e consolidação

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Inativos consultáveis administrativamente com filtro, indisponíveis para novos vínculos.
- Cadastros básicos globais mantidos somente Recovery, com permissões específicas.
- Consolidar versão atual, separar histórico/pendências e depois avançar à arquitetura .NET/React.

## E39 — Equipes e avaliação arquitetural

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Várias equipes com módulos independentes.
- Avaliar arquitetura, padrões, estrutura de pastas e frameworks em detalhe; nenhuma infraestrutura adicional imposta nesta resposta.
- Propor opção de implantação simples para começar; backend independente por módulo não é exigência confirmada.
- Propostas em `04-arquitetura/proposta-arquitetural.md` permanecem recomendações, não decisões técnicas aprovadas.

## E34 — Volume desconhecido, Workflow aberto e retirada de acesso

- Fonte: respostas do solicitante na conversa, 2026-10-31.
- Quantidade de usuários cadastrados/simultâneos e escritórios ainda desconhecida.
- Recorte de Workflow precisa de mais detalhes; não aprovada limitação administrativa proposta.
- Desativação retira acesso sem aguardar transferência de responsabilidades, que fica pendente.
- Pendências: processo de transferência, alertas, recorte funcional e futura estimativa de carga.
