using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class TeamStanding
	{
		public string TeamName { get; set; }
		public int Points { get; set; }

		public TeamStanding(string teamName, IEnumerable<Game> groupGames)
		{
			this.TeamName = teamName;
			var finalTeamGames = groupGames.Where(x => x.HasTeam(this.TeamName) && x.IsFinal).ToList();
			this.Points = finalTeamGames == null || finalTeamGames.Count == 0
								? 0
								: finalTeamGames.Sum(x => x.GetTeamPointsFromGame(this.TeamName) ?? 0);
		}

		private TeamStanding(string teamName, int points)
		{
			this.TeamName = teamName;
			this.Points = points;
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
		}
	}
}
