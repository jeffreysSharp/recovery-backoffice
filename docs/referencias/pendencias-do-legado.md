# Pendências e decisões provisórias

Responsável pelas respostas de negócio: a indicar pelo solicitante/time. Itens permanecem abertos salvo resolução explicitamente registrada. Nenhuma premissa deve ser considerada aprovada por ausência de resposta.

## Respostas recebidas em 2026-10-31

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
