using BeatSaverSharp;
using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.WebDownload;

public static class MapDownloader
{
    public static async Task<List<Task<Beatmap?>>> DownloadBeatmaps(IEnumerable<BPList> bpLists, BeatSaver beatSaverApi)
    {
        List<Task<Beatmap?>> beatmapDownloadTasks = new();
        
        foreach (BPList bpList in bpLists)
        {
            foreach (SongInfo songInfo in bpList.songs)
            {
                Task<Beatmap?> task = GetBeatmap(songInfo, beatSaverApi);
                beatmapDownloadTasks.Add(task);
                await task;
            }
        }

        return beatmapDownloadTasks;
    }

    public static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps)
    {
        int maxRetries = 4;
        int msBeforeRetry = 5000;
        int waitInterval = 100;
        
        foreach (Beatmap beatmap in beatmaps)
        {
            Console.WriteLine("Requesting download for " + beatmap.Name);
            var task = DownloadAndUnzipZipFile(beatmap);
            
            //todo implement retries
            // int retryCount = 0;
            // int msElapsed = 0;
            //
            // while (task.Result == null || retryCount < maxRetries)
            // {
            //     if (msElapsed > msBeforeRetry)
            //     {
            //         Console.WriteLine("Retrying .zip file download for " + beatmap.Name);
            //         task = DownloadAndUnzipZipFile(beatmap);
            //         retryCount++;
            //         msElapsed = 0;
            //     }
            //     await Task.Delay(waitInterval);
            //     msElapsed += waitInterval;
            // }
            
            await task;
        }

        Console.WriteLine("Requested " + beatmaps.Count + " map zip files");
    }

    private static async Task<string?> DownloadAndUnzipZipFile(Beatmap beatmap)
    {
        Task<byte[]?> downloadZipContents = beatmap.LatestVersion.DownloadZIP();
        await downloadZipContents;

        if (downloadZipContents is { IsCompletedSuccessfully: true, Result: not null })
        {
            Console.WriteLine("Downloaded .zip byte[] for " + beatmap.Name);
            
            string zipFilePath = FileManager.GetZipFilePath(beatmap);
            await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result);
            FileManager.UnzipFile(zipFilePath, out var mapFolderPath);
            Console.WriteLine("Unzipped map to: " + mapFolderPath);
            return mapFolderPath;
        }
        else
        {
            Console.WriteLine("Failed to download zip file for " + beatmap.Name);
        }

        return null;
    }

    private static async Task<Beatmap?> GetBeatmap(SongInfo songInfo, BeatSaver beatSaverApi)
    {
        if (Cache.Beatmaps.ContainsKey(songInfo.hash))
        {
            Console.WriteLine("\tBeatmap found in cache for: " + songInfo.songName + " - " + songInfo.hash);
            return Cache.Beatmaps[songInfo.hash];
        }
        else
        {
            Console.WriteLine("Downloading Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
            Beatmap? beatmap = await beatSaverApi.BeatmapByHash(songInfo.hash);
            if (beatmap != null)
            {
                Cache.CacheBeatmap(songInfo, beatmap);
            }
            Console.WriteLine("\tDownloaded Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
            return beatmap;
        }
    }
}