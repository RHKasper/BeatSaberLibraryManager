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
    public static async Task Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        BeatSaver beatSaverApi = new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));
        
        List<BPList> allFilteredBpLists = GetAllFilteredBpLists(beatSaverApi).ToList();
        List<BPList> unfilteredBpLists = GetAllUnfilteredBpLists(beatSaverApi).ToList();
        
        var downloadUnfilteredBeatMaps = DownloadBeatmaps(unfilteredBpLists, beatSaverApi);
        var downloadFilteredBeatmaps = DownloadBeatmaps(allFilteredBpLists, beatSaverApi);
        await downloadUnfilteredBeatMaps;
        await downloadFilteredBeatmaps;
        
        FilterOutput(downloadFilteredBeatmaps);
        
        await DownloadZipFiles(downloadUnfilteredBeatMaps);
        await DownloadZipFiles(downloadFilteredBeatmaps);

        Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
    }

    private static IEnumerable<BPList> GetAllFilteredBpLists(BeatSaver beatSaverApi)
    {
        List<Task<BPList?>> tasks = new();

        //Download all beatsaver playlists
        foreach (int id in Playlists.FilteredBeatSaverPlaylists.Values)
        {
            Task<BPList?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
            tasks.Add(t);
        }
        
        // todo: Download all beatsaver user playlists
        // todo: Download all non-beatsaver playlists

        Task.WaitAll(tasks.Cast<Task>().ToArray());

        return tasks.Where(t => t.Result != null).Cast<Task<BPList>>().Select(task => task.Result);
    }

    private static IEnumerable<BPList> GetAllUnfilteredBpLists(BeatSaver beatSaverApi)
    {
        List<Task<BPList?>> tasks = new();
        
        foreach (int id in Playlists.UnfilteredBeatSaverPlaylists.Values)
        {
            Task<BPList?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
            tasks.Add(t);
        }
        
        // todo: Generate spotify playlists

        Task.WaitAll(tasks.Cast<Task>().ToArray());

        return tasks.Where(t => t.Result != null).Cast<Task<BPList>>().Select(task => task.Result);
    }

    private static void FilterOutput(Task<Beatmap?[]> allBpLists)
    {
        // foreach (var  in allBpLists)
        // {
        //     for (int i = bpList.songs.Count; i >= 0; i--)
        //     {
        //         if(bpList.songs[i].)
        //     }
        // }
    }

    private static Task<Beatmap?[]> DownloadBeatmaps(IEnumerable<BPList> bpLists, BeatSaver beatSaverApi)
    {
        List<Task<Beatmap?>> beatmapDownloadTasks = new();
        
        foreach (BPList bpList in bpLists)
        {
            Console.WriteLine("\n" + bpList.playlistTitle);
            Console.WriteLine("============================================");

            foreach (SongInfo songInfo in bpList.songs)
            {
                Console.WriteLine(songInfo.songName);
                beatmapDownloadTasks.Add(beatSaverApi.BeatmapByHash(songInfo.hash));
            }
        }

        return Task.WhenAll(beatmapDownloadTasks);
    }

    private static async Task DownloadZipFiles(Task<Beatmap?[]> downloadUnfilteredBeatMaps)
    {
        List<Task> zipFileDownloadAndUnzipTasks = new();
        foreach (Beatmap? beatmap in downloadUnfilteredBeatMaps.Result)
        {
            if (beatmap != null)
            {
                zipFileDownloadAndUnzipTasks.Add(DownloadAndUnzipZipFile(beatmap));
            }
        }

        await Task.WhenAll(zipFileDownloadAndUnzipTasks.ToArray());
    }

    private static async Task DownloadAndUnzipZipFile(Beatmap beatmap)
    {
        var downloadZipContents = beatmap.LatestVersion.DownloadZIP();
        await downloadZipContents;
        
        if (downloadZipContents.Result != null)
        {
            string zipFilePath = FileManager.GetZipFilePath(beatmap);
            await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result);
            FileManager.UnzipFile(zipFilePath, out var mapFolderPath);
            Console.WriteLine("Unzipped map to: " + mapFolderPath);
        }   
    }
}