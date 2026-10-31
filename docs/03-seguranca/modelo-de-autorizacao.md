# Modelo de autorização — proposta inicial

Estado: proposta para discussão; não é política corporativa aprovada.

## Elementos da decisão

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
