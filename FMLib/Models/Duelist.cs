using FMLib.Utility;

namespace FMLib.Models;

/// <summary>
/// 
/// </summary>
public class Duelist
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public Duelist(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 
    /// </summary>
    public int[] Deck { get; } = new int[Static.MaxCards];
    /// <summary>
    /// 
    /// </summary>
    public Rank Drop { get; } = new Rank();
}