using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class IsoPoint
	{
		public int PosXPx { get; set; }
		public int PosYPx { get; set; }
		public int PosXMatrix { get; set; }
		public int PosYMatrix { get; set; }

		public IsoPoint() { PosXPx = 0; PosXMatrix = 0; PosYPx = 0; PosYMatrix = 0; }





		/*	
			======================================
			CALCULATE POSITION ON SCREEN (IN PX):
			======================================
				*/
		public void calc_pos_px(int x, int y, int TILE_WIDTH, int TILE_HEIGHT)
		{
			this.PosXPx = (x - y) * (TILE_WIDTH / 2) - (TILE_WIDTH / 2);
			this.PosYPx = (x + y) * (TILE_HEIGHT / 2);

			this.PosXMatrix = x;
			this.PosYMatrix = y;


		}


		/*	
			======================================
			CALCULATE POSITION IN MATRIX (INDICES):
			======================================
				*/
		public int calc_pos_matrix(int x, int y, int TILE_WIDTH, int TILE_HEIGHT)
		{
			float i = x;
			float j = y;

			PosXPx = x;
			PosYPx = y;

			int newPosX = (int)(i / (TILE_WIDTH / 2) + j / (TILE_HEIGHT / 2)) / 2;
			int newPosY = (int)(j / (TILE_HEIGHT / 2) - i / (TILE_WIDTH / 2)) / 2;

			if (newPosX == PosXMatrix && newPosY == PosYMatrix) return 0;

			PosXMatrix = newPosX;
			PosYMatrix = newPosY;
			return 1;
		}

		/*	
			==========================================
			COMPARE TWO POINTS (MATRIX COORDINATES !)
			(RETURNS 1 IF EQUAL, 0 IF NOT):
			==========================================
				*/
		public int Compare(IsoPoint cmp)
		{
			if (PosXMatrix == cmp.PosXMatrix && PosYMatrix == cmp.PosYMatrix)
			{
				return 1;
			}
			return 0;
		}
	}
}
