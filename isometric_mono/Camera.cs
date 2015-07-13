using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class Camera
	{

		public int x { get; set; }
		public int y { get; set; }
		public int dx { get; set; }
		public int dy { get; set; }

		public Camera() { x = 0; y = 0; dx = 0; dy = 0; }
	}
}
