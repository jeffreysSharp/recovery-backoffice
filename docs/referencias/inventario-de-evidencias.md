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
