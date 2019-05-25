using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class PlaceGroupResult
	{
		public int Place { get; private set; }
		public int MaxPoints { get; private set; }
		public int MinPoints { get; private set; }

		public bool IsTeamLocked { get; private set; }
		public string LockedTeamName { get; private set; }
		public int? GoalsScored { get; private set; }
		public int? GoalsAllowed { get; private set; }
		public int? GoalDifference => this.GoalsScored - this.GoalsAllowed;

		public PlaceGroupResult(int place, int maxPoints, int minPoints)
		{
			this.Place = place;
			this.MaxPoints = maxPoints;
			this.MinPoints = minPoints;
			this.IsTeamLocked = false;
			this.LockedTeamName = null;
			this.GoalsScored = null;
			this.GoalsAllowed = null;
		}

		public void LockTeam(string teamName)
		{
			this.IsTeamLocked = true;
			this.LockedTeamName = teamName;
		}

		public void LockTeamGoals(int goalsScored, int goalsAllowed)
		{
			this.GoalsScored = goalsScored;
			this.GoalsAllowed = goalsAllowed;
		}
	}
}
