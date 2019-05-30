using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class BasicTeamStanding : IComparable<BasicTeamStanding>
	{
		public string TeamName { get; private set; }
		public int Points { get; private set; }
		public int GoalsScored { get; private set; }
		public int GoalsAllowed { get; private set; }
		public int GamesPlayed { get; private set; }
		public int FutureGamesAdded { get; private set; }

		public int GoalDifference => this.GoalsScored - this.GoalsAllowed;

		public BasicTeamStanding(string teamName, IEnumerable<Game> groupGames)
		{
			this.InitializeObject(teamName);
			var finalTeamGames = groupGames.Where(x => x.HasTeam(this.TeamName) && x.IsFinal).ToList();
			if (finalTeamGames != null && finalTeamGames.Count > 0)
			{
				foreach (var finalTeamGame in finalTeamGames)
				{
					this.GamesPlayed++;
					this.Points += finalTeamGame.GetTeamPointsFromGame(this.TeamName).Value;
					this.GoalsScored += finalTeamGame.GetTeamGoalsScored(this.TeamName).Value;
					this.GoalsAllowed += finalTeamGame.GetOpponentGoalsScored(this.TeamName).Value;
				}
			}
		}

		private BasicTeamStanding(string teamName, int points)
		{
			this.InitializeObject(teamName);
			this.Points = points;
		}

		private void InitializeObject(string teamName)
		{
			this.TeamName = teamName;
			this.Points = 0;
			this.GoalsScored = 0;
			this.GoalsAllowed = 0;
			this.FutureGamesAdded = 0;
			this.GamesPlayed = 0;
		}

		public override string ToString()
		{
			return $"{this.TeamName}-{this.Points}";
		}

		private const int TEAM_IS_BETTER = -1;
		private const int OTHER_IS_BETTER = 1;
		private const int TEAMS_TIED = 0;
		public int CompareTo(BasicTeamStanding other)
		{
			bool includeTiebreakers = this.FutureGamesAdded == 0 && other.FutureGamesAdded == 0;
			if (this.Points > other.Points)
			{
				return TEAM_IS_BETTER;
			}
			else if (this.Points < other.Points)
			{
				return OTHER_IS_BETTER;
			}
			else if (!includeTiebreakers)
			{
				return TEAMS_TIED;
			}

			if (this.GoalDifference > other.GoalDifference)
			{
				return TEAM_IS_BETTER;
			}
			else if (this.GoalDifference < other.GoalDifference)
			{
				return OTHER_IS_BETTER;
			}

			if (this.GoalsScored > other.GoalsScored)
			{
				return TEAM_IS_BETTER;
			}
			else if (this.GoalsScored < other.GoalsScored)
			{
				return OTHER_IS_BETTER;
			}

			return TEAMS_TIED;
		}

		public static BasicTeamStanding CopyTeamStanding(BasicTeamStanding other)
		{
			return new BasicTeamStanding(other.TeamName, other.Points);
		}

		public void AddResult(Game game)
		{
			if (!game.IsFinal) { return; }
			this.Points += game.GetTeamPointsFromGame(this.TeamName).Value;
			this.GoalsScored += game.GetTeamGoalsScored(this.TeamName).Value;
			this.GoalsAllowed += game.GetOpponentGoalsScored(this.TeamName).Value;
			this.FutureGamesAdded++;
		}
	}
}
