# Jornadas do devedor no MVP

**Reclassificação E05:** contexto futuro da plataforma, fora da entrega e do refinamento inicial do MVP Backoffice. Preservado para rastreabilidade; questões e critérios abaixo não são bloqueadores do MVP atual. Não aprofundar boletos ou integração financeira nesta etapa.

Fonte E04: confirmação do solicitante em 2026-10-31. O devedor é público do MVP, com as seguintes capacidades aprovadas em escopo:

| ID | Capacidade | Estado |
|---|---|---|
| DEV-001 | Consultar dívidas | Aprovada em escopo; detalhes pendentes |
| DEV-002 | Emitir boleto | Aprovada em escopo; integração e semântica pendentes |
| DEV-003 | Acompanhar acordos | Aprovada em escopo; dados e estados pendentes |

Negociação não foi incluída na seleção atual de jornadas. Inclusão futura exige decisão explícita; não inferir criação ou alteração de acordos a partir de acompanhamento.

## Controles propostos

- Validar a identidade e o vínculo do devedor com cada dívida, acordo e boleto solicitado no backend.
- Não aplicar ao devedor o alcance de visualização de carteiras concedido aos operadores de escritórios.
- Não usar somente CPF/documento ou identificador previsível como prova de identidade.
- Registrar consultas e emissão conforme política de auditoria e proteção de dados a definir.

## Questões abertas

1. Como comprovar identidade e titularidade? Há cadastro prévio e contato verificado na origem?
2. “Emitir boleto” significa recuperar segunda via já existente ou solicitar nova emissão? Qual sistema/banco é a fonte?
3. Quais dados e estados serão exibidos para dívidas e acordos? Como refletir pagamentos e atualizações?
4. Há representantes, pessoas jurídicas ou múltiplos titulares? Não conceder representação implicitamente.

## Critérios de aceite propostos

- Devedor autenticado consulta somente recursos cuja titularidade ou representação tenha sido validada.
- Alterar o identificador da dívida, acordo ou boleto na requisição não permite consultar recursos de outra pessoa.
- Acompanhamento não permite alterar condições do acordo.
- Boleto indisponível ou integração com falha produz resultado explícito; a aplicação não inventa dados de pagamento.

Os fluxos detalhados dependem das respostas acima e das integrações. Não há emissão financeira implementada.
