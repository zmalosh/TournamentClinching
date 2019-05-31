using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class GroupStage
	{
		private List<Group> Groups;
		private List<string> TeamNames;
		private int TeamsAdvancingCount;

		private int TeamsAdvancingPerGroup => this.TeamsAdvancingCount / this.Groups.Count;
		private int TeamEliminatedPerGroup => (this.TeamNames.Count - this.TeamsAdvancingCount) / this.Groups.Count;
		private int WildCardTeamCount => this.TeamsAdvancingCount - (this.TeamsAdvancingPerGroup * this.Groups.Count);

		private List<string> WCGroupsClinched;
		private List<string> WCGroupsEliminated;
		private List<string> WCGroupsAlive;

		public GroupStage(IEnumerable<Game> games, int teamsAdvancing)
		{
			this.Groups = games.GroupBy(x => x.Group).Select(y => new Group(y.ToList())).ToList();
			this.TeamNames = this.Groups.SelectMany(x => x.TeamNames).ToList();
			this.TeamsAdvancingCount = teamsAdvancing;

			this.WCGroupsClinched = null;
			this.WCGroupsEliminated = null;
			this.WCGroupsAlive = null;
		}

		public List<FinalTeamStanding> GetStandings()
		{
			this.CalculateBaseTeamStandings();
			this.CalculateAdvancement();
			var standings = this.CalculateFinalStandings();
			return this.CalculateFinalStandings();
		}

		private void CalculateBaseTeamStandings()
		{
			foreach (var group in this.Groups)
			{
				group.SimulateScenarios();
			}
		}

		private void CalculateAdvancement()
		{
			// wc = wild card
			// eliminated = no longer in competition
			// clinched = guaranteed wc advancement
			// alive = neither eliminated or clinched
			// locked = the representative for the group cannot change
			// - locked team = the team for the group is locked, but the goals being locked is undefined
			// - locked goals = the team and goals are both locked

			int wcPlace = this.TeamsAdvancingPerGroup + 1;
			int totalTeamsToEliminate = this.Groups.Count - this.WildCardTeamCount;
			var wcGroupEntries = this.Groups.SelectMany(x => x.PlaceGroupResults).Where(x => x.Place == wcPlace).ToList();

			// MIN POINTS TO ADVANCE (wcMinPointsReq) ARE THE NUMBER OF POINTS FOR THE FINAL QUALIFIER AT THIS PLACE IF THE MIN POINTS HAPPEN FOR EACH GROUP
			// A GROUP CANNOT HAVE THEIR TEAM ADVANCE IF THEIR MAX IS UNDER THE MIN REQUIRED
			var allMinPointVals = wcGroupEntries.Select(x => x.MinPoints).OrderByDescending(x => x).ToList();
			int wcMinPointsReq = allMinPointVals[wcPlace - 1];

			// MAX POINTS TO ADVANCE (wcMaxPointsReq) ARE THE NUMBER OF POINTS FOR THE FINAL QUALIFIER AT THIS PLACE IF THE MAX POINTS HAPPEN FOR EACH GROUP
			// A GROUP WILL DEFINITIVELY HAVE THEIR TEAM ADVANCE IF THEIR MIN IS OVER THE MAX REQUIRED
			var allMaxPointVals = wcGroupEntries.Select(x => x.MaxPoints).OrderByDescending(x => x).ToList();
			int wcMaxPointsReq = allMaxPointVals[wcPlace - 1];

			// 3 LISTS BELOW ARE MUTUALLY EXCLUSIVE
			var groupsEliminated = new List<PlaceGroupResult>();
			var groupsAlive = new List<PlaceGroupResult>();
			var groupsClinched = new List<PlaceGroupResult>();

			// DETERMINE GROUPS ADVANCING REGARDLESS OF TIE BREAKS
			// ANY GROUP NOT USING TIE-BREAKS SHOULD BE CATEGORIZED HERE
			foreach (var wcGroupEntry in wcGroupEntries)
			{
				// IF THE GROUP CAN'T REACH THE MIN POSSIBLE POINTS TO ADVANCE, THEY ARE ELIMINATED
				// IF THE GROUP MIN IS BETTER THAN THE MAX POSSIBLE POINTS TO ADVANCE, THEY ARE CLINCHED
				// IF NEITHER AND THE GROUP IS NOT TIE-BREAK ELIGIBLE, THEY ARE STILL ALIVE
				if (wcMinPointsReq > wcGroupEntry.MaxPoints)
				{
					groupsEliminated.Add(wcGroupEntry);
				}
				else if (wcMaxPointsReq < wcGroupEntry.MinPoints)
				{
					groupsClinched.Add(wcGroupEntry);
				}
				else if (!wcGroupEntry.HasLockedGoals)
				{
					groupsAlive.Add(wcGroupEntry);
				}
			}

			// ALL GROUPS HAVE LOCKED GOALS MOVING FORWARD
			var groupsUnassigned = wcGroupEntries
									.Where(x => groupsEliminated.Any(y => y.GroupName == x.GroupName)
												&& !groupsClinched.Any(y => y.GroupName == x.GroupName))
									.OrderBy(x => x)
									.ToList();

			int wcClinchesRemaining = this.WildCardTeamCount - groupsClinched.Count; // ADVANCEMENT SPOTS STILL TO BE AWARDED
			int wcEliminationsRemaining = totalTeamsToEliminate - groupsClinched.Count;

			// CATEGORIZE TIEBREAKABLE TEAMS IF WE CAN
			for (int i = 0; i < groupsUnassigned.Count; i++)
			{
				var wcGroupEntry = groupsUnassigned[i];
				int betterTeams = i;
				int worseTeams = groupsUnassigned.Count - i;

				// IF NUMBER OF LOCKED GROUPS BETTER THAN THIS GROUP IS GT wcClinchesRemaining, THIS GROUP CANNOT ADVANCE
				// IF NUMBER OF LOCKED GROUPS WORSE THAN THIS GROUP IS GT wcEliminationsRemaining, THIS GROUP IS THROUGH
				// NO DETERMINATION CAN BE MADE
				if (betterTeams > wcClinchesRemaining)
				{
					groupsEliminated.Add(wcGroupEntry);
				}
				else if (worseTeams > wcEliminationsRemaining)
				{
					groupsClinched.Add(wcGroupEntry);
				}
				else
				{
					groupsAlive.Add(wcGroupEntry);
				}
			}

			this.WCGroupsAlive = groupsAlive.Select(x => x.GroupName).ToList();
			this.WCGroupsClinched = groupsClinched.Select(x => x.GroupName).ToList();
			this.WCGroupsEliminated = groupsClinched.Select(x => x.GroupName).ToList();
		}

		private List<FinalTeamStanding> CalculateFinalStandings()
		{
			var result = new List<FinalTeamStanding>();
			foreach (var group in this.Groups)
			{
				int worstClinched = this.TeamsAdvancingPerGroup;
				int bestEliminated = (group.TeamNames.Count + 1) - this.TeamEliminatedPerGroup;

				if (this.WCGroupsClinched.Contains(group.GroupName))
				{
					worstClinched++;
				}
				if (this.WCGroupsEliminated.Contains(group.GroupName))
				{
					bestEliminated--;
				}

				for (int i = 0; i < group.TeamGroupResults.Count; i++)
				{
					var team = group.TeamGroupResults[i];
					string teamName = team.TeamName;
					var baseStanding = group.GetBaseTeamStanding(teamName);
					bool isEliminated = team.BestPlace >= bestEliminated;
					bool isClinched = team.WorstPlace <= worstClinched;
					var finalTeamStanding = new FinalTeamStanding(
						teamName: teamName,
						groupName: group.GroupName,
						place: i + 1,
						wins: baseStanding.Wins,
						draws: baseStanding.Draws,
						losses: baseStanding.Losses,
						goalsScored: baseStanding.GoalsScored,
						goalsAgainst: baseStanding.GoalsAllowed,
						hasBeenEliminated: isEliminated,
						hasClinchedAdvancement: isClinched,
						bestGroupPlace: team.BestPlace,
						worstGroupPlace: team.WorstPlace,
						maxPoints: team.MaxPoints,
						minPoints: team.MinPoints);
					result.Add(finalTeamStanding);
				}
			}
			return result;
		}
	}
}
