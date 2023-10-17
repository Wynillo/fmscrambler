using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Logging;

/// <summary>
/// 
/// </summary>
public class Logger
{
    private readonly int _seed;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seed"></param>
    public Logger(int seed)
    {
        _seed = seed;
    }

    // TODO: Better Log File Logic + HTML/JSON/XML Format
    /// <summary>
    /// </summary>
    public void WriteFusionSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));
        var logFileName = altered
            ? $@"{outputDir}\\fusions_spoiler_#{_seed}.log"
            : $@"{outputDir}\\unaltered_fusions_spoiler_#{_seed}.log";
        if (!File.Exists(logFileName))
            File.CreateText(logFileName).Close();

        var logStream = new StreamWriter(logFileName);

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        foreach (var c in Static.Cards)
        {
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"=> #{c.Id} {c.Name} ({c.Attack}/{c.Defense})");
            logStream.WriteLine("=> FUSIONS:");

            foreach (var fus in c.Fusions.OrderBy(fus => fus.Cards1).ThenBy(fus => fus.Cards2))
                logStream.WriteLine(
                    $"    => {fus.Cards1} + {fus.Cards2} = {fus.Result}         " +
                    $"({(fus.Cards1 < 723 && fus.Cards1 > 0? Static.Cards.Single(card => card.Id == fus.Cards1).Name: fus.Cards1)} " +
                    $"+ {(fus.Cards2 < 723 && fus.Cards2 > 0 ? Static.Cards.Single(card => card.Id == fus.Cards2).Name : fus.Cards2)} " +
                    $"= {(fus.Result < 723 && fus.Result > 0? Static.Cards.Single(card => card.Id == fus.Result).Name : fus.Result)}");
        }

        logStream.Close();
    }


    // TODO: Better Log File Logic + HTML/JSON/XML Format
    /// <summary>
    /// </summary>
    public void WriteNewFusionSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));
        var logFileName = altered
            ? $@"{outputDir}\\NEW_fusions_spoiler_#{_seed}.log"
            : $@"{outputDir}\\NEW_unaltered_fusions_spoiler_#{_seed}.log";

        if (!File.Exists(logFileName))
            File.CreateText(logFileName).Close();

        var logStream = new StreamWriter(logFileName);

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        foreach (var c in Static.Cards)
        {
            if (!RandomHelper.IsMonsterCard(c.Type)) continue;

            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"=> #{c.Id} {c.Name} ({c.Attack}/{c.Defense})");
            logStream.WriteLine("=> FUSIONS:");

            if (!Static.AllFusions.Exists(f => f.Cards1 == c.Id)) continue;

            foreach (var fus in Static.AllFusions.Where(f => f.Cards1 == c.Id).OrderBy(fus => fus.Cards1).ThenBy(fus => fus.Cards2))
                logStream.WriteLine(
                    $"    => {fus.Cards1} + {fus.Cards2} = {fus.Result}         " +
                    $"({(fus.Cards1 < 723 && fus.Cards1 > 0? Static.Cards.Single(card => card.Id == fus.Cards1).Name: fus.Cards1)} " +
                    $"+ {(fus.Cards2 < 723 && fus.Cards2 > 0 ? Static.Cards.Single(card => card.Id == fus.Cards2).Name : fus.Cards2)} " +
                    $"= {(fus.Result < 723 && fus.Result > 0? Static.Cards.Single(card => card.Id == fus.Result).Name : fus.Result)}");
        }

        logStream.Close();
    }

    /// <summary>
    /// </summary>
    public void WriteStarchipSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));

        if (!File.Exists($@"{outputDir}\\starchip_spoiler_#{_seed}.log"))
            File.CreateText($"{outputDir}\\starchip_spoiler_#{_seed}.log").Close();

        var logStream = new StreamWriter($@"{outputDir}\\starchip_spoiler_#{_seed}.log");

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Starchip Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        foreach (var c in Static.Cards)
        {
            logStream.WriteLine($"    => #{c.Id} {c.Name}");
            logStream.WriteLine($"        Cost: {c.Starchip.Cost} Password: {c.Starchip.PasswordStr}");
        }

        logStream.Close();
    }

    /// <summary>
    /// </summary>
    public void WriteDropsSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));

        if (!File.Exists($@"{outputDir}\\drops_spoiler_#{_seed}.log"))
            File.CreateText($"{outputDir}\\drops_spoiler_#{_seed}.log").Close();

        var logStream = new StreamWriter($@"{outputDir}\\drops_spoiler_#{_seed}.log");

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Drops Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        // Get drop map as well as sort by drop rate
        Dictionary<Duelist, List<KeyValuePair<Card, int>>> GetDropMap(DropType dropType)
        {
            var dropMap = new Dictionary<Duelist, List<KeyValuePair<Card, int>>>();

            foreach (var d in Static.Duelist)
            {
                var sortedMap = new List<KeyValuePair<Card, int>>();

                foreach (var c in Static.Cards)
                    if (d.Drop.GetDropArray(dropType)[c.Id - 1] > 0)
                        sortedMap.Add(new KeyValuePair<Card, int>(c, d.Drop.GetDropArray(dropType)[c.Id - 1]));

                sortedMap.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                dropMap.Add(d, sortedMap);
            }

            return dropMap;
        }

        var sapDMap = GetDropMap(DropType.Sapow);
        var bcdDMap = GetDropMap(DropType.Bcdpow);
        var satDMap = GetDropMap(DropType.Satec);

        foreach (var d in Static.Duelist)
        {
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"{d.Name} S/A-Tec drops");
            logStream.WriteLine($"Possibilities: {d.Drop.SaTec.Sum(x => x > 0 ? 1 : 0)}");
            logStream.WriteLine($"Total Rate: {d.Drop.SaTec.Sum()}");

            var dropMap = satDMap[d];

            foreach (var p in dropMap)
            {
                logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
            }

            logStream.WriteLine();
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"{d.Name} B/C/D drops");
            logStream.WriteLine($"Possibilities: {d.Drop.BcdPow.Sum(x => x > 0 ? 1 : 0)}");
            logStream.WriteLine($"Total Rate: {d.Drop.BcdPow.Sum()}");

            dropMap = bcdDMap[d];

            foreach (var p in dropMap)
            {
                logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
            }

            logStream.WriteLine();
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"{d.Name} S/A-Pow drops");
            logStream.WriteLine($"Possibilities: {d.Drop.SaPow.Sum(x => x > 0 ? 1 : 0)}");
            logStream.WriteLine($"Total Rate: {d.Drop.SaPow.Sum()}");

            dropMap = sapDMap[d];

            foreach (var p in dropMap)
            {
                logStream.WriteLine($"    => #{p.Key.Id} {p.Key.Name}");
                logStream.WriteLine($"        Rate: {p.Value}/{Static.MaxRateDrop}");
            }

            logStream.WriteLine();
        }

        logStream.Close();
    }

    /// <summary>
    /// </summary>
    public void WriteStarterDecksSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));

        if (!File.Exists($@"{outputDir}\\starter_decks_spoiler_#{_seed}.log"))
            File.CreateText($"{outputDir}\\starter_decks_spoiler_#{_seed}.log").Close();

        var logStream = new StreamWriter($@"{outputDir}\\starter_decks_spoiler_#{_seed}.log");

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Starter Decks Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        // Get drop map as well as sort by drop rate
        Dictionary<StarterDeck, List<KeyValuePair<Card, int>>> GetStarterDeckMap()
        {
            var dropMap = new Dictionary<StarterDeck, List<KeyValuePair<Card, int>>>();

            foreach (var d in Static.StarterDeck)
            {
                var sortedMap = new List<KeyValuePair<Card, int>>();

                foreach (var c in Static.Cards)
                    if (d.Cards[c.Id - 1] > 0)
                        sortedMap.Add(new KeyValuePair<Card, int>(c, d.Cards[c.Id - 1]));

                sortedMap.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                dropMap.Add(d, sortedMap);
            }

            return dropMap;
        }

        var starterDeckMap = GetStarterDeckMap();

        foreach (var (d, i) in Static.StarterDeck.Select((d, i) => (d, i)))
        {
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"Starter Deck Part: {i + 1}");
            logStream.WriteLine($"Possibilities: {d.Cards.Sum(x => x > 0 ? 1 : 0)}");
            logStream.WriteLine($"Total Rate: {d.Cards.Sum()}");
            logStream.WriteLine($"Total Get Cards: {d.Dropped}");

            var dropMap = starterDeckMap[d];

            foreach (var p in dropMap)
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
    /// </summary>
    public void WriteDuelistDecksSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));

        if (!File.Exists($@"{outputDir}\\duelist_decks_spoiler_#{_seed}.log"))
            File.CreateText($"{outputDir}\\duelist_decks_spoiler_#{_seed}.log").Close();

        var logStream = new StreamWriter($@"{outputDir}\\duelist_decks_spoiler_#{_seed}.log");

        logStream.WriteLine("== YU-GI-OH! Forbidden Memories Duelist Decks Scrambler Spoiler File ==");
        logStream.WriteLine($"== Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} ==");
        logStream.WriteLine("====================================================================\r\n");

        // Get duelist map as well as sort by drop rate

        var duelistDeckMap = GetDuelistDeckMap();

        foreach (var d in Static.Duelist)
        {
            logStream.WriteLine("====================================================================");
            logStream.WriteLine($"{d.Name} Deck");
            logStream.WriteLine($"Possibilities: {d.Deck.Sum(x => x > 0 ? 1 : 0)}");
            logStream.WriteLine($"Total Rate: {d.Deck.Sum()}");

            var dropMap = duelistDeckMap[d];

            foreach (var p in dropMap)
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

    private static Dictionary<Duelist, List<KeyValuePair<Card, int>>> GetDuelistDeckMap()
    {
        var duelistMap = new Dictionary<Duelist, List<KeyValuePair<Card, int>>>();

        foreach (var d in Static.Duelist)
        {
            var sortedMap = new List<KeyValuePair<Card, int>>();

            foreach (var c in Static.Cards)
                if (d.Deck[c.Id - 1] > 0)
                    sortedMap.Add(new KeyValuePair<Card, int>(c, d.Deck[c.Id - 1]));

            sortedMap.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            duelistMap.Add(d, sortedMap);
        }

        return duelistMap;
    }

    /// <summary>
    /// </summary>
    public void WriteHtmlFusionSpoilerFile(bool altered = true)
    {
        var outputDir = Static.WaPath?.Substring(0, Static.WaPath.LastIndexOf('\\'));

        if (!File.Exists($@"{outputDir}\\fusions_spoiler_#{_seed}.html"))
            File.CreateText($"{outputDir}\\fusions_spoiler_#{_seed}.html").Close();

        var logStream = new StreamWriter($@"{outputDir}\\fusions_spoiler_#{_seed}.html");

        var template =
            $$"""
              <!DOCTYPE html>
                              <html lang=\"en\">
                              <head>
                              <meta charset=\"UTF-8\">
                              <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">
                              <meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\">
                              <title>{{_seed}}</title>
                              <style>
                                  html, body { width: 100%; height: auto; margin: 0 auto; }
                                  table { width: 90%; margin: 0 auto; border: 1px solid lightgrey; border-spacing: 0; }
                                  thead { background-color: #f2ffeb; padding: 2px; }
                                  thead th { padding: 3px; text-align: center; }
                                  table td { border-bottom: 1px solid lightgrey!important; padding: 2px!important; text-align: center; }
                                  table tr:last-child td { border-bottom: 0!important; }
                                  tbody tr td { border-right: 1px solid lightgrey; }
                                  tbody tr td:last-child { border-right: 0!important; }
                              </style>
                              </head>
                              <body>
                              <div style=\"width: 100 %; text-align: center;\">
              """;

        const string table2Template =
            """
              <span style=\"text-align: center; font-size: 120%; font-weight: bold; margin-bottom: 5px;\">Changed Fusions</span>
              <table>
                <thead>
                    <tr>
                        <th style=\"width: 5%;\">ID 1</th>
                        <th style=\"width: 25%;\">Name 1</th>
                        <th style=\"width: 5%;\">ID 2</th>
                        <th style=\"width: 25%;\">Name 2</th>
                        <th style=\"width: 5%;\">ResultID</th>
                        <th style=\"width: 25%;\">Result Name</th>
                    </tr>
                </thead>
                <tbody>
            """;

        logStream.WriteLine(template +
                            $" <h1>YU-GI-OH! Forbidden Memories Fusion Scrambler Spoiler File</h1> <h4>Version {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion}</h4>");

        logStream.WriteLine(table2Template);
        Console.WriteLine("Writing tmpFusions");
        foreach (var c in Static.Cards)
        foreach (var fus in c.Fusions)
        {
            logStream
                .WriteLine(
                    $"<tr>" +
                    $"<td>{fus.Cards1}</td> " +
                    $"<td>{Static.Cards.Single(card => card.Id == fus.Cards1).Name}</td> " +
                    $"<td>{fus.Cards2}</td> <td>{Static.Cards.Single(card => card.Id == fus.Cards2).Name}</td> " +
                    $"<td>{fus.Result}</td> " +
                    $"<td>{Static.Cards.Single(card => card.Id == fus.Result).Name}</td>" +
                    $"</tr>");
        }

        logStream.WriteLine("</tbody></table></div></body></html>");
        logStream.Close();
    }
}