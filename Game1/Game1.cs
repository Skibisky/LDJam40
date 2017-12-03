using Game1.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Net;
using Newtonsoft.Json;

namespace Game1 {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public EntityManager manager;
		public static Game1 Game;

		public bool IsPlaying = false;
		public int IdleFor = 0;

		List<KeyValuePair<string, int>> highScores = new List<KeyValuePair<string, int>>();

		bool doHighscore = false;
		string hsName;
		
		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Game = this;
			
		}

		int botThink = 0;
		Entity botTarget = null;
		int lastScore = 0;

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			// TODO: Add your initialization logic here
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.SynchronizeWithVerticalRetrace = true;
			graphics.PreferMultiSampling = true;
			graphics.ApplyChanges();

			manager = new EntityManager(Content);
			base.Initialize();


			new System.Threading.Thread(() => {
				try {
					using (var wc = new WebClient()) {

						var scores = wc.DownloadString("http://gmscoreboard.com/handle_score.php?tagid=5a22c5e4e3a0915122283242312&getscore=5");
						var sd = JsonConvert.DeserializeObject<Dictionary<string, string>>(scores);
						if (sd.Any()) {
							var mk = sd.Keys.Where(k => k.Length < 3).Select(k => int.Parse(k.Substring(1))).Max();
							for (int i = 0; i < mk; i++) {
								highScores.Add(new KeyValuePair<string, int>(sd["p" + (i + 1)], int.Parse(sd["s" + (i + 1)])));
							}
						}
					}
				}
				catch {

				}
			}).Start();
			/*
			if (File.Exists("highscore.txt")) {
				var lines = File.ReadAllLines("highscore.txt");
				foreach (var l in lines) {
					var n = l.Split('|').First();
					var s = int.Parse(l.Split('|').Last());
					highScores.Add(new KeyValuePair<string, int>(n, s));
				}
			}*/
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);



			// TODO: use this.Content to load your game content here

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() {
			// TODO: Unload any non ContentManager content here
			//File.WriteAllLines("highscore.txt", highScores.OrderByDescending(kv => kv.Value).Take(10).Select(kv => kv.Key + "|" + kv.Value));
		}

		bool waitUpEnter;
		Dictionary<Keys, bool> pressed = new Dictionary<Keys, bool>();

		void SpawnReset() {
			manager.Entities.Clear();
			lastScore = 0;

			manager.Add(new Player() {
				X = 120,
				Y = 240,
			});

			manager.Add(new Powerup() {
				X = 240,
				Y = 240,
			});
		}

		bool openCrawl = true;
		int crawlSound = 0;
		int crawlNext = 0;
		int crawlOut = 0;
		string crawlText = "";
		string crawlSource = @"Watch out!
Abstract shapes from another dimension are invading.
And it's up to ace pilot INSERT NAME HERE to fend them off.

