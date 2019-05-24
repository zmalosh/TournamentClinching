using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class TeamGroupResult
	{
		public string TeamName { get; private set; }
		public int MaxPoints { get; private set; }
		public int MinPoints { get; private set; }
		public int BestPlace { get; private set; }
		public int WorstPlace { get; private set; }

		public override string ToString()
		{
			string pointsString = this.MaxPoints == this.MinPoints ? this.MaxPoints.ToString() : $"{this.MinPoints}-{this.MaxPoints}";
			string placeString = this.BestPlace == this.WorstPlace ? this.BestPlace.ToString() : $"{this.BestPlace}-{this.WorstPlace}";
			return $"{this.TeamName} - PTS:{pointsString} - RK:{placeString}";
		}

		public TeamGroupResult(string teamName, int maxPoints, int minPoints, int bestPlace, int worstPlace)
		{
			this.TeamName = teamName;
			this.MaxPoints = maxPoints;
			this.MinPoints = minPoints;
			this.BestPlace = bestPlace;
			this.WorstPlace = worstPlace;
		}
	}
}
