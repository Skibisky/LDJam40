using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities {
	public class ogpEffect : Entity {
		public string tname;

		public object Tag;
		public int type;
		public float rate;

		public ogpEffect(float x = 0, float y = 0) : base() {
			X = x;
			Y = y;
			this.inertia = 0.99f;
		}

		public override void Create(ContentManager Content) {
			if (!string.IsNullOrWhiteSpace(tname)) {
				texture = Content.LoadFlyWeight<Texture2D>(tname);
			}
			base.Create(Content);
		}

		public override void Update() {
		}
	}
}
