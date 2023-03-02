using System.Collections.Generic;
using FMLib.Models;

namespace FMLib.Utility
{
    /// <summary>
    /// Static Variables for application wide useage
    /// </summary>
    public class Static
    {
        /// <summary>
        /// Options (true / false)
        /// </summary>

        public static bool RandomFusions = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomAttributes = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomGuardianStars = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomCardDrops = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomMonstersTypes = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomStarchips = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomDecks = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomEquips = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool HighId = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool RandomAtkdef = false;
        /// <summary>
        /// 
        /// </summary>
        public static bool Spoiler = true;

        /// <summary>
        /// 
        /// </summary>
        public static bool RandomNames = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool RandomDescriptions = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool RandomCardImages = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool RandomStarterDecks = false;

        /// <summary>
        /// Count of Cards as Integer
        /// </summary>
        public static int CardCount;

        /// <summary>
        /// 
        /// </summary>
        public static byte DropCount;

        /// <summary>
        /// 
        /// </summary>
        public static ushort StarChipsDuel;

        /// <summary>
        /// 
        /// </summary>
        public static readonly ushort MinStarChipsDuel = 5;

        /// <summary>
        /// Máximo de Cartas
        /// </summary>
        public static readonly int MaxCards = 722;

        /// <summary>
        /// Máximo de Duelistas
        /// </summary>
        public static readonly int MaxDuelists = 39;

        /// <summary>
        /// Taxa Máxima de Drop das Cartas
        /// </summary>
        public static readonly int MaxRateDrop = 2048;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int MaxStarterDeck = 7;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int MinMonstersEquips = 1;

        /// <summary>
        /// Card Array for all the 722 Cards in the game
        /// </summary>
        public static Card[] Cards = new Card[MaxCards];

        /// <summary>
        /// Array com todos os Duelistas
        /// </summary>
        public static Duelist[] Duelist = new Duelist[MaxDuelists];

        /// <summary>
        /// 
        /// </summary>
        public static StarterDeck[] StarterDeck = new StarterDeck[MaxStarterDeck];

        /// <summary>
        /// 
        /// </summary>
        public static FilterCards FilterStarterDeckCards = new FilterCards();

        /// <summary>
        /// 
        /// </summary>
        public static FilterCards FilterDuelistDeckCards = new FilterCards();
        
        /// <summary>
        /// Method to set the Card Count
        /// </summary>
        /// <param name="c">Card Count as Integer</param>
        public static void SetCardCount(int c)
        {
            CardCount = c;
        }

        /// <summary>
        /// Path to the Game Folder
        /// </summary>
        public static string GameBinPath = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public static string SlusPath;
        /// <summary>
        /// 
        /// </summary>
        public static string WaPath;
        /// <summary>
        /// 
        /// </summary>
        public static string IsoPath;

        /// <summary>
        /// 
        /// </summary>
        public static bool UsedIso = false;
        /// <summary>
        /// 
        /// </summary>
        public static string RandomizerFileName;

        /// <summary>
        /// Helper - Dictionary to map chars from data to readable chars
        /// </summary>
        public static Dictionary<byte, char> Dict = new Dictionary<byte, char>();

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<char, byte> RDict = new Dictionary<char, byte>();

        /// <summary>
        /// 
        /// </summary>
        public enum GuardianStar
        {
            /// <summary>
            /// Guardian Star - None
            /// </summary>
			None = 0,
            /// <summary>
            /// Guardian Star - Mars
            /// </summary>
            Mars = 1,
            /// <summary>
            /// Guardian Star - Jupiter
            /// </summary>
            Jupiter = 2,
            /// <summary>
            /// Guardian Star - Saturn
            /// </summary>
            Saturn = 3,
            /// <summary>
            /// Guardian Star - Uranus
            /// </summary>
            Uranus = 4,
            /// <summary>
            /// Guardian Star - Pluto
            /// </summary>
            Pluto = 5,
            /// <summary>
            /// Guardian Star - Neptune
            /// </summary>
            Neptune = 6,
            /// <summary>
            /// Guardian Star - Mercury
            /// </summary>
            Mercury = 7,
            /// <summary>
            /// Guardian Star - Sun
            /// </summary>
            Sun = 8,
            /// <summary>
            /// Guardian Star - Moon
            /// </summary>
            Moon = 9,
            /// <summary>
            /// Guardian Star - Venus
            /// </summary>
            Venus = 10
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Attribute
        {
            /// <summary>
            /// Attribute - Light
            /// </summary>
            Light = 0,
            /// <summary>
            /// Attribute - Dark
            /// </summary>
            Dark = 1,
            /// <summary>
            /// Attribute - Earth
            /// </summary>
            Earth = 2,
            /// <summary>
            /// Attribute - Water
            /// </summary>
            Water = 3,
            /// <summary>
            /// Attribute - Fire
            /// </summary>
            Fire = 4,
            /// <summary>
            /// Attribute - Wind
            /// </summary>
            Wind = 5,
            /// <summary>
            /// Attribute - Equips, Rituals and Magics
            /// </summary>
            Spell = 6,
            /// <summary>
            /// Attribute - Magics with Conditions and Traps
            /// </summary>
            Trap = 7
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Type
        {
            /// Monsters
            /// <summary>
            /// 
            /// </summary>
            Dragon = 0,
            /// <summary>
            /// 
            /// </summary>
            Spellcaster = 1,
            /// <summary>
            /// 
            /// </summary>
            Zombie = 2,
            /// <summary>
            /// 
            /// </summary>
            Warrior = 3,
            /// <summary>
            /// 
            /// </summary>
            Beast_Warrior = 4,
            /// <summary>
            /// 
            /// </summary>
            Beast = 5,
            /// <summary>
            /// 
            /// </summary>
            Winged_Beast = 6,
            /// <summary>
            /// 
            /// </summary>
            Fiend = 7,
            /// <summary>
            /// 
            /// </summary>
            Fairy = 8,
            /// <summary>
            /// 
            /// </summary>
            Insect = 9,
            /// <summary>
            /// 
            /// </summary>
            Dinosaur = 10,
            /// <summary>
            /// 
            /// </summary>
            Reptile = 11,
            /// <summary>
            /// 
            /// </summary>
            Fish = 12,
            /// <summary>
            /// 
            /// </summary>
            Sea_Serpent = 13,
            /// <summary>
            /// 
            /// </summary>
            Machine = 14,
            /// <summary>
            /// 
            /// </summary>
            Thunder = 15,
            /// <summary>
            /// 
            /// </summary>
            Aqua = 16,
            /// <summary>
            /// 
            /// </summary>
            Pyro = 17,
            /// <summary>
            /// 
            /// </summary>
            Rock = 18,
            /// <summary>
            /// 
            /// </summary>
            Plant = 19,
            /// <summary>
            /// 
            /// </summary>
            Magic = 20,
            /// <summary>
            /// 
            /// </summary>
            Trap = 21,
            /// <summary>
            /// 
            /// </summary>
            Ritual = 22,
            /// <summary>
            /// 
            /// </summary>
            Equip = 23
        }
    }
}
