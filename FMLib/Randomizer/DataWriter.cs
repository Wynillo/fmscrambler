using System;
using System.IO;
using System.Linq;
using FMLib.Utility;

namespace FMLib.Randomizer;

/// <summary>
/// 
/// </summary>
public class DataWriter
{
    const int ChunkSize = 2352;
    const int DataSize = 2048;

    /// <summary>
    /// 
    /// </summary>
    public void WriteChangesToFile()
    {
        using var fileStream = new FileStream(Static.WaPath!, FileMode.Open);
        // Writing Random Fusions
        WriteFusionChanges(fileStream);

        // Write Card Images
        if (Static.RandomCardImages
            || Static.RandomNames)
            for (var i = 0; i < Static.MaxCards; i++)
            {
                fileStream.Position = 0x169000 + 0x3800 * i;
                fileStream.Write(Static.Cards[i].BigImage.getData(), 0, Static.Cards[i].BigImage.getData().Length);

                fileStream.Position = i * 0x800;
                fileStream.Write(Static.Cards[i].SmallImage.getData(), 0,
                    Static.Cards[i].SmallImage.getData().Length);
                fileStream.Position = 0x16BAE0 + 0x3800 * i;
                fileStream.Write(Static.Cards[i].SmallImage.getData(), 0,
                    Static.Cards[i].SmallImage.getData().Length);

                fileStream.Position = 0x16B840 + 0x3800 * i;
                fileStream.Write(Static.Cards[i].NameImage.getData(), 0,
                    Static.Cards[i].NameImage.getData().Length);
            }

        // Randomize ATK/DEF, Guardian Stars, Types, Attributes, Names and Descriptions
        if (Static.RandomAtkdef
            || Static.RandomGuardianStars
            || Static.RandomMonstersTypes
            || Static.RandomAttributes
            || Static.RandomNames
            || Static.RandomDescriptions)
        {
            using var fileStreamSl =
                new FileStream(Static.SlusPath ?? throw new InvalidOperationException(), FileMode.Open);
            fileStreamSl.Position = 0x1C4A44L;

            // Write ATK/DEF, Guardian Stars and Types
            using (var memoryStream = new MemoryStream(2888))
            {
                for (var i = 0; i < Static.MaxCards; ++i)
                {
                    var value = ((Static.Cards[i].Attack / 10) & 0x1FF) |
                                (((Static.Cards[i].Defense / 10) & 0x1FF) << 9) |
                                ((Static.Cards[i].GuardianStar2 & 0xF) << 18) |
                                ((Static.Cards[i].GuardianStar1 & 0xF) << 22) |
                                ((Static.Cards[i].Type & 0x1F) << 26);

                    memoryStream.Write(value.Int32ToByteArray(), 0, 4);
                }

                var arr = memoryStream.ToArray();
                fileStreamSl.Write(arr, 0, arr.Length);
            }

            // Write Cards level and attribute
            fileStreamSl.Position = 0x1C5B33L;

            using (var memoryStream = new MemoryStream(Static.MaxCards))
            {
                for (var i = 0; i < Static.MaxCards; i++)
                {
                    var buffer = new[]
                    {
                        (byte)((Static.Cards[i].Level & 0xF) | ((Static.Cards[i].Attribute & 0xF) << 4))
                    };

                    memoryStream.Write(buffer, 0, 1);
                }

                var arr = memoryStream.ToArray();
                fileStreamSl.Write(arr, 0, arr.Length);
            }

            // Card description
            using (var memoryStreamColor = new MemoryStream())
            {
                using (var memoryStreamText = new MemoryStream())
                {
                    for (var i = 0; i < Static.MaxCards; i++)
                    {
                        var bufferColor = new byte[]
                        {
                            0xF8,
                            10,
                            (byte)Static.Cards[i].DescriptionColor
                        };

                        memoryStreamColor.Write(((short)(memoryStreamText.Position + 0x9f4L)).Int16ToByteArray(), 0,
                            2);

                        if (Static.Cards[i].DescriptionColor != 0) memoryStreamText.Write(bufferColor, 0, 3);

                        var bufferText = Static.Cards[i].Description.TextToArray(Static.RDict);
                        memoryStreamText.Write(bufferText, 0, bufferText.Length);
                    }

                    fileStreamSl.Position = 0x1B0A02L;
                    fileStreamSl.Write(memoryStreamColor.ToArray(), 0, (int)memoryStreamColor.Length);

                    fileStreamSl.Position = 0x1B11F4L;
                    fileStreamSl.Write(memoryStreamText.ToArray(), 0, (int)memoryStreamText.Length);
                }
            }

            // Card name
            using (var memoryStream = new MemoryStream())
            {
                var lastIndexStop = 0;
                var overBufferSize = false;

                using (var memoryStreamText = new MemoryStream(0x2BFD))
                {
                    for (var i = 0; i < Static.MaxCards; i++)
                    {
                        var bufferColor = new byte[]
                        {
                            0xF8,
                            10,
                            (byte)Static.Cards[i].NameColor
                        };

                        var bufferText = Static.Cards[i].Name.TextToArray(Static.RDict);

                        if (memoryStreamText.Position + bufferText.Length > memoryStreamText.Capacity)
                        {
                            overBufferSize = true;
                            lastIndexStop = i;

                            while (memoryStreamText.Position < memoryStreamText.Capacity)
                                memoryStreamText.WriteByte(0);

                            break;
                        }

                        memoryStream.Write(
                            ((short)(memoryStreamText.Position + 0x6000L - 0x130L)).Int16ToByteArray(), 0, 2);

                        if (Static.Cards[i].NameColor != 0) memoryStreamText.Write(bufferColor, 0, 3);

                        memoryStreamText.Write(bufferText, 0, bufferText.Length);
                    }

                    fileStreamSl.Position = 0x1C66D0L;
                    fileStreamSl.Write(memoryStreamText.ToArray(), 0, (int)memoryStreamText.Length);
                }

                if (overBufferSize)
                {
                    using var memoryStreamText = new MemoryStream();
                    for (var i = lastIndexStop; i < Static.MaxCards; i++)
                    {
                        var bufferColor = new byte[]
                        {
                            0xF8,
                            10,
                            (byte)Static.Cards[i].NameColor
                        };

                        memoryStream.Write(
                            ((short)(memoryStreamText.Position + 0x6000L + 0x3200L)).Int16ToByteArray(), 0, 2);

                        var bufferText = Static.Cards[i].Name.TextToArray(Static.RDict);

                        if (Static.Cards[i].NameColor != 0) memoryStreamText.Write(bufferColor, 0, 3);

                        memoryStreamText.Write(bufferText, 0, bufferText.Length);
                    }

                    fileStreamSl.Position = 0x1C9A00L;
                    fileStreamSl.Write(memoryStreamText.ToArray(), 0, (int)memoryStreamText.Length);
                }

                fileStreamSl.Position = 0x1C6002L;
                fileStreamSl.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            }

            // Write Star Chips per Duel
            if (Static.StarChipsDuel >= Static.MinStarChipsDuel)
            {
                fileStreamSl.Position = 0x126DCL;
                var changeStarChip = Static.StarChipsDuel > Static.MinStarChipsDuel;

                Static.StarChipsDuel -= Static.MinStarChipsDuel;

                var tempArray = Static.StarChipsDuel.Int16ToByteArray();
                var starChipArray = new[]
                {
                    tempArray[0], tempArray[1], (byte)(changeStarChip ? 0x63 : 0x00),
                    (byte)(changeStarChip ? 0x24 : 0x00)
                };

                fileStreamSl.Write(starChipArray, 0, starChipArray.Length);
            }
        }

        // Randomize Starter Decks
        if (Static.RandomStarterDecks)
        {
            using var starterDeckStream =
                new FileStream(Static.WaPath ?? throw new InvalidOperationException(), FileMode.Open);
            for (var i = 0; i < Static.MaxStarterDeck; i++)
            {
                starterDeckStream.Position = 0xF92BD4 + 0x5B8 * i;

                using var memoryStream = new MemoryStream(0x5A6);
                var droppedUInt16 = (ushort)Static.StarterDeck[i].Dropped;

                memoryStream.Write(droppedUInt16.Int16ToByteArray(), 0, 2);

                foreach (var cardIdUInt16 in Static.StarterDeck[i].Cards.Select(s => (ushort)s))
                    memoryStream.Write(cardIdUInt16.Int16ToByteArray(), 0, 2);

                starterDeckStream.Write(memoryStream.ToArray(), 0, 0x5A6);
            }
        }

        // Randomize Decks and Card Drops
        if (Static.RandomDecks || Static.RandomCardDrops)
        {
            using var duelistStream =
                new FileStream(Static.WaPath ?? throw new InvalidOperationException(), FileMode.Open);
            for (var i = 0; i < Static.MaxDuelists; i++)
            {
                var num = 0xE9B000 + 0x1800 * i;

                // Randomize Decks
                if (Static.RandomDecks)
                {
                    duelistStream.Position = num;

                    using var memoryStream = new MemoryStream(1444);
                    var array = Static.Duelist[i].Deck;

                    foreach (var t in array)
                    {
                        var value = (short)t;
                        memoryStream.Write(value.Int16ToByteArray(), 0, 2);
                    }

                    var arr = memoryStream.ToArray();
                    duelistStream.Write(arr, 0, arr.Length);
                }

                // Randomize Card Drops
                if (Static.RandomCardDrops)
                {
                    duelistStream.Position = num + 0x5B4;

                    using (var memoryStream2 = new MemoryStream(1444))
                    {
                        var array = Static.Duelist[i].Drop.SaPow;

                        foreach (var t in array)
                        {
                            var value2 = (short)t;
                            memoryStream2.Write(value2.Int16ToByteArray(), 0, 2);
                        }

                        var arr = memoryStream2.ToArray();
                        duelistStream.Write(arr, 0, arr.Length);
                    }

                    duelistStream.Position = num + 0xB68;

                    using (var memoryStream3 = new MemoryStream(1444))
                    {
                        var array = Static.Duelist[i].Drop.BcdPow;

                        foreach (var t in array)
                        {
                            var value3 = (short)t;
                            memoryStream3.Write(value3.Int16ToByteArray(), 0, 2);
                        }

                        var arr = memoryStream3.ToArray();
                        duelistStream.Write(arr, 0, arr.Length);
                    }

                    duelistStream.Position = num + 0x111C;

                    using (var memoryStream4 = new MemoryStream(1444))
                    {
                        var array = Static.Duelist[i].Drop.SaTec;

                        foreach (var t in array)
                        {
                            var value4 = (short)t;
                            memoryStream4.Write(value4.Int16ToByteArray(), 0, 2);
                        }

                        var arr = memoryStream4.ToArray();
                        duelistStream.Write(arr, 0, arr.Length);
                    }
                }
            }
        }

        if (Static.RandomStarchips)
        {
            using var starchipStream =
                new FileStream(Static.WaPath ?? throw new InvalidOperationException(), FileMode.Open);
            starchipStream.Position = 0xFB9808;

            for (var i = 0; i < Static.MaxCards; ++i)
            {
                var costArr = Static.Cards[i].Starchip.Cost.Int32ToByteArray();
                var passArr = Static.Cards[i].Starchip.PasswordStr.StringToByteArray();
                var offset = 0;

                for (var j = costArr.Length - 2; j >= 0; --j)
                    if (costArr[j] == 0)
                        offset++;
                    else
                        break;

                for (var j = 0; j < costArr.Length - offset - 1; ++j) starchipStream.WriteByte(costArr[j]);

                for (var j = 0; j < offset; ++j) starchipStream.WriteByte(0);

                // Advance over unused byte
                starchipStream.Position += 1;

                for (var j = passArr.Length - 1; j >= 0; --j) starchipStream.WriteByte(passArr[j]);
            }
        }
    }

