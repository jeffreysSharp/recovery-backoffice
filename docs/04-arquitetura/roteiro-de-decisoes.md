# Próxima etapa: arquitetura .NET e React

Estado: roteiro, nenhuma tecnologia adicional aprovada. Base: [regras vigentes](../regras-vigentes.md). E38 autoriza consolidar e depois avançar à arquitetura.

## Ordem proposta

1. Definir limites de módulos backend e persistência: comparar monólito modular e serviços separados segundo operação, equipe e integrações.
2. Definir identidade e sessão: compatibilidade de CPF/senha temporária/MFA com provedor; sessão única, contexto e revogação.
3. Definir shell React e carregamento remoto: confiança no código, isolamento, contratos e implantação.
4. Modelar identidade, vínculos, grupos em grafo, concessões, limites, disponibilidade e auditoria durável.
5. Especificar APIs, concorrência, processamento de importação e consistência alteração/auditoria.
6. Definir ambientes, custos de homologação, observabilidade, testes de autorização e estratégia de implantação.

Cada escolha relevante terá ADR com alternativas, consequências e regras atendidas. Versões e capacidades externas devem ser verificadas em documentação oficial ao decidir. Não escolher pacotes, microserviços, framework de composição ou mecanismo de token como se fossem requisitos de negócio.

## Próximas informações úteis

- Tamanho da equipe e necessidade real de implantação independente dos módulos.
- Infraestrutura obrigatória: banco, containers, Azure ou padrões corporativos.
- Grau de autonomia para propor a arquitetura, considerando o objetivo de homologação econômica.

Volumes ainda desconhecidos: trabalhar com cenários explicitamente hipotéticos, não com garantias de capacidade ou gratuidade.
