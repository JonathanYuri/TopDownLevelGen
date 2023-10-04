using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneticOperator
{
    #region Reproduction

    static void PlaceContentsInChild(
        RoomIndividual child,
        Dictionary<RoomContents, List<Position>> contentsPositionsInFather,
        Dictionary<RoomContents, List<Position>> contentsPositionsInMother,
        HashSet<Position> contentsPositionsToBePlaced,
        List<Position> avaliablePositions)
    {
        foreach (RoomContents key in contentsPositionsInFather.Keys)
        {
            HashSet<Position> combinedPositions = Utils.CombinePositions(contentsPositionsInFather[key], contentsPositionsInMother[key]);
            combinedPositions = combinedPositions.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
            Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(contentsPositionsInFather[key].Count);

            foreach (Position pos in chosenPositions)
            {
                child.RoomMatrix.PutContentInPosition(key, contentsPositionsToBePlaced, pos);
                avaliablePositions.Remove(pos);
            }

            // nao colocou todos, colocar o resto aleatoriamente no que eu tenho
            int missing = contentsPositionsInFather[key].Count - chosenPositions.Length;
            if (missing == 0) continue;

            Position[] positions = avaliablePositions.SelectRandomDistinctElements(missing);
            foreach (Position pos in positions)
            {
                child.RoomMatrix.PutContentInPosition(key, contentsPositionsToBePlaced, pos);
                avaliablePositions.Remove(pos);
            }
        }
    }

    static RoomIndividual Crossover(RoomIndividual father, RoomIndividual mother)
    {
        RoomIndividual child = new(false);

        Dictionary<RoomContents, List<Position>> enemiesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(father.RoomMatrix.Values, father.RoomMatrix.EnemiesPositions);
        Dictionary<RoomContents, List<Position>> enemiesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mother.RoomMatrix.Values, mother.RoomMatrix.EnemiesPositions);

        Dictionary<RoomContents, List<Position>> obstaclesPositionsInFather = RoomOperations.GroupPositionsByRoomValue(father.RoomMatrix.Values, father.RoomMatrix.ObstaclesPositions);
        Dictionary<RoomContents, List<Position>> obstaclesPositionsInMother = RoomOperations.GroupPositionsByRoomValue(mother.RoomMatrix.Values, mother.RoomMatrix.ObstaclesPositions);

        if (enemiesPositionsInMother.Keys.Count != enemiesPositionsInFather.Keys.Count) throw new Exception("Inimigos diferentes no father e na mother");
        if (obstaclesPositionsInMother.Keys.Count != obstaclesPositionsInFather.Keys.Count) throw new Exception("Obstaculos diferentes no father e na mother");

        List<Position> avaliablePositions = new(GeneticAlgorithmConstants.ROOM.ChangeablesPositions);

        PlaceContentsInChild(child, enemiesPositionsInFather, enemiesPositionsInMother, child.RoomMatrix.EnemiesPositions, avaliablePositions);
        PlaceContentsInChild(child, obstaclesPositionsInFather, obstaclesPositionsInMother, child.RoomMatrix.ObstaclesPositions, avaliablePositions);

        if (child.RoomMatrix.EnemiesPositions.Count != father.RoomMatrix.EnemiesPositions.Count
                                        ||
            child.RoomMatrix.ObstaclesPositions.Count != father.RoomMatrix.ObstaclesPositions.Count)
        {
            throw new ReproductionException("Gerou individual monstro na Reproducao",
                father.RoomMatrix.EnemiesPositions, mother.RoomMatrix.EnemiesPositions, child.RoomMatrix.EnemiesPositions,
                father.RoomMatrix.ObstaclesPositions, mother.RoomMatrix.ObstaclesPositions, child.RoomMatrix.ObstaclesPositions);
        }

        return child;
    }

    static RoomIndividual[] TournamentSelection(RoomIndividual[] population)
    {
        RoomIndividual[] parents = new RoomIndividual[GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT];

        for (int i = 0; i < GeneticAlgorithmConstants.NUM_PARENTS_TOURNAMENT; i++)
        {
            List<RoomIndividual> tournament = population.SelectRandomDistinctElements(GeneticAlgorithmConstants.TOURNAMENT_SIZE).ToList();

            // O vencedor do torneio (quem tem a maior fitness dos selecionados aleatoriamente) é selecionado para reprodução
            parents[i] = tournament.MaxBy(individual => individual.Value);
        }

        return parents;
    }

    public static RoomIndividual[] PerformReproduction(RoomIndividual[] population)
    {
        RoomIndividual[] newPopulation = new RoomIndividual[GeneticAlgorithmConstants.POPULATION_SIZE];

        for (int count = 0; count < GeneticAlgorithmConstants.POPULATION_SIZE; count += 2)
        {
            RoomIndividual[] parents = TournamentSelection(population);

            if (Random.value < GeneticAlgorithmConstants.CROSSOVER_PROBABILITY)
            {
                RoomIndividual children1 = Crossover(parents[0], parents[1]);
                RoomIndividual children2 = Crossover(parents[1], parents[0]);

                newPopulation[count] = children1;
                newPopulation[count + 1] = children2;
            }
            else
            {
                // Se nao houver cruzamento, copie os fathers diretamente para a nova populacao
                newPopulation[count] = parents[0];
                newPopulation[count + 1] = parents[1];
            }
        }

        return newPopulation;
    }

    #endregion

    #region Mutation

    static void SwapRoomPositionsRandomly(RoomIndividual individual, Position position1)
    {
        int idx2 = Random.Range(0, GeneticAlgorithmConstants.ROOM.ChangeablesPositions.Count);
        Position position2 = GeneticAlgorithmConstants.ROOM.ChangeablesPositions.ElementAt(idx2);

        RoomContents content1 = individual.RoomMatrix.Values[position1.X, position1.Y];
        RoomContents content2 = individual.RoomMatrix.Values[position2.X, position2.Y];

        individual.RoomMatrix.UpdateContentInPosition(position1, content1, content2);
        individual.RoomMatrix.UpdateContentInPosition(position2, content2, content1);
    }

    static void Mutate(RoomIndividual individual, HashSet<Position> positionsToMutate)
    {
        Position[] positionsToChange = positionsToMutate.SelectRandomDistinctElements(Random.Range(0, positionsToMutate.Count));
        foreach (Position position in positionsToChange)
        {
            SwapRoomPositionsRandomly(individual, position);
        }
    }

    static void Mutate(RoomIndividual individual)
    {
        // escolher inimigos ou obstaculos para mudar
        if (Random.value < 0.5f)
        {
            Mutate(individual, individual.RoomMatrix.EnemiesPositions);
        }
        else
        {
            Mutate(individual, individual.RoomMatrix.ObstaclesPositions);
        }
    }

    public static void MutatePopulation(RoomIndividual[] population)
    {
        foreach (RoomIndividual individual in population)
        {
            if (Random.value < GeneticAlgorithmConstants.MUTATION_PROBABILITY)
            {
                Mutate(individual);
                individual.ItWasModified = true;
            }
        }
    }

    #endregion
}
