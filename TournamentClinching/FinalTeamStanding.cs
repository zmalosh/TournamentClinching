using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class FinalTeamStanding
	{
		public string TeamName { get; private set; }
		public string GroupName { get; private set; }
		public int Place { get; private set; }
		public int Wins { get; private set; }
		public int Draws { get; private set; }
		public int Losses { get; private set; }
		public int GoalsScored { get; private set; }
		public int GoalsAgainst { get; private set; }
		public bool HasClinchedAdvancement { get; private set; }
		public bool HasBeenEliminated { get; private set; }
		public int BestGroupPlace { get; private set; }
		public int WorstGroupPlace { get; private set; }
		public int MaxPoints { get; private set; }
		public int MinPoints { get; private set; }
		public int GamesPlayed => this.Wins + this.Draws + this.Losses;
		public int Points => (this.Wins * 3) + this.Draws;
		public int GoalDifference => this.GoalsScored - this.GoalsAgainst;

		public FinalTeamStanding(
			string teamName,
			string groupName,
			int place,
			int wins,
			int draws,
			int losses,
			int goalsScored,
			int goalsAgainst,
			bool hasClinchedAdvancement,
			bool hasBeenEliminated,
			int bestGroupPlace,
			int worstGroupPlace,
			int minPoints,
			int maxPoints)
		{
			this.TeamName = teamName;
			this.GroupName = groupName;
			this.Place = place;
			this.Wins = wins;
			this.Draws = draws;
			this.Losses = losses;
			this.GoalsScored = goalsScored;
			this.GoalsAgainst = goalsAgainst;
			this.HasClinchedAdvancement = hasClinchedAdvancement;
			this.HasBeenEliminated = hasBeenEliminated;
			this.BestGroupPlace = bestGroupPlace;
			this.WorstGroupPlace = worstGroupPlace;
			this.MaxPoints = maxPoints;
			this.MinPoints = minPoints;
		}
	}
}
