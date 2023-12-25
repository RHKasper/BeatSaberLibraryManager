using BeatSaverSharp.Models;

namespace BeatSaberLibraryManager.MapEvaluation
{
	/// <summary>
	/// todo: cleanup the duplicate methods here
	/// </summary>
	public static class MapFilters
	{
		public const float MaxNps = 6.5f;
		public const float MinScore = .7f;
		public const float MinScoreForManyRatings = .8f;

		public static readonly BeatmapDifficulty.BeatSaverBeatmapDifficulty[] AcceptableSpotifyDifficulties = 
		{
			BeatmapDifficulty.BeatSaverBeatmapDifficulty.Hard, 
			BeatmapDifficulty.BeatSaverBeatmapDifficulty.Expert,
			BeatmapDifficulty.BeatSaverBeatmapDifficulty.ExpertPlus
		};

		public static bool FailsAnyQualityFilter(this Doc d)
		{
			Version v = d.GetLatestVersion();
			return d.IsPoorlyRated() || v.HasTooManyParityErrors() || v.NpsIsTooHigh();
		}
		
		public static bool HasTooManyParityErrors(this Version v, int maxParityErrors = 5)
		{
			foreach (Diff diff in v.diffs)
			{
				if(diff.difficulty == Diff.Expert || diff.difficulty == Diff.ExpertPlus)
					if (diff.paritySummary.errors > maxParityErrors)
						return true;
			}
			return false;
		}

		private static bool NpsIsTooHigh(this Version version)
		{
			return version.diffs.All(d => d.nps > MaxNps);
		}

		public static bool IsPoorlyRated(this Doc doc)
		{
			bool hasManyRatings = doc.stats.upvotes + doc.stats.downvotes > 50;

			if (hasManyRatings)
				return doc.stats.score < MinScoreForManyRatings;
			else
				return doc.stats.score < MinScore;
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

		public static bool NpsIsTooHigh(this BeatmapVersion version)
		{
			return version.Difficulties.All(d => d.NPS > MaxNps);
		}

		public static bool IsPoorlyRated(this Beatmap beatmap)
		{
			bool hasManyRatings = beatmap.Stats.Upvotes + beatmap.Stats.Downvotes > 50;

			if (hasManyRatings)
				return beatmap.Stats.Score < MinScoreForManyRatings;
			else
				return beatmap.Stats.Score < MinScore;
		}

		public static bool HasAnyOfRequestedDifficulties(this BeatmapVersion v, BeatmapDifficulty.BeatSaverBeatmapDifficulty[] desiredDiffs) => v.Difficulties.Any(d => desiredDiffs.Any(desired => d.Difficulty == desired));
	}
}