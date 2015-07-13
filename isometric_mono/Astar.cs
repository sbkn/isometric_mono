using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class Astar
	{
		public Route route { get; set; }
		public Gmap gamemap { get; set; }
		public Astar(Route route, Gmap gamemap)
		{
			this.route = route;
			this.gamemap = gamemap;
		}

		public void find_path()
		{
			/*	NO_DIAG_MOV OVERRIDES HALF_DIAG_MOV !!	*/

			/*	
				BY TURNING THIS ON (=TRUE) YOU TELL ASTAR
				TO SKIP DIAGONALLY ADJACENT NODES WHEN EXPANDING:
					*/
			bool NO_DIAG_MOV = false;
			/*	Is half-diagonal movement valid ?
			 *	(This means you can move diagonally, 
			 *	but only if there is no obstacle at
			 *	the adjacent tiles):
			 */
			bool HALF_DIAG_MOV = true;
			/*	What is the index in map[][] for
			 *  obstacle ?:	
			 */
			int INDEX_FOR_OBSTACLE = 1;

			//	The node which is currently being processed:
			Node cur_node = null;
			/*	The open list (Nodes yet to check),
			 *  this list is always sorted according to the
				score of its nodes:
			 */
			List<Node> open_list = null;
			// The closed list (Nodes already checked):
			List<Node> closed_list = null;

			bool done = false;
			int cum_cost_path = -1;
			int ite_cnt = 0;
			int tmp_cost;
			Node tmp = null;

			/*	This will compare nodes by their score:	*/
			IComparer<Node> nodeComparer = new NodeScoring(route.finish.pos_x, route.finish.pos_y);




			/*	ADD THE FIRST NODE TO THE LIST:	*/
			//route.Steps.Add(route.start);

			cur_node = route.start;


			/*	
			 *	MAIN LOOP:
			 */
			while (!done)
			{
				/*	If we've reached the destination:	*/
				if (cur_node.equals(route.finish))
				{
					done = true;
					cum_cost_path = cur_node.cost;

					route.Steps.Add(cur_node);
					tmp = cur_node;
					while (tmp.parent != null)
					{
						route.Steps.Add(tmp.parent);
						tmp = tmp.parent;
					}
					route.Steps.Reverse();
					route.routeActive = true;
				}
				/*	If not yet:	*/
				else
				{
					/*	EXPAND THE CURRENT NODE:	*/
					for (int i = -1; i < 2; i++)
					{
						for (int j = -1; j < 2; j++)
						{
							/*	Current node is already expanded:	*/
							if (i == 0 && j == 0)
							{
								continue;
							}
							/*	If we're out of bounds:	*/
							if (cur_node.pos_x + i < 0 || cur_node.pos_x + i >= gamemap.width || cur_node.pos_y + j < 0 || cur_node.pos_y + j >= gamemap.height)
							{
								continue;
							}
							/*	If it's an obstacle:	*/
							if (gamemap.tileType(cur_node.pos_y + j, cur_node.pos_x + i) == INDEX_FOR_OBSTACLE)
							{
								continue;
							}
							/*	Is this neighbor already done with ?:	*/
							if (closed_list != null)
							{
								tmp = closed_list.Find(a => (a.pos_x == cur_node.pos_x + i && a.pos_y == cur_node.pos_y + j));
								if (tmp != null)
								{
									tmp = null;
									continue;
								}
							}

							/*	Skip diagonally adjacent nodes IF NO_DIAG_MOV == true:	*/
							if (i != 0 && j != 0 && NO_DIAG_MOV)
							{
								continue;
							}
							/*	THIS IS FOR PSEUDO-NO_DIAG_MOV
								YOU SHALL NOT MOVE DIAGONALLY IF
								AN OBSTACLE IS ADJACENT TO current_node
								AND THIS NODE: 	
							 */
							if (i != 0 && j != 0 && HALF_DIAG_MOV)
							{
								if (gamemap.tileType(cur_node.pos_y, cur_node.pos_x + i) == INDEX_FOR_OBSTACLE)
									continue;
								if (gamemap.tileType(cur_node.pos_y + j, cur_node.pos_x) == INDEX_FOR_OBSTACLE)
									continue;
							}
							/*	Check whether this neighbor is already on the open list,
							 *  if yes - update its costs accordingly:
							 */
							if (open_list != null)
							{
								tmp = open_list.Find(a => (a.pos_x == cur_node.pos_x + i && a.pos_y == cur_node.pos_y + j));
							}

							if (open_list != null && tmp != null)
							{
								/* checking for diagonal vs (horizontal / vertical step): */
								if (i != 0 && j != 0)
								{
									/*	Is cur_node the better predecessor
									 *  than what we have atm ?:
									 */
									if (tmp.cost > cur_node.cost + 14)
									{
										tmp.cost = cur_node.cost + 14;
										tmp.parent = cur_node;
									}
								}
								else
								{
									if (tmp.cost > cur_node.cost + 10)
									{
										tmp.cost = cur_node.cost + 10;
										tmp.parent = cur_node;
									}
								}

							}
							/*	tmp is neither on the open_list nor on the closed_list
							 *  so we gotta add it to the open list:
							 */
							else
							{
								if (i != 0 && j != 0)
								{
									tmp_cost = cur_node.cost + 14;
								}
								else
								{
									tmp_cost = cur_node.cost + 10;
								}
								tmp = new Node(cur_node.pos_x + i, cur_node.pos_y + j, tmp_cost);
								tmp.parent = cur_node;
								if (open_list == null)
								{
									open_list = new List<Node>();
								}
								open_list.Add(tmp);
								open_list.Sort(nodeComparer);
							}


						}
					}

					/*	ADD CUR_NODE TO THE closed_list:	*/
					if (closed_list == null)
					{
						closed_list = new List<Node>();
					}
					closed_list.Add(cur_node);
					/*	REMOVE CUR_NODE FROM THE open_list:	*/
					open_list.Remove(cur_node);



					/* PATH SCORING: */

					/* 
					F = G + H,
	
					where F is the score, G the cost to move from starting point to the given point on the grid
					and H the approximative cost to reach the destination ( f.e. Manhattan distance )	
					 */


					/*	if open_list is empty, there are no open nodes left, even though destination is not reached yet:	*/
					if (open_list.Count == 0)
					{
						route.Steps = null;
						route.finish = null;
						route.routeActive = false;
						done = true;
					}
					else
					{
						cur_node = open_list[0];
					}

					ite_cnt++;
				}




			}
		}

	}
}
