using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace isometric_mono
{
	class Unit
	{
		public Sprite Texture { get; set; }
		public IsoPoint pos { get; set; }
		public IsoPoint dest_pos { get; set; }
		public int movement_speed_x { get; set; }
		public int movement_speed_y { get; set; }
		public Route route { get; set; }


		/*	Did the unit go from one tile to
		 *  a different one at the last call of
		 *  unit::move ?:
		 */
		public bool hasChangedTiles { get; set; }

		public Unit()
		{
			pos = new IsoPoint();
			dest_pos = new IsoPoint();
			route = new Route();
			hasChangedTiles = false;
			route.routeActive = false;
		}

		/*	
	======================================
	CALCULATE POSITION ON SCREEN (IN PX):
	======================================
		*/
		public void calc_pos_px(int x, int y, int TILE_WIDTH, int TILE_HEIGHT)
		{
			//this.pos.pos_x_px = (x - y) * (TILE_WIDTH / 2) + TILE_WIDTH / 2;
			this.pos.PosXPx = (x - y) * (TILE_WIDTH / 2);
			this.pos.PosYPx = (x + y) * (TILE_HEIGHT / 2) + TILE_HEIGHT / 2;

			this.pos.PosXMatrix = x;
			this.pos.PosYMatrix = y;


		}
		public void calc_dest_pos_px(int x, int y, int TILE_WIDTH, int TILE_HEIGHT)
		{
			//this.pos.pos_x_px = (x - y) * (TILE_WIDTH / 2) + TILE_WIDTH / 2;
			this.dest_pos.PosXPx = (x - y) * (TILE_WIDTH / 2);
			this.dest_pos.PosYPx = (x + y) * (TILE_HEIGHT / 2) + TILE_HEIGHT / 2;

			this.dest_pos.PosXMatrix = x;
			this.dest_pos.PosYMatrix = y;


		}

		/*	MOVE THE UNIT TOWARDS ITS dest_pos:	*/
		public void move(int TILE_WIDTH, int TILE_HEIGHT)
		{
			/*	x-axis:	*/
			if (this.pos.PosXPx < this.dest_pos.PosXPx)
			{
				this.movement_speed_x = 1;
			}
			else if (this.pos.PosXPx > this.dest_pos.PosXPx)
			{
				this.movement_speed_x = -1;
			}
			else
			{
				this.movement_speed_x = 0;
			}

			/*	y-axis:	*/
			if (this.pos.PosYPx < this.dest_pos.PosYPx)
			{
				this.movement_speed_y = 1;
			}
			else if (this.pos.PosYPx > this.dest_pos.PosYPx)
			{
				this.movement_speed_y = -1;
			}
			else
			{
				this.movement_speed_y = 0;
			}
			if (this.pos.PosXPx == this.dest_pos.PosXPx && this.pos.PosYPx == this.dest_pos.PosYPx)
			{
				if (this.route.Steps.Count > 0)
				{
					this.calc_dest_pos_px(this.route.Steps[0].pos_x, this.route.Steps[0].pos_y, TILE_WIDTH, TILE_HEIGHT);
					this.route.Steps.RemoveAt(0);
				}
				else
				{
					this.route.routeActive = false;
				}
			}


			//Update this's px position:
			this.pos.PosXPx += this.movement_speed_x;
			this.pos.PosYPx += this.movement_speed_y;
			//Update this's matrix position:
			int changed = this.pos.calc_pos_matrix(this.pos.PosXPx, this.pos.PosYPx, TILE_WIDTH, TILE_HEIGHT);

			if (changed == 1)
			{
				hasChangedTiles = true;
			}
			else { hasChangedTiles = false; }
		}


	}
}
