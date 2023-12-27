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

    public static async Task DownloadZipFiles(HashSet<Beatmap> beatmaps, BeatSaver beatSaverClient)
    {
        Console.WriteLine("Requesting " + beatmaps.Count + " map zip files");

        foreach (Beatmap beatmap in beatmaps)
        {
            try
            {
                await DownloadAndUnzipZipFile(beatmap, beatSaverClient);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
        }
    }

    private static async Task DownloadAndUnzipZipFile(Beatmap beatmap, BeatSaver beatSaver)
    {
        string mapDirectory = FileManager.GetMapDirectory(beatmap);
        
        if (Cache.MapFolders.TryGetValue(beatmap.ID, out var cacheFolder))
        {
            Console.WriteLine("\tFound map folder in cache: " + beatmap.Name);
            FileManager.CopyDirectory(cacheFolder, mapDirectory);
        }
        else
        {
            Console.WriteLine("Downloading .zip byte[] for " + beatmap.Name);

            Task<byte[]?> downloadZipContents = beatSaver.DownloadZIP(beatmap.LatestVersion);
            await downloadZipContents;

            if (downloadZipContents is { IsCompletedSuccessfully: true, Result: not null })
            {
                string zipFilePath = FileManager.GetZipFilePath(beatmap);
                Console.WriteLine("\tDownloaded .zip byte[] for " + beatmap.Name);
                await File.WriteAllBytesAsync(zipFilePath, downloadZipContents.Result);
                FileManager.UnzipFile(zipFilePath, mapDirectory);
                Console.WriteLine("\tUnzipped map to: " + mapDirectory);
                Cache.CacheMapFolder(mapDirectory);
            }
            else
            {
                Console.WriteLine("Failed to download zip file for " + beatmap.Name);
            }
        }
    }

    private static async Task<Beatmap?> GetBeatmap(SongInfo songInfo, BeatSaver beatSaverApi)
    {
        Beatmap? beatmap = null;
        try
        {
            if (Cache.Beatmaps.TryGetValue(songInfo.hash, out beatmap))
            {
                Console.WriteLine("\tBeatmap found in cache for: " + songInfo.songName + " - " + songInfo.hash);
            }
            else
            {
                Console.WriteLine("Downloading Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
                beatmap = await beatSaverApi.BeatmapByHash(songInfo.hash);
                if (beatmap != null)
                {
                    Cache.CacheBeatmap(songInfo, beatmap);
                }

                Console.WriteLine("\tDownloaded Beatmap for: " + songInfo.songName + " - " + songInfo.hash);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
        }
        return beatmap;
    }
}