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
        }

        private void Bullet_OnCollide(object sender, Entity e) {
            if (e is Killable k) {
                if (Live && (k is Obstacle || (Owner == Player.Current && k is Enemy) || (Owner != Player.Current && k is Player))) {
                    Live = false;
                    k.health -= damage;
                    Game1.Game.manager.Removals.Add(Id);
                }
            }
        }

        public override void Create(ContentManager Content) {
            this.texture = Content.LoadFlyWeight<Texture2D>("textures/bullet");
            base.Create(Content);
        }

        public override void Update() {
            if (X > 900) {
                Game1.Game.manager.Removals.Add(this.Id);
            }
            else if (X < -100) {
                Game1.Game.manager.Removals.Add(this.Id);
            }
            if (Y > 900) {
                Game1.Game.manager.Removals.Add(this.Id);
            }
            else if (Y < -100) {
                Game1.Game.manager.Removals.Add(this.Id);
            }
        }
    }
}
