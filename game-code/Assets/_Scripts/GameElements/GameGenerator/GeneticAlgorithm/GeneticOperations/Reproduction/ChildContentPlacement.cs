using System.Collections.Generic;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public class ChildContentPlacement
    {
        HashSet<Position> availablePositions = new();

        internal void SetAvaliablePositions(HashSet<Position> changeablesPositions)
        {
            availablePositions = new(changeablesPositions);
        }

        /// <summary>
        /// Places room contents in the child individual by combining the contents from the father and mother individuals.
        /// </summary>
        /// <param name="child">The child individual to place the contents in.</param>
        /// <param name="contentsPositionsInParent1">A dictionary of room contents positions in the father individual.</param>
        /// <param name="contentsPositionsInParent2">A dictionary of room contents positions in the mother individual.</param>
        internal void PlaceContentsInChild(
            RoomIndividual child,
            Dictionary<RoomContents, HashSet<Position>> contentsPositionsInParent1,
            Dictionary<RoomContents, HashSet<Position>> contentsPositionsInParent2)
        {
            foreach (var kvp in contentsPositionsInParent1)
            {
                RoomContents key = kvp.Key;
                int contentsCount = kvp.Value.Count;

                Position[] chosenPositions = GetChosenPositions(kvp.Value, contentsPositionsInParent2[key], contentsCount);
                int totalToPlaceRandomly = contentsCount - chosenPositions.Length;
                PlaceContentsInChosenPositions(child, chosenPositions, key, totalToPlaceRandomly);
            }
        }

        Position[] GetChosenPositions(HashSet<Position> positions1, HashSet<Position> positions2, int qntChoose)
        {
            HashSet<Position> chosenPositions = new();
            GetValidPositions(chosenPositions, positions1);
            GetValidPositions(chosenPositions, positions2);
            return chosenPositions.GetRandomElements(qntChoose);
        }

        void GetValidPositions(HashSet<Position> chosenPositions,
            HashSet<Position> positions)
        {
            foreach (Position pos in positions)
            {
                if (availablePositions.Contains(pos) && !chosenPositions.Contains(pos))
                {
                    chosenPositions.Add(pos);
                }
            }
        }

        void PlaceContentsInChosenPositions(RoomIndividual child, Position[] chosenPositions, RoomContents content, int totalToPlaceRandomly)
        {
            PlaceContentInPositions(child, chosenPositions, content);

            if (totalToPlaceRandomly == 0)
            {
                return;
            }

            PlaceContentInPositions(child, availablePositions.GetRandomElements(totalToPlaceRandomly), content);
        }

        void PlaceContentInPositions(RoomIndividual child, Position[] positions, RoomContents content)
        {
            foreach (Position position in positions)
            {
                child.RoomMatrix.PutContentInPosition(content, position);
                availablePositions.Remove(position);
            }
        }
    }
}