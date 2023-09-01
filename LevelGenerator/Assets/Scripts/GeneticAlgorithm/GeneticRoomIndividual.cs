using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GeneticRoomIndividual
{
    public Possibilidades[,] roomMatrix;
    public int? value;

    public GeneticRoomIndividual(Sala sala, bool generateRandomly = true)
    {
        value = null;
        roomMatrix = (Possibilidades[,])sala.matriz.Clone();

        if (generateRandomly)
        {
            GenerateRoomRandomly(sala);
        }
    }

    public GeneticRoomIndividual(GeneticRoomIndividual individual)
    {
        value = individual.value;
        roomMatrix = individual.roomMatrix;
    }

    void GenerateRoomRandomly(Sala sala)
    {
        List<Position> aux = new(sala.changeablesPositions);
        foreach (Enemies enemie in sala.enemies)
        {
            Possibilidades inimigo = Utils.TransformAElementFromEnumToPossibilidadesEnum(typeof(Possibilidades), enemie);

            int rand = Random.Range(0, aux.Count);
            roomMatrix[aux[rand].Row, aux[rand].Column] = inimigo;
            aux.RemoveAt(rand);
        }

        foreach (Obstacles obstacle in sala.obstacles)
        {
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
        int qntCaminhosEntrePortas = PathFinder.CountPathsBetweenDoors(roomMatrix, sala.doorsPositions);
        if (qntCaminhosEntrePortas == int.MinValue)
        {
            //Debug.Log("Mostro por causa do caminho entre portas");
            return true;
        }

        bool isPath = PathFinder.IsAPathBetweenDoorAndEnemies(roomMatrix, sala.doorsPositions);
        if (!isPath)
        {
            //Debug.Log("Mostro por causa do caminho ate inimigos");
            return true;
        }

        List<Position> enemiesPositions = RoomOperations.GetPositionsThatHas(roomMatrix, typeof(Enemies));
        List<Position> obstaclesPositions = RoomOperations.GetPositionsThatHas(roomMatrix, typeof(Obstacles));

        bool hasTheRightAmountOfEnemies = enemiesPositions.Count == sala.enemies.Count;
        if (!hasTheRightAmountOfEnemies)
        {
            //Debug.Log("Mostro por causa da quantidade de inimigos");
            return true;
        }

        bool hasTheRightAmountOfObstacles = obstaclesPositions.Count == sala.obstacles.Count;
        if (!hasTheRightAmountOfObstacles)
        {
            //Debug.Log("Mostro por causa da quantidade de obstaculos");
            return true;
        }

        /*
        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Enemies), enemiesPositions, sala.enemies.Cast<object>().ToList()))
        {
            //Debug.Log("Mostro por causa dos inimigos");
            return true;
        }

        if (!RoomOperations.HasTheRightObjects(roomMatrix, typeof(Obstacles), obstaclesPositions, sala.obstacles.Cast<object>().ToList()))
        {
            //Debug.Log("Mostro por causa dos obstaculo");
            return true;
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

        List<int> groups = GroupCounter.CountGroups(roomMatrix, typeof(Enemies));
        double media = groups.Average();

        //Debug.Log("Total de grupos de Enemies na matriz: " + groups.Count);
        //Debug.Log("Média do tamanho dos grupos: " + media);

        //int qntInimigosProximosDeObstaculos = Utils.CountEnemiesNextToObstacles(roomMatrix);
        value = - groups.Count - (int)media; // + qntInimigosProximosDeObstaculos;
    }
}
