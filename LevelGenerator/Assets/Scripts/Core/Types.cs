using System;
/// <summary>
/// Enumeration that defines possible room contents.
/// </summary>
public enum RoomContents
{
    [Mutable(true)][Traversable(true)] Ground,

    [Mutable(true)][Traversable(false)] Obstacle1,
    [Mutable(true)][Traversable(false)] Obstacle2,
    [Mutable(true)][Traversable(false)] Obstacle3,

    [Mutable(true)][Traversable(true)] Enemy1,
    [Mutable(true)][Traversable(true)] Enemy2,
    [Mutable(true)][Traversable(true)] Enemy3,

    [Mutable(false)][Traversable(true)] Nothing,
    [Mutable(false)][Traversable(true)] Door,
    [Mutable(false)][Traversable(false)] Wall,
    [Mutable(false)][Traversable(true)] LevelEnd,
}

/// <summary>
/// Attribute that defines whether content is mutable.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MutableAttribute : Attribute
{
    public bool IsMutable { get; }
    public MutableAttribute(bool isMutable) => IsMutable = isMutable;
}

/// <summary>
/// Attribute that defines whether content is traversable.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class TraversableAttribute : Attribute
{
    public bool IsTraversable { get; }
    public TraversableAttribute(bool isTraversable) => IsTraversable = isTraversable;
}