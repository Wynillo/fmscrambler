using System;
using System.IO;
using FMLib.Logging;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Randomizer;

/// <summary>
/// 
/// </summary>
public class DataReader
{
    /// <summary>
    /// Loads Data From WA_MRG File
    /// </summary>
    public void LoadDataFromWaMrg()
    {
        // WA_MRG process
        var waMrgStream = new FileStream(Static.WaPath ?? throw new InvalidOperationException(), FileMode.Open);

        // Drop count
        Static.DropCount = waMrgStream.ExtractPiece(0, 1, 0xBC1C78)[0];

        ReadFusions(waMrgStream);

        ReadEquipment(waMrgStream);

        ReadCardCostsAndPassword(waMrgStream);

        ReadCardImages(waMrgStream);

        ReadDuelistsDecksAndCardDrops(waMrgStream);

        ReadStarterDeck(waMrgStream);

        waMrgStream.Close();
    }

    private static void ReadStarterDeck(FileStream waMrgStream)
    {
        // Starter deck
        for (var i = 0; i < Static.MaxStarterDeck; i++)
        {
            Static.StarterDeck[i] = new StarterDeck();

            waMrgStream.Position = 0xF92BD4 + 0x5B8 * i;

            using var memoryStream = new MemoryStream(waMrgStream.ExtractPiece(0, 0x5A6));
            Static.StarterDeck[i].Dropped = memoryStream.ExtractPiece(0, 2).ExtractUInt16();

            for (var j = 0; j < Static.MaxCards; j++)
                Static.StarterDeck[i].Cards[j] = memoryStream.ExtractPiece(0, 2).ExtractUInt16();
        }
    }

    private static void ReadDuelistsDecksAndCardDrops(FileStream waMrgStream)
    {
        // Read duelist decks and card drops
        for (var i = 0; i < Static.MaxDuelists; i++)
        {
            var memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i));

