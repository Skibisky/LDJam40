using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1.Entities {
	public class Player : Killable {

		public static Player Current;

		public Player() :base() {
			Current = this;
			this.health = 100;
			this.healthMax = 100;
			this.depth = -100;
			this.OnCollide += Player_OnCollide;

		}

		private void Player_OnCollide(object sender, Entity e) {
			var p = e as Powerup;
			var n = e as Enemy;
			var o = e as Obstacle;
			if (p != null) {
				if (p.type == 0) {
					if (rand.Next(0, 2) == 0)
						Game1.Game.PlaySound("sounds/dollar1");
					else
						Game1.Game.PlaySound("sounds/dollar2");
					dollars++;
				}
				else {
					if (rand.Next(0, 2) == 0)
						Game1.Game.PlaySound("sounds/powerup1");
					else
						Game1.Game.PlaySound("sounds/powerup2");
					this.powerups++;
					health = Math.Min(healthMax, health + 1);
					if (p.type == 1) {
						powerupHealth++;
						health = Math.Min(healthMax, health + 10);
						var tef = new ogtEffect() {
							X = X,
							Y = Y,
							fname = "fonts/debug",
							text = "** HEALTH UP **",
							VelY = -0.2f,
							scale = new Vector2(1.3f, 1.3f)
						};
						tef.OnUpdate += (a, b) => {
							var f = a as ogtEffect;
							f.blend = Color.Red;
							f.rotation += 0.07f;
							if ((f.type / 10) % 2 == 0) {
								f.blend = Color.Yellow;
							}
							f.type++;
							if (f.type > 120) {
								Game1.Game.manager.Removals.Add(f.Id);
							}
						};
						Game1.Game.manager.Additions.Add(tef);
					}
					else if (p.type == 2) {
						powerupDamage++;
						var tef = new ogtEffect() {
							X = X,
							Y = Y,
							fname = "fonts/debug",
							text = "** DAMAGE UP **",
							VelY = -0.2f,
							scale = new Vector2(1.3f, 1.3f)
						};
						tef.OnUpdate += (a, b) => {
							var f = a as ogtEffect;
							f.rotation += 0.07f;
							f.blend = Color.Red;
							if ((f.type / 10) % 2 == 0) {
								f.blend = Color.Yellow;
							}
							f.type++;
							if (f.type > 120) {
								Game1.Game.manager.Removals.Add(f.Id);
							}
						};
						Game1.Game.manager.Additions.Add(tef);
					}
				}
				Game1.Game.manager.Removals.Add(p.Id);
			}
			if (n != null) {
				this.health -= n.health;
				n.Kill();
			}
			if (o != null) {
				if (rand.Next(0, 2) == 0)
					Game1.Game.PlaySound("sounds/explode1");
				else
					Game1.Game.PlaySound("sounds/explode2");
				this.health -= o.health;
				o.Kill();
			}
		}

		protected override void Death() {
			Game1.Game.PlaySound("sounds/explode3");
			for (int i = 0; i < 10; i ++) {
				var dust = new ogpEffect() {
					X = this.X + bOx,
					Y = this.Y + bOy,
					tname = "textures/cloud1",
					VelX = rand.Next(-5, 5),
					VelY = rand.Next(-5, 5),
					type = rand.Next(0, 3) - 1,
					rotation = (float)(rand.NextDouble() * Math.PI * 2),
				};
				dust.scale.X = (float)(0.5 + rand.NextDouble());
				dust.scale.Y = (float)(0.5 + rand.NextDouble());
				switch (rand.Next(0, 3)) {
					case 0:
						dust.blend = Color.Red.WithA(150);
						break;
					case 1:
						dust.blend = Color.Yellow.WithA(150);
						break;
					default:
						dust.blend = Color.Gray.WithA(150);
						break;
				}
				dust.OnUpdate += (a, b) => {
					var e = a as ogpEffect;
					e.rotation += e.type / 10f;
					e.blend.A -= 1;
					e.VelX -= 0.1f;
					e.VelY *= 0.96f;
					if (e.blend.A < 0)
						Game1.Game.manager.Removals.Add(e.Id);
				};
				Game1.Game.manager.Additions.Add(dust);
			}
		}

		public int damage = 1;

		public int dodged;
		public int kills;
		public int dollars;
		public int powerups;
		private int pudm;
		public int powerupDamage { get { return pudm; } set { pudm = value; damage = 1 + pudm + powerups / 10; } }
		private int puhp;
		public int powerupHealth { get { return puhp; } set { puhp = value; healthMax = 100 + puhp * 2 + powerups / 5; } }
		public int damageDealt;
		public int damageTaken;
		public int distTravelled;
		public int speed = 5;

		public int shootTime = 10;
		public int shootCool = 0;

		public override void Create(ContentManager Content) {
			this.texture = Content.LoadFlyWeight<Texture2D>("textures/plane");
			base.Create(Content);
		}

		const float VelDamp = 0.85f;

		int horKey = 0;
		int verKey = 0;
		int keyMax = 10;
		float keyDiv = 5f;
		float keyPow = 0.3f;

		int bOx = 0;
		int bOy = 5;

		static Random rand = new Random();

		public bool MoveLeft;
		public bool MoveRight;
		public bool MoveUp;
		public bool MoveDown;
		public bool TryShoot;

		public override void Update() {
			distTravelled += speed;
			speed = 5 + kills / 30 + powerups / 10 + (int)Math.Log(distTravelled / 1000 + 1);

			var ks = Keyboard.GetState();

			if (Game1.Game.IsPlaying) {
				TryShoot = ks.IsKeyDown(Keys.Space);
				MoveUp = ks.IsKeyDown(Keys.W);
				MoveDown = ks.IsKeyDown(Keys.S);
				MoveLeft = ks.IsKeyDown(Keys.A);
				MoveRight = ks.IsKeyDown(Keys.D);
			}

			shootCool -= 1;
			if (ks.IsKeyDown(Keys.L)) {
				this.health = 0;
			}
			if (TryShoot && shootCool <= 0) {
				shootCool = shootTime;
				if (rand.Next(0, 2) == 0)
					Game1.Game.PlaySound("sounds/laser1");
				else
					Game1.Game.PlaySound("sounds/laser2");

				Game1.Game.manager.Additions.Add(new Bullet() {
					X = this.X + bOx,
					Y = this.Y + bOy,
					VelX = 15,
					blend = Color.Yellow,
					Owner = this,
					damage = this.damage,
				});
				var dust = new ogpEffect() {
					X = this.X + bOx,
					Y = this.Y + bOy,
					blend = Color.Gray.WithA(150),
					tname = "textures/cloud1",
					VelY = -3,
					type = rand.Next(0, 3) - 1,
					rotation = (float)(rand.NextDouble() * Math.PI * 2),
				};
				dust.scale.X = (float)(0.2 + rand.NextDouble() / 3);
				dust.scale.Y = (float)(0.2 + rand.NextDouble() / 3);
				dust.OnUpdate += (a, b) => {
					var e = a as ogpEffect;
					e.rotation += e.type / 10f;
					e.blend.A -= 1;
					e.VelX -= 0.1f;
					e.VelY *= 0.96f;
					if (e.blend.A < 0)
						Game1.Game.manager.Removals.Add(e.Id);
				};
				Game1.Game.manager.Additions.Add(dust);
				var dakka = new ogpEffect() {
					X = this.X + bOx,
					Y = this.Y + bOy,
					blend = Color.Red.WithA(150),
					tname = "textures/dakka",
					depth = -1000,
				};
				if (rand.Next(0, 2) == 0) {
					dakka.blend = Color.Yellow.WithA(150);
				}
				dakka.OnUpdate += (a, b) => {
					var e = a as Entity;
					e.blend.A -= 10;
					if (e.blend.A <= 10)
						Game1.Game.manager.Removals.Add(e.Id);
				};
				Game1.Game.manager.Additions.Add(dakka);
				var shell = new ogpEffect() {
					X = this.X + bOx,
					Y = this.Y + bOy,
					VelY = -4,
					blend = Color.Gold.WithA(150),
					tname = "textures/bullet",
					depth = -1000,
					type = rand.Next(0, 3) - 1,
				};
				shell.OnUpdate += (a, b) => {
					var e = a as ogpEffect;
					e.VelY += 0.2f;
					e.VelX -= 0.01f;
					e.rotation += e.type / 10f;
					if (e.Y > 700)
						Game1.Game.manager.Removals.Add(e.Id);
				};
				Game1.Game.manager.Additions.Add(shell);
			}

			if (MoveUp) {
				if (horKey < keyMax)
					horKey++;  
				VelY -= (float)Math.Pow(horKey, keyPow) / keyDiv;
			}
			else if (MoveDown) {
				if (horKey < keyMax)
					horKey++;
				VelY += (float)Math.Pow(horKey, keyPow) / keyDiv;
			}
			else {
				horKey = 0;
				if (VelY != 0) {
					VelY *= VelDamp;
				}
				if (Math.Abs(VelY) < 0.1) {
					VelY = 0;
				}
			}
			if (MoveLeft) {
				if (verKey < keyMax)
					verKey++;
				VelX -= (float)Math.Pow(verKey, keyPow) / keyDiv;
			}
			else if (MoveRight) {
				if (verKey < keyMax)
					verKey++;
				VelX += (float)Math.Pow(verKey, keyPow) / keyDiv;
			}
			else {
				verKey = 0;
				if (VelX != 0) {
					VelX *= VelDamp;
				}
				if (Math.Abs(VelX) < 0.1) {
					VelX = 0;
				}
			}
			if (X < 50) {
				X = 50;
				VelX = 0.3f;
			}
			else if (X > 750) {
				X = 750;
				VelX = -0.3f;
			}
			if (Y < 50) {
				Y = 50;
				VelY = 0.3f;
			}
			else if (Y > 550) {
				Y = 550;
				VelY = -0.3f;
			}
		}
	}
}
