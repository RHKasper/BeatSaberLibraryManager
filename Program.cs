using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;
using BeatSaberLibraryManager.MapEvaluation;
using BeatSaberLibraryManager.Outputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;

namespace BeatSaberLibraryManager;

public class Program
{
    private const int ZipDownloadTimeoutSeconds = 20;
    
    public static async Task Main()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        BeatSaver beatSaverApi = new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));

        var getAllFilteredBpLists = GetFilteredBpLists(beatSaverApi);
        var getAllUnfilteredBpLists = GetUnfilteredBpLists(beatSaverApi);
        await getAllFilteredBpLists;
        await getAllUnfilteredBpLists;
        
        var downloadUnfilteredBeatmaps = DownloadBeatmaps(getAllUnfilteredBpLists.Result, beatSaverApi);
        var downloadFilteredBeatmaps = DownloadBeatmaps(getAllFilteredBpLists.Result, beatSaverApi);
        await downloadUnfilteredBeatmaps;
        await downloadFilteredBeatmaps;
        List<Beatmap> unfilteredBeatmaps = downloadUnfilteredBeatmaps.Result.Where(b => b != null).Cast<Beatmap>().ToList();
        List<Beatmap> filteredBeatmaps = downloadFilteredBeatmaps.Result.Where(b => b != null).Cast<Beatmap>().ToList();
        
        FilterOutput(filteredBeatmaps);
        
        FileManager.PrepareMapZipCacheDirectory();
        FileManager.PrepareOutputDirectories();
        
        await DownloadZipFiles(unfilteredBeatmaps.Concat(filteredBeatmaps).ToHashSet());

        Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
    }

    private static async Task<IEnumerable<BPList>> GetFilteredBpLists(BeatSaver beatSaverApi)
    {
        List<Task<BPList?>> tasks = new();

        //Download all beatsaver playlists
        foreach (int id in Playlists.FilteredBeatSaverPlaylists.Values)
        {
            Task<BPList?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
            tasks.Add(t);
        }
        
        // //Download all beatsaver mapper playlists
        // foreach (int id in Playlists.BeatSaverMapperPlaylists.Values)
        // {
        //     Task<BPList?> t = HighLevelTasks.GetBeatSaverMapperPlaylist(id, beatSaverApi);
        //     tasks.Add(t);
        // }
        
        //Download all non-beatsaver BPLists
        var webPlaylistUrls = Playlists.BeastSaberPlaylists.Values;
        foreach (string url in webPlaylistUrls)
        {
            Task<BPList?> t = HighLevelTasks.GetWebBpList(url);
            tasks.Add(t);
        }

        foreach (Task<BPList> task in tasks)
        {
            await task;
        }

        Debug.Assert(tasks.TrueForAll(t => t.IsCompletedSuccessfully));
        Debug.Assert(tasks.TrueForAll(t => t.Result != null));
        
        return tasks.Where(t => t.Result != null).Cast<Task<BPList>>().Select(task => task.Result);
    }

    private static async Task<IEnumerable<BPList>> GetUnfilteredBpLists(BeatSaver beatSaverApi)
    {
        List<Task<BPList?>> tasks = new();
        
        foreach (int id in Playlists.UnfilteredBeatSaverPlaylists.Values)
        {
            Task<BPList?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
            tasks.Add(t);
        }
        
        // todo: Generate spotify playlists

        await Task.WhenAll(tasks.Cast<Task>().ToArray());
        await Task.Delay(5000);
        
        Debug.Assert(tasks.TrueForAll(t => t.IsCompletedSuccessfully));
        Debug.Assert(tasks.TrueForAll(t => t.Result != null));
        
        return tasks.Where(t => t.Result != null).Cast<Task<BPList>>().Select(task => task.Result);
    }

    private static void FilterOutput(List<Beatmap> beatmaps)
    {
        for (var i = beatmaps.Count - 1; i >= 0; i--)
        {
            if (beatmaps[i].FailsAnyQualityFilter())
            {
                Console.WriteLine("Filtering out: " + beatmaps[i].Name + " - " + beatmaps[i].Stats.Score + "; " + beatmaps[i].LatestVersion.Difficulties.Most(d => d.Parity.Errors).Parity.Errors + " parity errors");
                beatmaps.RemoveAt(i);
            }
        }
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
                Console.WriteLine(songInfo.songName + " - " + songInfo.hash);
                beatmapDownloadTasks.Add(beatSaverApi.BeatmapByHash(songInfo.hash));
            }
        }

        return Task.WhenAll(beatmapDownloadTasks);
    }

    private static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        List<Task> zipFileDownloadAndUnzipTasks = new();
        foreach (Beatmap beatmap in beatmaps)
        {
            Console.WriteLine("Requesting download for " + beatmap.Name);
            zipFileDownloadAndUnzipTasks.Add(DownloadAndUnzipZipFile(beatmap, cancellationTokenSource.Token));
        }

        Console.WriteLine("Requested " + beatmaps.Count + " map zip files");
        
        int secondsWaited = 0;
        while (zipFileDownloadAndUnzipTasks.Any() && secondsWaited < ZipDownloadTimeoutSeconds)
        {
            Console.WriteLine("waiting on " + zipFileDownloadAndUnzipTasks.Count(t => !t.IsCompletedSuccessfully) + " zip download tasks");
            await Task.Delay(1000);
            secondsWaited += 1;
        }

        //todo: retry downloads that didn't successfully finish?
        cancellationTokenSource.Cancel();
    }

    private static async Task DownloadAndUnzipZipFile(Beatmap beatmap, CancellationToken cancellationToken)
    {
        var downloadZipContents = beatmap.LatestVersion.DownloadZIP(cancellationToken);

        while (cancellationToken.IsCancellationRequested == false && downloadZipContents.IsCompletedSuccessfully == false)
        {
            await Task.Delay(250, cancellationToken);
        }

        if (downloadZipContents is { IsCompletedSuccessfully: true, Result: not null })
        {
            Console.WriteLine("Downloaded .zip byte[] for " + beatmap.Name);
            
            string zipFilePath = FileManager.GetZipFilePath(beatmap);
            await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result, cancellationToken);
            FileManager.UnzipFile(zipFilePath, out var mapFolderPath);
            Console.WriteLine("Unzipped map to: " + mapFolderPath);
        }
        else
        {
            Console.WriteLine("Failed to download zip file for " + beatmap.Name);
        }
    }
}