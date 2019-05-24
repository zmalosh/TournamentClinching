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
		public List<Game> FinishedGames { get; private set; }
		public List<Game> RemainingGames { get; private set; }
		public List<TeamStanding> CurrentStandings { get; private set; }
		public List<Scenario> Scenarios { get; private set; }

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

		#region POSSIBLE OUTCOMES
		public void PopulateScenarios()
		{
			var possibleOutcomes = this.GetPossibleOutcomes(this.RemainingGames.Count);
			this.Scenarios = new List<Scenario>();
			foreach (var possibleOutcome in possibleOutcomes)
			{
				var scenarioGames = new List<Game>();
				for (int i = 0; i < this.RemainingGames.Count; i++)
				{
					var homeResult = possibleOutcome[i];
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
				var scenario = new Scenario(possibleOutcome, this.CurrentStandings, scenarioGames);
				this.Scenarios.Add(scenario);
			}
		}

		private static Dictionary<int, List<string>> PossibleOutcomesByGamesRemaining = new Dictionary<int, List<string>>();
		public List<string> GetPossibleOutcomes(int gameCount)
		{
			if (!PossibleOutcomesByGamesRemaining.TryGetValue(gameCount, out List<string> result))
			{
				result = CalculatePossibleOutcomesRecursive(gameCount);
				PossibleOutcomesByGamesRemaining[gameCount] = result;
			}
			return result;
		}

		private List<string> CalculatePossibleOutcomesRecursive(int gameCount)
		{
			if (gameCount == 1)
			{
				return new List<string> { "W", "D", "L" };
			}
			var subresults = CalculatePossibleOutcomesRecursive(gameCount - 1);
			var results = new List<string>();
			foreach (var subresult in subresults)
			{
				results.AddRange(new List<string> { subresult + "W", subresult + "D", subresult + "L" });
			}
			return results;
		}
		#endregion POSSIBLE OUTCOMES
	}
}
