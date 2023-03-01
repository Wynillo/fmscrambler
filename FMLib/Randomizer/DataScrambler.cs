using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FMLib.Models;
using FMLib.Utility;
using System.ComponentModel;

namespace FMLib.Randomizer
{
    /// <summary>
    /// 
    /// </summary>
    public class DataScrambler
    {
        private readonly Random _random;
        private readonly int _seed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        public DataScrambler(int seed)
        {

            // Initialize RNG with the Seed
            _random = new Random(seed);
            _seed = seed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public int GetRandomNumber(int min, int max)
        {
            lock (_random)
            {
                return _random.Next(min, max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetRandomNext()
        {
            lock (_random)
            {
                return _random.Next();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] ConvertHexStringToByteArray(string hexString)
        {
            if ((hexString.Length % 2) > 0)
            {
                return new byte[] { };
            }

            var buffer = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 0x10);
            }

            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="hexString"></param>
        private void PutHex(byte[] bytes, int offset, string hexString)
        {
            var sourceArray = ConvertHexStringToByteArray(hexString);

            Array.Copy(sourceArray, 0, bytes, offset, sourceArray.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDataFromSlus()
        {
            var memStream = new MemoryStream(File.ReadAllBytes(Static.SlusPath))
            {
                // General card data
                Position = 0x1C4A44L
            };

            for (int i = 0; i < Static.MaxCards; i++)
            {
                int int32 = memStream.ExtractPiece(0, 4).ExtractInt32();

                Static.Cards[i] = new Card
                {
                    Id = i + 1,
                    Attack = (int32 & 0x1FF) * 10,          // Unwrap attack value
                    Defense = (int32 >> 9 & 0x1FF) * 10,    // Unwrap defense value
                    GuardianStar2 = int32 >> 18 & 0xF,      // Unwrap 1st guardian star
                    GuardianStar1 = int32 >> 22 & 0xF,      // Unwrap 2nd guardian star
                    Type = int32 >> 26 & 0x1F               // Unwrap card's type
                };
            }

            // Cards level and attribute
            memStream.Position = 0x1C5B33L;

            for (int i = 0; i < Static.MaxCards; i++)
            {
                byte num = memStream.ExtractPiece(0, 1)[0];

                Static.Cards[i].Level = num & 0xF;          // Unwrap card level
                Static.Cards[i].Attribute = num >> 4 & 0xF; // Unwrap card's attribute
            }

            // Card name
            for (int i = 0; i < Static.MaxCards; i++)
            {
                memStream.Position = 0x1C6002 + i * 2;

                int num = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;

                memStream.Position = 0x1C6800 + num - 0x6000;
                Static.Cards[i].Name = memStream.GetText(Static.Dict);
            }

            // Card description
            for (int i = 0; i < Static.MaxCards; i++)
            {
                memStream.Position = 0x1B0A02 + i * 2;

                int num3 = memStream.ExtractPiece(0, 2).ExtractUInt16();

                memStream.Position = 0x1B11F4 + (num3 - 0x9F4);
                Static.Cards[i].Description = memStream.GetText(Static.Dict);
            }

            // Duelist names
            for (int i = 0; i < Static.MaxDuelists; i++)
            {
                memStream.Position = 0x1C6652 + i * 2;
                memStream.Position = 0x1C6800 + memStream.ExtractPiece(0, 2).ExtractUInt16() - 0x6000;

                Static.Duelist[i] = new Duelist(memStream.GetText(Static.Dict));
            }

            // Star chips per Duel
            memStream.Position = 0x126DCL;
            Static.StarChipsDuel = memStream.ExtractPiece(0, 2).ExtractUInt16();
            Static.StarChipsDuel += 5;

            memStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnableDropCount(int dropCount)
        {
            if (dropCount > 15)
            {
                dropCount = 15;
            }

            if (dropCount <= 1 && Static.DropCount != 0)
            {
                // Remove drop
                var SLUS_BytesArray = File.ReadAllBytes(Static.SlusPath);

                string hexString = "1880023C8C8742241400BFAF6439020C21800202";
                int offset = 0x12034;
                PutHex(SLUS_BytesArray, offset, hexString);

                hexString = "ACA906080000000038B322A43C0082A400140200";
                offset = 0x1246C;
                PutHex(SLUS_BytesArray, offset, hexString);

                hexString = "1D80033C0A80013C";
                offset = 0x285FC;
                PutHex(SLUS_BytesArray, offset, hexString);

                offset = 0x19B400;
                hexString = "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                PutHex(SLUS_BytesArray, offset, hexString);

                offset = 0x12710;
                hexString = "3C0044842586000C00000000";
                PutHex(SLUS_BytesArray, offset, hexString);

                File.WriteAllBytes(Static.SlusPath, SLUS_BytesArray);

                var WA_BytesArray = File.ReadAllBytes(Static.WaPath);
                hexString = "0C0007140193143F0200003F0000013F0000023F0000031F000102030C0193143F0000003F0200012300010C3F000000040C003914390193143A110A003F1700010400015514110A003F15000104000189140198143F0000003F000001260A00010C3F000004050C0493143F0200003F0000013F0000023F0000031F000102030C050C049314018914390A06000A000C0193142F0C0198140DFF42281E0040142110D50247DF020C00000000801D0496000000001A00440002008014000000000D000700FFFF0124040081140080013C02004114000000000D00060010180000000000000A006014FFFF822401000224901902A61401AB8F2401AC8F21280000FA3E050C21206C01461D05082110D502801D02A62110D5029019429400000000980040101800A4273800A5275800A7270C01AD8F6000A2271000A2AF1400A0AF0400A6252130C602F347050C2130D3001680033CE0B7632410006294000000001600E2A612006294000000000E00E2A62401AE8F0401AB8F000000002110CB012110C202B21E4390000000000400E3A2B31E4390000000000500E3A2B41E42902120D5020600E2A2C8198294400003240D00E3A24011020080FF42240C00E2A2C81982941500E3A2401102009FFF42241400E2A2C81982947F0003241D00E3A24011020080FF42241C00E2A2C81982942500E3A2401102009FFF42242400E2A290198294000000003C00401021880000C06114003801B28F3401BE8F408114001801ACAFD000A427211012022110C202001A4694001A4584403406001A40050C0334060021300000C01811001801AD8F3001AE8F1C01A58F2110AE012140620084030725B800A39484030295B800A42421186200B800A3A4020082940200E3940100C62421104300020082A4040082940400E3940000000021104300040082A40400C228EFFF40140800A5242120E002D000A527010006247C47050C2138C000211812022118C302001A629400000000FFFF4224001A62A42801AB8F0000000021107E012110C20290194294010031262A102202CBFF4014020010262180D502";

                for (int i = 1; i < 8; i++)
                {
                    offset = 0xB4C400 + (i * 0x75800);
                    PutHex(WA_BytesArray, offset, hexString);
                }

                File.WriteAllBytes(Static.WaPath, WA_BytesArray);
            }
            else if (dropCount > 1)
            {
                // Enable drop
                var SLUS_BytesArray = File.ReadAllBytes(Static.SlusPath);

                string hexString = "95AB0608000000001400BFAF6439020C00000000";
                int offset = 0x12034;
                PutHex(SLUS_BytesArray, offset, hexString);

                hexString = "10AB06080000000038B322A43C0082A400140200";
                offset = 0x1246C;
                PutHex(SLUS_BytesArray, offset, hexString);

                hexString = "9DAB06080000000000000000";
                offset = 0x285FC;
                PutHex(SLUS_BytesArray, offset, hexString);

                offset = 0x19B400;
                hexString = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001B001D3C00ACBD270000A2AF0400A3AF0800B0AF0C00A4AF1000A5AF1400BFAF1800B6AF1C00B7AF2000A0A32000B693000000000100D626060017241D00D7122000A0A32000B6A31B80103C50AE108E00000000A8AB060800000000FF0742300100452421200000211880000000029600000000212082002A108500060040100100622401006324D2026228F7FF40140200102621100000020017241800F60212B8000021B8B7032000E2A61BAB0608000000000000A28F000000000400A38F000000000800B08F000000000C00A48F000000001000A58F000000001400BF8F000000001800B68F000000001C00B78F0000000020801D3C1D80033C0A80013C28FFBD271D870008000000001B80043C00AC8424000092AC21908000040056AE080057AE200040A220005692000000000100D626060017240C00D712200040A2200056A2020017241800F60212B8000021B857022000E496000000002586000C000000005AAB0608000000000400568E000000000800578E000000000000528E00000000C6870008000000001B80023C00AC4224000056AC040057AC080044AC20005690000000000100D626050017240200D712200040A0200056A0020017241800F60212B8000021B857002000F796000000001D80043C300497A7A85697A40000568C000000000400578C000000000800448C00000000008002343004838700000000C7DF0008212862000000000020801D3C1B80033C50AE63241880023C8C87422421800202000070AC0F860008000000002080023CA0FE422406004810008002343004838700000000C7DF0008212862000000000073AB0608000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C0000000027AB06080000000000000000000000000000000000000000";
                PutHex(SLUS_BytesArray, offset, hexString);

                offset = 0x12710;
                hexString = "53AB06080000000000000000";
                PutHex(SLUS_BytesArray, offset, hexString);

                File.WriteAllBytes(Static.SlusPath, SLUS_BytesArray);

                var WA_BytesArray = File.ReadAllBytes(Static.WaPath);

                WA_BytesArray[0xBC1C78] = (byte)(dropCount + 1);
                WA_BytesArray[0xBC17E4] = (byte)dropCount;
                WA_BytesArray[0xBC1DEC] = (byte)dropCount;

                hexString = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001B001D3C00ACBD270000A2AF0400A3AF0800B0AF0C00A4AF1000A5AF1400BFAF1800B6AF1C00B7AF2000A0A32000B693000000000100D626060017241D00D7122000A0A32000B6A31B80103C50AE108E00000000A8AB060800000000FF0742300100452421200000211880000000029600000000212082002A108500060040100100622401006324D2026228F7FF40140200102621100000020017241800F60212B8000021B8B7032000E2A61BAB0608000000000000A28F000000000400A38F000000000800B08F000000000C00A48F000000001000A58F000000001400BF8F000000001800B68F000000001C00B78F0000000020801D3C1D80033C0A80013C28FFBD271D870008000000001B80043C00AC8424000092AC21908000040056AE080057AE200040A220005692000000000100D626060017240C00D712200040A2200056A2020017241800F60212B8000021B857022000E496000000002586000C000000005AAB0608000000000400568E000000000800578E000000000000528E00000000C6870008000000001B80023C00AC4224000056AC040057AC080044AC20005690000000000100D626050017240200D712200040A0200056A0020017241800F60212B8000021B857002000F796000000001D80043C300497A7A85697A40000568C000000000400578C000000000800448C00000000008002343004838700000000C7DF0008212862000000000020801D3C1B80033C50AE63241880023C8C87422421800202000070AC0F860008000000002080023CA0FE422406004810008002343004838700000000C7DF0008212862000000000073AB0608000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C000000006439020C0000000027AB06080000000000000000000000000000000000000000";

                for (int i = 1; i < 8; i++)
                {
                    offset = 0xB4C400 + (i * 0x75800);
                    PutHex(WA_BytesArray, offset, hexString);

                    offset = 0xB4C478 + (i * 0x75800);
                    WA_BytesArray[offset] = (byte)(dropCount + 1);

                    offset = 0xB4C574 + (i * 0x75800);
                    WA_BytesArray[offset] = (byte)(dropCount + 1);

                    offset = 0xB4C5EC + (i * 0x75800);
                    WA_BytesArray[offset] = (byte)dropCount;
                }

                File.WriteAllBytes(Static.WaPath, WA_BytesArray);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDataFromWaMrg()
        {
            // WA_MRG process
            var WA_MRGStream = new FileStream(Static.WaPath, FileMode.Open);

            // Drop count
            Static.DropCount = WA_MRGStream.ExtractPiece(0, 1, 0xBC1C78)[0];

            // Fusions
            var memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 0x10000, 0xB87800));

            for (int i = 0; i < Static.MaxCards; ++i)
            {
                memStream.Position = 2 + i * 2;
                memStream.Position = memStream.ExtractPiece(0, 2).ExtractUInt16() & ushort.MaxValue;

                if (memStream.Position != 0L)
                {
                    int num1 = memStream.ReadByte();

                    if (num1 == 0)
                    {
                        num1 = 511 - memStream.ReadByte();
                    }

                    int num2 = num1;

                    while (num2 > 0)
                    {
                        int num3 = memStream.ReadByte();
                        int num4 = memStream.ReadByte();
                        int num5 = memStream.ReadByte();
                        int num6 = memStream.ReadByte();
                        int num7 = memStream.ReadByte();
                        int num9 = (num3 & 3) << 8 | num4;
                        int num11 = (num3 >> 2 & 3) << 8 | num5;
                        int num13 = (num3 >> 4 & 3) << 8 | num6;
                        int num15 = (num3 >> 6 & 3) << 8 | num7;

                        Static.Cards[i].Fusions.Add(new Fusion(i + 1, num9, num11));
                        --num2;

                        if (num2 <= 0)
                        {
                            continue;
                        }

                        Static.Cards[i].Fusions.Add(new Fusion(i + 1, num13, num15));
                        --num2;
                    }
                }
            }

            memStream.Close();

            // Equips
            memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 0x2800, 0xB85000));

            while (true)
            {
                int num6 = memStream.ExtractPiece(0, 2).ExtractUInt16();

                if (num6 == 0)
                {
                    break;
                }

                int num7 = memStream.ExtractPiece(0, 2).ExtractUInt16();

                for (int num8 = 0; num8 < num7; num8++)
                {
                    int num9 = memStream.ExtractPiece(0, 2).ExtractUInt16();
                    Static.Cards[num6 - 1].Equips.Add(num9 - 1);
                }
            }

            memStream.Close();

            // Card costs/passwords
            memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, Static.MaxCards * 8, 0xFB9808));

            for (var i = 0; i < Static.MaxCards; ++i)
            {
                Static.Cards[i].Starchip = new Starchips();

                var cost_bytes = new byte[4];
                cost_bytes[0] = (byte)memStream.ReadByte();
                cost_bytes[1] = (byte)memStream.ReadByte();
                cost_bytes[2] = (byte)memStream.ReadByte();
                cost_bytes[3] = (byte)memStream.ReadByte();

                Static.Cards[i].Starchip.Cost = cost_bytes.ExtractInt32();

                var pass_bytes = new byte[4];
                pass_bytes[0] = (byte)memStream.ReadByte();
                pass_bytes[1] = (byte)memStream.ReadByte();
                pass_bytes[2] = (byte)memStream.ReadByte();
                pass_bytes[3] = (byte)memStream.ReadByte();

                var res_pass = "";

                for (var j = 3; j >= 0; --j)
                {
                    var str = pass_bytes[j].ToString("X");

                    if (str.Length == 1)
                    {
                        str = str.Insert(0, "0");
                    }

                    res_pass += str;
                }

                int.TryParse(res_pass, out int out_pass);

                Static.Cards[i].Starchip.Password = out_pass;
                Static.Cards[i].Starchip.PasswordStr = res_pass;
            }

            memStream.Close();

            // Card image
            memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 0x1A3F1C0, 0x169000));

            for (int i = 0; i < Static.MaxCards; i++)
            {
                memStream.Position = 0x3800 * i;

                Static.Cards[i].BigImage = new FMLib.Models.Card.TIM(memStream.ExtractPiece(0, 0x2840, -1), 0x66, 0x60, 8, true, false);
                Static.Cards[i].NameImage = new FMLib.Models.Card.TIM(memStream.ExtractPiece(0, 0x2A0, -1), 0x60, 14, 4, false, true);
                Static.Cards[i].SmallImage = new FMLib.Models.Card.TIM(memStream.ExtractPiece(0, 0x580, -1), 40, 0x20, 8, true, false);
            }

            memStream.Close();

            // Read duelist decks and card drops
            for (var i = 0; i < Static.MaxDuelists; i++)
            {
                memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i));

                for (int j = 0; j < Static.MaxCards; ++j)
                {
                    Static.Duelist[i].Deck[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();
                }

                memStream.Close();

                memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0x5B4));

