using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace isometric_mono
{
	class Sprite
	{
		private Texture2D Texture;

		private int frameOffsetX = 0;
		private int frameOffsetY = 0;
		private int frameWidth = 64;
		private int frameHeight = 64;

		private int frames = 0;
		private int currentFrame = 0;

		private int posX = 0;
		private int posY = 0;

		private long last_frame_chg;

		bool animated = false;

		public float TotalElapsed { get; set; }
		public float TimePerFrame { get; set; }

		public Sprite(Texture2D tx, int offsetx, int offsety, int width, int height, int max_frames, bool state )
		{
			this.Texture = tx;
			this.frameOffsetX = offsetx;
			this.frameOffsetY = offsety;
			this.frameWidth = width;
			this.frameHeight = height;
			this.frames = max_frames;
			this.last_frame_chg = 0;
			this.animated = state;

			this.TotalElapsed = 0;
			this.TimePerFrame = 0.1F;
		}
		~Sprite() { this.Texture.Dispose(); }

		public Texture2D get_texture() { return Texture; }
		public void set_pos_x(int x) { posX = x; }
		public int get_pos_x() { return posX; }
		public void set_pos_y(int y) { posY = y; }
		public int get_pos_y() { return posY; }
		public bool sprite_animated() { return animated; }
		public void set_animated(bool state) { animated = state; }
		public int get_max_frames() { return frames; }
		public void set_max_frames(int a) { frames = a; }
		public int get_cur_frame() { return currentFrame; }
		public void set_cur_frame(int a) { currentFrame = a; }
		public int get_frame_offset_x() { return frameOffsetX; }
		public void set_frame_offset_x(int a) { frameOffsetX = a; }
		public int get_frame_offset_y() { return frameOffsetY; }
		public void set_frame_offset_y(int a) { frameOffsetY = a; }
		public int get_frame_width() { return frameWidth; }
		public void set_frame_width(int a) { frameWidth = a; }
		public int get_frame_height() { return frameHeight; }
		public void set_frame_height(int a) { frameHeight = a; }
		public long get_last_frame_chg() { return last_frame_chg; }
		public void set_last_frame_chg(long a) { last_frame_chg = a; }

	}
}
