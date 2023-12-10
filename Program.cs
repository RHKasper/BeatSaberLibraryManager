using System.Diagnostics;
using BeatSaberLibraryManager.Inputs;
using BeatSaberLibraryManager.WebDownload;
using BeatSaverSharp;
using BeatSaverSharp.Models;
using BeatSaverSharp.Models.Pages;

namespace BeatSaberLibraryManager;

public class Program
{
	public static Task Main()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		BeatSaver beatSaverApi =  new(nameof(BeatSaberLibraryManager), new System.Version(0, 1));

		List<Task<Playlist?>> tasks = new(Playlists.BeatSaverPlaylists.Count + Playlists.SpotifyPlaylistUrls.Count);
		
		
		 foreach (int id in Playlists.BeatSaverPlaylists.Values)
		 {
			 Task<Playlist?> t = HighLevelTasks.GetBeatSaverPlaylist(id, beatSaverApi);
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

		foreach (Task<Playlist> task in tasks)
		{
			Console.WriteLine(task.Result.Name);
		}
		
		//todo: export playlists 
		Console.WriteLine("All tasks complete in " + stopwatch.ElapsedMilliseconds / 1000f + " seconds");
		
		return Task.CompletedTask;
	}
}