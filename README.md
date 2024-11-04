# TopDownLevelGen: Geração Procedural de Níveis com Controle de Dificuldade

Este projeto implementa um gerador procedural de níveis para jogos 2D top-down, utilizando algoritmos genéticos para controle de dificuldade. O objetivo é criar níveis desafiadores e equilibrados, com foco em adaptar a dificuldade para melhorar a experiência do jogador.

## Estrutura do Projeto

O projeto é organizado nas seguintes pastas:

- **`/game-code`**: Contém o código-fonte do jogo, incluindo scripts, assets e lógica do jogo. Esta é a implementação principal do projeto, responsável pela geração procedural dos níveis e pela interação com o jogador.

- **API**: O código da API usada para armazenar e gerenciar os dados de análise do jogo está disponível no [Glitch](https://glitch.com/edit/#!/game-runs). A API centraliza as informações de desempenho e dificuldade coletadas durante o jogo.

- **`/analysis-results`**: Contém os scripts e as imagens geradas pelas análises de dados realizadas com Python. Essas análises examinam, de forma geral, o desempenho dos jogadores e a distribuição de dificuldade nos níveis gerados. As imagens ilustrativas estão organizadas em `/analysis-results/figs` para facilitar o acesso.

## Download da Release

A build final do jogo está disponível para download na seção de [Releases](https://github.com/JonathanYuri/TopDownLevelGen/releases/tag/v1.0).
