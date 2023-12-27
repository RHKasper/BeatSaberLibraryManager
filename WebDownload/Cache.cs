using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.WebDownload;

public static class Cache
{
    public static IReadOnlyList<BPList> PreFilterBpListsUnfiltered {get; private set;}
    public static IReadOnlyList<BPList> PreFilterBpListsFiltered {get; private set;}
    public static Dictionary<string, Beatmap> Beatmaps {get; private set;}

    public static void InitializeCache()
    {
        var cachedPreFilterBpLists = FileManager.GetCachedPreFilterBpLists();
        PreFilterBpListsFiltered = cachedPreFilterBpLists.filtered.AsReadOnly();
        PreFilterBpListsUnfiltered = cachedPreFilterBpLists.unfiltered.AsReadOnly();

        Beatmaps = FileManager.GetCachedBeatmaps().ToHashSet().ToDictionary(beatmap => beatmap.ID);
    }
}