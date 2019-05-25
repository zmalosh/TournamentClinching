using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class TeamStanding
	{
		public string TeamName { get; private set; }
		public int Points { get; private set; }
		public int GoalsScored { get; private set; }
		public int GoalsAllowed { get; private set; }
		public int GamesPlayed { get; private set; }
		public int FutureGamesAdded { get; private set; }

		public int GoalDifference => this.GoalsScored - this.GoalsAllowed;

		public TeamStanding(string teamName, IEnumerable<Game> groupGames)
		{
			this.Points = 0;
			this.GoalsScored = 0;
			this.GoalsAllowed = 0;
			this.FutureGamesAdded = 0;
			this.TeamName = teamName;
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

		private TeamStanding(string teamName, int points)
		{
			this.TeamName = teamName;
			this.Points = points;
			this.Points = 0;
			this.GoalsScored = 0;
			this.GoalsAllowed = 0;
			this.FutureGamesAdded = 0;
		}

		public override string ToString()
		{
			return $"{this.TeamName}-{this.Points}";
		}

		public static TeamStanding CopyTeamStanding(TeamStanding other)
		{
			return new TeamStanding(other.TeamName, other.Points);
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