            for (var j = 0; j < Static.MaxCards; ++j)
                Static.Duelist[i].Deck[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();

            memStream.Close();

            memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0x5B4));

            for (var j = 0; j < Static.MaxCards; ++j)
                Static.Duelist[i].Drop.SaPow[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();

            memStream.Close();

            memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0xB68));

            for (var j = 0; j < Static.MaxCards; ++j)
                Static.Duelist[i].Drop.BcdPow[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();

            memStream.Close();

            memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0x111C));

            for (var j = 0; j < Static.MaxCards; ++j)
                Static.Duelist[i].Drop.SaTec[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();

            memStream.Close();
        }
    }

    private static void ReadCardImages(FileStream waMrgStream)
    {
        // Card image
        var memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 0x1A3F1C0, 0x169000));

        for (var i = 0; i < Static.MaxCards; i++)
        {
            memStream.Position = 0x3800 * i;

            Static.Cards[i].BigImage = new Card.TIM(memStream.ExtractPiece(0, 0x2840), 0x66, 0x60, 8);
            Static.Cards[i].NameImage = new Card.TIM(memStream.ExtractPiece(0, 0x2A0), 0x60, 14, 4, false, true);
            Static.Cards[i].SmallImage = new Card.TIM(memStream.ExtractPiece(0, 0x580), 40, 0x20, 8);
        }

        memStream.Close();
    }

    private static void ReadCardCostsAndPassword(FileStream waMrgStream)
    {
        // Card costs/passwords
        var memStream = new MemoryStream(waMrgStream.ExtractPiece(0, Static.MaxCards * 8, 0xFB9808));

        for (var i = 0; i < Static.MaxCards; ++i)
        {
            Static.Cards[i].Starchip = new Starchips();

            var costBytes = new byte[4];
            costBytes[0] = (byte)memStream.ReadByte();
            costBytes[1] = (byte)memStream.ReadByte();
            costBytes[2] = (byte)memStream.ReadByte();
            costBytes[3] = (byte)memStream.ReadByte();

            Static.Cards[i].Starchip.Cost = costBytes.ExtractInt32();

            var passBytes = new byte[4];
            passBytes[0] = (byte)memStream.ReadByte();
            passBytes[1] = (byte)memStream.ReadByte();
            passBytes[2] = (byte)memStream.ReadByte();
            passBytes[3] = (byte)memStream.ReadByte();

            var resPass = "";

            for (var j = 3; j >= 0; --j)
            {
                var str = passBytes[j].ToString("X");

                if (str.Length == 1) str = str.Insert(0, "0");

                resPass += str;
            }

            int.TryParse(resPass, out var outPass);

            Static.Cards[i].Starchip.Password = outPass;
            Static.Cards[i].Starchip.PasswordStr = resPass;
        }

        memStream.Close();
    }

    private static void ReadFusions(FileStream waMrgStream)
    {
        // Fusions
        var memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 0x10000, 0xB87800));

        // Iterate over all of the cards in the game and read their fusion data from the MemoryStream.
        // The fusion data for each card consists of a list of Fusion objects.
        // A Fusion object represents a possible fusion for the card.
        for (var i = 0; i < Static.MaxCards; ++i)
        {
            // Set the position of the MemoryStream to the offset of the card's fusion data in the file.
            memStream.Position = 2 + i * 2;

            // Read the first two bytes of the card's fusion data and extract the ushort value.
            // This ushort value is the number of fusions that the card has.
            memStream.Position = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;

            // If the number of fusions is zero, then skip to the next card.
            if (memStream.Position == 0L) continue;

            // Read the first byte of the card's fusion data.
            // This byte represents the number of cards that are required for the first fusion.
            var num1 = memStream.ReadByte();

            // If the first byte is zero, then the number of required cards is 511 - the value of the second byte.
            if (num1 == 0) num1 = 511 - memStream.ReadByte();

            // Create a new Fusion object for the first fusion.
            var num2 = num1;
            while (num2 > 0)
            {
                // Read the IDs of the required cards from the MemoryStream.
                var num3 = memStream.ReadByte();
                var num4 = memStream.ReadByte();
                var num5 = memStream.ReadByte();
                var num6 = memStream.ReadByte();
                var num7 = memStream.ReadByte();

                // Calculate the ID of the resulting card.
                var num9 = ((num3 & 3) << 8) | num4;
                var num11 = (((num3 >> 2) & 3) << 8) | num5;
                var num13 = (((num3 >> 4) & 3) << 8) | num6;
                var num15 = (((num3 >> 6) & 3) << 8) | num7;

                // Add the new Fusion object to the list of fusions for the card.
                Static.Cards[i].Fusions.Add(new Fusion(i + 1, num9, num11));
                
                // Decrement the number of remaining fusions.
                --num2;

                // If there are no more remaining fusions, then break from the loop.
                if (num2 <= 0) continue;

                // Add another new Fusion object to the list of fusions for the card.
                Static.Cards[i].Fusions.Add(new Fusion(i + 1, num13, num15));

                // Decrement the number of remaining fusions.
                --num2;
            }
            
            //Preperation for better Fusion Randomnes
            Static.AllFusions.AddRange(Static.Cards[i].Fusions);
        }

        var logger = new Logger(-1);
        logger.WriteNewFusionSpoilerFile(false);
        memStream.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waMrgStream"></param>
    private static void ReadEquipment(FileStream waMrgStream)
    {
        // Equips
        var memStream = new MemoryStream(waMrgStream.ExtractPiece(0, 0x2800, 0xB85000));

        while (true)
        {
            int num6 = memStream.ExtractPiece(0, 2).ExtractUInt16();

            if (num6 == 0) break;

            int num7 = memStream.ExtractPiece(0, 2).ExtractUInt16();

            for (var num8 = 0; num8 < num7; num8++)
            {
                int num9 = memStream.ExtractPiece(0, 2).ExtractUInt16();
                Static.Cards[num6 - 1].Equips.Add(num9 - 1);
            }
        }

        memStream.Close();
    }

    /// <summary>
    /// </summary>
    public void LoadDataFromSlus()
    {
        var memStream = new MemoryStream(File.ReadAllBytes(Static.SlusPath ?? throw new InvalidOperationException()))
        {
            // General card data
            Position = 0x1C4A44L
        };

        for (var i = 0; i < Static.MaxCards; i++)
        {
            var int32 = memStream.ExtractPiece(0, 4).ExtractInt32();

            Static.Cards[i] = new Card
            {
                Id = i + 1,
                Attack = (int32 & 0x1FF) * 10, // Unwrap attack value
                Defense = ((int32 >> 9) & 0x1FF) * 10, // Unwrap defense value
                GuardianStar2 = (int32 >> 18) & 0xF, // Unwrap 1st guardian star
                GuardianStar1 = (int32 >> 22) & 0xF, // Unwrap 2nd guardian star
                Type = (int32 >> 26) & 0x1F // Unwrap card's type
            };
        }

        // Cards level and attribute
        ReadCardLevelAndAttributes(memStream);

        // Card name
        ReadCardNames(memStream);

        // Card description
        ReadCardDescription(memStream);

        // Duelist names
        ReadDuelistNames(memStream);

        // Star chips per Duel
        SetStarChipsPerDuel(memStream);

        memStream.Close();
    }
    
    
    private static void ReadCardLevelAndAttributes(MemoryStream memStream)
    {
        memStream.Position = 0x1C5B33L;

        for (var i = 0; i < Static.MaxCards; i++)
        {
            var num = memStream.ExtractPiece(0, 1)[0];

            Static.Cards[i].Level = num & 0xF; // Unwrap card level
            Static.Cards[i].Attribute = (num >> 4) & 0xF; // Unwrap card's attribute
        }
    }

    private static void ReadDuelistNames(MemoryStream memStream)
    {
        for (var i = 0; i < Static.MaxDuelists; i++)
        {
            memStream.Position = 0x1C6652 + i * 2;
            memStream.Position = 0x1C6800 + memStream.ExtractPiece(0, 2).ExtractUInt16() - 0x6000;

            Static.Duelist[i] = new Duelist(memStream.GetText(Static.Dict));
        }
    }

    private static void ReadCardDescription(MemoryStream memStream)
    {
        for (var i = 0; i < Static.MaxCards; i++)
        {
            memStream.Position = 0x1B0A02 + i * 2;

            int num3 = memStream.ExtractPiece(0, 2).ExtractUInt16();

            memStream.Position = 0x1B11F4 + (num3 - 0x9F4);
            Static.Cards[i].Description = memStream.GetText(Static.Dict);
        }
    }

    private static void ReadCardNames(MemoryStream memStream)
    {
        for (var i = 0; i < Static.MaxCards; i++)
        {
            memStream.Position = 0x1C6002 + i * 2;

            var num = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;

            memStream.Position = 0x1C6800 + num - 0x6000;
            Static.Cards[i].Name = memStream.GetText(Static.Dict);
        }
    }
    
    private static void SetStarChipsPerDuel(MemoryStream memStream)
    {
        memStream.Position = 0x126DCL;
        Static.StarChipsDuel = memStream.ExtractPiece(0, 2).ExtractUInt16();
        Static.StarChipsDuel += 5;
    }
}