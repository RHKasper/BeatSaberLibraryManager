using Newtonsoft.Json;

namespace BeatSaberLibraryManager.WebDownload;

/// <summary>
/// This class is responsible for managing download requests from beatsaver.com to avoid making too many requests at once.
/// </summary>
public static class BeatSaverDownloadManager
{
	public static async Task<MapData> GetMapDataAsync(SongInfo songInfo)
	{
		string mapUrl = "https://api.beatsaver.com/maps/hash/" + songInfo.hash;
		Task<string> downloadTask = DownloadUtil.Get(mapUrl);
		await downloadTask;
		Console.WriteLine("MapData file downloaded");
		return JsonConvert.DeserializeObject<MapData>(downloadTask.Result) ?? throw new InvalidOperationException();
	}
	
	public static async Task<BPList> DownloadBpListFileAsync(string bpListUrl)
	{
		Task<string> downloadTask = DownloadUtil.Get(bpListUrl);
		await downloadTask;
		Console.WriteLine("BP List file downloaded");
		return JsonConvert.DeserializeObject<BPList>(downloadTask.Result) ?? throw new InvalidOperationException();
	}
}