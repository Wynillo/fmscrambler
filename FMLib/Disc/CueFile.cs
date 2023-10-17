﻿using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace FMLib.Disc;

/// <summary>
/// 
/// </summary>
public class CueFile
{
    /// <summary>
    /// 
    /// </summary>
    public readonly ArrayList TrackList = new ArrayList();
    /// <summary>
    /// 
    /// </summary>
    public readonly string BinFileName;
    private readonly string _cueFilePath;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cueFilePath"></param>
    /// <exception cref="ApplicationException"></exception>
    public CueFile(string cueFilePath)
    {
        this._cueFilePath = cueFilePath;
        Console.WriteLine("Reading the CUE file:");
        string cueLines;
        using (TextReader cueReader = new StreamReader(cueFilePath))
        {
            BinFileName = GetBinFileName(cueReader.ReadLine());
            cueLines = cueReader.ReadToEnd();
        }

        var trackRegex = new Regex(@"track\s+?(\d+?)\s+?(\S+?)[\s$]+?index\s+?\d+?\s+?(\S*)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var matches = trackRegex.Matches(cueLines);

        if (matches.Count == 0)
            throw new ApplicationException("No tracks was found.");

        Track track = null;
        Track prevTrack = null;
        foreach (Match trackMatch in matches)
        {
            track = new Track(
                trackMatch.Groups[1].Value,
                trackMatch.Groups[2].Value,
                trackMatch.Groups[3].Value);

            if (prevTrack != null)
            {
                prevTrack.Stop = track.StartPosition - 1;
                prevTrack.StopSector = track.StartSector;
            }
            TrackList.Add(track);
            prevTrack = track;
        }

        if (track == null) return;
        track.Stop = GetBinFileLength();
        track.StopSector = track.Stop / BinChunk.SectorLength;
        TrackList[TrackList.Count - 1] = track;
    }

    private long GetBinFileLength()
    {
        var fileInfo = new FileInfo(BinFileName);
        return fileInfo.Length;
    }

    private string GetBinFileName(string cueFirstLine)
    {
        var binRegex = new Regex(@"file\s+?""(.*?)""", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var match = binRegex.Match(cueFirstLine);
        var res = match.Groups[1].Value;
        var cueDirectory = Path.GetDirectoryName(_cueFilePath);
        res = Path.Combine(cueDirectory, Path.GetFileName(res));
        if (!File.Exists(res))
            res = Path.Combine(cueDirectory, Path.GetFileNameWithoutExtension(_cueFilePath) + ".bin");
        return res;
    }
}