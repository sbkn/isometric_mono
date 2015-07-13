using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class Route
	{
		public Node start { get; set; }
		public Node finish { get; set; }

		public List<Node> Steps;
		public bool routeActive { get; set; }

		public Route(  )
		{
			this.Steps = new List<Node>();
			this.start = new Node();
			this.finish = new Node();
			this.routeActive = false;
		}





	}
}
