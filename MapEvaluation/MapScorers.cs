using BeatSaverSharp.Models;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.MapEvaluation
{
	public static class MapScorers
	{
		public static double ScoreOverall(this Beatmap? beatmap, FullTrack fullTrack)
		{
			return beatmap.ScoreOnRating() * beatmap.ScoreOnSongNameMatch(fullTrack);
		}
		
		public static double ScoreOnSongNameMatch(this Beatmap? beatmap, FullTrack fullTrack)
		{
			string[] trackNameWords = MapEvalUtils.FilterToJustAlphaNumerics(fullTrack.Name).Split(' ');
			int trackNameWordsFound = beatmap.FindWordsInMapName(trackNameWords);
			double matchScore = trackNameWordsFound / .8f * trackNameWords.Length;
			return matchScore.Remap(0, 1, .75f, 1);
		}

		public static double ScoreOnRating(this Beatmap? beatmap)
		{
			return beatmap.Stats.Score;
		}
	}
}