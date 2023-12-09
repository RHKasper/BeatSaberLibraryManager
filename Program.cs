using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models.Pages;

namespace BeatSaberLibraryManager;

public class Program
{
	public static Task Main()
	{
		
		
		
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<Task<PlaylistDetail?>> tasks = new(Playlists.BeatSaverPlaylists.Count + Playlists.SpotifyPlaylistUrls.Count);

		BeatSaver beatSaver = new BeatSaver(nameof(BeatSaberLibraryManager), new System.Version(0, 1));
		
		 foreach (int id in Playlists.BeatSaverPlaylists.Values)
		 {
		 	Task<PlaylistDetail?> t = beatSaver.Playlist(id);
		 	tasks.Add(t);
		 }
		 
		 // foreach (int id in Playlists.BeatSaverMapperPlaylists.Values)
		 // {
			//  Task<PlaylistDetail?> t = beatSaver.(id);
			//  tasks.Add(t);
		 // }
		
		// foreach (string spotifyPlaylistUrl in Playlists.SpotifyPlaylistUrls.Values)
		// {
		// 	Task<BPList> t = BeatSaverDownloadManager.GenerateBpListFromSpotifyPlaylistAsync(spotifyPlaylistUrl);
		// 	tasks.Add(t);
		// }
		
		Task.WaitAll(tasks.Cast<Task>().ToArray());

		foreach (Task<PlaylistDetail?> task in tasks)
		{
			if (task.Result != null)
			{
				Console.WriteLine(task.Result.Playlist.Name);
			}
		}
		
		//todo: export playlists 
		Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
		
		return Task.CompletedTask;
	}
}