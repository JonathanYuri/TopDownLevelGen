# TCC

# Geração de Níveis em Jogos Top-Down com Algoritmo Genético

## Instruções de Instalação

Para experimentar o projeto ou contribuir com o desenvolvimento, siga uma das opções abaixo:

### 1. Abra o Projeto na Unity

- Certifique-se de que você possui o software Unity instalado em sua máquina.
- Baixe o projeto em seu computador clicando em "Clone or Download" e escolha "Download ZIP" ou usando o comando `git clone` se preferir.
- Abra a Unity e escolha a opção "Open Project".
- Navegue até a pasta do projeto baixado e selecione a pasta LevelGenerator e clique em "Open".

### 2. Jogar ou Testar

- Se você deseja apenas jogar ou testar o projeto sem mexer no código, você pode acessar uma versão já compilada diretamente no link a seguir:
- [Link para Build WebGL, Linux e Windows](https://github.com/JonathanYuri/level-generator)
- Baixe a versão adequada para o seu sistema operacional e siga as instruções de execução do jogo.

## Algoritmo Genético

O Algoritmo Genético (GA) é a base deste projeto de geração de níveis em um jogo top-down. Ele é usado para evoluir e otimizar as configurações das salas de forma a atender a critérios específicos. Abaixo, descrevemos como o GA é aplicado neste projeto e seus principais componentes.

### Introdução ao Algoritmo Genético

Um Algoritmo Genético é uma técnica de otimização inspirada no processo de seleção natural. Ele opera com uma população de indivíduos e utiliza operadores genéticos, como reprodução (crossover) e mutação, para gerar novas soluções ao longo de várias gerações.

### Aplicação no Projeto

Neste projeto, o Algoritmo Genético é aplicado para gerar configurações de salas em um jogo top-down. A otimização visa criar salas que atendam a critérios específicos, como a distribuição de inimigos, obstáculos e outros elementos do jogo.

### Funcionamento Básico

Os principais componentes do Algoritmo Genético neste projeto incluem:

#### População

A população é composta por indivíduos que representam configurações de salas. Cada indivíduo é uma possível solução.

#### Reprodução (Crossover)

O crossover envolve a combinação de informações genéticas de dois indivíduos pais para criar um novo indivíduo filho. No contexto deste projeto, a operação de crossover é realizada selecionando as posições dos inimigos presentes nos pais e, a partir desse conjunto, escolhendo a quantidade necessária de inimigos para a nova sala do filho. O mesmo princípio é aplicado às posições dos obstáculos. No entanto, que existe a possibilidade de alguns obstáculos não serem alocados, devido à presença de inimigos ocupando as posições. Nesse cenário, tomamos a decisão de distribuir os obstáculos restantes nas posições que ainda se encontram disponíveis na sala.

#### Mutação

A mutação envolve a introdução de pequenas alterações nas configurações das salas. Isso ajuda a explorar soluções diferentes daquelas geradas pelo crossover. No contexto deste projeto, a operação de mutação se dá escolhendo entre dois elementos: obstáculos ou inimigos. Em seguida, é feita uma segunda seleção para determinar quantos elementos terão suas posições alteradas. Por fim, as posições desses elementos selecionados são modificadas de forma aleatória.

#### Avaliação de Fitness

A função de fitness é usada para avaliar quão bem uma configuração de sala atende aos critérios desejados. Configurações melhores recebem uma pontuação de fitness mais alta. No contexto deste projeto, a avaliação de fitness é subdividida da seguinte forma:

Sendo que os passos 1 e 2 servem para normalizar as variáveis do fitness, para que cada variável contribua igualmente para a avaliação do indivíduo.

1. Passando por cada indivíduo da população e calculando e armazenando os valores de cada variável que será usada para calcular o valor de fitness.

2. Calculando os limites (máximo e mínimo) de cada variável.

3. Passando por cada indivíduo da população e atribuindo o valor somado das variáveis normalizadas.

A ideia por trás das variáveis é:

- Minimizar a quantidade de grupos.
- Minimizar a media de inimigos por grupos.
- Maximizar a distância entre os inimigos e as portas quando a dificuldade da sala for mínima (0)
- Minimizar essa distância quando a dificuldade atingir seu valor máximo (1)
- Para valores de dificuldade entre esses extremos, busca-se otimizar o valor por meio de uma interpolação linear que equilibre a maximização e minimização.

### Parâmetros e Configurações

Os seguintes parâmetros e configurações são usados no Algoritmo Genético deste projeto:

- **ITERATIONS_WITHOUT_IMPROVEMENT:** O número de iterações sem melhoria necessárias para parar o algoritmo.

- **CROSSOVER_PROBABILITY:** A probabilidade de ocorrer crossover durante a reprodução (varia de 0 a 1).

- **MUTATION_PROBABILITY:** A probabilidade de ocorrer mutação em um indivíduo (varia de 0 a 1).

- **POPULATION_SIZE:** O tamanho da população de indivíduos.

- **TOURNAMENT_SIZE:** O tamanho do torneio usado na seleção dos pais.

- **NUM_PARENTS_TOURNAMENT:** O número de pais selecionados em cada torneio.

- **ROOM:** A definição da sala do jogo, incluindo a dificuldade, as portas, os inimigos e os obstáculos.

### Resultados e Desempenho

O Algoritmo Genético é executado para gerar configurações de salas otimizadas para o jogo top-down. Os resultados incluem salas que atendem aos critérios de distribuição de elementos, dificuldade e outros requisitos do jogo.

### Exemplos de Saída

Aqui estão exemplos visuais das salas geradas pelo Algoritmo Genético:

## Licença

Este projeto é licenciado sob a Licença MIT - consulte o arquivo LICENSE.md para obter detalhes.

## Autor

- [Jonathan Yuri](https://github.com/JonathanYuri)

---
