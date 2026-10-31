# Modelo de autorização — proposta inicial

Estado: proposta para discussão; não é política corporativa aprovada.

Confirmações E07: múltiplos grupos/perfis por usuário, permissões variáveis por escritório, administração central e por escritório. Criar/alterar acessos não exige segunda aprovação. Ver [administração de identidade](../02-negocio/administracao-de-identidade.md). Diretrizes restantes neste documento continuam propostas.

Confirmações E08: escolha de escritório ativo; administradores de escritório cadastram usuários, copiam permissões e nomeiam administradores do próprio escritório. A cópia não tem seus limites detalhados ainda e não deve ser interpretada como concessão de administração central.

E09 delimita a cópia ao próprio escritório, com escolha entre acrescentar e substituir. A Recovery define limites delegáveis para a personalização de grupos/perfis pelo escritório; o conteúdo copiado e a representação desses limites ainda serão detalhados.

E10 confirma limite delegável único para todos os escritórios, cópia somente de permissões comuns e independência entre atribuições de origem e destino. Permissões efetivas continuam variando por usuário e escritório. Nomeação administrativa é uma operação separada da cópia; perfis mistos e alterações no limite global precisam de tratamento explícito.

## Elementos da decisão

E25 prevalece sobre a hipótese de seleção separada das ações: perfil global concede todas as ações comuns e seleção de qualquer escritório, concedido/revogado exclusivamente pela Recovery. Administração segue explícita; IP/MFA/contexto continuam obrigatórios. Hierarquia sem ciclos nem relações entre grupos personalizados de escritórios diferentes.

E24: perfil global Recovery habilita seleção de qualquer escritório, mantendo contexto ativo e controles. Hierarquia admite múltiplos superiores. Atualização de política IP e retirada de permissões alcançam sessões existentes em até 1 minuto, provisoriamente. Permissões de ação do perfil global e prevenção de ciclos ainda precisam ser detalhadas.

E23: política IP obrigatória do escritório e restrições adicionais opcionais de grupos/perfis. Etapa restrita pré-contexto condicionada à elegibilidade IP em pelo menos um escritório autorizado, sem dados operacionais. Usuários Recovery também usam escritório ativo. Não há autorização para contexto global alternativo; alcance amplo ainda a representar.

E22: IP exige configuração e validação no login, troca de escritório e requisições protegidas. Concessões comuns são unidas no contexto ativo, sem negativas explícitas, provisoriamente. União não supera demais controles. Política do login antes de seleção de contexto e provisionamento inicial ainda abertos.

Atualização E20: há herança de permissões dos grupos inferiores para superiores. Restrições de IP existem nos níveis escritório e grupo/perfil e são administradas somente pela Recovery. A composição e os limites da herança ainda precisam de decisão.

E21 resolve parte desses limites: herdar permissões comuns de todos os descendentes; administração exige atribuição explícita. IP: todas as restrições aplicáveis devem permitir, provisoriamente. Definição de quais restrições se aplicam ainda aberta.

A decisão considera identidade, estado da conta, sessão, ação, recurso, escopo e restrições aplicáveis. Grupos e perfis podem fornecer concessões; papéis de Workflow acrescentam requisitos operacionais. A composição exata depende das decisões P03, P05 e P06.

## Diretrizes propostas

- Negar quando não houver concessão explícita aplicável.
- Validar autorização no backend em cada operação protegida; a interface reflete a decisão para orientar o usuário.
- Combinar permissão funcional com o escopo do recurso, inclusive em exportações e operações em lote.
- Não presumir herança por hierarquia, união entre grupos ou acesso global de administradores.
- Não confiar em carteira, agência, país, usuário ou papel enviados pelo navegador sem validação no servidor.
- Não usar IP como comprovação de identidade; definir confiança em proxies antes de consumir cabeçalhos.
- Planejar revogação para sessões existentes e para caches, com prazo testável.
- Proteger alterações de privilégios e registrar resultados relevantes sem gravar senhas ou tokens.

## Cenários para validação futura

1. Acesso direto pela API a recurso fora do escopo.
2. Usuário bloqueado com sessão já aberta.
3. Alteração de grupos durante sessão ativa.
4. Usuário com múltiplos vínculos e concessões conflitantes.
5. Transição de Workflow sem papel ou responsabilidade exigida.
6. Alteração de IP durante sessão e requisição com cabeçalho encaminhado não confiável.
7. Administrador tentando conceder acesso fora de sua delegação.

Matriz de acesso, modelo de ameaças e controles técnicos serão detalhados a partir dos atores e fluxos confirmados. Ainda não há decisão de usar RBAC, ABAC, multi-tenancy, JWT ou sessões específicas.
