using System;
using System.Linq;
using FMLib.Models;

namespace FMLib.Utility;

/// <summary>
/// 
/// </summary>
public abstract class RandomHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static Random Random = null!;
    
    /// <summary>
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static int GetRandomNumber(int min, int max)
    {
        lock (Random)
        {
            return Random.Next(min, max);
        }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static int GetRandomNext()
    {
        lock (Random)
        {
            return Random.Next();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    private static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 > 0) return new byte[] { };

        var buffer = new byte[hexString.Length / 2];

        for (var i = 0; i < hexString.Length; i += 2) buffer[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 0x10);

        return buffer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static int GetRandomMonsterId(int id)
    {
        return GetRandomMonsterId(new[] { id });
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static int GetRandomMonsterId()
    {
        return GetRandomMonsterId(new int[] { });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static int GetRandomMonsterId(int[] notInHere)
    {
        var checkMonster = false;
        var cardId = -1;
        while (!checkMonster)
        {
            cardId = GetRandomNumber(1, Static.CardCount+1);
            var card = Static.Cards.Single(card => card.Id == cardId);
            checkMonster = IsMonsterCard(card.Type) && !notInHere.Contains(cardId); //prevent doubles
        }

        return cardId;
    }
    
    /// <summary>
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="hexString"></param>
    public static byte[] PutHex(byte[] bytes, int offset, string hexString)
    {
        var sourceArray = ConvertHexStringToByteArray(hexString);
        Array.Copy(sourceArray, 0, bytes, offset, sourceArray.Length);
        return bytes;
    }

    /// <summary>
    /// </summary>
    /// <param name="cardType"></param>
    public static bool IsMonsterCard(int cardType)
    {
        return cardType >= (int)Static.Type.Dragon && cardType <= (int)Static.Type.Plant;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Fusion CreateUniqueFusion()
    {
        var monsterId1 = RandomHelper.GetRandomMonsterId();
        var monsterId2 = RandomHelper.GetRandomMonsterId(monsterId1);
        var uniqueAndValid = CheckIfUniqueAndValid(monsterId1, monsterId2);
        
        while (!uniqueAndValid)
        {
            monsterId1 = RandomHelper.GetRandomMonsterId();
            monsterId2 = RandomHelper.GetRandomMonsterId(monsterId1);
            uniqueAndValid = CheckIfUniqueAndValid(monsterId1,monsterId2);
        }
        var result = RandomHelper.GetRandomMonsterId(new[] {monsterId1, monsterId2});
        return new Fusion(monsterId1, monsterId2, result);
    }

    private static bool CheckIfUniqueAndValid(int monsterId1, int monsterId2)
    {
        return (monsterId1 < monsterId2) && !Static.AllFusions.Exists(
            f => f.Cards1 == monsterId1 && f.Cards2 == monsterId2
                 || f.Cards1 == monsterId2 && f.Cards2 == monsterId1);
         
    }
}