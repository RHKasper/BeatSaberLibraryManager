using BeatSaberLibraryManager.Tasks;

Console.WriteLine("Hello, World!");


Queue<MyTask> queuedTasks = new Queue<MyTask>();
HashSet<MyTask> runningTasks = new HashSet<MyTask>();

void OnTaskFinished(MyTask task) => runningTasks.Remove(task);

//Enqueue initial tasks
queuedTasks.Enqueue(new BeatSaverMapDownloadTask("75879e3d3bdb21d6997c93b7fc4937cfb70546ab"));

while (queuedTasks.Any() || runningTasks.Any())
{
	Console.WriteLine("Waiting on " + queuedTasks.Count + " queued tasks and " + runningTasks.Count + " running tasks");
	Thread.Sleep(150);
	
	while (queuedTasks.Any())
	{
		var task = queuedTasks.Dequeue();

		task.OnFinished += OnTaskFinished;
		runningTasks.Add(task);
		task.Execute();
	}
}