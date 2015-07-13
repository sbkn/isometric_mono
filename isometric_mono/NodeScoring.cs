using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class NodeScoring : IComparer<Node>
	{

		public int pos_finish_x { get; set; }
		public int pos_finish_y { get; set; }

		public NodeScoring( int pos_finish_x, int pos_finish_y )
		{
			this.pos_finish_x = pos_finish_x;
			this.pos_finish_y = pos_finish_y;
		}

		/*	THIS COMPARES NODES,
		 *  THE LOWER THE SCORE, THE BETTER.
			THUS:
			a_score <= b_score  ----> 1,
			a_score > b_score  ----> -1
		 */
		public int Compare(Node a, Node b )
		{
			if (a == null || b == null)
			{
				return 1;
			}
			/* 
				F = G + H,

				where F is the score, G the cost to move from starting point to the given point on the grid
				and H the approximative cost to reach the destination ( f.e. Manhattan distance ):
			 */
			int score_a = a.cost + Math.Abs(this.pos_finish_x - a.pos_x) + Math.Abs(this.pos_finish_y - a.pos_y);
			int score_b = b.cost + Math.Abs(this.pos_finish_x - b.pos_x) + Math.Abs(this.pos_finish_y - b.pos_y);

			if (score_a > score_b)
			{
				return 1;
			}
			else
			{
				return -1;
			}


		}
	}
}