    private static void WriteFusionChanges(Stream fileStream)
    {
        if (!Static.RandomFusions) return;
        int[] fusionsTable =
        {
            12089344,
            12570624,
            13051904,
            13533184,
            14014464,
            14495744,
            14977024
        };

        using var memStream1 = new MemoryStream(1444);
        using var memStream2 = new MemoryStream(64092);

        memStream1.Position = 2L;
        memStream2.Position = 2L;

        foreach (var card in Static.Cards)
        {
            var count = Static.AllFusions.Count(f => f.Cards1 == card.Id);
            var num1 = count != 0 ? (short)(memStream2.Position + 1444L) : (short)0;
            memStream1.WriteByte((byte)(num1 & 0xFF));
            memStream1.WriteByte((byte)(num1 >> 8 & 0xFF));
            
            var fusionsForCurrentCard = Static.AllFusions.Where(f => f.Cards1 == card.Id).ToArray();
            switch (count)
            {
                case 0:
                    continue;
                case < 256:
                    memStream2.WriteByte((byte)count);
                    break;
                default:
                    memStream2.WriteByte(0);
                    memStream2.WriteByte((byte)Math.Abs(count - 511));
                    break;
            }

            for (var i = 0; i < fusionsForCurrentCard.Length; i++)
            {
                var monsterId2 = fusionsForCurrentCard[i].Cards2;
                var resultId = fusionsForCurrentCard[i].Result;

                var num2 = monsterId2 + 1 & byte.MaxValue;
                var num3 = resultId + 1 & byte.MaxValue;
                var num4 = 0;
                var num5 = 0;
                var num6 = monsterId2 + 1 >> 8 & 3 | (resultId + 1 >> 8 & 3) << 2;
                if (i < fusionsForCurrentCard.Length - 1)
                {
                    monsterId2 = fusionsForCurrentCard[i + 1].Cards2;
                    resultId = fusionsForCurrentCard[i + 1].Result;

                    num4 = (monsterId2 + 1) & byte.MaxValue;
                    num5 = (resultId + 1) & byte.MaxValue;
                    num6 |= (monsterId2 + 1 >> 8 & 3) << 4 |
                            (resultId + 1 >> 8 & 3) << 6;
                    ++i;
                }

                memStream2.WriteByte((byte)(num6 & byte.MaxValue));
                memStream2.WriteByte((byte)(num2 & byte.MaxValue));
                memStream2.WriteByte((byte)(num3 & byte.MaxValue));

                if (num4 == 0 && num5 == 0) continue;
                memStream2.WriteByte((byte)(num4 & byte.MaxValue));
                memStream2.WriteByte((byte)(num5 & byte.MaxValue));
            }
        }

        while (memStream2.Position < 64092L) memStream2.WriteByte(238);

        foreach (var num in fusionsTable)
        {
            fileStream.Position = num;
            fileStream.Write(memStream1.ToArray(), 0, 1444);
            fileStream.Write(memStream2.ToArray(), 0, 64092);
        }
    }
}