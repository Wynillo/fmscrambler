using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FMLib.Logging;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Randomizer;

/// <summary>
/// </summary>
public class DataRandomizer
{
    private readonly Logger _logger;
    
    /// <summary>
    /// </summary>
    /// <param name="seed"></param>
    public DataRandomizer(int seed)
    {
        _logger = new Logger(seed);
        Static.Seed = seed;
        RandomHelper.Random = new Random(seed);
    }
    
    /// <summary>
    /// </summary>
    private static void EnableDropCount(int dropCount)
    {
        if (dropCount > 15) dropCount = 15;

        switch (dropCount)
        {
            case <= 1 when Static.DropCount != 0:
            {
                // Remove drop
                var slusBytesArray = File.ReadAllBytes(Static.SlusPath ?? throw new InvalidOperationException());

                var hexString = "1880023C8C8742241400BFAF6439020C21800202";
                var offset = 0x12034;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                hexString = "ACA906080000000038B322A43C0082A400140200";
                offset = 0x1246C;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                hexString = "1D80033C0A80013C";
                offset = 0x285FC;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                offset = 0x19B400;
                hexString =
                    "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                offset = 0x12710;
                hexString = "3C0044842586000C00000000";
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                File.WriteAllBytes(Static.SlusPath, slusBytesArray);

                var waBytesArray = File.ReadAllBytes(Static.WaPath ?? throw new InvalidOperationException());
                hexString =
                    "0C0007140193143F0200003F0000013F0000023F0000031F000102030C0193143F0000003F0200012300010C3F000000040C003914390193143A110A003F1700010400015514110A003F15000104000189140198143F0000003F000001260A00010C3F000004050C0493143F0200003F0000013F0000023F0000031F000102030C050C049314018914390A06000A000C0193142F0C0198140DFF42281E0040142110D50247DF020C00000000801D0496000000001A00440002008014000000000D000700FFFF0124040081140080013C02004114000000000D00060010180000000000000A006014FFFF822401000224901902A61401AB8F2401AC8F21280000FA3E050C21206C01461D05082110D502801D02A62110D5029019429400000000980040101800A4273800A5275800A7270C01AD8F6000A2271000A2AF1400A0AF0400A6252130C602F347050C2130D3001680033CE0B7632410006294000000001600E2A612006294000000000E00E2A62401AE8F0401AB8F000000002110CB012110C202B21E4390000000000400E3A2B31E4390000000000500E3A2B41E42902120D5020600E2A2C8198294400003240D00E3A24011020080FF42240C00E2A2C81982941500E3A2401102009FFF42241400E2A2C81982947F0003241D00E3A24011020080FF42241C00E2A2C81982942500E3A2401102009FFF42242400E2A290198294000000003C00401021880000C06114003801B28F3401BE8F408114001801ACAFD000A427211012022110C202001A4694001A4584403406001A40050C0334060021300000C01811001801AD8F3001AE8F1C01A58F2110AE012140620084030725B800A39484030295B800A42421186200B800A3A4020082940200E3940100C62421104300020082A4040082940400E3940000000021104300040082A40400C228EFFF40140800A5242120E002D000A527010006247C47050C2138C000211812022118C302001A629400000000FFFF4224001A62A42801AB8F0000000021107E012110C20290194294010031262A102202CBFF4014020010262180D502";

                for (var i = 1; i < 8; i++)
                {
                    offset = 0xB4C400 + i * 0x75800;
                    waBytesArray = RandomHelper.PutHex(waBytesArray, offset, hexString);
                }

                File.WriteAllBytes(Static.WaPath ?? throw new InvalidOperationException(), waBytesArray);
                break;
            }
            case > 1:
            {
                // Enable drop
                var slusBytesArray = File.ReadAllBytes(Static.SlusPath ?? throw new InvalidOperationException());

                var hexString = "95AB0608000000001400BFAF6439020C00000000";
                var offset = 0x12034;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                hexString = "10AB06080000000038B322A43C0082A400140200";
                offset = 0x1246C;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                hexString = "9DAB06080000000000000000";
                offset = 0x285FC;
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                offset = 0x19B400;
                hexString =
                    "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001B001D3C00ACBD270000A2AF0400A3AF0800B0AF0C00A4AF1000A5AF1400BFAF1800B6AF1C00B7AF2000A0A32000B693000000000100D626060017241D00D7122000A0A32000B6A31B80103C50AE108E00000000A8AB060800000000FF0742300100452421200000211880000000029600000000212082002A108500060040100100622401006324D2026228F7FF40140200102621100000020017241800F60212B8000021B8B7032000E2A61BAB0608000000000000A28F000000000400A38F000000000800B08F000000000C00A48F000000001000A58F000000001400BF8F000000001800B68F000000001C00B78F0000000020801D3C1D80033C0A80013C28FFBD271D870008000000001B80043C00AC8424000092AC21908000040056AE080057AE200040A220005692000000000100D626060017240C00D712200040A2200056A2020017241800F60212B8000021B857022000E496000000002586000C000000005AAB0608000000000400568E000000000800578E000000000000528E00000000C6870008000000001B80023C00AC4224000056AC040057AC080044AC20005690000000000100D626050017240200D712200040A0200056A0020017241800F60212B8000021B857002000F796000000001D80043C300497A7A85697A40000568C000000000400578C000000000800448C00000000008002343004838700000000C7DF0008212862000000000020801D3C1B80033C50AE63241880023C8C87422421800202000070AC0F860008000000002080023CA0FE422406004810008002343004838700000000C7DF0008212862000000000073AB0608000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C0000000027AB06080000000000000000000000000000000000000000";
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                offset = 0x12710;
                hexString = "53AB06080000000000000000";
                slusBytesArray = RandomHelper.PutHex(slusBytesArray, offset, hexString);

                File.WriteAllBytes(Static.SlusPath, slusBytesArray);

                var waBytesArray = File.ReadAllBytes(Static.WaPath ?? throw new InvalidOperationException());

                waBytesArray[0xBC1C78] = (byte)(dropCount + 1);
                waBytesArray[0xBC17E4] = (byte)dropCount;
                waBytesArray[0xBC1DEC] = (byte)dropCount;

                hexString =
                    "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001B001D3C00ACBD270000A2AF0400A3AF0800B0AF0C00A4AF1000A5AF1400BFAF1800B6AF1C00B7AF2000A0A32000B693000000000100D626060017241D00D7122000A0A32000B6A31B80103C50AE108E00000000A8AB060800000000FF0742300100452421200000211880000000029600000000212082002A108500060040100100622401006324D2026228F7FF40140200102621100000020017241800F60212B8000021B8B7032000E2A61BAB0608000000000000A28F000000000400A38F000000000800B08F000000000C00A48F000000001000A58F000000001400BF8F000000001800B68F000000001C00B78F0000000020801D3C1D80033C0A80013C28FFBD271D870008000000001B80043C00AC8424000092AC21908000040056AE080057AE200040A220005692000000000100D626060017240C00D712200040A2200056A2020017241800F60212B8000021B857022000E496000000002586000C000000005AAB0608000000000400568E000000000800578E000000000000528E00000000C6870008000000001B80023C00AC4224000056AC040057AC080044AC20005690000000000100D626050017240200D712200040A0200056A0020017241800F60212B8000021B857002000F796000000001D80043C300497A7A85697A40000568C000000000400578C000000000800448C00000000008002343004838700000000C7DF0008212862000000000020801D3C1B80033C50AE63241880023C8C87422421800202000070AC0F860008000000002080023CA0FE422406004810008002343004838700000000C7DF0008212862000000000073AB0608000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C0000000027AB06080000000000000000000000000000000000000000";

                for (var i = 1; i < 8; i++)
                {
                    offset = 0xB4C400 + i * 0x75800;
                    waBytesArray = RandomHelper.PutHex(waBytesArray, offset, hexString);

                    offset = 0xB4C478 + i * 0x75800;
                    waBytesArray[offset] = (byte)(dropCount + 1);

                    offset = 0xB4C574 + i * 0x75800;
                    waBytesArray[offset] = (byte)(dropCount + 1);

                    offset = 0xB4C5EC + i * 0x75800;
                    waBytesArray[offset] = (byte)dropCount;
                }

                File.WriteAllBytes(Static.WaPath ?? throw new InvalidOperationException(), waBytesArray);
                break;
            }
        }
    }
    
