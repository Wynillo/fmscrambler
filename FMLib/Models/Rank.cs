using System;
using FMLib.Utility;

namespace FMLib.Models;

/// <summary>
/// 
/// </summary>
public enum DropType
{
    /// <summary>
    /// 
    /// </summary>
    Sapow,
    /// <summary>
    /// 
    /// </summary>
    Satec,
    /// <summary>
    /// 
    /// </summary>
    Bcdpow
}
/// <summary>
/// 
/// </summary>
public class Rank
{
    /// <summary>
    /// 
    /// </summary>
    public int[] SaPow { get; } = new int[Static.MaxCards];
    /// <summary>
    /// 
    /// </summary>
    public int[] SaTec { get; } = new int[Static.MaxCards];
    /// <summary>
    /// 
    /// </summary>
    public int[] BcdPow { get; } = new int[Static.MaxCards];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public int[] GetDropArray(DropType type)
    {
        return type switch
        {
            DropType.Sapow => SaPow,
            DropType.Satec => SaTec,
            DropType.Bcdpow => BcdPow,
            _ => throw new ApplicationException("Attempting unexisting drop type")
        };
    }
}