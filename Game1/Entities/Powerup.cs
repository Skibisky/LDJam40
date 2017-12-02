using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Game1.Entities {
    public class Powerup : Entity {
        public Texture2D texture2;
        public Texture2D texture3;
        public Texture2D quest;

        public Color blend2 = Color.White;
        public Color blend3 = Color.White;
        static Random rand = new Random();

       public int type = -1;

        public override void Create(ContentManager Content) {
            this.texture = Content.LoadFlyWeight<Texture2D>("textures/powerup1");
            base.Create(Content);
            this.texture2 = Content.LoadFlyWeight<Texture2D>("textures/powerup2");
            this.texture3 = Content.LoadFlyWeight<Texture2D>("textures/powerup3");
            this.quest = Content.LoadFlyWeight<Texture2D>("textures/question");
            blend = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);
            blend2 = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);
            blend3 = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);

            if (type == -1)
                type = rand.Next(1, 3);
            if (type == 0) {
                texture = Content.LoadFlyWeight<Texture2D>("textures/dollar");
                texture2 = null; 
                texture3 = null;
            }
            else if (type == 1) {
                this.texture = texture2;
                this.texture3 = texture2;
            }
            else if (type == 2) {
                this.texture = texture3;
                this.texture2 = texture3;
            }
        }

        public override void Update() {
            rotation += 0.1f;
            scale.X = 1 + (float)Math.Sin(rotation) / 5f;
            scale.Y = 1 + (float)Math.Sin(rotation) / 5f;
        }

        public override void Render(SpriteBatch sb) {
            sb.Draw(texture, new Vector2(X, Y), texture.Bounds, blend, rotation, origin, scale, SpriteEffects.None, (depth - 1 + 10000) / 20000f);
            if (texture2 != null)
                sb.Draw(texture2, new Vector2(X, Y), texture.Bounds, blend2, rotation + (float)(Math.PI * 2 / 3), origin, scale, SpriteEffects.None, (depth + 10000) / 20000f);
            if (texture3 != null)
                sb.Draw(texture3, new Vector2(X, Y), texture.Bounds, blend3, rotation - (float)(Math.PI * 2 / 3), origin, scale, SpriteEffects.None, (depth + 1 + 10000) / 20000f);
            if (X > 800) {
                sb.Draw(quest, new Vector2(800 - (X - 800) / 10, Y), quest.Bounds, Color.Teal, 0, new Vector2(quest.Width / 2, quest.Height / 2), 1, SpriteEffects.None, 0);
            }
        }
    }
}
