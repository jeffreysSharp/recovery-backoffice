# Pendências atuais após consolidação

Data: 2026-10-31. Complementa [regras vigentes](../regras-vigentes.md). Histórico cronológico em `pendencias-do-legado.md`; evidências E01–E38 no inventário. Responsáveis a indicar com o time.

| Tema | Decisões/evidências ainda necessárias | Efeito |
|---|---|---|
| Identidade corporativa | Confirmar IdP, integração de CPF/login e senha temporária; método/inscrição/recuperação de MFA | Define autenticação e credenciais |
| Cadastro e edição | Unicidade do email, correção de CPF pela Recovery, tipos de usuário, campos globais editáveis e processo de novo vínculo autorizado | Fecha fluxos de identidade |
| Email e recuperação | Validade/consumo/reemissão de links, entrega, múltiplas solicitações e email inicial confirmado | Fecha autosserviço e primeiro acesso |
| Acesso administrativo | Catálogo de ações comuns/administrativas, autoalteração, último administrador, perfis mistos, gestão do limite delegável | Evita elevação indevida |
| Cópia e perfis | Vínculos ou permissões materializadas; preservação da administração existente ao substituir; efeito de alterações de perfis compartilhados | Define implementação da cópia |
| Hierarquia | Modelos centrais e distribuição; classificação/alteração de ações | Define origem das concessões |
| Escopos | Agência versus escritório, países/empresas, elegibilidade de vínculos inativos | Define fronteiras de dados |
| Sessões e login | Atividade, contadores, prazo de substituição, abas/contexto, logout IdP, MFA durante primeiro acesso | Fecha estados e concorrência |
| IP | Faixas/CIDR/IPv6, proxies, regras inativas/vazias, aplicabilidade via grupos herdados, primeiro provisionamento e acesso emergencial | Evita bloqueio ou liberação indevida |
| Portais | Campos e profundidade de menus, ações em andamento, APIs compartilhadas e escopo de configuração | Fecha disponibilidade e navegação |
| Microfrontends | Composição, isolamento, sessão/tema/contexto, manifesto, integridade/versão, implantação e retorno | Requer arquitetura |
| Auditoria | Volume, retenção, eventos globais visíveis ao escritório, exportação, persistência consistente e reentrega | Dimensiona armazenamento e proteção |
| Cadastros/importação | Campos e prioridades adiados; formatos, limites, duplicidade, falha de persistência e reenvio de lote | Detalhar por entrega |
| Workflow | Processo concreto, atores, estados, transições e recorte adiados; execução nova confirmada | Não especificado para implementação |
| Responsabilidades | Alertas e executor/prazo da transferência após retirada de acesso | Evita pendências operacionais invisíveis |
| Operação | Volumes desconhecidos, metas de disponibilidade, hospedagem/banco, migração/coexistência, ambientes e orçamento | Direciona decisões técnicas |

Não bloquear todo o projeto por itens adiados. Decisões dependentes devem permanecer abertas ou usar premissa explicitamente registrada, sem tratar silêncio como aprovação.
