using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1.Entities {
	public class Obstacle : Killable {
		Texture2D exclam;

		public override void Create(ContentManager Content) {
			texture = Content.LoadFlyWeight<Texture2D>("textures/obstacle");
			exclam = Content.LoadFlyWeight<Texture2D>("textures/exclame");
			base.Create(Content);
			healthMax = 10;
			health = healthMax;
		}

		static Random rand = new Random();

		protected override void Death() {
			Game1.Game.PlaySound("sounds/explode4");

			var ef = new ogpEffect() {
				blend = blend,
				tname = "textures/rubble",
				X = X,
				Y = Y,
				rotation = (float)(rand.NextDouble() * Math.PI * 2),
				VelX = rand.Next(-10, 10),
				VelY = rand.Next(-10, 10),
			};
			ef.OnUpdate += Enemy.Ef_OnUpdate;
			Game1.Game.manager.Additions.Add(ef);
			ef = new ogpEffect() {
				blend = blend,
				tname = "textures/rubble",
				X = X,
				Y = Y,
				rotation = (float)(rand.NextDouble() * Math.PI * 2),
				VelX = rand.Next(-10, 10),
				VelY = rand.Next(-10, 10),
			};
			ef.OnUpdate += Enemy.Ef_OnUpdate;
			Game1.Game.manager.Additions.Add(ef); ef = new ogpEffect() {
				blend = blend,
				tname = "textures/rubble",
				X = X,
				Y = Y,
				rotation = (float)(rand.NextDouble() * Math.PI * 2),
				VelX = rand.Next(-10, 10),
				VelY = rand.Next(-10, 10),
			};
			ef.OnUpdate += Enemy.Ef_OnUpdate;
			Game1.Game.manager.Additions.Add(ef); ef = new ogpEffect() {
				blend = blend,
				tname = "textures/rubble",
				X = X,
				Y = Y,
				rotation = (float)(rand.NextDouble() * Math.PI * 2),
				VelX = rand.Next(-10, 10),
				VelY = rand.Next(-10, 10),
			};
			ef.OnUpdate += Enemy.Ef_OnUpdate;
			Game1.Game.manager.Additions.Add(ef);
		}

		public override void Update() {
			VelX = -Player.Current.speed / 3f;
			if (X < -50) {
				Game1.Game.manager.Removals.Add(this.Id);
				Player.Current.dodged++;
			}
			this.blend.R = (byte)(255 * health / healthMax);
			this.blend.G = (byte)(255 * health / healthMax);
			this.blend.B = (byte)(255 * health / healthMax);
		}

		public override void Render(SpriteBatch sb) {
			base.Render(sb);
			if (X > 800) {
				sb.Draw(exclam, new Vector2(800 - (X - 800) / 10, Y), exclam.Bounds, Color.Yellow, 0, new Vector2(exclam.Width / 2, exclam.Height / 2), 1, SpriteEffects.None, 0);
			}
		}
	}
}
