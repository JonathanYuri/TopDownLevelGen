using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        List<Position> aux = new(sala.changeablesPositions);
        foreach (Enemies enemie in sala.enemies)
        {
            // procurar o inimigo na possibilidades
            Possibilidades inimigo = Utils.TransformAElementFromEnumToPossibilidadesEnum(typeof(Possibilidades), enemie);

            int rand = Random.Range(0, aux.Count);
            roomMatrix[aux[rand].Row, aux[rand].Column] = inimigo;
            aux.RemoveAt(rand);
        }

        aux = new(sala.changeablesPositions);

        foreach (Obstacles obstacle in sala.obstacles)
        {
            // procurar o obstaculo na possibilidades
            Possibilidades obstaculo = Utils.TransformAElementFromEnumToPossibilidadesEnum(typeof(Possibilidades), obstacle);

            int rand = Random.Range(0, aux.Count);
            roomMatrix[aux[rand].Row, aux[rand].Column] = obstaculo;
            aux.RemoveAt(rand);
        }
    }

    void ChangePlaceOf(Sala sala, Type changePlace)
    {
        Position position = RoomOperations.ChooseLocationThatHas(roomMatrix, changePlace);
        if (position.Row != -1 && position.Column != -1)
        {
            // escolher um lugar que nao tenha o mesmo inimigo
            Position positionFreeFrom = RoomOperations.ChooseLocationFreeFrom(roomMatrix, sala.changeablesPositions, roomMatrix[position.Row, position.Column]);
            if (positionFreeFrom.Row != -1 && positionFreeFrom.Column != -1)
            {
                (roomMatrix[positionFreeFrom.Row, positionFreeFrom.Column], roomMatrix[position.Row, position.Column])
                                                                =
                (roomMatrix[position.Row, position.Column], roomMatrix[positionFreeFrom.Row, positionFreeFrom.Column]);
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

        List<Position> enemiesPositions = RoomOperations.GetPositionsThatHas(roomMatrix, typeof(Enemies));
        List<Position> obstaclesPositions = RoomOperations.GetPositionsThatHas(roomMatrix, typeof(Obstacles));

        bool hasTheRightAmountOfEnemies = enemiesPositions.Count == sala.enemies.Count;
        if (!hasTheRightAmountOfEnemies)
        {
            return true;
        }

        bool hasTheRightAmountOfObstacles = obstaclesPositions.Count == sala.obstacles.Count;
        if (!hasTheRightAmountOfObstacles)
        {
            return true;
        }

        // TODO: REFACTOR ENEMIES AND OBSTACLES, melhorar essas paradas do enum, tao ficando complicadas ja


        // hasTheRightEnemies;
        /*
        var enumValues = Enum.GetValues(typeof(Enemies));
        List<Enemies> enemies = new();
        foreach (Position enemyPosition in enemiesPositions)
        {
            // pegar o inimigo equivalente no enum de Possibilidades // TODO: separar esse codigo em funcao, usa bastante, na GenerateRoomRandomly ja usa isso
            foreach (Enemies value in enumValues)
            {
                if (roomMatrix[enemyPosition.Row, enemyPosition.Column].ToString() == value.ToString())
                {
                    enemies.Add(value);
                }
            }
        }
        enemies.Sort();
        List<Enemies> aux = new(sala.enemies);
        aux.Sort();
        if (!enemies.SequenceEqual(aux))
        {
            return false;
        }
        */

        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Enemies), enemiesPositions, sala.enemies.Cast<object>().ToList()))
        {
            Debug.Log("MONSTRO");
            return true;
        }

        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Obstacles), obstaclesPositions, sala.obstacles.Cast<object>().ToList()))
        {
            Debug.Log("MONSTRO2");
            return true;
        }

        /////////////////////////////////////////

        // hasTheRightObstacles
        /*
        var values = Enum.GetValues(typeof(Obstacles));
        List<Obstacles> obstacles = new();
        foreach (Position obstaclePosition in obstaclesPositions)
        {
            // pegar o inimigo equivalente no enum de Possibilidades // TODO: separar esse codigo em funcao, usa bastante, na GenerateRoomRandomly ja usa isso
            foreach (Obstacles value in enumValues)
            {
                if (roomMatrix[obstaclePosition.Row, obstaclePosition.Column].ToString() == value.ToString())
                {
                    obstacles.Add(value);
                }
            }
        }
        obstacles.Sort();
        List<Obstacles> aux1 = new(sala.obstacles);
        aux1.Sort();
        if (!obstacles.SequenceEqual(aux1))
        {
            return false;
        }
        */

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
