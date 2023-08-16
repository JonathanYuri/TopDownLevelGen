using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticRoomIndividual
{
    public Possibilidades[,] roomMatrix;
    public int? value;

    // usado pra cruzamento
    public GeneticRoomIndividual(int rows, int cols)
    {
        value = null;
        roomMatrix = new Possibilidades[rows, cols];
    }

    // usado pra criar aleatoriamente a sala, quando gera a populacao
    public GeneticRoomIndividual(Sala sala) : this(sala.Rows, sala.Cols) // chama o construtor
    {
        // gerar aleatoriamente a sala
        GenerateRoomRandomly(sala);
    }

    public GeneticRoomIndividual(GeneticRoomIndividual individual)
    {
        value = individual.value;
        roomMatrix = individual.roomMatrix;
    }

    void GenerateRoomRandomly(Sala sala)
    {
        roomMatrix = (Possibilidades[,])sala.matriz.Clone();

        List<Tuple<int, int>> aux = new(sala.changeablesPositions);
        foreach (Enemies enemie in sala.enemies)
        {
            // procurar o inimigo na possibilidades
            var enumValues = Enum.GetValues(typeof(Possibilidades));

            Possibilidades inimigo = Possibilidades.Inimigo1;
            foreach (Possibilidades value in enumValues)
            {
                if (enemie.ToString() == value.ToString())
                {
                    inimigo = value;
                    break;
                }
            }

            int rand = Random.Range(0, aux.Count);
            roomMatrix[aux[rand].Item1, aux[rand].Item2] = inimigo;
            aux.RemoveAt(rand);
        }

        aux = new(sala.changeablesPositions);
        foreach (Obstacles obstacles in sala.obstacles)
        {
            // procurar o obstaculo na possibilidades
            var enumValues = Enum.GetValues(typeof(Obstacles));
            Possibilidades obstacle = Possibilidades.Obstaculo1;
            foreach (Possibilidades value in enumValues)
            {
                if (obstacles.ToString() == value.ToString())
                {
                    obstacle = value;
                    break;
                }
            }

            int rand = Random.Range(0, aux.Count);
            roomMatrix[aux[rand].Item1, aux[rand].Item2] = obstacle;
            aux.RemoveAt(rand);
        }
    }

    void ChangePlaceOf(Sala sala, Type changePlace)
    {
        Tuple<int, int> position = Utils.ChooseLocationThatHas(roomMatrix, sala.changeablesPositions, changePlace);
        if (position.Item1 != -1 && position.Item2 != -1)
        {
            // escolher um lugar que nao tenha o mesmo inimigo
            Tuple<int, int> positionFreeFrom = Utils.ChooseLocationFreeFrom(roomMatrix, sala.changeablesPositions, roomMatrix[position.Item1, position.Item2]);
            if (positionFreeFrom.Item1 != -1 && positionFreeFrom.Item2 != -1)
            {
                (roomMatrix[positionFreeFrom.Item1, positionFreeFrom.Item2], roomMatrix[position.Item1, position.Item2])
                                                                =
                (roomMatrix[position.Item1, position.Item2], roomMatrix[positionFreeFrom.Item1, positionFreeFrom.Item2]);
            }
        }
    }

    public void Mutate(Sala sala)
    {
        // escolher um inimigo ou um obstaculo para mudar
        if (Random.value < 0.5f)
        {
            ChangePlaceOf(sala, typeof(Enemies));
        }
        else
        {
            ChangePlaceOf(sala, typeof(Obstacles));
        }
    }

    bool IsMonstrousIndividual(Sala sala)
    {
        int qntCaminhosEntrePortas = Utils.CountPathsBetweenDoors(roomMatrix, sala.doorsPositions);
        if (qntCaminhosEntrePortas == int.MinValue)
        {
            return true;
        }

        bool isPath = Utils.IsPathBetweenDoorAndEnemies(roomMatrix, sala.doorsPositions);
        if (!isPath)
        {
            return true;
        }

        bool hasTheRightAmountOfEnemies = Utils.CountOccurrences(roomMatrix, typeof(Enemies)) == sala.enemies.Count;
        if (!hasTheRightAmountOfEnemies)
        {
            return true;
        }

        bool hasTheRightAmountOfObstacles = Utils.CountOccurrences(roomMatrix, typeof(Obstacles)) == sala.obstacles.Count;
        if (!hasTheRightAmountOfObstacles)
        {
            return true;
        }

        return false;
    }

    public void Evaluate(Sala sala)
    {
        if (IsMonstrousIndividual(sala))
        {
            value = int.MinValue;
            return;
        }

        List<int> groups = Utils.CountGroups(roomMatrix, typeof(Enemies));
        float media = Utils.CalculateAverage(groups);
        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);
        value = - groups.Count - (int)media; // + qntInimigosProximosDeObstaculos;
    }
}
