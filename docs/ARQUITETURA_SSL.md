# Arquitetura de referência — RobotSoccer SSL

Este documento registra a direção atual do projeto para aproximar o robô de uma arquitetura real da **RoboCup Small Size League (SSL)**.

> **Status:** conceito de engenharia. Os componentes abaixo são referências reais ou classes de componentes reais, mas a seleção final de compra ainda será fechada antes do CAD de fabricação.

## Restrições de projeto

- Envelope máximo SSL 2026: **cilindro de 180 mm de diâmetro por 150 mm de altura**.
- O robô deve ter abertura frontal para interação com a bola sem aprisioná-la.
- A arquitetura passa a considerar **4 rodas omni**, driblador frontal, sensor local de posse e chutador central.
- A posição global da bola e dos robôs vem do **SSL-Vision**; o robô recebe comandos do computador da equipe por rádio.
- Um **sensor IR no driblador** confirma localmente quando a bola está efetivamente posicionada para condução/chute.

## Componentes de referência

| Subsistema | Referência atual | Status |
|---|---|---|
| Tração | 4 × motores BLDC com encoder | Modelo final a definir |
| Motor de referência SSL | Maxon EC 45 flat, 50 W, 18 V | Referência usada por equipes SSL |
| Rodas | 4 × rodas omni de aproximadamente 54–60 mm | Diâmetro final a definir |
| Drivers de tração | 4 × drivers BLDC compatíveis com os motores selecionados | A definir |
| Controlador embarcado | ESP32-S3 DevKitC-1 no protótipo | Definido como referência inicial |
| PCB final | Placa dedicada integrando controle, comunicação e potência | Etapa posterior |
| Driblador | Rolete frontal de alta aderência, motorizado | Geometria a definir |
| Motor do driblador | Referência: Maxon EC-max 22, 25 W | Modelo final a definir |
| Sensor de posse | Emissor IR + receptor/fototransistor na boca do driblador | Arquitetura definida; modelo a escolher |
| Chute | Solenoide/atuador eletromagnético + banco de capacitores | Dimensionamento a definir |
| Bateria | LiPo | Tensão/capacidade dependem da tração escolhida |
| Comunicação | Rádio entre computador da equipe e robô | Tecnologia final a definir |
| Visão global | SSL-Vision | Arquitetura definida |

## Correção importante

O desenho conceitual anterior mostrava **motores BLDC** junto com drivers **DRV8876**. Essa associação não deve ser usada no projeto: o DRV8876 é um driver para **motor DC escovado**. Para os motores BLDC serão escolhidos drivers BLDC compatíveis com tensão, corrente, encoder e estratégia de controle do motor selecionado.

## Referências técnicas

- Regras oficiais SSL 2026: https://robocup-ssl.github.io/ssl-rules/
- Visão geral técnica SSL: https://ssl.robocup.org/technical-overview-of-the-small-size-league/
- TDPs/ETDPs das equipes: https://ssl.robocup.org/team-description-papers/
- ITAndroids 2025: usa Maxon EC-45 50 W 18 V na tração.
- SPbUnited 2025: usa Maxon EC 45 flat 50 W na tração, rodas de 57 mm, Maxon EC-max 22 25 W no driblador e driver BLDC customizado.

## Próximas definições antes do CAD final

1. Selecionar o motor BLDC de tração e o driver correspondente.
2. Selecionar a roda omni e obter o modelo CAD/dimensões oficiais.
3. Dimensionar a alimentação e selecionar a bateria.
4. Selecionar o motor e o rolete do driblador.
5. Selecionar emissor e receptor IR da detecção de posse.
6. Dimensionar solenoide, capacitor e circuito de carga do chutador.
7. Reorganizar o `RobotSoccer_All.fs` ao redor desses componentes reais.
