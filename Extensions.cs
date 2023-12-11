namespace BeatSaberLibraryManager;

public static class Extensions
{
	public static async Task AwaitAll(this IEnumerable<Task> tasks)
	{
		foreach (Task task in tasks)
		{
			await task;
		}
	}
}