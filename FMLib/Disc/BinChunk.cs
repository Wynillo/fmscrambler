using System;
using System.IO;
using System.Threading.Tasks;
using DiscUtils.Iso9660;
using DiscUtils.Streams;
using FMLib.Utility;
using Nito.AsyncEx;

namespace FMLib.Disc;

/// <summary>
/// 
/// </summary>
public class BinChunk
{
    /// <summary>
    /// 
    /// </summary>
    public const int SectorLength = 2352;
    private const string CueExtension = ".cue";

    private readonly string _outFileNameBase = $"FM_Randomizer_[{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}]_{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
    private string _outFileName;
    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    public void ExtractBin(string cueFileName)
    {
        if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\" + _outFileNameBase))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + _outFileNameBase);
        }
        var saveDir = Directory.GetCurrentDirectory() + @"\" + _outFileNameBase;
        try
        {
            CueFile cueFile;
            try
            {
                cueFileName = Path.ChangeExtension(cueFileName, CueExtension);
                cueFile = new CueFile(cueFileName);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Could not read CUE {cueFileName}:\n{e.Message}");
            }

            Stream binStream;
            try
            {
                File.Copy(cueFile.BinFileName, saveDir + @"\"+ _outFileNameBase + ".bin");
                binStream = File.OpenRead(cueFile.BinFileName);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Could not open BIN {cueFile.BinFileName}: {e.Message}");
            }

            Console.WriteLine(Environment.NewLine + "Writing tracks:");
            foreach (Track curTrack in cueFile.TrackList)
            {
                Console.WriteLine(curTrack.Modes);
                Console.WriteLine(curTrack.FileExtension);
                Console.WriteLine(cueFile.TrackList.Count);
                // Include track number when more than 1 track.
                    
                if (cueFile.TrackList.Count > 1)
                    _outFileName =
                        $"{_outFileNameBase}{curTrack.TrackNumber:00}.{curTrack.FileExtension.ToString().ToLower()}";
                else
                    _outFileName = $"{_outFileNameBase}.{curTrack.FileExtension.ToString().ToLower()}";
                curTrack.Write(binStream, _outFileName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("discutils");
        using var isoStream = File.OpenRead(_outFileName);
        var cd = new CDReader(isoStream, false);
        Console.WriteLine(cd.Root.FullName);

        string[] files = cd.GetFiles(cd.Root.FullName);
        string[] dirs = cd.GetDirectories(cd.Root.FullName);
        var mrgdone = false;

        foreach (var c in files)
        {
            Console.WriteLine(c);
            if (c == @"\SLUS_014.11;1")
            {
                var fs = File.Create(saveDir + @"\SLUS_014.11");
                var isf = cd.OpenFile(c, FileMode.Open);
                var dat = new byte[isf.Length];

                var result = AsyncContext.Run(() => isf.ReadAsync(dat, 0, (int)isf.Length));

                var task = Task.Run(async () => { await fs.WriteAsync(dat, 0, dat.Length); });
                task.Wait();
                fs.Close();
            }

            foreach (var e in cd.GetFiles("\\DATA"))
            {
                Console.WriteLine(e);
                if (e == @"\DATA\WA_MRG.MRG;1" && !mrgdone)
                {
                    var fs = File.Create(saveDir + @"\WA_MRG.MRG");
                    var isf = cd.OpenFile(e, FileMode.Open);
                    var dat = new byte[isf.Length];
                    var result = AsyncContext.Run(() => isf.ReadAsync(dat, 0, (int) isf.Length));
                    var task = Task.Run(async () => { await fs.WriteAsync(dat, 0, dat.Length); });
                    task.Wait();
                    mrgdone = true;
                    fs.Close();
                    break;
                }
            }

            Static.SlusPath = saveDir + @"\SLUS_014.11";
            Static.WaPath = saveDir + @"\WA_MRG.MRG";
            Static.IsoPath = saveDir + @"\" + _outFileNameBase + ".bin";

        }
        isoStream.Close();
        File.Delete(_outFileName);
    }
}