# Microfrontends — requisitos e decisões abertas

## Requisitos confirmados

- MFE-005 (E31): microfrontends desenvolvidos somente pela equipe Recovery. Aprovada em 2026-10-31. Não altera a decisão de permitir qualquer origem HTTPS; autoria e hospedagem são dimensões diferentes.
- MFE-006 (E31): falha de carregamento deve mostrar erro apenas no módulo afetado, permitir nova tentativa e preservar o restante do portal. Aprovada. Isso é isolamento da experiência de falha, não comprovação de isolamento de segurança entre códigos.
- AUD-002 (E31): auditar alterações de módulos, menus, URLs, versões e permissões, incluindo ator, data, contexto e diferenças antes/depois. Ver [auditoria](../03-seguranca/auditoria-e-revogacao.md).

Critérios MFE-006: remoto indisponível ou inválido não impede uso da navegação essencial nem de outros módulos disponíveis; usuário consegue tentar carregar novamente. Falha não dispara retorno automático de versão, pois MFE-004 define retorno explícito. Timeouts, classificação das falhas e preservação de trabalho não salvo ainda serão definidos.

- Frontend React com microfrontends no foco inicial (E05).
- Administração de módulos/menus exclusiva Recovery, com estados por portal (E26–E29).
- MFE-001 (E29): cadastrar endereço remoto de carregamento.
- Módulo inativo bloqueia suas funcionalidades sem reescrever os estados individuais. Backend nega novas requisições após salvar; navegação reflete mudança em até 1 minuto.

## Fronteira de confiança

## Decisões E30 — 2026-10-31

| ID | Regra | Estado |
|---|---|---|
| MFE-002 | Permitir cadastro de qualquer origem HTTPS, sem lista prévia de origens autorizadas | Aprovada pelo solicitante |
| MFE-003 | Alterar endereço de carregamento exige permissão administrativa específica de gestão de microfrontends | Aprovada; atribuição explícita Recovery |
| MFE-004 | Registrar versão, manter histórico e permitir retorno explícito ao endereço/versão anterior | Aprovada em capacidade; processo técnico pendente |

MFE-002 substitui a proposta de lista de origens abaixo. Exceções HTTP locais não foram aprovadas nesta rodada. HTTPS não comprova confiabilidade do código remoto. Autorizar cadastro de origem não define isolamento, compartilhamento de sessão ou aceitação de manifestos arbitrários.

Retorno explícito é alteração administrativa auditável, não aprovação de fallback automático. Referência de versão não garante artefato imutável; identificar como comprovar correspondência entre versão e conteúdo na arquitetura.

Risco a tratar: remoto executado no mesmo contexto JavaScript do shell pode alcançar dados e capacidades desse contexto. Sem lista de origens, decidir isolamento e contrato de sessão antes da execução de código remoto. Não pressupor que microfrontend representa fronteira de segurança por si só.

O endereço remoto identifica código que será carregado no navegador. A autorização para alterar esse endereço é uma capacidade sensível, distinta da simples alteração de um rótulo de menu. Nenhum mecanismo de isolamento entre remotos foi escolhido ou comprovado nesta etapa.

## Controles propostos para decisão

- Administração do catálogo remoto restrita a permissão administrativa explícita Recovery.
- HTTPS obrigatório nas origens cadastradas (MFE-002); proposta anterior de lista de origens não adotada. Isolamento e exceções de desenvolvimento permanecem pendentes.
- Referência a versão identificável do artefato e processo de retorno à versão anterior.
- Validação do endereço ao salvar; caso o servidor consulte o endereço, controlar destinos e redirecionamentos para evitar requisições indevidas a serviços internos.
- Histórico das alterações de endereço, versão e estado; não colocar segredos no cadastro ou URL.
- Falha no remoto não deve comprometer autenticação, seleção de contexto e navegação essencial do shell.

Salvo os itens confirmados por MFE-002/003/004, esses controles permanecem propostas. Não basta confiar em uma origem para considerar todo código hospedado nela aprovado.

## Decisões técnicas pendentes

Framework de composição, formato do endpoint remoto/manifesto, origem por ambiente, versionamento, integridade e política de conteúdo; contrato shell/remoto para rotas, identidade, contexto, tema e tratamento de erros; estratégia de testes e implantação. Não escolher tecnologia somente a partir da necessidade de URL remota.
