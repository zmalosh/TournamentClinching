using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class PlaceGroupResult : IComparable<PlaceGroupResult>
	{
		public string GroupName { get; private set; }
		public int Place { get; private set; }
		public int MaxPoints { get; private set; }
		public int MinPoints { get; private set; }

		public bool IsTeamLocked { get; private set; }
		public string LockedTeamName { get; private set; }
		public int? GoalsScored { get; private set; }
		public int? GoalsAllowed { get; private set; }
		public int? GoalDifference => this.GoalsScored - this.GoalsAllowed;
		public bool HasLockedGoals => this.IsTeamLocked && this.GoalDifference.HasValue;

		public PlaceGroupResult(string groupName, int place, int maxPoints, int minPoints)
		{
			this.GroupName = groupName;
			this.Place = place;
			this.MaxPoints = maxPoints;
			this.MinPoints = minPoints;
			this.IsTeamLocked = false;
			this.LockedTeamName = null;
			this.GoalsScored = null;
			this.GoalsAllowed = null;
		}

		public override string ToString() => $"{this.Place}{this.GroupName}{(this.IsTeamLocked ? $"({this.LockedTeamName})" : "")} - MAX:{this.MaxPoints} - MIN:{this.MinPoints}";

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

		private const int TEAM_IS_BETTER = -1;
		private const int OTHER_IS_BETTER = 1;
		private const int TEAMS_TIED = 0;
		public int CompareTo(PlaceGroupResult other)
		{
			bool includeTiebreakers = this.IsTeamLocked && this.GoalDifference.HasValue;
			if (this.MinPoints > other.MaxPoints)
			{
				return TEAM_IS_BETTER;
			}
			else if (this.MaxPoints < other.MinPoints)
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
	}
}
