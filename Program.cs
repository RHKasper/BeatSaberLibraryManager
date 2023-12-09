using BeatSaberLibraryManager.Inputs;
using BeatSaberLibraryManager.Tasks;

namespace BeatSaberLibraryManager;

public class Program
{
	public static async Task Main()
	{
		await BeatSaverDownloadManager.DownloadWebBpListAndSongsAsync(Playlists.BpListUrls.First().Value);

		// Queue<MyTask> queuedTasks = new Queue<MyTask>();
		// HashSet<MyTask> runningTasks = new HashSet<MyTask>();
		//
		// void OnTaskFinished(MyTask task) => runningTasks.Remove(task);
		//
		// //Enqueue initial tasks
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		// queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));
		//
		// while (queuedTasks.Any() || runningTasks.Any())
		// {
		// 	while (queuedTasks.Any())
		// 	{
		// 		var task = queuedTasks.Dequeue();
		//
		// 		task.OnFinished += OnTaskFinished;
		// 		runningTasks.Add(task);
		// 		ThreadPool.QueueUserWorkItem(_ => task.Execute());
		// 	}
		// 	
		// 	Console.WriteLine("Waiting on " + queuedTasks.Count + " queued tasks and " + runningTasks.Count + " running tasks");
		// 	Thread.Sleep(150);
		// }
	}

	public static void EnqueueBpListDownloadTasks(Queue<MyTask> taskQueue)
	{
		foreach (string url in Playlists.BpListUrls.Values)
		{
			
		}
	}
}