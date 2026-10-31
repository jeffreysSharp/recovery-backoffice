# Registro inicial de regras

Este inventário inicia o refinamento. As descrições resumem o conteúdo disponível e não substituem os fluxos completos. **Nenhuma regra abaixo está aprovada para implementação nesta versão.** Referência E01: [inventário](../referencias/inventario-de-evidencias.md).

| ID novo | Conteúdo a refinar | Evidência | Decisão | Dependência |
|---|---|---|---|---|
| USR-001 | Cadastro com identificador, nome, sobrenome, documento, país e contato; obrigatoriedade e unicidade a validar | E01, USR003 | Proposta | P04 |
| USR-002 | Desativação considera responsabilidades em casos e Workflow | E01, USR015–016 | Proposta | P06 |
| USR-003 | Reativar usuário exige decidir quais vínculos serão restaurados | E01, seção 2, OCR parcial | Proposta | P04 |
| AUTH-001 | Definir autenticação de internos e externos e necessidade de MFA | Proposta para o novo sistema | Proposta | P01, P02 |
| AUTH-002 | Retorno de autenticação não deve expor desnecessariamente a existência ou estado da conta | Proposta para o novo sistema | Proposta | P02 |
| PWD-001 | Troca e recuperação devem comprovar identidade e vincular a autorização à conta destinatária | Proposta; riscos relatados em E01, PWD003/007 | Proposta | P02 |
| SES-001 | Definir duração, simultaneidade e revogação de sessões | E01, SES; proposta nova | Proposta | P07 |
| GRP-001 | Definir quantidade de grupos por usuário e composição das permissões | E01, GRP006–008 | Proposta | P03 |
| HIE-001 | Distinguir administração de subordinados, visibilidade e herança de permissões | E01, USR011 e ACL001 | Proposta | P03 |
| ACL-001 | Autorizar ação e recurso no backend e aplicar escopo de dados | Proposta para o novo sistema | Proposta | P03, P05 |
| PRF-001 | Definir relação entre perfil, grupo e concessões diretas; resolver conflitos | E01, PRF/OBJ/ACL | Proposta | P03 |
| SCP-001 | Validar carteiras, agências e países em consultas e alterações | Proposta; escopos relatados em E01, SCP | Proposta | P05 |
| IP-001 | Definir políticas de IP, proxies confiáveis, exceções e reavaliação | E01, IP | Proposta | P08 |
| WFL-001 | Separar papel pessoal, funcional, responsável e permissão de ação | E01, WFR | Proposta | P06 |
| WFL-002 | Listagem, leitura e transição devem aplicar autorização sobre o recurso solicitado | Proposta; riscos relatados em E01, WFA | Proposta | P06 |
| INA-001 | Definir evento de atividade, relógio, limites e exceções para bloqueio/desativação | E01, INA e AUD006 | Proposta | P09 |
| AUD-001 | Registrar ator, ação, recurso, resultado e horário; definir retenção e acesso aos registros | Proposta; limitações relatadas em E01, AUD | Proposta | P10 |
| CAC-001 | Definir prazo verificável para revogação e atualização de políticas | Proposta; riscos relatados em E01, CAC/ACL007 | Proposta | P07 |

## Modelo para detalhamento

Cada regra será desdobrada com objetivo, atores, pré-condições, fluxo principal, alternativas, validações, estados/transições, acesso/escopo, eventos de auditoria, exemplos de aceite e evidências. Critérios verificáveis ainda pendentes não devem ser apresentados como concluídos.

Exemplo proposto para ACL-001: dado um usuário com acesso à carteira A e sem acesso à B, quando solicitar diretamente um recurso de B pela API, a operação deve ser negada sem devolver seu conteúdo. A resposta HTTP e a política de ocultação da existência do recurso serão definidas no contrato.
