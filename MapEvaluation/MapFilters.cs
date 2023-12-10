using BeatSaverSharp.Models;
using SpotifyAPI.Web;

namespace BeatSaberLibraryManager.MapEvaluation
{
	/// <summary>
	/// todo: cleanup the duplicate methods here
	/// </summary>
	public static class MapFilters
	{
		public static bool FailsAnyQualityFilter(this Doc d)
		{
			Version v = d.GetLatestVersion();
			return d.IsPoorlyRated() || v.HasTooManyParityErrors() || v.NpsIsTooHigh();
		}
		
		public static bool HasTooManyParityErrors(this Version v, int maxParityErrors = 5)
		{
			foreach (Diff diff in v.diffs)
			{
				if(diff.difficulty != Diff.Normal && diff.difficulty != Diff.Easy)
					if (diff.paritySummary.errors > maxParityErrors)
						return true;
			}
			return false;
		}

		public static bool NpsIsTooHigh(this Version version, float maxNps = 6.5f)
		{
			return version.diffs.All(d => d.nps > maxNps);
		}

		public static bool IsPoorlyRated(this Doc doc)
		{
			bool hasManyRatings = doc.stats.upvotes + doc.stats.downvotes > 50;

			if (hasManyRatings)
				return doc.stats.score < .8f;
			else
				return doc.stats.score < .7f;
		}
		
		public static bool FailsAnyQualityFilter(this Beatmap map)
		{
			BeatmapVersion v = map.LatestVersion;
			return map.IsPoorlyRated() || v.HasTooManyParityErrors() || v.NpsIsTooHigh();
		}
		
		public static bool HasTooManyParityErrors(this BeatmapVersion v, int maxParityErrors = 5)
		{
			foreach (BeatmapDifficulty diff in v.Difficulties)
			{
				if(diff.Difficulty != BeatmapDifficulty.BeatSaverBeatmapDifficulty.Normal && diff.Difficulty != BeatmapDifficulty.BeatSaverBeatmapDifficulty.Easy)
				{
					if (diff.Parity.Errors > maxParityErrors)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool NpsIsTooHigh(this BeatmapVersion version, float maxNps = 6.5f)
		{
			return version.Difficulties.All(d => d.NPS > maxNps);
		}

		public static bool IsPoorlyRated(this Beatmap beatmap)
		{
			bool hasManyRatings = beatmap.Stats.Upvotes + beatmap.Stats.Downvotes > 50;

			if (hasManyRatings)
				return beatmap.Stats.Score < .8f;
			else
				return beatmap.Stats.Score < .7f;
		}

		public static bool HasAnyOfRequestedDifficulties(this Version v, string[] desiredDiffs) => v.diffs.Any(d => desiredDiffs.Any(desired => d.difficulty == desired));

		public static bool ContainsArtistName(this Doc doc, FullTrack fullTrack)
		{
			string[] artistNameWords = MapEvalUtils.FilterToJustAlphaNumerics(fullTrack.Artists.First().Name).Split(' ');
			int artistNameWordsFound = doc.FindWordsInMapName(artistNameWords);
			bool containsArtistName = artistNameWordsFound > .7f * artistNameWords.Length;

			return containsArtistName;
		}
	}
}