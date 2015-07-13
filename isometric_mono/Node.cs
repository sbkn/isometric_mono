using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class Node
	{
		public int pos_x { get; set; }
		public int pos_y { get; set; }

		/*	Cost represents the costs of
		 *  the best known path to this node:
		 */
		public int cost { get; set; }
		public Node parent { get; set; }


		/*	Constructors:	*/
		public Node( int pos_x, int pos_y, int cost )
		{
			this.pos_x = pos_x;
			this.pos_y = pos_y;
			this.cost = cost;

			this.parent = null;
		}
		public Node(int pos_x, int pos_y)
		{
			this.pos_x = pos_x;
			this.pos_y = pos_y;
			this.cost = 0;

			this.parent = null;
		}
		public Node()
		{
			this.pos_x = 0;
			this.pos_y = 0;
			this.cost = 0;

			this.parent = null;
		}


		/*	CHECKING WHETHER TWO NODES ARE EQUAL:	*/
		public bool equals( Node a)
		{
			if (a.pos_x == this.pos_x && a.pos_y == this.pos_y)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	}
}
