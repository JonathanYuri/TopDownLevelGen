using RoomGeneticAlgorithm.Constants;
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public class ChildContentPlacement
    {
        List<Position> avaliablePositions = new();

        internal void SetAvaliablePositions(HashSet<Position> changeablesPositions)
        {
            avaliablePositions = new(changeablesPositions);
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
            foreach (RoomContents key in contentsPositionsInParent1.Keys)
            {
                int contentsCount = contentsPositionsInParent1[key].Count;
                HashSet<Position> allContents = new(contentsPositionsInParent1[key]);
                allContents.UnionWith(contentsPositionsInParent2[key]);

                HashSet<Position> combinedPositions = allContents.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
                Position[] chosenPositions = combinedPositions.GetRandomElements(contentsCount);

                int totalToPlaceRandomly = contentsCount - chosenPositions.Length;
                PlaceContentsInChosenPositions(child, chosenPositions, key, totalToPlaceRandomly);
            }
        }

        void PlaceContentsInChosenPositions(RoomIndividual child, Position[] chosenPositions, RoomContents content, int totalToPlaceRandomly)
        {
            PlaceContentInPositions(child, chosenPositions, content);

            if (totalToPlaceRandomly == 0)
            {
                return;
            }

            PlaceContentInPositions(child, avaliablePositions.GetRandomElements(totalToPlaceRandomly), content);
        }

        void PlaceContentInPositions(RoomIndividual child, Position[] positions, RoomContents content)
        {
            foreach (Position position in positions)
            {
                child.RoomMatrix.PutContentInPosition(content, position);
                avaliablePositions.Remove(position);
            }
        }
    }
}