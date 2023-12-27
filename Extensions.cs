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

	public static string SanitizeForFileName(this string originalFileName)
	{
		var invalids = Path.GetInvalidFileNameChars();
		var newName = String.Join("_", originalFileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries) ).TrimEnd('.');
		return newName;
	}
}