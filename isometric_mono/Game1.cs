using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
/*	THESE 4 ARE FOR THE CURSOR:	*/
using System.Windows.Forms;
// For the NativeMethods helper class:
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace isometric_mono
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		private SpriteFont font;
		SpriteBatch spriteBatch;
		Sprite grid;
		Sprite obstacle;
		Sprite obstacle_blueprint;
		Unit ghost;
		Sprite highlighted_grid;

		/*	This is so we can compare KeyboardStates and
		 *  MouseStates from frame to frame:	
		 */
		KeyboardState KBstate_old;
		MouseState MSstate_old;
		IsoPoint _cursor;
		/*	This represents the last build order,
		 *	this can be either 0 == delete or
								1 == build:
		 */
		int _lastBuildOrder;

		/*	This is the TileList
		 *  which will hold all the objects
			sorted according to the 
			rendering order:
		 */
		TileList _theList;

		/*	DELETE THIS:	*/
		string _someString = new string('a',5);

		//THESE ARE CURRENTLY BEING MALUSED: CARE !!!!!!!
		const int SCREEN_WIDTH = 800;
		const int SCREEN_HEIGHT = 640;

		const int LEVEL_WIDTH = 16;
		const int LEVEL_HEIGHT = 16;

		const int TILE_WIDTH = 128;
		const int TILE_HEIGHT = 64;

		const double FPS = 60;


		Gmap _gamemap;

		Camera _cam;

		private SoundEffect _sfxBuildObstacle;



		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			/*	
			 * if changing GraphicsDeviceManager properties outside 
			 * your game constructor also call:
			 * graphics.ApplyChanges();
			 */
			/*	THIS IS FOR THE BORDERLESS FULLSCREEN:	*/
			var screen = Screen.AllScreens[0];
			Window.IsBorderless = true;
			Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
			graphics.PreferredBackBufferWidth = screen.Bounds.Width;
			graphics.PreferredBackBufferHeight = screen.Bounds.Height;

			TargetElapsedTime = TimeSpan.FromSeconds(1 / FPS);


		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_gamemap = new Gmap(@"Content\gamefile.txt", @"Content\heightmap.txt");
			_cam = new Camera();

			/*	THIS IS FOR THE CUSTOM CURSOR:	*/
			this.IsMouseVisible = true;
			Cursor myCursor = NativeMethods.LoadCustomCursor(@"Content\myCursor.cur");
			Form winForm = (Form)Form.FromHandle(this.Window.Handle);
			winForm.Cursor = myCursor;
			/*	CURSOR END.	*/

			/*	GET A FIRST KEYBOARD STATE:	*/
			KBstate_old = Keyboard.GetState();
			_cursor = new IsoPoint();


			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures:
			spriteBatch = new SpriteBatch(GraphicsDevice);

			/*	LOAD THE FONT:	*/
			font = Content.Load<SpriteFont>(@"8bit_font");

			/*	CREATE GRID:	*/
			grid = new Sprite(Content.Load<Texture2D>("iso_grid_spooky"), 0, 0, 64, 64, 1, false);
			grid.set_pos_x(0);
			grid.set_pos_y(0);

			/*	CREATE OBSTACLE:	*/
			obstacle = new Sprite(Content.Load<Texture2D>("iso_obstacle_spooky_box_closed"), 0, 0, 64, 96, 1, false);
			obstacle.set_pos_x(0);
			obstacle.set_pos_y(0);

			/*	CREATE OBSTACLE BLUEPRINT:	*/
			obstacle_blueprint = new Sprite(Content.Load<Texture2D>("iso_obstacle_spooky_box_closed_blueprint"), 0, 0, 64, 96, 1, false);
			obstacle_blueprint.set_pos_x(0);
			obstacle_blueprint.set_pos_y(0);

			/*	CREATE GHOST:	*/
			ghost = new Unit();
			//ghost.movement_speed_x = -1;
			//ghost.movement_speed_y = 1;
			ghost.calc_pos_px(0, 0, TILE_WIDTH, TILE_HEIGHT);
			ghost.route.start = new Node(ghost.pos.PosXMatrix, ghost.pos.PosYMatrix);
			ghost.route.finish = new Node(15, 14);
			Astar astarGhost = new Astar(ghost.route, _gamemap);
			ThreadStart TAstar = new ThreadStart(astarGhost.find_path);
			Thread thread = new Thread(TAstar);
			thread.Start();
			//pathfinder.find_path(ghost.route, gamemap);
			if (ghost.route.routeActive)
			{
				ghost.calc_dest_pos_px(ghost.route.Steps[0].pos_x, ghost.route.Steps[0].pos_y, TILE_WIDTH, TILE_HEIGHT);
				ghost.route.Steps.RemoveAt(0);			
			}
			//ghost.pos.calc_pos_matrix(0, 512, TILE_WIDTH, TILE_HEIGHT);
			ghost.Texture = new Sprite(Content.Load<Texture2D>("ghost_2"), 0, 0, 64, 64, 6, true);

			/*	CREATE HIGHLIGHTED GRID:	*/
			highlighted_grid = new Sprite(Content.Load<Texture2D>("iso_highlighted_grid"), 0, 0, 64, 32, 0, false);
			highlighted_grid.set_pos_x(0);
			highlighted_grid.set_pos_y(0);
			

			_sfxBuildObstacle = Content.Load<SoundEffect>("build_obstacle");

			/*	INIT THE LIST:	*/
			_theList = new TileList( _gamemap, ghost );
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
			_sfxBuildObstacle.Dispose();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				this.Exit();

			// TODO: Add your update logic here
			/*	HANDLE INPUT HERE:	*/
			UpdateInput();

			/*	MOVE THE GHOST:	*/
			ghost.move(TILE_WIDTH, TILE_HEIGHT);

			if (ghost.hasChangedTiles)
			{
				DepthSortedEntity tmp = _theList.EntityList.Find(a => (a.tileType == 7));
				tmp.x = ghost.pos.PosXMatrix;
				tmp.y = ghost.pos.PosYMatrix;
				tmp.z = _gamemap.getHeight(ghost.pos.PosXMatrix, ghost.pos.PosYMatrix);
				_theList.SortList();
			}

			/*	Adjust the camera:	*/
			_cam.x += _cam.dx;
			_cam.y += _cam.dy;



			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			MouseState myMouse = Mouse.GetState();
			//IsoPoint cursor = new IsoPoint();
			_cursor.calc_pos_matrix(myMouse.X + _cam.x * TILE_WIDTH / 2, myMouse.Y + _cam.y * TILE_HEIGHT / 2, TILE_WIDTH, TILE_HEIGHT);

			spriteBatch.Begin();

			/*	THE NEW DRAWING METHOD (BY SORTING):	*/
			foreach (var item in _theList.EntityList)
			{
				/*	Draw the grid:	*/
				if (item.tileType == 0)
				{
					spriteBatch.Draw(grid.get_texture(), new Rectangle((item.y - item.x) * (TILE_WIDTH / 2) - (TILE_WIDTH / 2) - _cam.x * TILE_WIDTH / 2, (item.y + item.x) * (TILE_HEIGHT / 2) - _cam.y * TILE_HEIGHT / 2 - _gamemap.getHeight(item.x,item.y), 64 * 2, 64 * 2), Color.White);
				}
				/*	Draw the obstacles:	*/
				if (item.tileType == 1)
				{
					spriteBatch.Draw(obstacle.get_texture(), new Rectangle((item.y - item.x) * (TILE_WIDTH / 2) - (TILE_WIDTH / 2) - _cam.x * TILE_WIDTH / 2, (item.y + item.x) * (TILE_HEIGHT / 2) - _cam.y * TILE_HEIGHT / 2 - TILE_HEIGHT - _gamemap.getHeight(item.x, item.y), 64 * 2, 96 * 2), Color.White);
				}
				/*	Draw the obstacle_blueprint:	*/
				if (item.tileType == 2)
				{
					spriteBatch.Draw(obstacle_blueprint.get_texture(), new Rectangle((item.y - item.x) * (TILE_WIDTH / 2) - (TILE_WIDTH / 2) - _cam.x * TILE_WIDTH / 2, (item.y + item.x) * (TILE_HEIGHT / 2) - _cam.y * TILE_HEIGHT / 2 - TILE_HEIGHT - _gamemap.getHeight(item.x, item.y), 64 * 2, 96 * 2), Color.White);
				}
				/*	Draw the highlighted_grid:	*/
				if (_cursor.PosXMatrix == item.y && _cursor.PosYMatrix == item.x)
				{
					spriteBatch.Draw(highlighted_grid.get_texture(), new Rectangle((item.y - item.x) * (TILE_WIDTH / 2) - (TILE_WIDTH / 2) - _cam.x * TILE_WIDTH / 2, (item.y + item.x) * (TILE_HEIGHT / 2) - _cam.y * TILE_HEIGHT / 2 - _gamemap.getHeight(item.x, item.y), 64 * 2, 32 * 2), Color.White);
				}
				if (item.tileType == 7)
				{
					/*	Draw the ghost:	*/
					float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
					ghost.Texture.TotalElapsed += elapsed;
					if (ghost.Texture.TotalElapsed > ghost.Texture.TimePerFrame)
					{
						/*	Go to the next animation frame:	*/
						ghost.Texture.set_cur_frame(ghost.Texture.get_cur_frame() + 1);
						if (ghost.Texture.get_cur_frame() > ghost.Texture.get_max_frames() - 1)
						{
							ghost.Texture.set_cur_frame(0);
						}
						ghost.Texture.TotalElapsed -= ghost.Texture.TimePerFrame;
					}
					spriteBatch.Draw(ghost.Texture.get_texture(), new Rectangle(ghost.pos.PosXPx - _cam.x * TILE_WIDTH / 2 - TILE_WIDTH / 2, ghost.pos.PosYPx - _cam.y * TILE_HEIGHT / 2 - TILE_HEIGHT - TILE_HEIGHT / 2 - _gamemap.getHeight(ghost.pos.PosYMatrix, ghost.pos.PosXMatrix), 64 * 2, 64 * 2), new Rectangle(64 * ghost.Texture.get_cur_frame(), 0, 64, 64), Color.White);

				}
			}
			/*	NEW METHOD END.	*/

			







			/*	DRAW SOME TEXT:	*/
			spriteBatch.DrawString(font, "ghost.pos_mat:", new Vector2(0, 0), Color.White);
			spriteBatch.DrawString(font, ghost.pos.PosXMatrix.ToString() + "," + ghost.pos.PosYMatrix.ToString(), new Vector2(20, 50), Color.White);
			spriteBatch.DrawString(font, "ghost.pos_px:", new Vector2(0, 100), Color.White);
			spriteBatch.DrawString(font, ghost.pos.PosXPx.ToString() + "," + ghost.pos.PosYPx.ToString(), new Vector2(20, 150), Color.White);
			spriteBatch.DrawString(font, "cursor.pos_matrix:", new Vector2(0, 200), Color.White);
			spriteBatch.DrawString(font, _cursor.PosXMatrix.ToString() + "," + _cursor.PosYMatrix.ToString(), new Vector2(20, 250), Color.White);
			spriteBatch.DrawString(font, "cursor.pos_px:", new Vector2(0, 300), Color.White);
			spriteBatch.DrawString(font, _cursor.PosXPx.ToString() + "," + _cursor.PosYPx.ToString(), new Vector2(20, 350), Color.White);
			spriteBatch.DrawString(font, "SomeString:", new Vector2(0, 400), Color.White);
			spriteBatch.DrawString(font, _someString, new Vector2(20, 450), Color.White);
			if (ghost.route.routeActive)
			{
				spriteBatch.DrawString(font, "ghost.route.finish:", new Vector2(0, 500), Color.White);
				spriteBatch.DrawString(font, ghost.route.finish.pos_x.ToString() + "," + ghost.route.finish.pos_y.ToString(), new Vector2(20, 550), Color.White);
				spriteBatch.DrawString(font, "ghost.dest_pos:", new Vector2(0, 700), Color.White);
				spriteBatch.DrawString(font, ghost.dest_pos.PosXPx.ToString() + "," + ghost.dest_pos.PosYPx.ToString(), new Vector2(20, 750), Color.White);
			}
			


			spriteBatch.End();

			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}


		/*	=====================================
		 *	THIS UPDATES THE INPUT IN EACH FRAME:
		 *	=====================================	*/
		private void UpdateInput()
		{
			KeyboardState KBstate_new = Keyboard.GetState();
			MouseState MSstate_new = Mouse.GetState();

			/*	If curNewCng == 1 -> the cursor has changed tiles.	*/
			int curNewCng = _cursor.calc_pos_matrix(MSstate_new.X + _cam.x * TILE_WIDTH / 2, MSstate_new.Y + _cam.y * TILE_HEIGHT / 2, TILE_WIDTH, TILE_HEIGHT);

			DepthSortedEntity tmp;

			/*	KEYBOARD - MAP SCROLLING:	*/
			if (KBstate_new.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
			{
				_cam.dx = -1;
			}
			else if (KBstate_new.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
			{
				_cam.dx = 1;
			}
			else
			{
				_cam.dx = 0;
			}
			if (KBstate_new.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
			{
				_cam.dy = -1;
			}
			else if (KBstate_new.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
			{
				_cam.dy = 1;
			}
			else
			{
				_cam.dy = 0;
			}
			/*	KEYBOARD - ESC PRESSED:	*/
			if (KBstate_new.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && !(KBstate_old.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)))
			{
				Exit();
			}
			/*	MOUSE:	*/
			/*	LMB PRESSED ONCE:	*/
			if ((MSstate_new.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MSstate_old.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released))
			{
				/*	Find the according Entity in TheList:	*/
				tmp = _theList.EntityList.Find((a => (a.y == _cursor.PosXMatrix) && (a.x == _cursor.PosYMatrix)));
				/*	If there is nothing:	*/
				if (_gamemap.tileType(_cursor.PosYMatrix, _cursor.PosXMatrix) == 0)
				{
					_gamemap.setTileType(_cursor.PosYMatrix, _cursor.PosXMatrix, 2);
					tmp.tileType = 2;
					_sfxBuildObstacle.Play();
					_lastBuildOrder = 1;
				}
				/*	If there already is a blueprint:	*/
				else if (_gamemap.tileType(_cursor.PosYMatrix, _cursor.PosXMatrix) == 2)
				{
					_gamemap.setTileType(_cursor.PosYMatrix, _cursor.PosXMatrix, 0);
					tmp.tileType = 0;
					_lastBuildOrder = 0;
				}

			}
			/*	LMB PRESSED CONTINOUSLY:	*/
			else if (((MSstate_new.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MSstate_old.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				&& curNewCng == 1))
			{
				/*	Find the according Entity in TheList:	*/
				tmp = _theList.EntityList.Find((a => (a.y == _cursor.PosXMatrix) && (a.x == _cursor.PosYMatrix)));
				/*	If there is nothing AND last time we've build a blueprint:	*/
				if (_gamemap.tileType(_cursor.PosYMatrix, _cursor.PosXMatrix) == 0 && _lastBuildOrder == 1)
				{
					_gamemap.setTileType(_cursor.PosYMatrix, _cursor.PosXMatrix, 2);
					tmp.tileType = 2;
					_sfxBuildObstacle.Play();
					_lastBuildOrder = 1;
				}
				/*	If there already is a blueprint AND last time a blueprint was deleted:	*/
				else if (_gamemap.tileType(_cursor.PosYMatrix, _cursor.PosXMatrix) == 2 && _lastBuildOrder == 0)
				{
					_gamemap.setTileType(_cursor.PosYMatrix, _cursor.PosXMatrix, 0);
					tmp.tileType = 0;
					_lastBuildOrder = 0;
				}
			}

			/*	RMB PRESSED ONCE:	*/
			if (MSstate_new.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
			{
				/*	Find the according Entity in TheList:	*/
				//tmp = TheList.EntityList.Find((a => (a.y == cursor_new.pos_x_matrix) && (a.x == cursor_new.pos_y_matrix)));
				tmp = _theList.EntityList.Find(a => (a.tileType == 7));
				_someString = tmp.depthValue.ToString();
			}






			KBstate_old = KBstate_new;
			MSstate_old = MSstate_new;
		}


	}
}
