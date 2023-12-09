using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;

namespace BeatSaberLibraryManager;

public class Program
{
	public static Task Main()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<Task<BPList>> tasks = new List<Task<BPList>>(Playlists.BpListUrls.Count + Playlists.SpotifyPlaylistUrls.Count);
		
		tasks.Add(BeatSaverDownloadManager.GetWebBpListAsync(Playlists.BpListUrls.Values.First()));
		// foreach (string webBpListUrl in Playlists.BpListUrls.Values)
		// {
		// 	Task<BPList> t = BeatSaverDownloadManager.GetWebBpListAsync(webBpListUrl);
		// 	tasks.Add(t);
		// }
		//
		// foreach (string spotifyPlaylistUrl in Playlists.SpotifyPlaylistUrls.Values)
		// {
		// 	Task<BPList> t = BeatSaverDownloadManager.GenerateBpListFromSpotifyPlaylistAsync(spotifyPlaylistUrl);
		// 	tasks.Add(t);
		// }
		
		Task.WaitAll(tasks.Cast<Task>().ToArray());
		//todo: export playlists 
		Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
		
		return Task.CompletedTask;
	}
}