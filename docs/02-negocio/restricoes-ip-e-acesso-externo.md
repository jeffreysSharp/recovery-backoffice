# Restrições de IP e acesso externo

## Regras confirmadas em E20

| ID | Regra | Estado |
|---|---|---|
| IP-002 | Restrições de IP podem ser configuradas por escritório e por grupo/perfil | Aprovada em capacidade; composição pendente |
| IP-003 | Somente administração autorizada Recovery pode administrar restrições de IP | Aprovada |

Configuração por escritório e por grupo/perfil não define se as listas se somam ou se ambas devem aceitar o endereço. Não implementar composição permissiva por suposição.

Atualização E21: IP-004 define provisoriamente que **todas as restrições aplicáveis devem permitir o endereço**. Uma restrição aplicável que negue impede acesso, mesmo que outra permita. O conjunto de regras aplicáveis, ausência de regra, listas vazias e herança de restrições ainda precisam de definição. Várias faixas dentro da mesma regra não são automaticamente várias restrições independentes.

## Pendências

E24: mudanças de política IP devem produzir efeito nas sessões abertas em até 1 minuto, provisoriamente (SES-006). Reavaliar requisições protegidas conforme IP-006 não implica ignorar esse limite de atualização dos dados da política. Perfil global Recovery permite selecionar qualquer escritório, mas continua sujeito às regras IP desse contexto.

### Decisões E23 — 2026-10-31

- IP-007: política de IP obrigatória por escritório; regras de grupo/perfil opcionais e somente restritivas. Aprovada. Ausência de regra opcional não bloqueia por si só; ausência da política obrigatória impede acesso.
- IP-008: antes de selecionar escritório, permitir somente etapa restrita de autenticação/seleção se o IP for aceito por pelo menos um escritório autorizado ao usuário. Não liberar dados operacionais; ao selecionar, aplicar todas as regras do contexto. Aprovada. Detalhar ordem das verificações, proteção contra enumeração e regras de grupo/perfil na elegibilidade.
- IAM-015: administradores Recovery também devem utilizar escritório ativo. Aprovada. Não criar contexto administrativo global alternativo por presunção.

Essas decisões resolvem o nível obrigatório e o fluxo pré-contexto citados nas pendências históricas abaixo. Provisionamento inicial e representação dos vínculos de usuários Recovery continuam abertos.

Atualização E22:

- IP-005: configuração de IP é obrigatória; contexto sem configuração não permite acesso. Aprovada pelo solicitante em 2026-10-31.
- IP-006: validar IP no login, na troca de escritório e em requisições protegidas. Aprovada.

Essas decisões resolvem os itens abaixo sobre ausência total de configuração e momento de avaliação. Ainda definir se os dois níveis precisam sempre de regra, qual contexto vale antes de selecionar escritório e como provisionar o primeiro acesso administrativo Recovery. Não presumir que ausência em um nível opcional equivale a ausência total de política.

Critérios adicionais: senha e MFA corretos não autorizam contexto sem política de IP; troca de escritório exige validação da política do destino; requisições protegidas verificam a política aplicável. Tratamento de listas vazias e regras inativas continua pendente.

- Quais restrições se aplicam em múltiplos grupos/perfis e na hierarquia; combinação entre restrições já definida provisoriamente em IP-004.
- Efeito de ausência de configuração, lista vazia e regra inativa.
- Aplicação no login, seleção/troca de escritório e requisições subsequentes.
- IPv4/IPv6, faixas/CIDR, VPN, proxies confiáveis e origem do endereço.
- Exceções, acesso de emergência e prevenção de bloqueio dos administradores.
- Validade de acesso externo por data e horário; não inferida apenas da política de IP.

## Diretrizes propostas de implementação

- Usar comparação de endereços/faixas válidos, sem busca textual por fragmentos.
- Aceitar cabeçalhos de encaminhamento somente de proxies explicitamente confiáveis.
- Registrar alteração de regras e decisões relevantes de bloqueio sem expor dados desnecessários.
- Usuário de escritório não altera regras de IP diretamente pela API, mesmo que consiga manipular a interface.

## Critérios iniciais

- Recovery autorizada consegue administrar regras nos dois níveis.
- Administrador de escritório não consegue criar, editar ou remover restrições de IP.
- Se restrição de escritório e restrição de grupo/perfil forem aplicáveis, endereço permitido por somente uma delas é rejeitado (IP-004, provisória).