You aren't the only one to benifit from their powerups,
and they really want the money you're taking...";

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			var ks = Keyboard.GetState();

			if (!doHighscore) {
				if (!pressed.ContainsKey(Keys.M))
					pressed.Add(Keys.M, false);
				if (pressed[Keys.M] && ks.IsKeyUp(Keys.M)) {
					muted = !muted;
				}
				pressed[Keys.M] = ks.IsKeyDown(Keys.M);
			}

			manager.Update();

			if (openCrawl) {
				if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
					openCrawl = false;
					SpawnReset();
					waitUpEnter = true;
				}
				crawlSound--;
				if (crawlSound <= 0) {
					if (crawlText.Length < crawlSource.Length)
						PlaySound("sounds/beep2");
					crawlSound = random.Next(7, 12);
				}
				crawlNext--;
				if (crawlNext <= 0) {
					crawlNext = 5;
					if (crawlText.Length < crawlSource.Length)
						crawlText = crawlSource.Substring(0, crawlText.Length + 1);
					else
						crawlOut++;
				}

				if (crawlOut > 40) {
					openCrawl = false;
					SpawnReset();
				}


				if (nextShape < 0) {
					nextShape = 20;
					var ef = new ogpEffect() {
						X = random.Next(800, 1000),
						Y = random.Next(-200, 0),
						depth = -10,
						VelX = -3,
						VelY = 3,
						blend = Extensions.HSVtoRGB(random.Next(0, 360), 0.8f, 0.8f),
					};

					ef.tname = "textures/" + shapeNames[random.Next(0, shapeNames.Length)];
					ef.OnUpdate += (a, b) => {
						var e = a as ogpEffect;
						e.rotation += 0.1f;
					};
					manager.Add(ef);
				}
				nextShape--;

				return;
			}

			if (!pressed.ContainsKey(Keys.Enter))
				pressed.Add(Keys.Enter, false);
			if (pressed[Keys.Enter] && ks.IsKeyUp(Keys.Enter)) {
				if (doHighscore) {
					if (hsName.Length > 0) {
						highScores.Add(new KeyValuePair<string, int>(hsName, lastScore));
						new System.Threading.Thread(() => {
							try {
								using (var wc = new WebClient()) {
									wc.DownloadString("http://gmscoreboard.com/handle_score.php?tagid=5a22c5e4e3a0915122283242312&player=" + hsName + "&score=" + lastScore);
								}
							}
							catch {

							}
						}).Start();
						doHighscore = false;
						IsPlaying = false;
					}
				}
				else if ((!Player.Current.Alive || !IsPlaying) && !waitUpEnter) {
					SpawnReset();
					IsPlaying = true;
				}
			}
			pressed[Keys.Enter] = ks.IsKeyDown(Keys.Enter);
			if (!pressed[Keys.Enter])
				waitUpEnter = false;

			if (doHighscore) {
				for (var k = Keys.A; k < Keys.Z + 1; k++) {
					if (!pressed.ContainsKey(k))
						pressed.Add(k, false);
					if (pressed[k] && ks.IsKeyUp(k)) {
						if (ks.IsKeyDown(Keys.LeftShift))
							hsName += k.ToString();
						else
							hsName += k.ToString().ToLower();
					}
					pressed[k] = ks.IsKeyDown(k);
				}

				if (!pressed.ContainsKey(Keys.Space))
					pressed.Add(Keys.Space, false);
				if (pressed[Keys.Space] && ks.IsKeyUp(Keys.Space)) {
					hsName += " ";
				}
				pressed[Keys.Space] = ks.IsKeyDown(Keys.Space);

				if (!pressed.ContainsKey(Keys.Subtract))
					pressed.Add(Keys.Subtract, false);
				if (pressed[Keys.Subtract] && ks.IsKeyUp(Keys.Subtract) && ks.IsKeyDown(Keys.LeftShift)) {
					hsName += "_";
				}
				pressed[Keys.Subtract] = ks.IsKeyDown(Keys.Subtract);

				if (!pressed.ContainsKey(Keys.Back))
					pressed.Add(Keys.Back, false);
				if (pressed[Keys.Back] && ks.IsKeyUp(Keys.Back)) {
					if (hsName.Length > 0)
						hsName = hsName.Substring(0, hsName.Length - 1);
				}
				pressed[Keys.Back] = ks.IsKeyDown(Keys.Back);

			}

			if (!Player.Current.Alive) {
				if (lastScore == 0)
					lastScore = Score();
				if (!doHighscore && IsPlaying && (highScores.Count < 5 || highScores.Any(kv => kv.Value < Score()))) {
					hsName = "";
					doHighscore = true;
				}
				IdleFor++;
			}
			else {
				IdleFor = 0;
				if (!IsPlaying) {
					Player.Current.TryShoot = true;
					botThink++;

					if (botTarget == null || botThink > 100) {
						botThink = 0;
						if (random.Next(0, 2) == 0) {
							var pul = manager.Entities.Select(kv => kv.Value).Where(e => e is Powerup && e.X > 50 && e.X < 750).ToList();
							if (pul.Any())
								botTarget = pul.ElementAt(random.Next(0, pul.Count));
						}
						else {
							var enl = manager.Entities.Select(kv => kv.Value).Where(e => e is Enemy && e.X > 50).ToList();
							if (enl.Any())
								botTarget = enl.ElementAt(random.Next(0, enl.Count));
						}
					}

					var po = botTarget as Powerup;
					var en = botTarget as Enemy;
					if (po != null) {
						if (Player.Current.Y > po.Y + 8) {
							Player.Current.MoveUp = true;
							Player.Current.MoveDown = false;
						}
						else if (Player.Current.Y < po.Y - 8) {
							Player.Current.MoveDown = true;
							Player.Current.MoveUp = false;
						}
						if (Player.Current.X < po.X - 8) {
							Player.Current.MoveRight = true;
							Player.Current.MoveLeft = false;
						}
						else if (Player.Current.X > po.X + 8) {
							Player.Current.MoveLeft = true;
							Player.Current.MoveRight = false;
						}
						if (!manager.Entities.ContainsKey(po.Id)) {
							botTarget = null;
						}
					}
					else if (en != null) {
						if (!en.Alive || !manager.Entities.ContainsKey(en.Id)) {
							botTarget = null;
						}
						Player.Current.MoveLeft = true;
						if (Player.Current.Y > en.Y + 8) {
							Player.Current.MoveUp = true;
							Player.Current.MoveDown = false;
						}
						else if (Player.Current.Y < en.Y - 8) {
							Player.Current.MoveDown = true;
							Player.Current.MoveUp = false;
						}
					}
					else {
						Player.Current.MoveLeft = true;
					}
				}
			}

			if (!doHighscore && IdleFor > 200) {
				IsPlaying = false;
				SpawnReset();
			}
			else if (doHighscore && IdleFor > 1200) {
				IsPlaying = false;
				SpawnReset();
				doHighscore = false;
			}

			// TODO: Add your update logic here

			base.Update(gameTime);

			if (nextCloud < 0) {
				nextCloud = 10;
				manager.Add(new Cloud() {
					X = 1000,
					Y = random.Next(0, 600),
					depth = 10000,
				});
			}
			nextCloud--;

			if (nextEnemy < 0) {
				nextEnemy = NextEnemyTime();
				manager.Add(new Enemy() {
					X = 1600,
					Y = random.Next(80, 520),
					depth = -10,
					VelX = -2,
				});
			}
			nextEnemy--;

			if (nextPower < 0) {
				nextPower = NextPowerTime();
				manager.Add(new Powerup() {
					X = 1600,
					Y = random.Next(80, 520),
					depth = -10,
					VelX = -2,
				});
			}
			nextPower--;

			if (nextObst < 0) {
				nextObst = NextObstacleTime();
				manager.Add(new Obstacle() {
					X = 1600,
					Y = random.Next(80, 520),
					depth = -10,
					VelX = -2,
				});
			}
			nextObst--;

		}

		public static int NextEnemyTime() {
			return (int)(200 / (1 + Player.Current.kills / 40f + Player.Current.dollars / 20f));
		}

		public static int NextPowerTime() {
			return (int)(150 * (1 + (Player.Current.dollars + Player.Current.powerups * 3 + Player.Current.dollars) / 10f));
		}

		public static int NextObstacleTime() {
			return (int)(500 / (1 + (Player.Current.dollars / 2 + Player.Current.powerups * 2 + Player.Current.dodged) / 30f));
		}

		Random random = new Random(); 
		int nextCloud = 0;
		int nextEnemy = 0;
		int nextPower = 0;
		int nextObst = 0;
		int nextShape = 0;
		string[] shapeNames = new string[]{ "arms", "bits", "body", "dollar", "powerup1", "powerup2", "powerup3", "wings" };

		bool muted = false;

		public static int Score() {
			return (int)(
				Player.Current.dollars * 5 +
				Player.Current.kills * 2 + 
				Player.Current.dodged + 
				Player.Current.powerups * 2 +
				Player.Current.healthMax / 10 +
				Player.Current.distTravelled / 1000 +
				Player.Current.speed / 10
				);
		}

		public void PlaySound(string fname) {
			if (!muted)
				Content.LoadFlyWeight<SoundEffect>(fname).Play();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			base.Draw(gameTime);


			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap);

			manager.Render(spriteBatch);

			if (openCrawl) {
				GraphicsDevice.Clear(Color.Black);
				var sfd = Content.LoadFlyWeight<SpriteFont>("fonts/debug");
				spriteBatch.DrawString(sfd, crawlText, new Vector2(50, 50), Color.White);

				if ((flash / 20) % 2 == 0) {
					DrawStringCentered("fonts/debug", "-- Press Enter --", 400, 400, Color.White);
				}
			}
			else {

				//spriteBatch.Draw(Content.LoadFlyWeight<Texture2D>("textures/background"), new Rectangle(0, 0, 800, 600), new Rectangle(0, 0, 800, 600), Color.White, 0, Vector2.Zero, SpriteEffects.None, -0);
				//spriteBatch.Draw(Content.LoadFlyWeight<Texture2D>("textures/cloud1"), new Vector2(300, 300) , Color.White);
				if (muted)
					DrawStringCentered("fonts/debug", "M: Unmute", 400, 570, Color.Black);
				else
					DrawStringCentered("fonts/debug", "M: Mute", 400, 570, Color.Black);

				var sfd = Content.LoadFlyWeight<SpriteFont>("fonts/debug");
				if (Player.Current != null) {
					spriteBatch.DrawString(sfd, "Kills: " + Player.Current.kills, new Vector2(10, 10), Color.Black);
					spriteBatch.DrawString(sfd, "Spawn Time: " + NextEnemyTime(), new Vector2(10, 25), Color.Black);
					spriteBatch.DrawString(sfd, "Enemy HP: " + Enemy.EnemyHealth(), new Vector2(10, 40), Color.Black);
					spriteBatch.DrawString(sfd, "Enemy Damage: " + Enemy.EnemyDamage(), new Vector2(10, 55), Color.Black);

					spriteBatch.DrawString(sfd, "Health: " + Player.Current.health + "/" + Player.Current.healthMax, new Vector2(350, 10), Color.Black);
					spriteBatch.DrawString(sfd, "Distance: " + Player.Current.distTravelled / 100, new Vector2(350, 25), Color.Black);
					spriteBatch.DrawString(sfd, "Speed: " + Player.Current.speed, new Vector2(350, 40), Color.Black);

					spriteBatch.DrawRectangle(new Rectangle(300, 70, Player.Current.health * 200 / Player.Current.healthMax, 20),
						Extensions.HSVtoRGB(Math.Max(0, -20 + Player.Current.health * 140 / Player.Current.healthMax), 0.8f, 0.8f));

					spriteBatch.DrawRectangle(new Rectangle(300, 90, ((Player.Current.HealTime() - Player.Current.nextHeal) * 200 / Player.Current.HealTime()), 10), Color.Teal);

					spriteBatch.DrawString(sfd, "Buffs: " + Player.Current.powerups, new Vector2(650, 10), Color.Black);
					spriteBatch.DrawString(sfd, "Buff Time: " + NextPowerTime(), new Vector2(650, 25), Color.Black);
					spriteBatch.DrawString(sfd, "Damage: " + Player.Current.damage, new Vector2(650, 40), Color.Black);


					spriteBatch.DrawString(sfd, "Dodged: " + Player.Current.dodged, new Vector2(10, 565), Color.Black);
					spriteBatch.DrawString(sfd, "Obstacle Time: " + NextObstacleTime(), new Vector2(10, 580), Color.Black);

					spriteBatch.DrawString(sfd, "$: " + Player.Current.dollars, new Vector2(650, 565), Color.Black);
					spriteBatch.DrawString(sfd, "Score: " + Score(), new Vector2(650, 580), Color.Black);


					
				}
				if (!IsPlaying) {
					DrawStringCentered("fonts/menu", "High Scores", 400, 250, Color.Black);
					var yy = 23;
					foreach (var score in highScores.OrderByDescending(kv => kv.Value).Take(5)) {
						DrawStringCentered("fonts/score", score.Key + ": " + score.Value, 400, 250 + yy, Color.Black);
						yy += 23;
					}
					if ((flash / 20) % 2 == 0) {
						DrawStringCentered("fonts/debug", "-- Press Enter --", 400, 400, Color.Black);
					}
				}
				else if (!Player.Current?.Alive ?? false) {
					DrawStringCentered("fonts/menu", "You lose!", 400, 300, Color.Black);
					DrawStringCentered("fonts/score", "Score: " + lastScore, 400, 330, Color.Black);
					if (doHighscore) {
						DrawStringCentered("fonts/debug", "*NEW* High Score!", 400, 350, Color.Black);
						DrawStringCentered("fonts/debug", "Name: " + hsName, 400, 370, Color.Black);
					}
					if ((flash / 20) % 2 == 0) {
						DrawStringCentered("fonts/debug", "-- Press Enter --", 400, 400, Color.Black);
					}
				}
				
			}
			spriteBatch.End();
			flash++;
		}

		public void DrawStringCentered(string fontname, string text, int x, int y, Color c) {
			var sf = Content.LoadFlyWeight<SpriteFont>(fontname);
			var sz = sf.MeasureString(text);
			var v = new Vector2(x - sz.X / 2, y - sz.Y / 2);
			spriteBatch.DrawString(sf, text, v, c, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
		}

		int flash = 0;
	}
}
