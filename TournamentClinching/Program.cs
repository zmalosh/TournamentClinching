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
			var gamesXml = XDocument.Load("TestFiles/2015WWC.xml");
			var games = gamesXml.Descendants("game").Select(x => new Game(x)).ToList();
			int advancerCount = int.Parse(gamesXml.Root.Attribute("advancers").Value);
			var groupStage = new GroupStage(games, advancerCount);
			groupStage.SimulateGroupStage();
			var a = 1;
		}
	}
}
