namespace BeatSaberLibraryManager.Tasks;

public abstract class MyTask
{
	public Action<MyTask>? OnFinished;
	public abstract void Execute();

	protected void HandleExecutionFinished()
	{
		OnFinishedInternal();
		OnFinished?.Invoke(this);
	}

	protected abstract void OnFinishedInternal();
}