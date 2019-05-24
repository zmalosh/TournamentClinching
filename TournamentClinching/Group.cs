using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class Group
	{
		public string GroupName { get; private set; }
		public List<TeamGroupResult> TeamGroupResults { get; private set; }

		private List<Scenario> Scenarios;
		private List<TeamStanding> CurrentStandings;
		private List<Game> FinishedGames;
		private List<Game> RemainingGames;

		public Group(IEnumerable<Game> games)
		{
			this.GroupName = games.Select(x => x.Group).Distinct().Single();

			var teamNames = new List<string>();
			this.FinishedGames = new List<Game>();
			this.RemainingGames = new List<Game>();
			foreach (var game in games)
			{
				if (!teamNames.Contains(game.HomeTeam))
				{
					teamNames.Add(game.HomeTeam);
				}
				if (!teamNames.Contains(game.AwayTeam))
				{
					teamNames.Add(game.AwayTeam);
				}
				if (game.IsFinal)
				{
					this.FinishedGames.Add(game);
				}
				else
				{
					this.RemainingGames.Add(game);
				}
			}

			this.CurrentStandings = new List<TeamStanding>();
			foreach (var teamName in teamNames)
			{
				var currentTeamStanding = new TeamStanding(teamName, this.FinishedGames);
				this.CurrentStandings.Add(currentTeamStanding);
			}
		}

		public override string ToString() => this.GroupName;

		public void SimulateScenarios()
		{
			var possibleScenarios = this.GetPossibleScenarios(this.RemainingGames.Count);
			this.Scenarios = new List<Scenario>();
			foreach (var possibleScenario in possibleScenarios)
			{
				var scenarioGames = new List<Game>();
				for (int i = 0; i < this.RemainingGames.Count; i++)
				{
					var homeResult = possibleScenario[i];
					Game scenarioGame;
					var baseGame = this.RemainingGames[i];
					if (homeResult == 'W')
					{
						scenarioGame = new Game(this.GroupName, baseGame.HomeTeam, baseGame.AwayTeam, 1, 0);
					}
					else if (homeResult == 'D')
					{
						scenarioGame = new Game(this.GroupName, baseGame.HomeTeam, baseGame.AwayTeam, 0, 0);
					}
					else if (homeResult == 'L')
					{
						scenarioGame = new Game(this.GroupName, baseGame.HomeTeam, baseGame.AwayTeam, 0, 1);
					}
					else { throw new ArgumentException($"Result not expected:{homeResult}"); }
					scenarioGames.Add(scenarioGame);
				}
				var scenario = new Scenario(possibleScenario, this.CurrentStandings, scenarioGames);
				this.Scenarios.Add(scenario);
			}

			var scenarioResultsByTeam = this.Scenarios
											.SelectMany(x => x.TeamOutcomes)
											.GroupBy(y => y.TeamName)
											.ToList();
			this.TeamGroupResults = scenarioResultsByTeam
											.Select(x =>
												new TeamGroupResult(
													teamName: x.Key,
													maxPoints: x.Max(y => y.Points),
													minPoints: x.Min(y => y.Points),
													bestPlace: x.Min(y => y.BestResult),
													worstPlace: x.Max(y => y.WorstResult)))
											.ToList();
		}

		private static Dictionary<int, List<string>> PossibleScenariosByGamesRemaining = new Dictionary<int, List<string>>();
		public List<string> GetPossibleScenarios(int gameCount)
		{
			if (!PossibleScenariosByGamesRemaining.TryGetValue(gameCount, out List<string> result))
			{
				result = CalculatePossibleScenariosRecursive(gameCount);
				PossibleScenariosByGamesRemaining[gameCount] = result;
			}
			return result;
		}

		private List<string> CalculatePossibleScenariosRecursive(int gameCount)
		{
			if (gameCount == 1)
			{
				return new List<string> { "W", "D", "L" };
			}
			var subresults = CalculatePossibleScenariosRecursive(gameCount - 1);
			var results = new List<string>();
			foreach (var subresult in subresults)
			{
				results.AddRange(new List<string> { subresult + "W", subresult + "D", subresult + "L" });
			}
			return results;
		}
	}
}