    /// <summary>
    /// </summary>
    private void RandomizeFusions()
    {
        if (!Static.RandomFusions) return;
        // for (var i = 0; i < Static.MaxCards; i++)
        // {
        //     foreach (var t in Static.Cards[i].Fusions)
        //     {
        //         // FUSION RANDOMIZING
        //         t.Cards2 = RandomHelper.GetRandomNumber(Static.HighId ? 1 : i, Static.CardCount);
        //         t.Result = RandomHelper.GetRandomNumber(Static.HighId ? 1 : i, Static.CardCount);
        //     }
        // }

        var maxFusions = Static.AllFusions.Count();
        Static.AllFusions = new List<Fusion>();
        
        // New approach for random fusions:
        for (var i = 0; i < maxFusions; ++i)
        {
            Static.AllFusions.Add(RandomHelper.CreateUniqueFusion());
        }
        
        if (Static.Spoiler) 
            _logger.WriteNewFusionSpoilerFile();
    }

    /// <summary>
    /// </summary>
    /// <param name="minAtk"></param>
    /// <param name="maxAtk"></param>
    /// <param name="minDef"></param>
    /// <param name="maxDef"></param>
    /// <param name="minCost"></param>
    /// <param name="maxCost"></param>
    private void RandomizeCardInfo(int minAtk = 1000, int maxAtk = 3000, int minDef = 1000, int maxDef = 3000,
        int minCost = 1, int maxCost = 999999)
    {
        var cardListRandomName =
            Static.Cards.Select(x => new { x.Name, x.NameImage }).OrderBy(_ => RandomHelper.GetRandomNext()).ToList();
        var cardListRandomDescription = Static.Cards.Select(x => x.Description).OrderBy(_ => RandomHelper.GetRandomNext()).ToList();
        var cardListRandomImage = Static.Cards.Select(x => new { x.BigImage, x.SmallImage })
            .OrderBy(_ => RandomHelper.GetRandomNext()).ToList();

        for (var i = 0; i < Static.MaxCards; i++)
        {
            if (Static.RandomAttributes)
                Static.Cards[i].Attribute = RandomHelper.GetRandomNumber(
                    (int)Static.Attribute.Light,
                    (int)Static.Attribute.Trap);

            if (RandomHelper.IsMonsterCard(Static.Cards[i].Type))
            {
                // ATK/DEF RANDOMIZING
                if (Static.RandomAtkdef)
                {
                    Static.Cards[i].Attack = RandomHelper.GetRandomNumber(minAtk, maxAtk);
                    Static.Cards[i].Defense = RandomHelper.GetRandomNumber(minDef, maxDef);
                }

                // Guardian Stars
                if (Static.RandomGuardianStars)
                {
                    Static.Cards[i].GuardianStar1 =
                        RandomHelper.GetRandomNumber((int)Static.GuardianStar.Mars, (int)Static.GuardianStar.Venus);
                    Static.Cards[i].GuardianStar2 =
                        RandomHelper.GetRandomNumber((int)Static.GuardianStar.Mars, (int)Static.GuardianStar.Venus);

                    if (Static.Cards[i].GuardianStar1 == Static.Cards[i].GuardianStar2)
                    {
                        if (Static.Cards[i].GuardianStar2 == (int)Static.GuardianStar.Venus)
                        {
                            Static.Cards[i].GuardianStar2 -= RandomHelper.GetRandomNumber(2, 4);
                        }
                        else if (Static.Cards[i].GuardianStar2 == (int)Static.GuardianStar.Mars)
                        {
                            Static.Cards[i].GuardianStar2 += RandomHelper.GetRandomNumber(2, 4);
                        }
                        else
                        {
                            if (RandomHelper.GetRandomNumber(1, 2) == 2)
                                Static.Cards[i].GuardianStar2++;
                            else
                                Static.Cards[i].GuardianStar2--;
                        }
                    }
                }

                // Monster Type
                if (Static.RandomMonstersTypes)
                    Static.Cards[i].Type = RandomHelper.GetRandomNumber((int)Static.Type.Dragon, (int)Static.Type.Plant);
            }

            if (Static.RandomEquips && Static.Cards[i].Type == (int)Static.Type.Equip)
            {
                // Limpamos a lista de monstros antiga.
                Static.Cards[i].Equips.RemoveRange(0, Static.Cards[i].Equips.Count());

                // Buscamos todos os monstros existentes.
                var allMonstersId = Static.Cards
                    .Where(x => RandomHelper.IsMonsterCard(x.Type))
                    .Select(y => y.Id)
                    .OrderBy(_ => RandomHelper.GetRandomNext())
                    .ToList();

                var equipCount = allMonstersId.Count() <= Static.MinMonstersEquips
                    ? allMonstersId.Count()
                    : RandomHelper.GetRandomNumber(Static.MinMonstersEquips, allMonstersId.Count());

                for (var j = 0; j < equipCount; j++)
                {
                    var randomIndex = RandomHelper.GetRandomNumber(0, allMonstersId.Count());

                    Static.Cards[i].Equips.Add(allMonstersId.ElementAt(randomIndex));
                    allMonstersId.RemoveAt(randomIndex);
                }
            }

            if (Static.RandomStarchips) Static.Cards[i].Starchip.Cost = RandomHelper.GetRandomNumber(minCost, maxCost);

            if (Static.RandomNames)
            {
                var randomIndex = RandomHelper.GetRandomNumber(0, cardListRandomName.Count());

                Static.Cards[i].Name = cardListRandomName.ElementAt(randomIndex).Name;
                Static.Cards[i].NameImage = cardListRandomName.ElementAt(randomIndex).NameImage;

                cardListRandomName.RemoveAt(randomIndex);
            }

            if (Static.RandomDescriptions)
            {
                var randomIndex = RandomHelper.GetRandomNumber(0, cardListRandomDescription.Count());
                Static.Cards[i].Description = cardListRandomDescription.ElementAt(randomIndex);
                cardListRandomDescription.RemoveAt(randomIndex);
            }

            if (Static.RandomCardImages)
            {
                var randomIndex = RandomHelper.GetRandomNumber(0, cardListRandomImage.Count());

                Static.Cards[i].BigImage = cardListRandomImage.ElementAt(randomIndex).BigImage;
                Static.Cards[i].SmallImage = cardListRandomImage.ElementAt(randomIndex).SmallImage;

                cardListRandomImage.RemoveAt(randomIndex);
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="size"></param>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    /// <param name="sum"></param>
    /// <returns></returns>
    private int[] GenerateRandomNumbers(int size, int minimum, int maximum, int sum)
    {
        if (size <= 1) return new[] { sum };

        if (minimum * size > sum) minimum = sum / size;

        if (maximum * size < sum) maximum += sum - maximum * size;

        if (minimum > maximum) (minimum, maximum) = (maximum, minimum);

        var numberList = new int[size];

        for (var index = 0; index < size - 1; index++)
        {
            var rest = numberList.Length - (index + 1);

            var restMinimum = minimum * rest;
            var restMaximum = maximum * rest;

            minimum = Math.Max(minimum, sum - restMaximum);
            maximum = Math.Min(maximum, sum - restMinimum);

            var newRandomValue = RandomHelper.GetRandomNumber(minimum, maximum);
            numberList[index] = newRandomValue;
            sum -= newRandomValue;
        }

        numberList[size - 1] = sum;

        return numberList;
    }

    /// <summary>
    /// </summary>
    private void RandomizeCardDrops()
    {
        if (!Static.RandomCardDrops) return;
        var allCardsId = Static.Cards
            .Select(y => y.Id)
            .OrderBy(_ => RandomHelper.GetRandomNext())
            .ToList();

        foreach (var t1 in Static.Duelist)
        {
            // Limpamos a lista antiga.
            Array.Clear(t1.Drop.BcdPow, 0, Static.MaxCards);
            Array.Clear(t1.Drop.SaPow, 0, Static.MaxCards);
            Array.Clear(t1.Drop.SaTec, 0, Static.MaxCards);

            // BCD - POW/TEC
            var randomSize = RandomHelper.GetRandomNumber(20, 100);
            var randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 100, Static.MaxRateDrop);

            foreach (var randomNumber in randomNumbersArray)
                if (allCardsId.Count > 0)
                {
                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }
                else
                {
                    allCardsId = Static.Cards
                        .Select(y => y.Id)
                        .OrderBy(_ => RandomHelper.GetRandomNext())
                        .ToList();

                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }

            // SA - POW
            randomSize = RandomHelper.GetRandomNumber(20, 100);
            randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 100, Static.MaxRateDrop);

            foreach (var randomNumber in randomNumbersArray)
                if (allCardsId.Count > 0)
                {
                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }
                else
                {
                    allCardsId = Static.Cards
                        .Select(y => y.Id)
                        .OrderBy(_ => RandomHelper.GetRandomNext())
                        .ToList();

                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }

            // SA - TEC
            randomSize = RandomHelper.GetRandomNumber(20, 100);
            randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 100, Static.MaxRateDrop);

            foreach (var randomNumber in randomNumbersArray)
                if (allCardsId.Count > 0)
                {
                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }
                else
                {
                    allCardsId = Static.Cards
                        .Select(y => y.Id)
                        .OrderBy(_ => RandomHelper.GetRandomNext())
                        .ToList();

                    var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count());

                    t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    allCardsId.RemoveAt(randomIndex);
                }
        }
    }

    /// <summary>
    /// </summary>
    private void RandomizeDuelistDecks()
    {
        if (!Static.RandomDecks) return;
        foreach (var t1 in Static.Duelist)
        {
            var randomSize = RandomHelper.GetRandomNumber(20, 100);
            var randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 100, Static.MaxRateDrop);

            // Limpamos a lista antiga.
            Array.Clear(t1.Deck, 0, Static.MaxCards);

            // Buscamos todas as cartas, exceto cartas de tipo ritual
            // e aplicamos o filtro de ATK/DEF caso exista e seja cartas de monstros.
            var allCardsId = Static.Cards
                .Where(x => x.Type != (int)Static.Type.Ritual
                            && ((Static.FilterDuelistDeckCards.EnableAtkDef
                                 && x.Attack >= Static.FilterDuelistDeckCards.MinimumAttack
                                 && x.Attack <= Static.FilterDuelistDeckCards.MaximumAttack
                                 && x.Defense >= Static.FilterDuelistDeckCards.MinimumDefense
                                 && x.Defense <= Static.FilterDuelistDeckCards.MaximumDefense
                                 && RandomHelper.IsMonsterCard(x.Type))
                                || Static.FilterDuelistDeckCards.EnableAtkDef == false
                                || RandomHelper.IsMonsterCard(x.Type) == false))
                .Select(y => y.Id)
                .OrderBy(_ => RandomHelper.GetRandomNext())
                .ToList();

            var allBannedCards = Static.FilterDuelistDeckCards.BannedCards
                .Select(x => TypeDescriptor.GetProperties(x)["Id"].GetValue(x) as int? ?? 0).ToList();

            allCardsId = allCardsId
                .Where(x => allBannedCards.Contains(x) == false)
                .ToList();

            foreach (var randomNumber in randomNumbersArray)
            {
                var randomIndex = RandomHelper.GetRandomNumber(0, allCardsId.Count);

                t1.Deck[allCardsId.ElementAt(randomIndex) - 1] = randomNumber;

                allCardsId.RemoveAt(randomIndex);
            }
        }
    }

    /// <summary>
    /// </summary>
    private void RandomizeStarterDecks()
    {
        if (!Static.RandomStarterDecks) return;
        var allBannedCards = Static.FilterStarterDeckCards.BannedCards
            .Select(x => TypeDescriptor.GetProperties(x)["Id"].GetValue(x) as int? ?? 0).ToList();

        // Static.StarterDeck[4] = Magics (Dark Hole and Raigeki)
        // Static.StarterDeck[5] = Magics (Forest, Wasteland, Mountain, Sogen, Umi and Yami)
        // Static.StarterDeck[6] = Magics (Equips)

        // Randomize equips. (Existem no máximo 34 equipamentos.)
        var randomSize = RandomHelper.GetRandomNumber(20, 34);
        var randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 140, Static.MaxRateDrop);

        // Aumentamos o número de cartas equipamentos obtidas.
        Static.StarterDeck[6].Dropped++;

        // Limpamos a lista antiga.
        Array.Clear(Static.StarterDeck[6].Cards, 0, Static.MaxCards);

        // Buscamos todos os equipamentos existentes.
        var allEquipsId = Static.Cards
            .Where(x => x.Type == (int)Static.Type.Equip)
            .Select(y => y.Id)
            .OrderBy(_ => RandomHelper.GetRandomNext())
            .ToList();

        allEquipsId = allEquipsId
            .Where(x => allBannedCards.Contains(x) == false)
            .ToList();

        foreach (var randomNumber in randomNumbersArray)
        {
            var randomIndex = RandomHelper.GetRandomNumber(0, allEquipsId.Count());

            Static.StarterDeck[6].Cards[allEquipsId.ElementAt(randomIndex) - 1] = randomNumber;

            allEquipsId.RemoveAt(randomIndex);
        }

        // Randomize magics. (Existem no máximo 33 mágicas.)
        randomSize = RandomHelper.GetRandomNumber(20, 33);
        randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 140, Static.MaxRateDrop);

        // Aumentamos o número de cartas mágicas obtidas.
        Static.StarterDeck[5].Dropped++;

        // Limpamos a lista antiga.
        Array.Clear(Static.StarterDeck[5].Cards, 0, Static.MaxCards);

        // Buscamos todas as mágicas existentes.
        var allMagicsId = Static.Cards
            .Where(x => x.Type == (int)Static.Type.Magic)
            .Select(y => y.Id)
            .OrderBy(_ => RandomHelper.GetRandomNext())
            .ToList();

        allMagicsId = allMagicsId
            .Where(x => allBannedCards.Contains(x) == false)
            .ToList();

        foreach (var randomNumber in randomNumbersArray)
        {
            var randomIndex = RandomHelper.GetRandomNumber(0, allMagicsId.Count());

            Static.StarterDeck[5].Cards[allMagicsId.ElementAt(randomIndex) - 1] = randomNumber;

            allMagicsId.RemoveAt(randomIndex);
        }

        // Randomize traps. (Existem no máximo 10 traps.)
        randomSize = RandomHelper.GetRandomNumber(1, 10);
        randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 310, Static.MaxRateDrop);

        // Limpamos a lista antiga.
        Array.Clear(Static.StarterDeck[4].Cards, 0, Static.MaxCards);

        // Buscamos todas as armadilhas existentes.
        var allTrapsId = Static.Cards
            .Where(x => x.Type == (int)Static.Type.Trap)
            .Select(y => y.Id)
            .OrderBy(_ => RandomHelper.GetRandomNext())
            .ToList();

        allTrapsId = allTrapsId
            .Where(x => allBannedCards.Contains(x) == false)
            .ToList();

        foreach (var randomNumber in randomNumbersArray)
        {
            var randomIndex = RandomHelper.GetRandomNumber(0, allTrapsId.Count());

            Static.StarterDeck[4].Cards[allTrapsId.ElementAt(randomIndex) - 1] = randomNumber;

            allTrapsId.RemoveAt(randomIndex);
        }

        // Randomize monsters.
        // Iremos diminuir a quantidade de monstros do primeiro e segundo 'set' de cartas (16 -> 15)
        // para aumentar a quantidade de mágicas e equipamentos que podem ser obtidas. (1 -> 2)
        Static.StarterDeck[0].Dropped--;
        Static.StarterDeck[1].Dropped--;

        for (var i = 0; i < Static.MaxStarterDeck - 3; i++)
        {
            randomSize = RandomHelper.GetRandomNumber(60, 100);
            randomNumbersArray = GenerateRandomNumbers(randomSize, 1, 100, Static.MaxRateDrop);

            // Limpamos a lista antiga.
            Array.Clear(Static.StarterDeck[i].Cards, 0, Static.MaxCards);

            // Buscamos todos os monstros existentes
            // e aplicamos os filtros de ATK/DEF caso existam.
            var allMonstersId = Static.Cards
                .Where(x => RandomHelper.IsMonsterCard(x.Type)
                            && x.Id != 722 /* Magician of Black Chaos*/
                            && ((Static.FilterStarterDeckCards.EnableAtkDef
                                 && x.Attack >= Static.FilterStarterDeckCards.MinimumAttack
                                 && x.Attack <= Static.FilterStarterDeckCards.MaximumAttack
                                 && x.Defense >= Static.FilterStarterDeckCards.MinimumDefense
                                 && x.Defense <= Static.FilterStarterDeckCards.MaximumDefense)
                                || Static.FilterStarterDeckCards.EnableAtkDef == false))
                .Select(y => y.Id)
                .OrderBy(_ => RandomHelper.GetRandomNext())
                .ToList();

            allMonstersId = allMonstersId
                .Where(x => allBannedCards.Contains(x) == false)
                .ToList();

            foreach (var randomNumber in randomNumbersArray)
            {
                var randomIndex = RandomHelper.GetRandomNumber(0, allMonstersId.Count());

                Static.StarterDeck[i].Cards[allMonstersId.ElementAt(randomIndex) - 1] = randomNumber;

                allMonstersId.RemoveAt(randomIndex);
            }
        }
    }


    /// <summary>
    /// </summary>
    /// <param name="minAtk"></param>
    /// <param name="maxAtk"></param>
    /// <param name="minDef"></param>
    /// <param name="maxDef"></param>
    /// <param name="minCost"></param>
    /// <param name="maxCost"></param>
    /// <param name="dropCount"></param>
    /// <param name="starChipsDuel"></param>
    /// <param name="minAtkStarter"></param>
    /// <param name="maxAtkStarter"></param>
    /// <param name="minDefStarter"></param>
    /// <param name="maxDefStarter"></param>
    /// <param name="minAtkDuelist"></param>
    /// <param name="maxAtkDuelist"></param>
    /// <param name="minDefDuelist"></param>
    /// <param name="maxDefDuelist"></param>
    /// <returns></returns>
    public bool PerformScrambling(
        int minAtk = 0,
        int maxAtk = 0,
        int minDef = 0,
        int maxDef = 0,
        int minCost = 0,
        int maxCost = 999999,
        int dropCount = 1,
        ushort starChipsDuel = 5,
        int minAtkStarter = 0,
        int maxAtkStarter = 0,
        int minDefStarter = 0,
        int maxDefStarter = 0,
        int minAtkDuelist = 0,
        int maxAtkDuelist = 0,
        int minDefDuelist = 0,
        int maxDefDuelist = 0)
    {
        if (Static.FilterStarterDeckCards.EnableAtkDef)
        {
            Static.FilterStarterDeckCards.MinimumAttack = minAtkStarter;
            Static.FilterStarterDeckCards.MaximumAttack = maxAtkStarter;
            Static.FilterStarterDeckCards.MinimumDefense = minDefStarter;
            Static.FilterStarterDeckCards.MaximumDefense = maxDefStarter;
        }

        if (Static.FilterDuelistDeckCards.EnableAtkDef)
        {
            Static.FilterDuelistDeckCards.MinimumAttack = minAtkDuelist;
            Static.FilterDuelistDeckCards.MaximumAttack = maxAtkDuelist;
            Static.FilterDuelistDeckCards.MinimumDefense = minDefDuelist;
            Static.FilterDuelistDeckCards.MaximumDefense = maxDefDuelist;
        }
        
        RandomizeFusions();
        RandomizeCardInfo(minAtk, maxAtk, minDef, maxDef, minCost, maxCost);
        RandomizeCardDrops();
        RandomizeStarterDecks();
        RandomizeDuelistDecks();

        if (starChipsDuel < 5) starChipsDuel = 5;

        if (starChipsDuel > 100) starChipsDuel = 100;

        Static.StarChipsDuel = starChipsDuel;

        var writer = new DataWriter();
        writer.WriteChangesToFile();
        EnableDropCount(dropCount);

        if (!Static.Spoiler) return true;

        _logger.WriteStarchipSpoilerFile();
        _logger.WriteDropsSpoilerFile();
        _logger.WriteStarterDecksSpoilerFile();
        _logger.WriteDuelistDecksSpoilerFile();
        //_logger.WriteHtmlFusionSpoilerFile();

        return true;
    }
}