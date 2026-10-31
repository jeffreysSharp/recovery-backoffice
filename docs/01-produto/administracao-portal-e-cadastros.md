# Administração do Portal e cadastros básicos

Fonte E05: áreas incluídas no foco inicial pelo solicitante. O detalhamento abaixo é proposta de refinamento, sem aprovação automática de funcionalidades.

Complemento E06: Administração do Portal abrange tanto o próprio Backoffice quanto o portal do devedor. Essa administração não antecipa a entrega das jornadas operacionais do devedor, adiadas em E05.

## Administração do Portal — catálogo candidato

- Cadastro de aplicações/módulos e seu estado de disponibilidade.
- Navegação e menus associados às permissões do usuário.
- Configurações administrativas com responsáveis e histórico.
- Associação de módulos às funcionalidades protegidas.

Os dois portais estão incluídos; funcionalidades específicas, configurações permitidas e responsáveis ainda serão refinados. Menu visível não substitui autorização de API. Alterações destinadas a um portal devem ter seu destino explícito, evitando aplicação acidental ao outro.

## Cadastros básicos — inventário candidato

| Cadastro | Situação |
|---|---|
| Usuários, grupos, perfis e permissões | Já identificados no refinamento de Identidade |
| Escritórios e vínculos com carteiras | Regras de escopo parcialmente confirmadas |
| Carteiras e países | Referenciados; definir fonte mestre e possibilidade de edição |
| Outros cadastros operacionais | Solicitar lista priorizada ao time |

### Lista inicial informada pelo solicitante (E06)

- Cidade.
- Estado.
- País.
- Sexo.
- Gênero.
- Tipo de documento.
- Marcas e veículos — relação exata e eventual cadastro de modelos ainda não definidos.

Esses itens foram identificados para o catálogo inicial; prioridade individual, campos, relacionamentos e fonte dos dados serão tratados depois, conforme orientação do solicitante. A lista é aberta e não implica implementação imediata de todos os itens. Sexo e gênero permanecem termos distintos; finalidade de uso e necessidade de coleta em cadastros de pessoas não são presumidas pela existência das tabelas de referência.

Para cada cadastro registrar proprietário do dado, campos, identificador, unicidade, estados, vínculos, exclusão/inativação, escopo, ações autorizadas, auditoria e critérios de aceite. Não presumir que a aplicação será a fonte mestre de todos os cadastros.

## Microfrontends — direcionamento

Microfrontends fazem parte do foco inicial. Ainda definir limites dos módulos, shell, carregamento, navegação, sessão compartilhada, tratamento de falhas e publicação. Escolha de framework de composição e independência de implantação exigem decisão arquitetural posterior.

Proposta de segurança: o shell orienta navegação e contexto; APIs verificam identidade, ação e escopo. Um microfrontend carregado não adquire privilégios adicionais por estar no portal.
