using BeatSaberLibraryManager.Utils;

namespace BeatSaberLibraryManager.Tasks;

public class BeatSaverMapDownloadTask : MyTask
{
	public MapData? DownloadedMapData { get; private set; }
	private int taskNumber;

	private readonly string _mapHash;
	private static int taskCount = 0;

	public BeatSaverMapDownloadTask(string mapHash)
	{
		taskNumber = taskCount;
		taskCount++;
		_mapHash = mapHash;
	}
	
	public override void Execute()
	{
		Console.WriteLine("Started Task: " + nameof(BeatSaverMapDownloadTask) + " #" + taskNumber);
		DownloadUtil.GetMapData(_mapHash, mapData =>
		{
			DownloadedMapData = mapData;
			HandleExecutionFinished();
		});
	}

	protected override void OnFinishedInternal()
	{
		Console.WriteLine("Downloaded map: " + DownloadedMapData.name);
	}
}