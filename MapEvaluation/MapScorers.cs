using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.MapEvaluation
{
	public static class MapScorers
	{
		public static double ScoreOverall(this Doc doc, FullTrack fullTrack)
		{
			return doc.ScoreOnRating() * doc.ScoreOnSongNameMatch(fullTrack);
		}
		
		public static double ScoreOnSongNameMatch(this Doc doc, FullTrack fullTrack)
		{
			string[] trackNameWords = MapEvalUtils.FilterToJustAlphaNumerics(fullTrack.Name).Split(' ');
			int trackNameWordsFound = doc.FindWordsInMapName(trackNameWords);
			double matchScore = trackNameWordsFound / .8f * trackNameWords.Length;
			return matchScore.Remap(0, 1, .75f, 1);
		}

		public static double ScoreOnRating(this Doc doc)
		{
			return doc.stats.score;
		}
	}
}