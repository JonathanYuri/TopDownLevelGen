
using System.Collections.Generic;
using System.Linq;

namespace RoomGeneticAlgorithm.GeneticOperations
{
    public static class ChildContentPlacement
    {
        internal static List<Position> avaliablePositions = new();

        /// <summary>
        /// Places room contents in the child individual by combining the contents from the father and mother individuals.
        /// </summary>
        /// <param name="child">The child individual to place the contents in.</param>
        /// <param name="contentsPositionsInParent1">A dictionary of room contents positions in the father individual.</param>
        /// <param name="contentsPositionsInParent2">A dictionary of room contents positions in the mother individual.</param>
        internal static void PlaceContentsInChild(
            RoomIndividual child,
            Dictionary<RoomContents, List<Position>> contentsPositionsInParent1,
            Dictionary<RoomContents, List<Position>> contentsPositionsInParent2)
        {
            foreach (RoomContents key in contentsPositionsInParent1.Keys)
            {
                HashSet<Position> combinedPositions = Utils.CombinePositions(contentsPositionsInParent1[key], contentsPositionsInParent2[key]);
                combinedPositions = combinedPositions.Intersect(avaliablePositions).ToHashSet(); // pra sempre ser uma posicao valida
                Position[] chosenPositions = combinedPositions.SelectRandomDistinctElements(contentsPositionsInParent1[key].Count);

                int totalToPlaceRandomly = contentsPositionsInParent1[key].Count - chosenPositions.Length;
                PlaceContentsInChosenPositions(child, chosenPositions, key, totalToPlaceRandomly);
            }
        }

        static void PlaceContentsInChosenPositions(RoomIndividual child, Position[] chosenPositions, RoomContents content, int totalToPlaceRandomly)
        {
            PlaceContentInPositions(child, chosenPositions, content);

            if (totalToPlaceRandomly == 0)
            {
                return;
            }

            PlaceContentInPositions(child, avaliablePositions.SelectRandomDistinctElements(totalToPlaceRandomly), content);
        }

        static void PlaceContentInPositions(RoomIndividual child, Position[] positions, RoomContents content)
        {
            foreach (Position position in positions)
            {
                child.RoomMatrix.PutContentInPosition(content, position);
                avaliablePositions.Remove(position);
            }
        }
    }
}