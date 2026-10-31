# Backlog inicial de refinamento

Este backlog organiza a preparação das entregas. Não contém estimativas nem arquitetura aprovada.

Direcionamento E05: priorizar Identidade, Administração do Portal, estrutura de microfrontends e inventário dos cadastros básicos. Jornadas do devedor e integração de boletos ficam fora da entrega inicial. O levantamento de Workflow mantém a cobertura documental acordada, com execução operacional a dimensionar separadamente.

Complemento E06: administrar ambos os portais. Inventário inicial de cadastros registrado; aprofundamento adiado pelo solicitante. Próximo refinamento: ciclo de vida do usuário, vínculos de acesso e administração dos módulos, sem depender dos campos desses cadastros.

Atualização E35: execução de Workflow será nova, porém processo concreto e recorte foram adiados. Não tratar o levantamento como concluído nem escolher motor antes do refinamento; continuar demais entregas independentes.

| Ordem | Trabalho | Resultado verificável | Dependências |
|---|---|---|---|
| 1 | Confirmar atores e fronteiras | Catálogo validado e respostas P01/P05 | Negócio |
| 2 | Definir identidade e autenticação | Fluxos de entrada, saída, recuperação e integração | P02 |
| 3 | Refinar usuários e vínculos | Campos, estados, transições e critérios de aceite | P03/P04 |
| 4 | Refinar autorização e escopos | Matriz ator × ação × recurso × escopo, exemplos permitidos e negados | P03/P05 |
| 5 | Mapear Workflow | Processos, etapas, ações, responsáveis, alçadas e critérios de aceite | P06; iniciar levantamento em paralelo |
| 6 | Refinar IP, sessões e inatividade | Políticas com valores e eventos definidos | P07/P08/P09 |
| 7 | Refinar auditoria e operação | Eventos, retenção, revogação e metas mensuráveis | P10/P12 |
| 8 | Projetar .NET, React e persistência | Arquitetura, modelo de dados, ADRs e contratos de API | Decisões funcionais correspondentes |
| 9 | Implementar fatias funcionais | Fluxo completo com backend, frontend e validações de acesso | Critérios da funcionalidade definidos |

## Verificação das entregas

Relacionar os critérios de aceite aos IDs de regras. Cobrir acesso permitido e negado, recursos fora do escopo, mudanças durante sessão, concorrência relevante e trilha de auditoria. Documentar limitações conhecidas e não declarar uma política validada apenas com testes da interface.

O escopo completo permanece no MVP; a ordem acima não exclui módulos. Pendências bloqueiam somente decisões e entregas que dependam de suas respostas.
