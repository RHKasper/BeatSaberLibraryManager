using System.Collections.Concurrent;
using BeatSaberLibraryManager.Utils;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager;

public static class BeatSaverDownloadManager
{
	public static async Task<BPList> GetWebBpListAsync(string bpListUrl)
	{
		Console.WriteLine("Starting " + nameof(GetWebBpListAsync));
		Task<BPList> downloadBpListFileTask = DownloadBpListFileAsync(bpListUrl);
		await downloadBpListFileTask;
		Console.WriteLine("Downloaded BPList: " + downloadBpListFileTask.Result.playlistTitle);

		Task<MapData>[] tasks = new Task<MapData>[downloadBpListFileTask.Result.songs.Count];
		for (int i = 0; i < downloadBpListFileTask.Result.songs.Count; i++)
		{
			tasks[i] = GetMapDataAsync(downloadBpListFileTask.Result.songs[i]);
		}

		Task.WaitAll(tasks);
		
		// filter bplist
		
		Console.WriteLine("Finished " + nameof(GetWebBpListAsync));
		return downloadBpListFileTask.Result;
	}

	public static async Task<BPList> GenerateBpListFromSpotifyPlaylistAsync(string spotifyPlaylistUrl)
	{
		Console.WriteLine("Starting " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
		await Task.Delay(1000);
		Console.WriteLine("Finished " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
		return new BPList();
	}

	private static async Task<BPList> DownloadBpListFileAsync(string bpListUrl)
	{
		Task<string> downloadTask = DownloadUtil.Get(bpListUrl);
		await downloadTask;
		Console.WriteLine("BP List file downloaded");
		return JsonConvert.DeserializeObject<BPList>(downloadTask.Result) ?? throw new InvalidOperationException();
	}

	private static async Task<MapData> GetMapDataAsync(SongInfo songInfo)
	{
		//trigger map download and wait for it to get back.
		await Task.Delay(1000);
		Console.WriteLine("MapData file downloaded");
		return new MapData();
	}
	
	public static async Task<T[]> FilterAsync<T>(IEnumerable<T> sourceEnumerable, Func<T, Task<bool>> predicateAsync)
	{
		return (await Task.WhenAll(
				sourceEnumerable.Select(
					v => predicateAsync(v)
						.ContinueWith(task => new { Predicate = task.Result, Value = v })))
			).Where(a => a.Predicate).Select(a => a.Value).ToArray();
	}
}