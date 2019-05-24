using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentClinching
{
	public class GroupStage
	{
		public List<Group> Groups { get; set; }

		public GroupStage(IEnumerable<Game> games)
		{
			this.Groups = games.GroupBy(x => x.Group).Select(y => new Group(y.ToList())).ToList();
		}

		public void PopulateScenarios()
		{
			foreach (var group in this.Groups)
			{
				group.PopulateScenarios();
			}
		}
	}
}