                for (int j = 0; j < Static.MaxCards; ++j)
                {
                    Static.Duelist[i].Drop.SaPow[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();
                }

                memStream.Close();

                memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0xB68));

                for (int j = 0; j < Static.MaxCards; ++j)
                {
                    Static.Duelist[i].Drop.BcdPow[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();
                }

                memStream.Close();

                memStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 1444, 0xE9B000 + 0x1800 * i + 0x111C));

                for (int j = 0; j < Static.MaxCards; ++j)
                {
                    Static.Duelist[i].Drop.SaTec[j] = memStream.ExtractPiece(0, 2).ExtractUInt16();
                }

                memStream.Close();
            }

            // Starter deck
            for (int i = 0; i < Static.MaxStarterDeck; i++)
            {
                Static.StarterDeck[i] = new StarterDeck();

                WA_MRGStream.Position = 0xF92BD4 + (0x5B8 * i);

                using (var memoryStream = new MemoryStream(WA_MRGStream.ExtractPiece(0, 0x5A6, -1)))
                {
                    Static.StarterDeck[i].Dropped = memoryStream.ExtractPiece(0, 2, -1).ExtractUInt16();

                    for (int j = 0; j < Static.MaxCards; j++)
                    {
                        Static.StarterDeck[i].Cards[j] = memoryStream.ExtractPiece(0, 2, -1).ExtractUInt16();
                    }
                }
            }

            WA_MRGStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeFusions()
        {
            if (Static.RandomFusions)
            {
                for (int i = 0; i < Static.MaxCards; i++)
                {
                    foreach (Fusion t in Static.Cards[i].Fusions)
                    {
                        // FUSION RANDOMIZING
                        t.Cards2 = GetRandomNumber(Static.HighId ? 1 : i, Static.CardCount);
                        t.Result = GetRandomNumber(Static.HighId ? 1 : i, Static.CardCount);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardType"></param>
        public bool IsMonsterCard(int cardType)
        {
            return cardType >= (int)Static.Type.Dragon && cardType <= (int)Static.Type.Plant;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAtk"></param>
        /// <param name="maxAtk"></param>
        /// <param name="minDef"></param>
        /// <param name="maxDef"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        public void RandomizeCardInfo(int minAtk = 1000, int maxAtk = 3000, int minDef = 1000, int maxDef = 3000, int minCost = 1, int maxCost = 999999)
        {
            var cardListRandomName = Static.Cards.Select(x => new { x.Name, x.NameImage }).OrderBy(y => _random.Next()).ToList();
            var cardListRandomDescription = Static.Cards.Select(x => x.Description).OrderBy(y => _random.Next()).ToList();
            var cardListRandomImage = Static.Cards.Select(x => new { x.BigImage, x.SmallImage }).OrderBy(y => _random.Next()).ToList();

            for (int i = 0; i < Static.MaxCards; i++)
            {
                if (Static.RandomAttributes)
                {
                    Static.Cards[i].Attribute = GetRandomNumber(
                        (int)Static.Attribute.Light,
                        (int)Static.Attribute.Trap);
                }

                if (IsMonsterCard(Static.Cards[i].Type))
                {
                    // ATK/DEF RANDOMIZING
                    if (Static.RandomAtkdef)
                    {
                        Static.Cards[i].Attack = GetRandomNumber(minAtk, maxAtk);
                        Static.Cards[i].Defense = GetRandomNumber(minDef, maxDef);
                    }

                    // Guardian Stars
                    if (Static.RandomGuardianStars)
                    {
                        Static.Cards[i].GuardianStar1 = GetRandomNumber((int)Static.GuardianStar.Mars, (int)Static.GuardianStar.Venus);
                        Static.Cards[i].GuardianStar2 = GetRandomNumber((int)Static.GuardianStar.Mars, (int)Static.GuardianStar.Venus);

                        if (Static.Cards[i].GuardianStar1 == Static.Cards[i].GuardianStar2)
                        {
                            if (Static.Cards[i].GuardianStar2 == (int)Static.GuardianStar.Venus)
                            {
                                Static.Cards[i].GuardianStar2 -= GetRandomNumber(2, 4);
                            }
                            else if (Static.Cards[i].GuardianStar2 == (int)Static.GuardianStar.Mars)
                            {
                                Static.Cards[i].GuardianStar2 += GetRandomNumber(2, 4);
                            }
                            else
                            {
                                if (GetRandomNumber(1, 2) == 2)
                                {
                                    Static.Cards[i].GuardianStar2++;
                                }
                                else
                                {
                                    Static.Cards[i].GuardianStar2--;
                                }
                            }
                        }
                    }

                    // Monster Type
                    if (Static.RandomMonstersTypes)
                    {
                        Static.Cards[i].Type = GetRandomNumber((int)Static.Type.Dragon, (int)Static.Type.Plant);
                    }
                }

                if (Static.RandomEquips && Static.Cards[i].Type == (int)Static.Type.Equip)
                {
                    // Limpamos a lista de monstros antiga.
                    Static.Cards[i].Equips.RemoveRange(0, Static.Cards[i].Equips.Count());

                    // Buscamos todos os monstros existentes.
                    var allMonstersId = Static.Cards
                        .Where(x => IsMonsterCard(x.Type))
                        .Select(y => y.Id)
                        .OrderBy(z => _random.Next())
                        .ToList();

                    var equipCount = (allMonstersId.Count() <= Static.MinMonstersEquips)
                        ? allMonstersId.Count()
                        : GetRandomNumber(Static.MinMonstersEquips, allMonstersId.Count());

                    for (int j = 0; j < equipCount; j++)
                    {
                        int randomIndex = GetRandomNumber(0, allMonstersId.Count());

                        Static.Cards[i].Equips.Add(allMonstersId.ElementAt(randomIndex));
                        allMonstersId.RemoveAt(randomIndex);
                    }
                }

                if (Static.RandomStarchips)
                {
                    Static.Cards[i].Starchip.Cost = GetRandomNumber(minCost, maxCost);
                }

                if (Static.RandomNames)
                {
                    int randomIndex = GetRandomNumber(0, cardListRandomName.Count());

                    Static.Cards[i].Name = cardListRandomName.ElementAt(randomIndex).Name;
                    Static.Cards[i].NameImage = cardListRandomName.ElementAt(randomIndex).NameImage;

                    cardListRandomName.RemoveAt(randomIndex);
                }

                if (Static.RandomDescriptions)
                {
                    int randomIndex = GetRandomNumber(0, cardListRandomDescription.Count());
                    Static.Cards[i].Description = cardListRandomDescription.ElementAt(randomIndex);
                    cardListRandomDescription.RemoveAt(randomIndex);
                }

                if (Static.RandomCardImages)
                {
                    int randomIndex = GetRandomNumber(0, cardListRandomImage.Count());

                    Static.Cards[i].BigImage = cardListRandomImage.ElementAt(randomIndex).BigImage;
                    Static.Cards[i].SmallImage = cardListRandomImage.ElementAt(randomIndex).SmallImage;

                    cardListRandomImage.RemoveAt(randomIndex);
                }
            }
        }

        //TODO: Need some adjustments to work properly.
        //private int[] GenerateRandomNumbers(int size, int minimum, int maximum, int sum)
        //{
        //    if (size <= 1)
        //    {
        //        return new int[1] { sum };
        //    }

        //    var numberArray = new int[size];

        //    for (int i = 0; i < numberArray.Length; i++)
        //    {
        //        var nextIndex = i + 1;

        //        if (nextIndex == numberArray.Length)
        //        {
        //            numberArray[i] = sum;
        //        }
        //        else
        //        {
        //            int rest = numberArray.Length - nextIndex;

        //            int restLowerBound = minimum * rest;
        //            int restUpperBound = maximum * rest;

        //            int myLowerBound = Math.Max(minimum, sum - restUpperBound);
        //            int myUpperBound = Math.Min(maximum, sum - restLowerBound);

        //            if (myLowerBound > myUpperBound)
        //            {
        //                (myLowerBound, myUpperBound) = (myUpperBound, myLowerBound);
        //            }

        //            numberArray[i] = GetRandomNumber(myLowerBound, myUpperBound);

        //            sum -= numberArray[i];
        //        }
        //    }

        //    return numberArray;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="randomSize"></param>
        /// <returns></returns>
        private int GetMaximum(int randomSize)
        {
            var result = 0;

            if (randomSize >= 11 && randomSize <= 19)
            {
                result = 126;
            }
            else if (randomSize >= 20 && randomSize <= 60)
            {
                result = 112;
            }
            else if (randomSize >= 61 && randomSize <= 120)
            {
                result = 96;
            }
            else if (randomSize >= 121 && randomSize <= 160)
            {
                result = 64;
            }
            else if (randomSize >= 161 && randomSize <= 200)
            {
                result = 48;
            }
            else
            {
                result = Static.MaxRateDrop / randomSize;

                if (result <= 0)
                {
                    result = 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minDropRate"></param>
        /// <param name="maxDropRate"></param>
        public void RandomizeCardDrops(int minDropRate = 1, int maxDropRate = 1)
        {
            if (minDropRate <= 0)
            {
                minDropRate = 1;
            }

            if (maxDropRate <= 0)
            {
                maxDropRate = 1;
            }

            if (minDropRate > Static.MaxRateDrop)
            {
                minDropRate = Static.MaxRateDrop;
            }

            if (maxDropRate > Static.MaxRateDrop)
            {
                maxDropRate = Static.MaxRateDrop;
            }

            if (minDropRate > maxDropRate)
            {
                (minDropRate, maxDropRate) = (maxDropRate, minDropRate);
            }

            if (Static.RandomCardDrops)
            {
                var allCardsId = Static.Cards
                    .Select(y => y.Id)
                    .OrderBy(z => _random.Next())
                    .ToList();

                foreach (Duelist t1 in Static.Duelist)
                {
                    // Limpamos a lista antiga.
                    Array.Clear(t1.Drop.BcdPow, 0, Static.MaxCards);
                    Array.Clear(t1.Drop.SaPow, 0, Static.MaxCards);
                    Array.Clear(t1.Drop.SaTec, 0, Static.MaxCards);

                    // BCD - POW/TEC
                    var randomSize = GetRandomNumber(20, 160);

                    for (int i = 0; i < randomSize; i++)
                    {
                        var rateDrop = GetRandomNumber(minDropRate, maxDropRate);

                        if (allCardsId.Count > 0)
                        {
                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                        else
                        {
                            allCardsId = Static.Cards
                                .Select(y => y.Id)
                                .OrderBy(z => _random.Next())
                                .ToList();

                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                    }

                    //if (randomSize * maxDropRate < Static.MaxRateDrop)
                    //{
                    //    maxDropRate = GetMaximum(randomSize);
                    //}

                    //var randomNumbersArray = GenerateRandomNumbers(randomSize, minDropRate, maxDropRate, Static.MaxRateDrop);

                    //foreach (var randomNumber in randomNumbersArray)
                    //{
                    //    if (allCardsId.Count > 0)
                    //    {
                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //    else
                    //    {
                    //        allCardsId = Static.Cards
                    //            .Select(y => y.Id)
                    //            .OrderBy(z => _random.Next())
                    //            .ToList();

                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.BcdPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //}

                    // SA - POW
                    randomSize = GetRandomNumber(20, 160);

                    for (int i = 0; i < randomSize; i++)
                    {
                        var rateDrop = GetRandomNumber(minDropRate, maxDropRate);

                        if (allCardsId.Count > 0)
                        {
                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                        else
                        {
                            allCardsId = Static.Cards
                                .Select(y => y.Id)
                                .OrderBy(z => _random.Next())
                                .ToList();

                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                    }

                    //if (randomSize * maxDropRate < Static.MaxRateDrop)
                    //{
                    //    maxDropRate = GetMaximum(randomSize);
                    //}

                    //randomNumbersArray = GenerateRandomNumbers(randomSize, minDropRate, maxDropRate, Static.MaxRateDrop);

                    //foreach (var randomNumber in randomNumbersArray)
                    //{
                    //    if (allCardsId.Count > 0)
                    //    {
                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //    else
                    //    {
                    //        allCardsId = Static.Cards
                    //            .Select(y => y.Id)
                    //            .OrderBy(z => _random.Next())
                    //            .ToList();

                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.SaPow[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //}

                    // SA - TEC
                    randomSize = GetRandomNumber(20, 160);

                    for (int i = 0; i < randomSize; i++)
                    {
                        var rateDrop = GetRandomNumber(minDropRate, maxDropRate);

                        if (allCardsId.Count > 0)
                        {
                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                        else
                        {
                            allCardsId = Static.Cards
                                .Select(y => y.Id)
                                .OrderBy(z => _random.Next())
                                .ToList();

                            int randomIndex = GetRandomNumber(0, allCardsId.Count());

                            t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += rateDrop;

                            allCardsId.RemoveAt(randomIndex);
                        }
                    }

                    //if (randomSize * maxDropRate < Static.MaxRateDrop)
                    //{
                    //    maxDropRate = GetMaximum(randomSize);
                    //}

                    //randomNumbersArray = GenerateRandomNumbers(randomSize, minDropRate, maxDropRate, Static.MaxRateDrop);

                    //foreach (var randomNumber in randomNumbersArray)
                    //{
                    //    if (allCardsId.Count > 0)
                    //    {
                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //    else
                    //    {
                    //        allCardsId = Static.Cards
                    //            .Select(y => y.Id)
                    //            .OrderBy(z => _random.Next())
                    //            .ToList();

                    //        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //        t1.Drop.SaTec[allCardsId.ElementAt(randomIndex) - 1] += randomNumber;

                    //        allCardsId.RemoveAt(randomIndex);
                    //    }
                    //}
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeDuelistDecks()
        {
            if (Static.RandomDecks)
            {
                foreach (Duelist t1 in Static.Duelist)
                {
                    var randomSize = GetRandomNumber(20, 200);
                    var minimumRandom = 1;
                    var maximumRandom = GetMaximum(randomSize);
                    //var randomNumbersArray = GenerateRandomNumbers(randomSize, minimumRandom, maximumRandom, Static.MaxRateDrop);

                    // Limpamos a lista antiga.
                    Array.Clear(t1.Deck, 0, Static.MaxCards);

                    // Buscamos todas as cartas, exceto cartas de tipo ritual
                    // e aplicamos o filtro de ATK/DEF caso exista e seja cartas de monstros.
                    var allCardsId = Static.Cards
                        .Where(x => x.Type != (int)Static.Type.Ritual
                            && ((Static.FilterDuelistDeckCards.EnableAtkDef == true
                                && x.Attack >= Static.FilterDuelistDeckCards.MinimumAttack
                                && x.Attack <= Static.FilterDuelistDeckCards.MaximumAttack
                                && x.Defense >= Static.FilterDuelistDeckCards.MinimumDefense
                                && x.Defense <= Static.FilterDuelistDeckCards.MaximumDefense
                                && IsMonsterCard(x.Type) == true)
                                || Static.FilterDuelistDeckCards.EnableAtkDef == false
                                || IsMonsterCard(x.Type) == false))
                        .Select(y => y.Id)
                        .OrderBy(z => _random.Next())
                        .ToList();

                    var allBannedCards = Static.FilterDuelistDeckCards.BannedCards
                        .Select(x => TypeDescriptor.GetProperties(x)["Id"].GetValue(x) as int? ?? 0).ToList();

                    allCardsId = allCardsId
                        .Where(x => allBannedCards.Contains(x) == false)
                        .ToList();

                    for (int i = 0; i < randomSize; i++)
                    {
                        int randomIndex = GetRandomNumber(0, allCardsId.Count());

                        t1.Deck[allCardsId.ElementAt(randomIndex) - 1] = GetRandomNumber(minimumRandom, maximumRandom);

                        allCardsId.RemoveAt(randomIndex);
                    }

                    //foreach (var randomNumber in randomNumbersArray)
                    //{
                    //    int randomIndex = GetRandomNumber(0, allCardsId.Count());

                    //    t1.Deck[allCardsId.ElementAt(randomIndex) - 1] = randomNumber;

                    //    allCardsId.RemoveAt(randomIndex);
                    //}
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RandomizeStarterDecks()
        {
            if (Static.RandomStarterDecks)
            {
                var allBannedCards = Static.FilterStarterDeckCards.BannedCards
                    .Select(x => TypeDescriptor.GetProperties(x)["Id"].GetValue(x) as int? ?? 0).ToList();

                // Static.StarterDeck[4] = Magics (Dark Hole and Raigeki)
                // Static.StarterDeck[5] = Magics (Forest, Wasteland, Mountain, Sogen, Umi and Yami)
                // Static.StarterDeck[6] = Magics (Equips)

                // Randomize equips. (Existem no máximo 34 equipamentos.)
                var randomSize = GetRandomNumber(20, 34);
                var minimumRandom = 1;
                var maximumRandom = GetMaximum(randomSize);
                //var randomNumbersArray = GenerateRandomNumbers(randomSize, minimumRandom, maximumRandom, Static.MaxRateDrop);

                // Aumentamos o número de cartas equipamentos obtidas.
                Static.StarterDeck[6].Dropped++;

                // Limpamos a lista antiga.
                Array.Clear(Static.StarterDeck[6].Cards, 0, Static.MaxCards);

                // Buscamos todos os equipamentos existentes.
                var allEquipsId = Static.Cards
                    .Where(x => x.Type == (int)Static.Type.Equip)
                    .Select(y => y.Id)
                    .OrderBy(z => _random.Next())
                    .ToList();

                allEquipsId = allEquipsId
                    .Where(x => allBannedCards.Contains(x) == false)
                    .ToList();

                for (int i = 0; i < randomSize; i++)
                {
                    int randomIndex = GetRandomNumber(0, allEquipsId.Count());

                    Static.StarterDeck[6].Cards[allEquipsId.ElementAt(randomIndex) - 1] = GetRandomNumber(minimumRandom, maximumRandom);

                    allEquipsId.RemoveAt(randomIndex);
                }

                //foreach (var randomNumber in randomNumbersArray)
                //{
                //    int randomIndex = GetRandomNumber(0, allEquipsId.Count());

                //    Static.StarterDeck[6].Cards[allEquipsId.ElementAt(randomIndex) - 1] = randomNumber;

                //    allEquipsId.RemoveAt(randomIndex);
                //}

                // Randomize magics. (Existem no máximo 33 mágicas.)
                randomSize = GetRandomNumber(20, 33);
                minimumRandom = 1;
                maximumRandom = GetMaximum(randomSize);
                //randomNumbersArray = GenerateRandomNumbers(randomSize, minimumRandom, maximumRandom, Static.MaxRateDrop);

                // Aumentamos o número de cartas mágicas obtidas.
                Static.StarterDeck[5].Dropped++;

                // Limpamos a lista antiga.
                Array.Clear(Static.StarterDeck[5].Cards, 0, Static.MaxCards);

                // Buscamos todas as mágicas existentes.
                var allMagicsId = Static.Cards
                    .Where(x => x.Type == (int)Static.Type.Magic)
                    .Select(y => y.Id)
                    .OrderBy(z => _random.Next())
                    .ToList();

                allMagicsId = allMagicsId
                    .Where(x => allBannedCards.Contains(x) == false)
                    .ToList();

                for (int i = 0; i < randomSize; i++)
                {
                    int randomIndex = GetRandomNumber(0, allMagicsId.Count());

                    Static.StarterDeck[5].Cards[allMagicsId.ElementAt(randomIndex) - 1] = GetRandomNumber(minimumRandom, maximumRandom);

                    allMagicsId.RemoveAt(randomIndex);
                }

                //foreach (var randomNumber in randomNumbersArray)
                //{
                //    int randomIndex = GetRandomNumber(0, allMagicsId.Count());

                //    Static.StarterDeck[5].Cards[allMagicsId.ElementAt(randomIndex) - 1] = randomNumber;

                //    allMagicsId.RemoveAt(randomIndex);
                //}

                // Randomize traps. (Existem no máximo 10 traps.)
                randomSize = GetRandomNumber(1, 10);
                minimumRandom = 1;
                maximumRandom = GetMaximum(randomSize);
                //randomNumbersArray = GenerateRandomNumbers(randomSize, minimumRandom, maximumRandom, Static.MaxRateDrop);

                // Limpamos a lista antiga.
                Array.Clear(Static.StarterDeck[4].Cards, 0, Static.MaxCards);

                // Buscamos todas as armadilhas existentes.
                var allTrapsId = Static.Cards
                    .Where(x => x.Type == (int)Static.Type.Trap)
                    .Select(y => y.Id)
                    .OrderBy(z => _random.Next())
                    .ToList();

                allTrapsId = allTrapsId
                    .Where(x => allBannedCards.Contains(x) == false)
                    .ToList();

                for (int i = 0; i < randomSize; i++)
                {
                    int randomIndex = GetRandomNumber(0, allTrapsId.Count());

                    Static.StarterDeck[4].Cards[allTrapsId.ElementAt(randomIndex) - 1] = GetRandomNumber(minimumRandom, maximumRandom);

                    allTrapsId.RemoveAt(randomIndex);
                }

                //foreach (var randomNumber in randomNumbersArray)
                //{
                //    int randomIndex = GetRandomNumber(0, allTrapsId.Count());

                //    Static.StarterDeck[4].Cards[allTrapsId.ElementAt(randomIndex) - 1] = randomNumber;

                //    allTrapsId.RemoveAt(randomIndex);
                //}

                // Randomize monsters.
                // Iremos diminuir a quantidade de monstros do primeiro e segundo 'set' de cartas (16 -> 15)
                // para aumentar a quantidade de mágicas e equipamentos que podem ser obtidas. (1 -> 2)
                Static.StarterDeck[0].Dropped--;
                Static.StarterDeck[1].Dropped--;

                for (int i = 0; i < Static.MaxStarterDeck - 3; i++)
                {
                    randomSize = GetRandomNumber(60, 120);
                    minimumRandom = 1;
                    maximumRandom = GetMaximum(randomSize);
                    //randomNumbersArray = GenerateRandomNumbers(randomSize, minimumRandom, maximumRandom, Static.MaxRateDrop);

                    // Limpamos a lista antiga.
                    Array.Clear(Static.StarterDeck[i].Cards, 0, Static.MaxCards);

                    // Buscamos todos os monstros existentes
                    // e aplicamos os filtros de ATK/DEF caso existam.
                    var allMonstersId = Static.Cards
                        .Where(x => IsMonsterCard(x.Type)
                            && x.Id != 722 /* Magician of Black Chaos*/
                            && ((Static.FilterStarterDeckCards.EnableAtkDef == true
                                && x.Attack >= Static.FilterStarterDeckCards.MinimumAttack
                                && x.Attack <= Static.FilterStarterDeckCards.MaximumAttack
                                && x.Defense >= Static.FilterStarterDeckCards.MinimumDefense
                                && x.Defense <= Static.FilterStarterDeckCards.MaximumDefense)
                                || Static.FilterStarterDeckCards.EnableAtkDef == false))
                        .Select(y => y.Id)
                        .OrderBy(z => _random.Next())
                        .ToList();

                    allMonstersId = allMonstersId
                        .Where(x => allBannedCards.Contains(x) == false)
                        .ToList();

                    for (int j = 0; j < randomSize; j++)
                    {
                        int randomIndex = GetRandomNumber(0, allMonstersId.Count());

                        Static.StarterDeck[i].Cards[allMonstersId.ElementAt(randomIndex) - 1] = GetRandomNumber(minimumRandom, maximumRandom);

                        allMonstersId.RemoveAt(randomIndex);
                    }

                    //foreach (var randomNumber in randomNumbersArray)
                    //{
                    //    int randomIndex = GetRandomNumber(0, allMonstersId.Count());

                    //    Static.StarterDeck[i].Cards[allMonstersId.ElementAt(randomIndex) - 1] = randomNumber;

                    //    allMonstersId.RemoveAt(randomIndex);
                    //}
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteChangesToFile()
        {
            using (var fileStream = new FileStream(Static.WaPath, FileMode.Open))
            {
                // Writing Random Fusions
                if (Static.RandomFusions)
                {
                    int[] numArray = {
                            0xB87800,
                            0xBFD000,
                            0xC72800,
                            0xCE8000,
                            0xD5D800,
                            0xDD3000,
                            0xE48800
                        };

                    var memStream1 = new MemoryStream(1444);
                    var memStream2 = new MemoryStream(64092);

                    memStream1.Position = 2L;
                    memStream2.Position = 2L;

                    foreach (Card card in Static.Cards)
                    {
                        short num1 = card.Fusions.Count != 0 ? (short)(memStream2.Position + 1444L) : (short)0;

                        memStream1.Write(num1.Int16ToByteArray(), 0, 2);

                        if (card.Fusions.Count != 0)
                        {
                            if (card.Fusions.Count < 256)
                            {
                                memStream2.WriteByte((byte)card.Fusions.Count);
                            }
                            else
                            {
                                memStream2.WriteByte(0);
                                memStream2.WriteByte((byte)Math.Abs(card.Fusions.Count - 511));
                            }

                            for (int i = 0; i < card.Fusions.Count; ++i)
                            {
                                int num2 = card.Fusions[i].Cards2 + 1 & byte.MaxValue;
                                int num3 = card.Fusions[i].Result + 1 & byte.MaxValue;
                                int num4 = 0;
                                int num5 = 0;
                                int num6 = card.Fusions[i].Cards2 + 1 >> 8 & 3 | (card.Fusions[i].Result + 1 >> 8 & 3) << 2;

                                if (i < card.Fusions.Count - 1)
                                {
                                    num4 = card.Fusions[i + 1].Cards2 + 1 & byte.MaxValue;
                                    num5 = card.Fusions[i + 1].Result + 1 & byte.MaxValue;
                                    num6 |= (card.Fusions[i + 1].Cards2 + 1 >> 8 & 3) << 4 |
                                            (card.Fusions[i + 1].Result + 1 >> 8 & 3) << 6;
                                    ++i;
                                }

                                memStream2.WriteByte((byte)(num6 & byte.MaxValue));
                                memStream2.WriteByte((byte)(num2 & byte.MaxValue));
                                memStream2.WriteByte((byte)(num3 & byte.MaxValue));

                                if (num4 != 0 || num5 != 0)
                                {
                                    memStream2.WriteByte((byte)(num4 & byte.MaxValue));
                                    memStream2.WriteByte((byte)(num5 & byte.MaxValue));
                                }
                            }
                        }
                    }

                    while (memStream2.Position < 64092L)
                    {
                        memStream2.WriteByte(0xEE);
                    }

                    foreach (int num in numArray)
                    {
                        fileStream.Position = num;

                        var mem_arr1 = memStream1.ToArray();
                        var mem_arr2 = memStream2.ToArray();

                        fileStream.Write(mem_arr1, 0, mem_arr1.Length);
                        fileStream.Write(mem_arr2, 0, mem_arr2.Length);
                    }

                    // Close memorystream after use
                    memStream2.Close();
                    memStream1.Close();
                }

                // Write Card Images
                if (Static.RandomCardImages
                    || Static.RandomNames)
                {
                    for (int i = 0; i < Static.MaxCards; i++)
                    {
                        fileStream.Position = 0x169000 + (0x3800 * i);
                        fileStream.Write(Static.Cards[i].BigImage.getData(), 0, Static.Cards[i].BigImage.getData().Length);

                        fileStream.Position = i * 0x800;
                        fileStream.Write(Static.Cards[i].SmallImage.getData(), 0, Static.Cards[i].SmallImage.getData().Length);
                        fileStream.Position = 0x16BAE0 + (0x3800 * i);
                        fileStream.Write(Static.Cards[i].SmallImage.getData(), 0, Static.Cards[i].SmallImage.getData().Length);

                        fileStream.Position = 0x16B840 + (0x3800 * i);
                        fileStream.Write(Static.Cards[i].NameImage.getData(), 0, Static.Cards[i].NameImage.getData().Length);
                    }
                }
            }

            // Randomize ATK/DEF, Guardian Stars, Types, Attributes, Names and Descriptions
            if (Static.RandomAtkdef
                || Static.RandomGuardianStars
                || Static.RandomMonstersTypes
                || Static.RandomAttributes
                || Static.RandomNames
                || Static.RandomDescriptions)
            {
                using (var fileStreamSl = new FileStream(Static.SlusPath, FileMode.Open))
                {
                    fileStreamSl.Position = 0x1C4A44L;

                    // Write ATK/DEF, Guardian Stars and Types
                    using (var memoryStream = new MemoryStream(2888))
                    {
                        for (int i = 0; i < Static.MaxCards; ++i)
                        {
                            int value = (Static.Cards[i].Attack / 10 & 0x1FF) | (Static.Cards[i].Defense / 10 & 0x1FF) << 9 |
                                        (Static.Cards[i].GuardianStar2 & 0xF) << 18 |
                                        (Static.Cards[i].GuardianStar1 & 0xF) << 22 | (Static.Cards[i].Type & 0x1F) << 26;

                            memoryStream.Write(value.Int32ToByteArray(), 0, 4);
                        }

                        var arr = memoryStream.ToArray();
                        fileStreamSl.Write(arr, 0, arr.Length);
                    }

                    // Write Cards level and attribute
                    fileStreamSl.Position = 0x1C5B33L;

                    using (var memoryStream = new MemoryStream(Static.MaxCards))
                    {
                        for (int i = 0; i < Static.MaxCards; i++)
                        {
                            var buffer = new byte[]
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
                            for (int i = 0; i < Static.MaxCards; i++)
                            {
                                var bufferColor = new byte[]
                                {
                                    0xF8,
                                    10,
                                    (byte)Static.Cards[i].DescriptionColor
                                };

                                memoryStreamColor.Write(((short)(memoryStreamText.Position + 0x9f4L)).Int16ToByteArray(), 0, 2);

                                if (Static.Cards[i].DescriptionColor != 0)
                                {
                                    memoryStreamText.Write(bufferColor, 0, 3);
                                }

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
                        int lastIndexStop = 0;
                        bool overBufferSize = false;

                        using (var memoryStreamText = new MemoryStream(0x2BFD))
                        {
                            for (int i = 0; i < Static.MaxCards; i++)
                            {
                                var bufferColor = new byte[]
                                {
                                    0xF8,
                                    10,
                                    (byte)Static.Cards[i].NameColor
                                };

                                var bufferText = Static.Cards[i].Name.TextToArray(Static.RDict);

                                if ((memoryStreamText.Position + bufferText.Length) > memoryStreamText.Capacity)
                                {
                                    overBufferSize = true;
                                    lastIndexStop = i;

                                    while (memoryStreamText.Position < memoryStreamText.Capacity)
                                    {
                                        memoryStreamText.WriteByte(0);
                                    }

                                    break;
                                }

                                memoryStream.Write(((short)((memoryStreamText.Position + 0x6000L) - 0x130L)).Int16ToByteArray(), 0, 2);

                                if (Static.Cards[i].NameColor != 0)
                                {
                                    memoryStreamText.Write(bufferColor, 0, 3);
                                }

                                memoryStreamText.Write(bufferText, 0, bufferText.Length);
                            }

                            fileStreamSl.Position = 0x1C66D0L;
                            fileStreamSl.Write(memoryStreamText.ToArray(), 0, (int)memoryStreamText.Length);
                        }

                        if (overBufferSize == true)
                        {
                            using (var memoryStreamText = new MemoryStream())
                            {
                                for (int i = lastIndexStop; i < Static.MaxCards; i++)
                                {
                                    var bufferColor = new byte[]
                                    {
                                        0xF8,
                                        10,
                                        (byte)Static.Cards[i].NameColor
                                    };

                                    memoryStream.Write(((short)((memoryStreamText.Position + 0x6000L) + 0x3200L)).Int16ToByteArray(), 0, 2);

                                    var bufferText = Static.Cards[i].Name.TextToArray(Static.RDict);

                                    if (Static.Cards[i].NameColor != 0)
                                    {
                                        memoryStreamText.Write(bufferColor, 0, 3);
                                    }

                                    memoryStreamText.Write(bufferText, 0, bufferText.Length);
                                }

                                fileStreamSl.Position = 0x1C9A00L;
                                fileStreamSl.Write(memoryStreamText.ToArray(), 0, (int)memoryStreamText.Length);
                            }
                        }

                        fileStreamSl.Position = 0x1C6002L;
                        fileStreamSl.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    }

                    // Write Star Chips per Duel
                    if (Static.StarChipsDuel >= Static.MinStarChipsDuel)
                    {
                        fileStreamSl.Position = 0x126DCL;
                        bool changeStarChip = Static.StarChipsDuel > Static.MinStarChipsDuel;

                        Static.StarChipsDuel -= Static.MinStarChipsDuel;

                        var tempArray = Static.StarChipsDuel.Int16ToByteArray();
                        var starChipArray = new byte[] { tempArray[0], tempArray[1], (byte)(changeStarChip ? 0x63 : 0x00), (byte)(changeStarChip ? 0x24 : 0x00) };

                        fileStreamSl.Write(starChipArray, 0, starChipArray.Length);
                    }
                }
            }

            // Randomize Starter Decks
            if (Static.RandomStarterDecks)
            {
                using (var starterDeckStream = new FileStream(Static.WaPath, FileMode.Open))
                {
                    for (int i = 0; i < Static.MaxStarterDeck; i++)
                    {
                        starterDeckStream.Position = 0xF92BD4 + (0x5B8 * i);

                        using (var memoryStream = new MemoryStream(0x5A6))
                        {
                            var droppedUInt16 = (ushort)Static.StarterDeck[i].Dropped;

                            memoryStream.Write(droppedUInt16.Int16ToByteArray(), 0, 2);

                            foreach (var cardIdUInt16 in Static.StarterDeck[i].Cards.Select(s => (ushort)s))
                            {
                                memoryStream.Write(cardIdUInt16.Int16ToByteArray(), 0, 2);
                            }

                            starterDeckStream.Write(memoryStream.ToArray(), 0, 0x5A6);
                        }
                    }
                }
            }

            // Randomize Decks and Card Drops
            if (Static.RandomDecks || Static.RandomCardDrops)
            {
                using (var duelistStream = new FileStream(Static.WaPath, FileMode.Open))
                {
                    for (int i = 0; i < Static.MaxDuelists; i++)
                    {
                        int num = 0xE9B000 + 0x1800 * i;

                        // Randomize Decks
                        if (Static.RandomDecks)
                        {
                            duelistStream.Position = num;

                            using (var memoryStream = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Deck;

                                foreach (int t in array)
                                {
                                    short value = (short)t;
                                    memoryStream.Write(value.Int16ToByteArray(), 0, 2);
                                }

                                var arr = memoryStream.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                        }

                        // Randomize Card Drops
                        if (Static.RandomCardDrops)
                        {
                            duelistStream.Position = num + 0x5B4;

                            using (var memoryStream2 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.SaPow;

                                foreach (int t in array)
                                {
                                    short value2 = (short)t;
                                    memoryStream2.Write(value2.Int16ToByteArray(), 0, 2);
                                }

                                var arr = memoryStream2.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }

                            duelistStream.Position = num + 0xB68;

                            using (var memoryStream3 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.BcdPow;

                                foreach (int t in array)
                                {
                                    short value3 = (short)t;
                                    memoryStream3.Write(value3.Int16ToByteArray(), 0, 2);
                                }

                                var arr = memoryStream3.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }

                            duelistStream.Position = num + 0x111C;

                            using (var memoryStream4 = new MemoryStream(1444))
                            {
                                int[] array = Static.Duelist[i].Drop.SaTec;

                                foreach (int t in array)
                                {
                                    short value4 = (short)t;
                                    memoryStream4.Write(value4.Int16ToByteArray(), 0, 2);
                                }

                                var arr = memoryStream4.ToArray();
                                duelistStream.Write(arr, 0, arr.Length);
                            }
                        }
                    }
                }
            }

            if (Static.RandomStarchips)
            {
                using (var starchipStream = new FileStream(Static.WaPath, FileMode.Open))
                {
                    starchipStream.Position = 0xFB9808;

                    for (var i = 0; i < Static.MaxCards; ++i)
                    {
                        var cost_arr = Static.Cards[i].Starchip.Cost.Int32ToByteArray();
                        var pass_arr = Static.Cards[i].Starchip.PasswordStr.StringToByteArray();
                        var offset = 0;

                        for (var j = cost_arr.Length - 2; j >= 0; --j)
                        {
                            if (cost_arr[j] == 0)
                            {
                                offset++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        for (var j = 0; j < cost_arr.Length - offset - 1; ++j)
                        {
                            starchipStream.WriteByte(cost_arr[j]);
                        }

                        for (var j = 0; j < offset; ++j)
                        {
                            starchipStream.WriteByte(0);
                        }

                        // Advance over unused byte
                        starchipStream.Position += 1;

                        for (var j = pass_arr.Length - 1; j >= 0; --j)
                        {
                            starchipStream.WriteByte(pass_arr[j]);
                        }
                    }
                }
            }
        }

        // TODO: Better Log File Logic + HTML/JSON/XML Format
        /// <summary>
        /// 
        /// </summary>
        public void WriteFusionSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\fusions_spoiler_#{_seed}.log"))
            {
                File.CreateText($"{output_dir}\\fusions_spoiler_#{_seed}.log").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\fusions_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            foreach (Card c in Static.Cards)
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"=> #{c.Id} {c.Name} ({c.Attack}/{c.Defense})");
                logStream.WriteLine("=> FUSIONS:");

                foreach (Fusion fus in c.Fusions)
                {
                    logStream.WriteLine($"    => {fus.Cards1} + {fus.Cards2} = {fus.Result}         ({(fus.Cards1 > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards1).Name)} + {(fus.Cards2 > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards2).Name)} = {(fus.Result > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Result).Name)})");
                }
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteStarchipSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\starchip_spoiler_#{_seed}.log"))
            {
                File.CreateText($"{output_dir}\\starchip_spoiler_#{_seed}.log").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\starchip_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Starchip Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            foreach (Card c in Static.Cards)
            {
                logStream.WriteLine($"    => #{c.Id} {c.Name}");
                logStream.WriteLine($"        Cost: {c.Starchip.Cost} Password: {c.Starchip.PasswordStr}");
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteDropsSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\drops_spoiler_#{_seed}.log"))
            {
                File.CreateText($"{output_dir}\\drops_spoiler_#{_seed}.log").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\drops_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Drops Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            // Get drop map as well as sort by drop rate
            Dictionary<Duelist, List<KeyValuePair<Card, int>>> get_drop_map(DropType dropType)
            {
                var drop_map = new Dictionary<Duelist, List<KeyValuePair<Card, int>>>();

                foreach (Duelist d in Static.Duelist)
                {
                    var sorted_map = new List<KeyValuePair<Card, int>>();

                    foreach (Card c in Static.Cards)
                    {
                        if (d.Drop.GetDropArray(dropType)[c.Id - 1] > 0)
                        {
                            sorted_map.Add(new KeyValuePair<Card, int>(c, d.Drop.GetDropArray(dropType)[c.Id - 1]));
                        }
                    }

                    sorted_map.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    drop_map.Add(d, sorted_map);
                }

                return drop_map;
            }

            var sap_d_map = get_drop_map(DropType.SAPOW);
            var bcd_d_map = get_drop_map(DropType.BCDPOW);
            var sat_d_map = get_drop_map(DropType.SATEC);

            foreach (Duelist d in Static.Duelist)
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} S/A-Tec drops");
                logStream.WriteLine($"Possibilities: {d.Drop.SaTec.Sum(x => x > 0 ? 1 : 0)}");
                logStream.WriteLine($"Total Rate: {d.Drop.SaTec.Sum()}");

                var drop_map = sat_d_map[d];

                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} B/C/D drops");
                logStream.WriteLine($"Possibilities: {d.Drop.BcdPow.Sum(x => x > 0 ? 1 : 0)}");
                logStream.WriteLine($"Total Rate: {d.Drop.BcdPow.Sum()}");

                drop_map = bcd_d_map[d];

                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} S/A-Pow drops");
                logStream.WriteLine($"Possibilities: {d.Drop.SaPow.Sum(x => x > 0 ? 1 : 0)}");
                logStream.WriteLine($"Total Rate: {d.Drop.SaPow.Sum()}");

                drop_map = sap_d_map[d];

                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
                }

                logStream.WriteLine();
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteStarterDecksSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\starter_decks_spoiler_#{_seed}.log"))
            {
                File.CreateText($"{output_dir}\\starter_decks_spoiler_#{_seed}.log").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\starter_decks_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Starter Decks Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            // Get drop map as well as sort by drop rate
            Dictionary<StarterDeck, List<KeyValuePair<Card, int>>> get_starter_deck_map()
            {
                var drop_map = new Dictionary<StarterDeck, List<KeyValuePair<Card, int>>>();

                foreach (StarterDeck d in Static.StarterDeck)
                {
                    var sorted_map = new List<KeyValuePair<Card, int>>();

                    foreach (Card c in Static.Cards)
                    {
                        if (d.Cards[c.Id - 1] > 0)
                        {
                            sorted_map.Add(new KeyValuePair<Card, int>(c, d.Cards[c.Id - 1]));
                        }
                    }

                    sorted_map.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    drop_map.Add(d, sorted_map);
                }

                return drop_map;
            }

            var starter_deck_map = get_starter_deck_map();

            foreach (var (d, i) in Static.StarterDeck.Select((d, i) => (d, i)))
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"Starter Deck Part: {i + 1}");
                logStream.WriteLine($"Possibilities: {d.Cards.Sum(x => x > 0 ? 1 : 0)}");
                logStream.WriteLine($"Total Rate: {d.Cards.Sum()}");
                logStream.WriteLine($"Total Get Cards: {d.Dropped}");

                var drop_map = starter_deck_map[d];

                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine();
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteDuelistDecksSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\duelist_decks_spoiler_#{_seed}.log"))
            {
                File.CreateText($"{output_dir}\\duelist_decks_spoiler_#{_seed}.log").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\duelist_decks_spoiler_#{_seed}.log");

            logStream.WriteLine("== YU-GI-OH! Forbidden Memories Duelist Decks Scrambler Spoiler File ==");
            logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
            logStream.WriteLine("====================================================================\r\n");

            // Get duelist map as well as sort by drop rate
            Dictionary<Duelist, List<KeyValuePair<Card, int>>> get_duelist_deck_map()
            {
                var duelist_map = new Dictionary<Duelist, List<KeyValuePair<Card, int>>>();

                foreach (Duelist d in Static.Duelist)
                {
                    var sorted_map = new List<KeyValuePair<Card, int>>();

                    foreach (Card c in Static.Cards)
                    {
                        if (d.Deck[c.Id - 1] > 0)
                        {
                            sorted_map.Add(new KeyValuePair<Card, int>(c, d.Deck[c.Id - 1]));
                        }
                    }

                    sorted_map.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    duelist_map.Add(d, sorted_map);
                }

                return duelist_map;
            }

            var duelist_deck_map = get_duelist_deck_map();

            foreach (Duelist d in Static.Duelist)
            {
                logStream.WriteLine("====================================================================");
                logStream.WriteLine($"{d.Name} Deck");
                logStream.WriteLine($"Possibilities: {d.Deck.Sum(x => x > 0 ? 1 : 0)}");
                logStream.WriteLine($"Total Rate: {d.Deck.Sum()}");

                var drop_map = duelist_deck_map[d];

                foreach (var p in drop_map)
                {
                    logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                    logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
                }

                logStream.WriteLine();
                logStream.WriteLine("====================================================================");
                logStream.WriteLine();
            }

            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteHtmlFusionSpoilerFile()
        {
            var output_dir = Static.WaPath.Substring(0, Static.WaPath.LastIndexOf('\\'));

            if (!File.Exists($@"{output_dir}\\fusions_spoiler_#{_seed}.html"))
            {
                File.CreateText($"{output_dir}\\fusions_spoiler_#{_seed}.html").Close();
            }

            var logStream = new StreamWriter($@"{output_dir}\\fusions_spoiler_#{_seed}.html");

            string template =
                $@"<!DOCTYPE html>
                <html lang=\""en\""> 
                <head>
                <meta charset=\""UTF-8\"">
                <meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">
                <meta http-equiv=\""X-UA-Compatible\"" content=\""ie=edge\"">
                <title>{_seed}</title>
                <style>
                    html, body {{ width: 100%; height: auto; margin: 0 auto; }}
                    table {{ width: 90%; margin: 0 auto; border: 1px solid lightgrey; border-spacing: 0; }}
                    thead {{ background-color: #f2ffeb; padding: 2px; }}
                    thead th {{ padding: 3px; text-align: center; }}
                    table td {{ border-bottom: 1px solid lightgrey!important; padding: 2px!important; text-align: center; }}
                    table tr:last-child td {{ border-bottom: 0!important; }}
                    tbody tr td {{ border-right: 1px solid lightgrey; }}
                    tbody tr td:last-child {{ border-right: 0!important; }}
                </style>
                </head>
                <body>
                <div style=\""width: 100 %; text-align: center;\"">";

            string table2Template =
                @"<span style=\""text-align: center; font-size: 120%; font-weight: bold; margin-bottom: 5px;\"">Changed Fusions</span>
                  <table>
                    <thead>
                        <tr>
                    <th style=\""width: 5%;\"">ID 1</th>
                    <th style=\""width: 25%;\"">Name 1</th>
                    <th style=\""width: 5%;\"">ID 2</th>
                    <th style=\""width: 25%;\"">Name 2</th>
                    <th style=\""width: 5%;\"">ResultID</th>
                    <th style=\""width: 25%;\"">Result Name</th>
                </tr>
            </thead>
            <tbody>";

            string tmpFusions = "";

            logStream.WriteLine(template + $" <h1>YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File</h1> <h4>Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion}</h4>");

            foreach (Card c in Static.Cards)
            {
                foreach (Fusion fus in c.Fusions)
                {
                    tmpFusions += "<tr>";
                    tmpFusions += $"<td>{fus.Cards1}</td> <td>{(fus.Cards1 > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards1).Name)}</td> <td>{fus.Cards2}</td> <td>{(fus.Cards2 > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Cards2).Name)}</td> <td>{fus.Result}</td> <td>{(fus.Result > Static.MaxCards ? "Glitch!" : Static.Cards.Single(card => card.Id == fus.Result).Name)}</td></tr>";
                }
            }

            //logStream.WriteLine("</tbody></table><br />");
            logStream.WriteLine(table2Template);
            Console.WriteLine("Writing tmpFusions");
            logStream.WriteLine(tmpFusions);
            logStream.WriteLine("</tbody></table></div></body></html>");
            logStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minAtk"></param>
        /// <param name="maxAtk"></param>
        /// <param name="minDef"></param>
        /// <param name="maxDef"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        /// <param name="minDropRate"></param>
        /// <param name="maxDropRate"></param>
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
            int minDropRate = 1,
            int maxDropRate = 1,
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
            RandomizeCardDrops(minDropRate, maxDropRate);
            RandomizeStarterDecks();
            RandomizeDuelistDecks();

            if (starChipsDuel < 5)
            {
                starChipsDuel = 5;
            }

            if (starChipsDuel > 100)
            {
                starChipsDuel = 100;
            }

            Static.StarChipsDuel = starChipsDuel;

            WriteChangesToFile();
            EnableDropCount(dropCount);

            if (Static.Spoiler)
            {
                WriteFusionSpoilerFile();
                WriteStarchipSpoilerFile();
                WriteDropsSpoilerFile();
                WriteStarterDecksSpoilerFile();
                WriteDuelistDecksSpoilerFile();
                //WriteHtmlFusionSpoilerFile();
            }

            return true;
        }
    }
}
