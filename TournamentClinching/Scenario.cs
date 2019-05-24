﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class Scenario
	{
		public string Key { get; set; }
		public List<ScenarioTeamOutcome> TeamOutcomes { get; private set; }

		public Scenario(string key, List<TeamStanding> baseTeamStandings, List<Game> futureGames)
		{
			this.Key = key;
			var localTeamStandings = baseTeamStandings.Select(x => TeamStanding.CopyTeamStanding(x)).ToList();
			foreach (var team in localTeamStandings)
			{
				foreach (var game in futureGames)
				{
					if (game.HasTeam(team.TeamName))
					{
						team.AddResult(game);
					}
				}
			}

			var outcomeTiers = localTeamStandings
								.GroupBy(x => x.Points)
								.Select(x =>
									new
									{
										Points = x.Key,
										TeamNames = x.Select(y => y.TeamName),
										Count = x.Count()
									})
								.OrderByDescending(x => x.Points)
								.ToList();
			this.TeamOutcomes = new List<ScenarioTeamOutcome>();
			int betterTeams = 0;
			foreach (var outcomeTier in outcomeTiers)
			{
				int bestResult = betterTeams + 1;
				int worstResult = betterTeams + outcomeTier.Count;
				foreach (var teamName in outcomeTier.TeamNames)
				{
					this.TeamOutcomes.Add(new ScenarioTeamOutcome(teamName, outcomeTier.Points, bestResult, worstResult));
				}
				betterTeams += outcomeTier.Count;
			}
		}

		public override string ToString()
		{
			return this.Key;
		}
	}
}
