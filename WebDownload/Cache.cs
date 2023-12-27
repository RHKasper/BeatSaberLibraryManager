using BeatSaverSharp.Models;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager.WebDownload;

public static class Cache
{
    public static Dictionary<string, BPList> PreFilterBpLists {get; private set;}
    public static Dictionary<string, Beatmap> Beatmaps {get; private set;}
    public static Dictionary<string, Beatmap?> SpotifySearchResults { get; private set; }

    public static void InitializeCache()
    {
        PreFilterBpLists = FileManager.GetCachedPreFilterBpLists();
        Beatmaps = FileManager.GetCachedBeatmaps();
    }
    
    public static void CacheBeatmap(SongInfo songInfo, Beatmap beatmap)
    {
        Beatmaps[songInfo.hash] = beatmap;
        string path = Path.Combine(FileManager.beatmapsCacheFolderPath, songInfo.hash + ".beatmap");
        File.WriteAllText(path, JsonConvert.SerializeObject(beatmap));
    }
    
    public static void CacheBpListPreFilter(string name, BPList bpList)
    {
        PreFilterBpLists[name] = bpList;
        string path = Path.Combine(FileManager.preFilterBpListsCacheFolderPath, name.SanitizeForFileName() + ".bplist");
        File.WriteAllText(path, JsonConvert.SerializeObject(bpList));
    }

}