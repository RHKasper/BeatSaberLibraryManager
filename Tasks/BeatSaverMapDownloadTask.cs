using BeatSaberLibraryManager.Utils;

namespace BeatSaberLibraryManager.Tasks;

public class BeatSaverMapDownloadTask : MyTask
{
	public MapData? DownloadedMapData { get; private set; }
	
	private readonly string _mapHash;

	public BeatSaverMapDownloadTask(string mapHash)
	{
		_mapHash = mapHash;
	}
	
	public override void Execute()
	{
		BeatSaverDownloadUtil.GetMapData(_mapHash, mapData =>
		{
			DownloadedMapData = mapData;
			OnExecutionFinished();
		});
	}

	protected override void OnFinishedInternal()
	{
		Console.WriteLine("Downloaded map: " + DownloadedMapData.name);
	}
}