using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TournamentClinching
{
	public class Game
	{
		public string Group { get; private set; }
		public string HomeTeam { get; private set; }
		public string AwayTeam { get; private set; }
		public int? HomeScore { get; private set; }
		public int? AwayScore { get; private set; }

		public bool IsFinal => this.HomeScore.HasValue && this.AwayScore.HasValue;
		public bool HasTeam(string teamName) => this.HomeTeam == teamName || this.AwayTeam == teamName;

		private void InitValues(string group, string home, string away, int? homeScore, int? awayScore)
		{
			if ((!this.AwayScore.HasValue && this.HomeScore.HasValue) || (this.AwayScore.HasValue && !this.HomeScore.HasValue))
			{
				throw new ArgumentException("Game only has one score set. Cannot determine if final or upcoming.");
			}
			this.Group = group;
			this.HomeTeam = home;
			this.AwayTeam = away;
			this.HomeScore = homeScore;
			this.AwayScore = awayScore;
		}

		#region CONSTRUCTORS
		public Game(string group, string home, string away)
		{
			this.InitValues(group, home, away, null, null);
		}

		public Game(string group, string home, string away, int homeScore, int awayScore)
		{
			this.InitValues(group, home, away, homeScore, awayScore);
		}

		public Game(XElement xe)
		{
			int itp; // intTryParse
			string group = xe.Attribute("group").Value;
			string home = xe.Attribute("home").Value;
			string away = xe.Attribute("away").Value;
			int? homeScore = int.TryParse(xe.Attribute("homeScore")?.Value, out itp) ? itp : (int?)null;
			int? awayScore = int.TryParse(xe.Attribute("awayScore")?.Value, out itp) ? itp : (int?)null;
			this.InitValues(group, home, away, homeScore, awayScore);
		}
		#endregion CONSTRUCTORS

		public override string ToString()
		{
			if (!this.IsFinal)
			{
				return $"{this.HomeTeam}-{this.AwayTeam}";
			}
			return $"{this.HomeTeam}({this.HomeScore})-{this.AwayTeam}({this.AwayScore})";
		}
	}

	public static partial class ExtensionMethods
	{
		public static int? GetTeamPointsFromGame(this Game game, string teamName)
		{
			if (!game.IsFinal)
			{
				return null;
			}
			else if (teamName == game.HomeTeam)
			{
				return GetTeamPointsGained(game.HomeScore.Value, game.AwayScore.Value);
			}
			else if (teamName == game.AwayTeam)
			{
				return GetTeamPointsGained(game.AwayScore.Value, game.HomeScore.Value);
			}
			throw new ArgumentException("teamName is not a part of this game");
		}

		private static int GetTeamPointsGained(int teamScore, int oppScore)
		{
			if (teamScore > oppScore)
			{
				return 3;
			}
			else if (oppScore > teamScore)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
	}
}
