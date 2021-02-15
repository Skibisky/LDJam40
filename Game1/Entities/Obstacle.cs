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


		public static int ObstacleHealth() {
			return 8 + Player.Current.powerupHealth / 2 + Player.Current.dollars / 5 + Player.Current.distTravelled / 1000;
		}


		static Random rand = new Random();

		protected override void Death() {
			Game1.Game.PlaySound("sounds/explode4");
			for (int i = 0; i < 10; i++) {
				var ef = new ogpEffect() {
					blend = Color.White,
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
			Game1.Game.manager.Additions.Add(new Powerup() {
				X = X,
				Y = Y,
				depth = -10,
				VelX = -2,
			});
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
			this.RenderUpcoming(sb, exclam, Color.Yellow);
		}
	}
}
