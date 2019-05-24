using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class ScenarioTeamOutcome
	{
		public string TeamName { get; private set; }
		public int Points { get; private set; }
		// NO TIE BREAKERS APPLIED FOR GOALS. TEAMS BEST OUTCOME IS BEST POSSIBLE w/ TIE BREAKERS
		public int BestResult { get; private set; }

		// NO TIE BREAKERS APPLIED FOR GOALS. TEAMS WORST OUTCOME IS WORST POSSIBLE w/ TIE BREAKERS
		public int WorstResult { get; private set; }

		public ScenarioTeamOutcome(string teamName, int points, int bestResult, int worstResult)
		{
			this.TeamName = teamName;
			this.Points = points;
			this.BestResult = bestResult;
			this.WorstResult = worstResult;
		}

		public override string ToString()
		{
			return $"{this.TeamName} ({this.Points}P) - BEST:{this.BestResult} - WORST:{this.WorstResult}";
		}
	}
}
