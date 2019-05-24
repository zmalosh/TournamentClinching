using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class Scenario
	{
		public string Key { get; set; }
		public List<TeamStanding> TeamStandings { get; private set; }

		public Scenario(string key, List<TeamStanding> baseTeamStandings, List<Game> futureGames)
		{
			this.Key = key;
			this.TeamStandings = baseTeamStandings.Select(x => TeamStanding.CopyTeamStanding(x)).ToList();
			foreach (var team in this.TeamStandings)
			{
				foreach (var game in futureGames)
				{
					if (game.HasTeam(team.TeamName))
					{
						team.AddResult(game);
					}
				}
			}
		}

		public override string ToString()
		{
			return this.Key;
		}
	}
}
