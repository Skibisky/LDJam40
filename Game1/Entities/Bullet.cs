using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities {
	class Bullet : Entity {

		public Entity Owner;
		public bool Live = true;
		public int damage = 1;

		public Bullet() : base() {
			OnCollide += Bullet_OnCollide;
			this.inertia = 1;
		}

		static Random rand = new Random();

		private void Bullet_OnCollide(object sender, Entity e) {
			var k = e as Killable;
			if (k != null) {
				if (Live && (k is Obstacle || (Owner == Player.Current && k is Enemy) || (Owner != Player.Current && k is Player))) {
					Live = false;
					k.health -= damage;
					Game1.Game.manager.Removals.Add(Id);
					if (k is Player) {
						if (rand.Next(0, 2) == 0)
							Game1.Game.PlaySound("sounds/hit1");
						else
							Game1.Game.PlaySound("sounds/hit2");

					}
				}
			}
		}

		public override void Create(ContentManager Content) {
			this.texture = Content.LoadFlyWeight<Texture2D>("textures/bullet");
			base.Create(Content);
		}

		public override void Update() {
			if (X > Game1.WIDTH + 100) {
				Game1.Game.manager.Removals.Add(this.Id);
			}
			else if (X < -100) {
				Game1.Game.manager.Removals.Add(this.Id);
			}
			if (Y > Game1.HEIGHT + 100) {
				Game1.Game.manager.Removals.Add(this.Id);
			}
			else if (Y < -100) {
				Game1.Game.manager.Removals.Add(this.Id);
			}
		}
	}
}
