using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;
using BeatSaberLibraryManager.Outputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;

namespace BeatSaberLibraryManager;

public class Program
{
    public static void Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        BeatSaver beatSaverApi = new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));
        List<BPList?> allBpLists = GetAllBpLists(beatSaverApi).ToList();
        FilterDownloadMapsAndExport(allBpLists, beatSaverApi);
        Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
    }

    private static IEnumerable<BPList?> GetAllBpLists(BeatSaver beatSaverApi)
    {
        List<Task<BPList?>> tasks = new();

        //Download all beatsaver playlists
        foreach (int id in Playlists.BeatSaverPlaylists.Values)
        {
            Task<BPList?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
            tasks.Add(t);
        }
        
        // todo: Download all beatsaver user playlists
        // todo: Downlaod all non-beatsaver playlists
        // todo: Generate spotify playlists

        Task.WaitAll(tasks.Cast<Task>().ToArray());

        return tasks.Select(task => task.Result);
    }

    private static void FilterDownloadMapsAndExport(List<BPList?> allBpLists, BeatSaver beatSaverApi)
    {
        foreach (BPList? bpList in allBpLists)
        {
            if (bpList != null)
            {
                Console.WriteLine("\n" + bpList.playlistTitle);
                Console.WriteLine("============================================");

                foreach (var songInfo in bpList.songs)
                {
                    Console.WriteLine(songInfo.songName);

                    // var zipContents = songInfo.Map.LatestVersion.DownloadZIP();
                    // zipContents.Wait();
                    // if (zipContents.Result != null)
                    // {
                    //     string zipFilePath = FileManager.GetZipFilePath(songInfo.Map);
                    //     File.WriteAllBytesAsync(zipFilePath, zipContents.Result);
                    //     FileManager.UnzipFile(zipFilePath, out var mapFolderPath);
                    // }
                }
            }
        }
    }
}