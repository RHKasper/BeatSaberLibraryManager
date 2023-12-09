using BeatSaberLibraryManager.WebDownload;
using Newtonsoft.Json;

namespace BeatSaberLibraryManager;

/// <summary>
/// Todo: figure out what to name this file.
/// </summary>
public static class HighLevelTasks
{
	public static async Task<BPList> GetWebBpListAsync(string bpListUrl)
	{
		Console.WriteLine("Starting " + nameof(GetWebBpListAsync));
		Task<BPList> downloadBpListFileTask = BeatSaverDownloadManager.DownloadBpListFileAsync(bpListUrl);
		await downloadBpListFileTask;
		Console.WriteLine("Downloaded BPList: " + downloadBpListFileTask.Result.playlistTitle);

		Task<MapData>[] tasks = new Task<MapData>[downloadBpListFileTask.Result.songs.Count];
		for (int i = 0; i < downloadBpListFileTask.Result.songs.Count; i++)
		{
			tasks[i] = BeatSaverDownloadManager.GetMapDataAsync(downloadBpListFileTask.Result.songs[i]);
		}

		Task.WaitAll(tasks);
		
		// filter bplist
		
		Console.WriteLine("Finished " + nameof(GetWebBpListAsync));

		foreach (Task<MapData> task in tasks)
		{
			Console.WriteLine("Downloaded map: " + task.Result.name);
		}
		
		return downloadBpListFileTask.Result;
	}

	// public static async Task<BPList> GenerateBpListFromSpotifyPlaylistAsync(string spotifyPlaylistUrl)
	// {
	// 	Console.WriteLine("Starting " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
	// 	await Task.Delay(1000);
	// 	Console.WriteLine("Finished " + nameof(GenerateBpListFromSpotifyPlaylistAsync));
	// 	return new BPList();
	// }

	
	
	// public static async Task<T[]> FilterAsync<T>(IEnumerable<T> sourceEnumerable, Func<T, Task<bool>> predicateAsync)
	// {
	// 	return (await Task.WhenAll(
	// 			sourceEnumerable.Select(
	// 				v => predicateAsync(v)
	// 					.ContinueWith(task => new { Predicate = task.Result, Value = v })))
	// 		).Where(a => a.Predicate).Select(a => a.Value).ToArray();
	// }
}