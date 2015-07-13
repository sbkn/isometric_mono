using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace isometric_mono
{
	class Gmap
	{
		public int width{get;set;}
		public int height { get; set; }
		private int[,] map;
		private int[,] heightmap;

		public Gmap(string path_to_gamefile, string path_to_heightmap)
		{
			String input = File.ReadAllText(path_to_gamefile);
			
			int counter = 0;
			string line;

			/*	READ THE DIMENSIONS:	*/
			System.IO.StreamReader file = new System.IO.StreamReader(path_to_gamefile);
			if ((line = file.ReadLine()) != null )
			{
				width = Convert.ToInt32(line);
			}
			if ((line = file.ReadLine()) != null)
			{
				height = Convert.ToInt32(line);
			}

			map = new int[width, height];

			/*	READ THE GAMEMAP:	*/
			while (counter < height && (line = file.ReadLine().Trim()) != null )
			{
				string[] values = line.Split(new string[] { " " }, StringSplitOptions.None);

				for (int i = 0; i < values.Length; i++)
				{
					map[counter, i] = int.Parse(values[i]);
				}
				counter++;
			}

			file.Close();
			/*	END.	*/


			/*	READ THE HEIGHTMAP:	*/			
			heightmap = new int[width, height];

			input = File.ReadAllText(@path_to_heightmap);

			counter = 0;

			file = new System.IO.StreamReader(@path_to_heightmap);
			while (counter < height && (line = file.ReadLine().Trim()) != null)
			{
				string[] values = line.Split(new string[] { " " }, StringSplitOptions.None);

				for (int i = 0; i < values.Length; i++)
				{
					heightmap[counter, i] = int.Parse(values[i]);
				}
				counter++;
			}

			file.Close();
			/*	END.	*/
		}
		public int tileType(int x, int y) 
		{
			if ((x < width && x >= 0) && (y < height && y >= 0))
			{
				return map[x, y];
			}
			else
			{
				return -1;
			}
		}

		public void setTileType(int x, int y, int type) 
		{
			if ((x < width && x >= 0 ) && (y < height && y >= 0))
			{
				map[x, y] = type; 
			}
		}



		public int getHeight(int x, int y)
		{
			if ((x < width && x >= 0) && (y < height && y >= 0))
			{
				return heightmap[x, y];
			}
			return -1;
		}
	}
}
