using System;
using System.ComponentModel;
using FMLib.Utility;

namespace FMScrambler.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _labelPath = "Nothing selected.";
        private string _textboxSeed;
        private bool _checkboxRandomAttributes;
        private bool _checkboxRandomMonstersTypes;
        private bool _checkboxRandomGuardianStars;
        private bool _checkboxRandomCardDrops;
        private bool _checkboxRandomDecks;
        private bool _checkboxAttackDefenseRandomizing;
        private bool _checkboxGlitchCards;
        private bool _checkboxRandomEquips;
        private bool _checkboxRandomFusions;
        private bool _checkboxRandomStarchips;
        private bool _checkboxRandomNames;
        private bool _checkboxRandomDescriptions;
        private bool _checkboxRandomStarterDecks;
        private bool _checkboxRandomCardImages;
        private int _textboxMinAttack = 1000;
        private int _textboxMaxAttack = 3000;
        private int _textboxMinDefense = 1000;
        private int _textboxMaxDefense = 3000;
        private int _textboxMinCost = 0;
        private int _textboxMaxCost = 999999;
        private int _textboxMinDropRate = 1;
        private int _textboxMaxDropRate = 1;
        private int _textboxDropCount = 1;
        private ushort _textboxStarChipsDuel = 5;
        private bool _checkboxIsoSeed = true;
        private bool _checkboxIsoDate;
        private bool _checkboxIsoOptions;
        private bool _checkboxSpoilerFiles = true;
        private string _labelIsoExample = "fmscrambler[12345678].bin";

        public string LabelPath
        {
            get => _labelPath;
            set
            {
                _labelPath = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LabelPath"));
            }
        }

        public string TextboxSeed
        {
            get => _textboxSeed;
            set
            {
                _textboxSeed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxSeed"));
            }
        }

        public bool CheckboxRandomAttributes
        {
            get => _checkboxRandomAttributes;
            set
            {
                _checkboxRandomAttributes = value;
                Static.RandomAttributes = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomAttributes"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomMonstersTypes
        {
            get => _checkboxRandomMonstersTypes;
            set
            {
                _checkboxRandomMonstersTypes = value;
                Static.RandomMonstersTypes = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomMonstersTypes"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomGuardianStars
        {
            get => _checkboxRandomGuardianStars;
            set
            {
                _checkboxRandomGuardianStars = value;
                Static.RandomGuardianStars = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomGuardianStars"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomCardDrops
        {
            get => _checkboxRandomCardDrops;
            set
            {
                _checkboxRandomCardDrops = value;
                Static.RandomCardDrops = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomCardDrops"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomDecks
        {
            get => _checkboxRandomDecks;
            set
            {
                _checkboxRandomDecks = value;
                Static.RandomDecks = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomDecks"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxAttackDefenseRandomizing
        {
            get => _checkboxAttackDefenseRandomizing;
            set
            {
                _checkboxAttackDefenseRandomizing = value;
                Static.RandomAtkdef = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxAttackDefenseRandomizing"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxGlitchCards
        {
            get => _checkboxGlitchCards;
            set
            {
                _checkboxGlitchCards = value;
                Static.GlitchFusions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxGlitchCards"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomEquips
        {
            get => _checkboxRandomEquips;
            set
            {
                _checkboxRandomEquips = value;
                Static.RandomEquips = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomEquips"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomFusions
        {
            get => _checkboxRandomFusions;
            set
            {
                _checkboxRandomFusions = value;
                Static.RandomFusions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomFusions"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomNames
        {
            get => _checkboxRandomNames;
            set
            {
                _checkboxRandomNames = value;
                Static.RandomNames = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomNames"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomCardImages
        {
            get => _checkboxRandomCardImages;
            set
            {
                _checkboxRandomCardImages = value;
                Static.RandomCardImages = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomCardImages"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomDescriptions
        {
            get => _checkboxRandomDescriptions;
            set
            {
                _checkboxRandomDescriptions = value;
                Static.RandomDescriptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomDescriptions"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomStarterDecks
        {
            get => _checkboxRandomStarterDecks;
            set
            {
                _checkboxRandomStarterDecks = value;
                Static.RandomStarterDecks = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomStarterDecks"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomStarchips
        {
            get => _checkboxRandomStarchips;
            set
            {
                _checkboxRandomStarchips = value;
                Static.RandomStarchips = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomStarchips"));
                onIsoCheckbox();
            }
        }

        public int TextboxMinAttack
        {
            get => _textboxMinAttack;
            set
            {
                _textboxMinAttack = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinAttack"));
            }
        }

        public int TextboxMaxAttack
        {
            get => _textboxMaxAttack;
            set
            {
                _textboxMaxAttack = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxAttack"));
            }
        }

        public int TextboxMinDefense
        {
            get => _textboxMinDefense;
            set
            {
                _textboxMinDefense = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinDefense"));
            }
        }

        public int TextboxMaxDefense
        {
            get => _textboxMaxDefense;
            set
            {
                _textboxMaxDefense = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxDefense"));
            }
        }

        public int TextboxMinCost
        {
            get => _textboxMinCost;
            set
            {
                _textboxMinCost = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinCost"));
            }
        }

        public int TextboxMaxCost
        {
            get => _textboxMaxCost;
            set
            {
                _textboxMaxCost = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxCost"));
            }
        }

        public int TextboxMinDropRate
        {
            get => _textboxMinDropRate;
            set
            {
                _textboxMinDropRate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinDropRate"));
            }
        }

        public int TextboxDropCount
        {
            get => _textboxDropCount;
            set
            {
                _textboxDropCount = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxDropCount"));
                onIsoCheckbox();
            }
        }

        public ushort TextboxStarChipsDuel
        {
            get => _textboxStarChipsDuel;
            set
            {
                _textboxStarChipsDuel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxStarChipsDuel"));
                onIsoCheckbox();
            }
        }

        public int TextboxMaxDropRate
        {
            get => _textboxMaxDropRate;
            set
            {
                _textboxMaxDropRate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxDropRate"));
            }
        }

        public string VersionMeta => $"{Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";

        public bool CheckboxIsoSeed
        {
            get => _checkboxIsoSeed;
            set
            {
                _checkboxIsoSeed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoSeed"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxIsoDate
        {
            get => _checkboxIsoDate;
            set
            {
                _checkboxIsoDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoDate"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxIsoOptions
        {
            get => _checkboxIsoOptions;
            set
            {
                _checkboxIsoOptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoOptions"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxSpoilerFiles
        {
            get => _checkboxSpoilerFiles;
            set
            {
                _checkboxSpoilerFiles = value;
                Static.Spoiler = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxSpoilerFiles"));
            }
        }

        public string LabelIsoExample
        {
            get => _labelIsoExample;
            set
            {
                _labelIsoExample = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LabelIsoExample"));
            }
        }

        private void onIsoCheckbox()
        {
            if (!CheckboxIsoSeed && !CheckboxIsoOptions && !CheckboxIsoDate) CheckboxIsoSeed = true; 

            LabelIsoExample = "fmscrambler";

            if (CheckboxIsoSeed) LabelIsoExample += $"[{_textboxSeed}]";
            if (CheckboxIsoOptions)
            {
                var options_str = "";
                if (Static.RandomAtkdef) options_str += "[ATKDEF]";
                if (Static.RandomAttributes) options_str += "[Attributes]";
                if (Static.RandomCardDrops) options_str += "[Drops]";
                if (Static.RandomDecks) options_str += "[Duelist_Decks]";
                if (Static.RandomEquips) options_str += "[Equips]";
                if (Static.RandomFusions) options_str += "[Fusions]";
                if (Static.RandomGuardianStars) options_str += "[Guardian_Stars]";
                if (Static.RandomMonstersTypes) options_str += "[Types]";
                if (Static.RandomStarchips) options_str += "[Starchips_Cost]";
                if (Static.RandomNames) options_str += "[Names]";
                if (Static.RandomDescriptions) options_str += "[Descriptions]";
                if (Static.RandomCardImages) options_str += "[Images]";
                if (TextboxDropCount != 1) options_str += $"[Drop_Count_{TextboxDropCount}]";
                if (TextboxStarChipsDuel != Static.MinStarChipsDuel) options_str += $"[Starchips_Duel_{TextboxStarChipsDuel}]";
                if (Static.RandomStarterDecks) options_str += "[Starter_Decks]";
                LabelIsoExample += options_str;
            }
            if (CheckboxIsoDate) LabelIsoExample += $"[{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}]";

            Static.RandomizerFileName = LabelIsoExample;

            LabelIsoExample += ".bin";
        }

    }
}
