using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities {
    public class Cloud : Entity {
        static Random rand = new Random();
        public override void Create(ContentManager Content) {
            texture = Content.LoadFlyWeight<Texture2D>("textures/cloud");
            base.Create(Content);
            VelX = -3;
            depth = 10000 + (int)Id * 10;
            scale.X = 0.4f + rand.Next(0, 10) / 10f;
            scale.Y = 0.4f + rand.Next(0, 10) / 10f;
            rotation = (float)(rand.NextDouble() * Math.PI * 2);
            blend.A = (byte)rand.Next(50, 100);
        }

        public override void Update() {
             VelX = -Player.Current.speed;
            if (X < -100) {
                Game1.Game.manager.Removals.Add(this.Id);
            }
        }
    }
}
