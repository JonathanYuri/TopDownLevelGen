using System;
using System.Collections.Generic;
using System.Linq;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position Move(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Position { Row = Row - 1, Column = Column },
            Direction.Down => new Position { Row = Row + 1, Column = Column },
            Direction.Left => new Position { Row = Row, Column = Column - 1 },
            Direction.Right => new Position { Row = Row, Column = Column + 1 },
            Direction _ => throw new ArgumentException("Direção desconhecida: " + direction)
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Position other = (Position)obj;
        return Row == other.Row && Column == other.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}

public static class Utils
{
    public static HashSet<string> GetEnumValueStrings(Type enumType)
    {
        return new HashSet<string>(Enum.GetValues(enumType).Cast<Enum>().Select(value => value.ToString()));
    }

    public static List<int> ResolveKnapsack(List<int> values, int capacity)
    {
        int n = values.Count;
        int[] dp = new int[capacity + 1];
        List<int>[] chosenItems = new List<int>[capacity + 1];
        for (int i = 0; i <= capacity; i++)
        {
            chosenItems[i] = new List<int>();
        }

        for (int w = 1; w <= capacity; w++)
        {
            int maxValue = dp[w];
            List<int> chosenItem = new();

            for (int i = 0; i < n; i++)
            {
                if (values[i] <= w)
                {
                    int newValue = dp[w - values[i]] + values[i];
                    // se o valor for melhor ou se o valor for igual e tiver menos itens, eu coloco
                    if (newValue > maxValue || (newValue == maxValue && chosenItems[w - values[i]].Count + 1 < chosenItem.Count))
                    {
                        maxValue = newValue;
                        chosenItem = new List<int>(chosenItems[w - values[i]]) { i };
                    }
                }
            }

            dp[w] = maxValue;
            chosenItems[w] = chosenItem;
        }

        return chosenItems[capacity];
    }

    public static Possibilidades TransformAElementFromEnumToPossibilidadesEnum<TEnum>(Type enumType, TEnum element)
    {
        var enumValues = Enum.GetValues(enumType);
        foreach (Possibilidades value in enumValues)
        {
            if (element.ToString().Equals(value.ToString()))
            {
                return value;
            }
        }

        throw new Exception("Enum element do not transform to Possibilidades Enum");
    }
}
