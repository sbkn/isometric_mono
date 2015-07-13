using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class DepthSortedEntity
	{
		public int x { get; set; }
		public int y { get; set; }
		public int z { get; set; }
		public int tileType { get; set; }
		public float depthValue { get; set; }

		
		public DepthSortedEntity(int x, int y, int z, int tileType, float depthValue)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.tileType = tileType;
			this.depthValue = depthValue;
		}




	}
}
