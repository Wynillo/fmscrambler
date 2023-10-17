﻿using System;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Linq;
using FMLib.Disc;
using FMLib.Randomizer;
using FMLib.Utility;
using Microsoft.Win32;

namespace FMScrambler;

/// <summary>
/// Interaktionslogik für MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private bool _isPasteEvent = false;
    private string _prevSeedText;
    private readonly Random _rnd = new Random();

    public MainWindow()
    {
        InitializeComponent();
    }

    // Randomizing via Game Image
    private async void btn_loadiso_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Location of Yu-Gi-Oh! Forbidden Memories NTSC CUE/BIN File",
            Filter = "*.cue|*.cue|*bin|*.bin"
        };

        if (dlg.ShowDialog() == true)
        {
            lbl_path.Content = Path.GetDirectoryName(dlg.FileName);

            MessageBox.Show(
                "Extracting game data can take a minute... please wait.",
                "Extracting data",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            pgr_back.Visibility = Visibility.Visible;

            var chunker = new BinChunk();

            await Task.Run(() => chunker.ExtractBin(dlg.FileName));

            pgr_back.Visibility = Visibility.Hidden;

            MessageBox.Show(
                "Extracting game data complete.",
                "Extracting data",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Static.UsedIso = true;
            btn_patchiso.IsEnabled = false;
            btn_perform.IsEnabled = true;

            LoadCardsFilters();
        }
    }

    private void LoadCardsFilters()
    {
        var dataReader = new DataReader();
        var cardStarterBanList = new List<dynamic>();
        var cardDuelistBanList = new List<dynamic>();

        dataReader.LoadDataFromSlus();
        dataReader.LoadDataFromWaMrg();

        txt_starChipsDuel.Value = Static.StarChipsDuel;
        txt_starChipsDuel.IsEnabled = true;

        txt_dropCount.Value = Static.DropCount - 1;
        txt_dropCount.IsEnabled = true;

        var starterCards = Static.Cards
            .Where(x => x.Type != (int)Static.Type.Ritual
                        && x.Id != 722 /* Magician of Black Chaos */)
            .Select(y => new
            {
                y.Id,
                y.Name,
                y.Description,
                y.Attack,
                y.Defense,
                y.BigImage
            })
            .ToList();

        var duelistCards = Static.Cards
            .Where(x => x.Type != (int)Static.Type.Ritual)
            .Select(y => new
            {
                y.Id,
                y.Name,
                y.Description,
                y.Attack,
                y.Defense,
                y.BigImage
            })
            .ToList();

        foreach (var card in starterCards)
        {
            cardStarterBanList.Add(new { card.Id, card.Name, Description = card.Description + $"\n\nATK: {card.Attack} \nDEF: {card.Defense}", Image = card.BigImage.CreateUnsafeBitmap() });
        }

        foreach (var card in duelistCards)
        {
            cardDuelistBanList.Add(new { card.Id, card.Name, Description = card.Description + $"\n\nATK: {card.Attack} \nDEF: {card.Defense}", Image = card.BigImage.CreateUnsafeBitmap() });
        }

        listb_staterdeckcardsFilters.ItemsSource = cardStarterBanList;
        listb_staterdeckcardsFilters.IsEnabled = true;

        listb_duelistsdeckcardsFilters.ItemsSource = cardDuelistBanList;
        listb_duelistsdeckcardsFilters.IsEnabled = true;

        tab_cardsFilters.IsEnabled = true;
    }

    private void btn_perform_Click(object sender, RoutedEventArgs e)
    {
        SyncScramble();
    }

    private void SyncScramble()
    {
        var cardCount = Static.MaxCards;

        Static.SetCardCount(cardCount);

        var fileHandler = new DataRandomizer(int.Parse(txt_seed.Text));

        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            fileHandler.PerformScrambling(
                (int)txt_minAtk.Value, 
                (int)txt_maxAtk.Value,
                (int)txt_minDef.Value, 
                (int)txt_maxDef.Value, 
                (int)txt_minCost.Value,
                (int)txt_maxCost.Value, 
                (int)txt_dropCount.Value, 
                (ushort)txt_starChipsDuel.Value,
                (int)txt_minAtkStarterDeck.Value, 
                (int)txt_maxAtkStarterDeck.Value,
                (int)txt_minDefStarterDeck.Value, 
                (int)txt_maxDefStarterDeck.Value,
                (int)txt_minAtkDuelistDeck.Value, 
                (int)txt_maxAtkDuelistDeck.Value,
                (int)txt_minDefDuelistDeck.Value, 
                (int)txt_maxDefDuelistDeck.Value);
        });

        MessageBox.Show(
            "Done scrambling, you may proceed with patching your game ISO now."
            + (Static.Spoiler ? " Spoiler files were generated as well" : ""),
            "Done scrambling.",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        btn_patchiso.IsEnabled = true;
        btn_perform.IsEnabled = false;
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");

        e.Handled = regex.IsMatch(e.Text);
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
        txt_seed.Text = _rnd.Next(10000, 214748364).ToString();
        txt_seed.Focus();
    }

    private void btn_patchiso_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Static.RandomizerFileName))
        {
            LabelUpdateSeed();
        }

        if (!Static.UsedIso)
        {
            MessageBox.Show(
                "Did you make a backup copy of your Image file before patching? If not, do so before pressing OK.",
                "Backup Info",
                MessageBoxButton.OK,
                MessageBoxImage.Question);

            var dlg = new OpenFileDialog
            {
                Title = "Forbidden Memories Image"
            };

            if (dlg.ShowDialog() == true)
            {
                DoPatch(dlg.FileName);
            }
        }
        else
        {
            DoPatch(Static.IsoPath);
        }

        pgr_back.Visibility = Visibility.Hidden;
    }

    private async void DoPatch(string path)
    {
        btn_patchiso.IsEnabled = false;
        pgr_back.Visibility = Visibility.Visible;

        var patcher = new ImagePatcher(path);

        Static.IsoPath = path;

        var patchResult = await Task.Run(() => patcher.PatchImage());

        if (patchResult == 1)
        {
            MessageBox.Show(
                "Image successfully patched! Have fun playing! Location of Randomized Image: "
                + Static.IsoPath,
                "Done patching.",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Process.Start(Directory.GetParent(Static.IsoPath).FullName);

            // Allow scrambling again
            btn_perform.IsEnabled = true;
            btn_patchiso.IsEnabled = false;
        }
        else
        {
            MessageBox.Show(
                "Error patching Image. Not Forbidden Memories or wrong version.",
                "Error patching.",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void txt_seed_Initialized(object sender, EventArgs e)
    {
        txt_seed.Text = _rnd.Next(10000, 214748364).ToString();
        _prevSeedText = txt_seed.Text;
        txt_seed.Focus();
    }

    private void txt_seed_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Key.V == e.Key && Keyboard.Modifiers == ModifierKeys.Control)
        {
            _prevSeedText = ((TextBox)sender).Name switch
            {
                "txt_seed" => txt_seed.Text,
                "txt_minAtk" => txt_minAtk.Value.ToString(),
                "txt_maxAtk" => txt_maxAtk.Value.ToString(),
                "txt_minDef" => txt_minDef.Value.ToString(),
                "txt_maxDef" => txt_maxDef.Value.ToString(),
                "txt_dropCount" => txt_dropCount.Value.ToString(),
                _ => _prevSeedText
            };

            _isPasteEvent = true;
        }
    }

    private void txt_seed_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isPasteEvent)
        {
            switch (((TextBox)sender).Name)
            {
                case "txt_seed":
                    txt_seed.Text = _prevSeedText;
                    break;
                case "txt_minAtk":
                    _prevSeedText = txt_minAtk.Value.ToString();
                    break;
                case "txt_maxAtk":
                    _prevSeedText = txt_maxAtk.Value.ToString();
                    break;
                case "txt_minDef":
                    _prevSeedText = txt_minDef.Value.ToString();
                    break;
                case "txt_maxDef":
                    _prevSeedText = txt_maxDef.Value.ToString();
                    break;
                case "txt_dropCount":
                    _prevSeedText = txt_dropCount.Value.ToString();
                    break;
            }

            _isPasteEvent = false;
        }

        if (txt_seed.Text.StartsWith("0"))
        {
            switch (((TextBox)sender).Name)
            {
                case "txt_seed":
                    txt_seed.Text = $"1{txt_seed.Text.Substring(1)}"; ;
                    break;
                case "txt_minAtk":
                    txt_minAtk.Value = int.Parse($"1{txt_minAtk.Value.ToString().Substring(1)}"); ;
                    break;
                case "txt_maxAtk":
                    txt_maxAtk.Value = int.Parse($"1{txt_maxAtk.Value.ToString().Substring(1)}"); ;
                    break;
                case "txt_minDef":
                    txt_minDef.Value = int.Parse($"1{txt_minDef.Value.ToString().Substring(1)}"); ;
                    break;
                case "txt_maxDef":
                    txt_maxDef.Value = int.Parse($"1{txt_maxDef.Value.ToString().Substring(1)}"); ;
                    break;
                case "txt_dropCount":
                    txt_dropCount.Value = int.Parse($"1{txt_dropCount.Value.ToString().Substring(1)}"); ;
                    break;
            }
        }

        LabelUpdateSeed();
    }

    private void LabelUpdateSeed()
    {
        if (lbl_isoExample != null)
        {
            var content = (string)lbl_isoExample.Content;
            var offset = content.IndexOf('[') + 1;
            content = content.Remove(offset, content.IndexOf(']') - offset);
            content = content.Insert(offset, txt_seed.Text);
            lbl_isoExample.Content = content;
            Static.RandomizerFileName = content.Substring(0, content.LastIndexOf('.'));
        }
    }

    private void MetroWindow_Initialized(object sender, EventArgs e)
    {
        Title = $"YGO! FM Fusion Scrambler Tool - {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";
        lbl_isoExample.Content = $"fmscrambler[{txt_seed.Text}].bin";

        InitCharTable();
    }

    private void InitCharTable()
    {
        var tablePath = @"./CharacterTable.txt";

        if (!File.Exists(tablePath))
        {
            MessageBox.Show(
                "CharacterTable.txt not found! Provide a path for it!",
                "Unable to find CharacterTable.txt",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            var ofd = new OpenFileDialog
            {
                Title = "CharacterTable file",
                Filter = "CharacterTable.txt|CharacterTable.txt"
            };

            if (ofd.ShowDialog() == true)
            {
                tablePath = ofd.FileName;
            }
            else
            {
                Close();

                return;
            }
        }

        var strReader = new StringReader(File.ReadAllText(tablePath));

        while (strReader.ReadLine() is { } input)
        {
            var match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

            if (!match.Success)
            {
                continue;
            }

            var k1 = Convert.ToChar(match.Groups[2].ToString());
            var k2 = (byte)int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);

            Static.Dict.Add(k2, k1);

            if (!Static.RDict.ContainsKey(k1))
            {
                Static.RDict.Add(k1, k2);
            }
        }

        // There should be 85 entries otherwise file got corrupted, misread or user manually provided a bad file
        if (Static.Dict.Values.Count != 85)
        {
            MessageBox.Show(
                "Provided CharacterTable.txt is incorrect or incomplete!",
                "Error reading CharacterTable.txt",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Close();
        }
    }

    private void btn_loadiso1_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Location of SLUS_014.11",
            Filter = "SLUS_014.11|SLUS_014.11"
        };

        var findSlus = false;
        var findWa = false;

        if (dlg.ShowDialog() == true)
        {
            Static.SlusPath = dlg.FileName;
            lbl_path.Content = Path.GetDirectoryName(dlg.FileName);
            findSlus = true;

            if (!File.Exists(Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG"))
            {
                dlg.Title = "Location of WA_MRG.MRG";
                dlg.Filter = "WA_MRG|WA_MRG.MRG";

                if (dlg.ShowDialog() == true)
                {
                    Static.WaPath = dlg.FileName;
                    btn_patchiso.IsEnabled = false;
                    btn_perform.IsEnabled = true;
                    Static.UsedIso = false;
                    findWa = true;
                }
            }
            else
            {
                Static.WaPath = Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG";
                btn_patchiso.IsEnabled = false;
                btn_perform.IsEnabled = true;
                Static.UsedIso = false;
                findWa = true;
            }
        }

        if (findSlus == true
            && findWa == true)
        {
            LoadCardsFilters();
        }
    }

    private void stackp_cardsFiltersImage_MouseEnter(object sender, MouseEventArgs e)
    {
        var stackPanelObject = e.Source as StackPanel;

        if (stackPanelObject != null
            && stackPanelObject.DataContext != null)
        {
            var bigImage = TypeDescriptor.GetProperties(stackPanelObject.DataContext)["Image"].GetValue(stackPanelObject.DataContext) as Bitmap;

            if (bigImage != null)
            {
                try
                {
                    var handleBigImage = bigImage.GetHbitmap();

                    img_bannedStarterCardId.Source = Imaging.CreateBitmapSourceFromHBitmap(handleBigImage, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    img_bannedStarterCardId.ToolTip = TypeDescriptor.GetProperties(stackPanelObject.DataContext)["Description"].GetValue(stackPanelObject.DataContext);
                    img_bannedStarterCardId.Visibility = Visibility.Visible;

                    img_bannedDuelistCardId.Source = Imaging.CreateBitmapSourceFromHBitmap(handleBigImage, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    img_bannedDuelistCardId.ToolTip = TypeDescriptor.GetProperties(stackPanelObject.DataContext)["Description"].GetValue(stackPanelObject.DataContext);
                    img_bannedDuelistCardId.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.InnerException?.Message, "Card Image Loader",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var tabControlObject = e.Source as TabControl;

        if (tabControlObject != null)
        {
            var tabItemObject = tabControlObject.SelectedValue as TabItem;

            if (tabItemObject != null)
            {
                if (tabItemObject.Header.ToString().Contains("Deck") == true
                    || tabItemObject.Header.ToString().Contains("Card") == true)
                {
                    if (Static.FilterStarterDeckCards.BannedCards.Count > 0)
                    {
                        for (var i = 0; i < Static.FilterStarterDeckCards.BannedCards.Count; i++)
                        {
                            listb_staterdeckcardsFilters.SelectedItems.Add(Static.FilterStarterDeckCards.BannedCards[i]);
                        }
                    }

                    if (Static.FilterDuelistDeckCards.BannedCards.Count > 0)
                    {
                        for (var i = 0; i < Static.FilterDuelistDeckCards.BannedCards.Count; i++)
                        {
                            listb_duelistsdeckcardsFilters.SelectedItems.Add(Static.FilterDuelistDeckCards.BannedCards[i]);
                        }
                    }
                }
                else
                {
                    // Card filter image clear
                    img_bannedStarterCardId.ToolTip = string.Empty;
                    img_bannedStarterCardId.Source = null;
                    img_bannedStarterCardId.Visibility = Visibility.Hidden;

                    img_bannedDuelistCardId.ToolTip = string.Empty;
                    img_bannedDuelistCardId.Source = null;
                    img_bannedDuelistCardId.Visibility = Visibility.Hidden;

                    if (listb_staterdeckcardsFilters.SelectedItems.Count > 0)
                    {
                        Static.FilterStarterDeckCards.BannedCards.Clear();

                        for (var i = 0; i < listb_staterdeckcardsFilters.SelectedItems.Count; i++)
                        {
                            Static.FilterStarterDeckCards.BannedCards.Add(listb_staterdeckcardsFilters.SelectedItems[i]);
                        }

                        listb_staterdeckcardsFilters.SelectedItems.Clear();
                    }

                    if (listb_duelistsdeckcardsFilters.SelectedItems.Count > 0)
                    {
                        Static.FilterDuelistDeckCards.BannedCards.Clear();

                        for (var i = 0; i < listb_duelistsdeckcardsFilters.SelectedItems.Count; i++)
                        {
                            Static.FilterDuelistDeckCards.BannedCards.Add(listb_duelistsdeckcardsFilters.SelectedItems[i]);
                        }

                        listb_duelistsdeckcardsFilters.SelectedItems.Clear();
                    }
                }
            }
        }
    }
}