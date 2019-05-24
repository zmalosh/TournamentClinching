using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TournamentClinching
{
	class Program
	{
		static void Main(string[] args)
		{
			var gamesXml = XDocument.Load("TestFiles/4Teams01.xml");
			var games = gamesXml.Descendants("game").Select(x => new Game(x)).ToList();
			var groupStage = new GroupStage(games);
			groupStage.PopulateScenarios();
			var a = 1;
		}
	}
}
