using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class TileList
	{
		public readonly List<DepthSortedEntity> EntityList;
		
		public TileList(Gmap gamemap, Unit ghost)
		{
			EntityList = new List<DepthSortedEntity>();

			DepthSortedEntity tmp = null;


			/*	Get the tiles (DIAMOND SHAPE !):	*/
			for (int i = 0; i < gamemap.height; i++)
			{
				for (int j = 0; j < i+1; j++)
				{
					tmp = new DepthSortedEntity( j, i-j, gamemap.getHeight(j, i-j), gamemap.tileType(j, i-j), 0F );
					EntityList.Add(tmp);
				}
			}
			for (int i = 1; i < gamemap.height+1; i++)
			{
				for (int j = 0; j < gamemap.width-i; j++)
				{
					tmp = new DepthSortedEntity(i + j, gamemap.width - j - 1, gamemap.getHeight(i + j, gamemap.width - j - 1), gamemap.tileType(i + j, gamemap.width - j - 1), 0F);
					EntityList.Add(tmp);
				}
			}

			/*	Add the ghost (HIS INDEX IS 7 FROM NOW ON !!!!):	*/
			tmp = new DepthSortedEntity(ghost.pos.PosXMatrix, ghost.pos.PosYMatrix, gamemap.getHeight(ghost.pos.PosXMatrix, ghost.pos.PosYMatrix), 7, 0F);
			EntityList.Add(tmp);

			this.SortList();


		}

		/*	Sort the list according to DEPTH ORDERING:	*/
		public void SortList()
		{
			IComparer<DepthSortedEntity> comparer = new EntityOrdering();
			EntityList.Sort(comparer);
		}





	}
}
