using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Entities {
    public class ogtEffect : Entity {
        public string fname;

        public string text;

        public SpriteFont font;

        public object Tag;
        public int type;
        public float rate;
        private float x;

        public override void Create(ContentManager Content) {
            if (!string.IsNullOrWhiteSpace(fname)) {
                font = Content.LoadFlyWeight<SpriteFont>(fname);
            }
            base.Create(Content);
        }

        public override void Update() {
        }

        public override void Render(SpriteBatch sb) {
            var sz = font.MeasureString(text);
            var v = new Vector2(X - sz.X, Y - sz.Y);
            sb.DrawString(font, text, v, blend, rotation, new Vector2(sz.X / 2, sz.Y/2), scale, SpriteEffects.None, depth);
        }
    }
}
