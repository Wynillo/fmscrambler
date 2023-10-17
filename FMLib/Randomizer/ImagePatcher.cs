using System;
using System.Collections.Generic;
using System.IO;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Randomizer;

/// <summary>
/// Patching of the Game Image File (BIN/ISO)
/// </summary>
public class ImagePatcher
{
    /// <summary>
    /// List of Files in the Game Image
    /// </summary>
    public List<GameFile> GameFile = new List<GameFile>();

    /// <summary>
    /// Filestream to handle the data
    /// </summary>
    private FileStream _fs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="file">Filename</param>        
    public ImagePatcher(string file)
    {
        _fs = new FileStream(file, FileMode.Open);
    }

    /// <summary>
    /// Method to patch the Game Image File
    /// </summary>
    /// <returns>1 for success, -1 for failure</returns>
    public int PatchImage()
    {
        ListDirectories(ref _fs, new[]
        {
            new GameFile
            {
                Offset = 0xCA20,
                Name = "",
                Size = 2048
            }
        });

        foreach (var k in GameFile)
        {
            // Choose which File to use based on the name of the Item in the loop
            var p = k.Name == "SLUS_014.11" ? Static.SlusPath : Static.WaPath;

            using var fs2 = new FileStream(p, FileMode.Open);
            // Filesize is different, abort with error
            if (k.Size != fs2.Length)
            {
                return -1;
            }

            _fs.Position = k.Offset + 24;

            for (var n = 0; n < fs2.Length / 2048L; n++)
            {
                _fs.Write(fs2.ExtractPiece(0, 2048), 0, 2048);
                _fs.Position += 0x130L;
            }
        }

        _fs.Dispose();
        _fs.Close();

        var outputDir = Static.IsoPath.Substring(0, Static.IsoPath.LastIndexOf('\\'));

        if (Static.UsedIso)
        {
            File.Copy(Static.IsoPath, $"{outputDir}\\{Static.RandomizerFileName}.bin");

        }
        else
        {
            File.Move(Static.IsoPath, $"{outputDir}\\{Static.RandomizerFileName}.bin");
        }

        Static.IsoPath = $"{outputDir}\\{Static.RandomizerFileName}.bin";
        string[] cueTemplate = { $"FILE \"{Static.RandomizerFileName}.bin\" BINARY", "  TRACK 01 MODE2/2352", "    INDEX 01 00:00:00" };
        File.WriteAllLines($"{outputDir}\\{Static.RandomizerFileName}.cue", cueTemplate);

        return 1;
    }

    private void ListDirectories(ref FileStream fs, IEnumerable<GameFile> iso)
    {
        var fileList = new List<GameFile>();

        foreach (var file in iso)
        {
            using var ms = new MemoryStream(fs.ExtractPiece(0, 2048, file.Offset));
            ms.Position = 120L;

            for (var j = ms.ReadByte(); j > 0; j = ms.ReadByte())
            {
                var tmpFile = new GameFile();
                var arr = ms.ExtractPiece(0, j - 1);

                tmpFile.Offset = arr.ExtractInt32(1) * 2352;
                tmpFile.Size = arr.ExtractInt32(9);
                tmpFile.IsDirectory = arr[24] == 2;
                tmpFile.NameSize = arr[31];
                tmpFile.Name = GetName(ref arr, tmpFile.NameSize);

                if (tmpFile.IsDirectory)
                {
                    fileList.Add(tmpFile);
                }

                switch (tmpFile.NameSize)
                {
                    case 13 when tmpFile.Name == "SLUS_014.11":
                    case 12 when tmpFile.Name == "WA_MRG.MRG":
                        GameFile.Add(tmpFile);
                        break;
                }
            }
        }
        
        if (fileList.Count > 0)
        {
            // ReSharper disable once TailRecursiveCall
            ListDirectories(ref fs, fileList.ToArray());
        }
    }

    private static string GetName(ref byte[] data, int size)
    {
        var text = string.Empty;

        for (var i = 0; i < size; i++)
        {
            var c = Convert.ToChar(data[32 + i]);

            if (c == ';')
            {
                break;
            }

            text += c.ToString();
        }

        return text;
    }
}