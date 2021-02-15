using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Game1.Entities {
	public class Enemy : Killable {
		public Texture2D wings;
		public Texture2D arms;
		public Texture2D bits;

		Texture2D arrow;

		public float wingRotation;
		public float armRotation;
		public float bitRotation;

		public Color wingBlend = Color.White;
		public Color armBlend = Color.White;
		public Color bitBlend = Color.White;

		static Random rand = new Random();

		int behaviour;

		int charge = 0;
		int chargeMax = 200;
		bool wasCharged;
		int refire = 0;
		bool alternate;

		float destX;
		float destY;
		float destDir;
		float velMax = 10;

		public Enemy() : base() {
			OnCollide += Enemy_OnCollide;
			OnDeath += Enemy_OnDeath;
			healthMax = EnemyHealth();
			health = healthMax;

			behaviour = rand.Next(0, 4); // 13
			if (behaviour == 2)
				chargeMax = 2000;
		}

		public static int EnemyHealth() {
			return 5 + Player.Current.powerupHealth / 3 + Player.Current.dollars / 12 + Player.Current.distTravelled / 1000;
		}

		public static int EnemyDamage() {
			return 1 + Player.Current.powerupDamage / 6 + Player.Current.dollars / 12 + Player.Current.distTravelled / 1000;
		}

		private void Enemy_OnDeath(object sender, EventArgs e) {
			Player.Current.kills++;

			// turn it's components into particles
			var blends = new Color[] {blend, armBlend, wingBlend, bitBlend };
			var texts = new Texture2D[] {texture, arms, wings, bits };
			var rots = new float[] {rotation, armRotation, wingRotation, bitRotation};

			for (int i = 0; i < blends.Length; i++) {

				var ef = new ogpEffect(X, Y) {
					blend = blends[i],
					texture = texts[i],
					rotation = rots[i],
					VelX = rand.Next(-10, 10),
					VelY = rand.Next(-10, 10),
				};
				ef.OnUpdate += Ef_OnUpdate;
				Game1.Game.manager.Additions.Add(ef);
			}
			
			Game1.Game.manager.Additions.Add(new Powerup() {
				X = X,
				Y = Y,
				type = 0,
				VelX = -3,
			});
			if (rand.Next(0, 2) == 0)
				Game1.Game.PlaySound("sounds/explode1");
			else 
				Game1.Game.PlaySound("sounds/explode2");
		}

		public static void Ef_OnUpdate(object sender, EventArgs e) {
			var f = sender as ogpEffect;
			f.VelY += 0.3f;
			if (f.blend.A > 50)
				f.blend.A -= 2;
			if (f.Y > Game1.HEIGHT + 100)
				Game1.Game.manager.Removals.Add(f.Id);
		}

		private void Enemy_OnCollide(object sender, Entity e) {

		}

		public override void Create(ContentManager Content) {
			this.texture = Content.LoadFlyWeight<Texture2D>("textures/body");
			this.wings = Content.LoadFlyWeight<Texture2D>("textures/wings");
			this.arms = Content.LoadFlyWeight<Texture2D>("textures/arms");
			this.bits = Content.LoadFlyWeight<Texture2D>("textures/bits");
			this.arrow = Content.LoadFlyWeight<Texture2D>("textures/arrow");
			base.Create(Content);
			blend = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);
			bitBlend = Extensions.HSVtoRGB(30 * behaviour, 0.8f, 0.8f);
		}

		public override void Update() {

			armRotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X) + Math.PI / 2f);
			wingRotation += (float)(Math.Sin(bitRotation) / 10);
			bitRotation += 0.1f;

			wingBlend = Extensions.HSVtoRGB((health * 120 / healthMax), 0.8f, 0.8f);
			armBlend = Extensions.HSVtoRGB(300, (charge / (float)chargeMax), 0.8f);

			// move and charge up
			switch (behaviour) {
				case 3:
					destX = Game1.WIDTH / 2 + Game1.WIDTH / 4 * (float)Math.Sin(charge / 50f);
					destY = Game1.HEIGHT / 2 + Game1.HEIGHT / 4 * (float)Math.Cos(charge / 50f);

					destX += 64 * (float)Math.Sin(Id);
					destY += 64 * (float)Math.Cos(Id);

					charge++;
					break;
				case 2:
					if (refire <= 0) {
						destDir = destDir + (float)((rand.NextDouble() - 0.5) * Math.PI / 1.5);
						destX = Player.Current.X + (float)(192 * Math.Sin(destDir));
						destY = Player.Current.Y + (float)(192 * Math.Cos(destDir));
						refire = 60;
					}
					charge++;
					refire--;
					break;
				case 1:
					charge++;
					destX = Game1.WIDTH - 100;
					destY = 100;
					if (Player.Current.Y < Game1.HEIGHT / 2) {
						destY = Game1.HEIGHT - 100;
					}

					destX += 64 * (float)Math.Sin(Id);
					destY += 64 * (float)Math.Cos(Id);
					
					break;
				case 0:
				default:
					velMax = 2;
					destX = Game1.WIDTH / 3f;
					destY = Y;
					if (X < Game1.WIDTH / 3f + 50) {
						charge++;
					}
					break;
			}

			VelX = (destX - X) / 50f;
			VelY = (destY - Y) / 50f;

			if (VelX > velMax)
				VelX = velMax;
			else if (VelX < -velMax)
				VelX = -velMax;
			if (VelY > velMax)
				VelY = velMax;
			else if (VelY < -velMax)
				VelY = -velMax;

			if (charge >= chargeMax) {
				wasCharged = true;
			}
			if (charge <= 0) {
				wasCharged = false;
			}

			
			// fire!
			switch (behaviour) {
				case 1:
					if (wasCharged) {
						refire--;
						if (refire <= 0) {
							charge -= 30;
							alternate = !alternate;
							if (rand.Next(0, 2) == 0)
								Game1.Game.PlaySound("sounds/enemy1");
							else
								Game1.Game.PlaySound("sounds/enemy2");

							ShootBullet(alternate);
							refire = 30;
						}
					}
					break;
				case 0:
				default:
					if (wasCharged) {
						charge = 0;
						wasCharged = false;
						if (rand.Next(0, 2) == 0)
							Game1.Game.PlaySound("sounds/enemy1");
						else
							Game1.Game.PlaySound("sounds/enemy2");
						
						ShootBullet(true);
						ShootBullet(false);
						if (behaviour == 2)
							this.Kill();
					}
					break;
			}
		}

		public void ShootBullet(bool isTop) {
			float bvelX = (Player.Current.X - X);
			float bvelY = (Player.Current.Y - Y);
			float bdist = (float)Math.Sqrt(bvelX * bvelX + bvelY * bvelY);
			bvelX = 10 * bvelX / bdist;
			bvelY = 10 * bvelY / bdist;

			health -= EnemyDamage() / 3;
			var b = new Bullet() {
				blend = Color.Red,
				damage = EnemyDamage(),
				X = X,
				Y = Y,
				rotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X)),
				VelX = bvelX,
				VelY = bvelY,
				Owner = this,
			};
			// TODO: properly?
			if (isTop) {
				b.X += (float)(32 * Math.Sin(armRotation));
				b.Y += (float)(32 * Math.Cos(armRotation));
			}
			else {
				b.X += (float)(32 * Math.Sin(armRotation + Math.PI));
				b.Y += (float)(32 * Math.Cos(armRotation + Math.PI));
			}
			Game1.Game.manager.Additions.Add(b);
		}

		public override void Render(SpriteBatch sb) {
			sb.Draw(texture, new Vector2(X, Y), texture.Bounds, blend, rotation, origin, scale, SpriteEffects.None, (depth - 1 + 10000) / 20000f);
			sb.Draw(bits, new Vector2(X, Y), texture.Bounds, bitBlend, bitRotation, origin, scale, SpriteEffects.None, (depth + 10000) / 20000f);
			sb.Draw(arms, new Vector2(X, Y), texture.Bounds, armBlend, armRotation, origin, scale, SpriteEffects.None, (depth + 1 + 10000) / 20000f);
			sb.Draw(wings, new Vector2(X, Y), texture.Bounds, wingBlend, wingRotation, origin, scale, SpriteEffects.None, (depth + 2 + 10000) / 20000f);
			this.RenderUpcoming(sb, arrow, Color.Red, true);
		}

	}
}
