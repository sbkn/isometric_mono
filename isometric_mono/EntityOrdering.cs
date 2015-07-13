using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class EntityOrdering : IComparer<DepthSortedEntity>
	{
		/*	
		 * THE COMPARER FOR DepthSortedEntities:
		 * */

			public int Compare(DepthSortedEntity a, DepthSortedEntity b)
			{
				int z_max = 17;
				int C_a = 0, C_b = 0;
				float Obstacle_a = 0, Obstacle_b = 0;

				float height_a = (float)a.z / z_max;
				float height_b = (float)b.z / z_max;

				/*	If a is the ghost:	*/
				if (a.tileType == 7)
				{
					C_a = 1;
				}
				/*	if a is an obstacle we need to 
				 *  make sure that it's covering the
				 *  ghost:*/
				else if (a.tileType == 1)
				{
					Obstacle_a = 0.99F - Math.Abs(height_a);
				}
				/*	If b is the ghost:	*/
				if (b.tileType == 7)
				{
					C_b = 1;
				}
				/*	if b is an obstacle we need to 
			 *  make sure that it's covering the
			 *  ghost:*/
				else if (b.tileType == 1)
				{
					Obstacle_b = 0.99F - Math.Abs(height_b);
				}

				float a_val = a.x + a.y + Math.Abs(height_a) + (C_a * 1.00001F) + Obstacle_a;
				float b_val = b.x + b.y + Math.Abs(height_b) + (C_b * 1.00001F) + Obstacle_b;

				a.depthValue = a_val;
				b.depthValue = b_val;

				/*	Compare a_val to b_val
				 *  [ 1 => a > b]
				 *  [-1 => b > a]:
				 *  */
				if (a_val > b_val)
				{
					return 1;
				}
				else if (b_val > a_val)
				{
					return -1;
				}
				else
				{
					if (a.y >= b.y)
					{
						return 1;
					}
					else
						return -1;
				}
			}

	}
}
