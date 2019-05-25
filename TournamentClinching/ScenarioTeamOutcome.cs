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
		public int WorstResult { get; private set; }

		public int GoalsScored { get; private set; }
		public int GoalsAllowed { get; private set; }
		public int GoalDifference => this.GoalsScored - this.GoalsAllowed;

		public ScenarioTeamOutcome(string teamName, int points, int bestResult, int worstResult, int goalsScored, int goalsAllowed)
		{
			this.TeamName = teamName;
			this.Points = points;
			this.BestResult = bestResult;
			this.WorstResult = worstResult;
			this.GoalsScored = goalsScored;
			this.GoalsAllowed = goalsAllowed;
		}

		public override string ToString()
		{
			return $"{this.TeamName} ({this.Points}P) - BEST:{this.BestResult} - WORST:{this.WorstResult}";
		}
	}
}
