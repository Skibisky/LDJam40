using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1 {
	public static class Extensions {
		private static Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>> ();

		public static T LoadFlyWeight<T>(this ContentManager content, string fname) {
			if (!data.ContainsKey(typeof(T))) {
				data.Add(typeof(T), new Dictionary<string, object>());
			}
			var d = data[typeof(T)];
			if (!d.ContainsKey(fname)) {
				d.Add(fname, content.Load<T>(fname));
			}
			return (T)d[fname];
		}

		public static Color WithA(this Color c, byte a) {
			var cc = new Color(c.R, c.G, c.B) {
				A = a,
			};
			return cc;
		}

		public static void DrawRectangle(this SpriteBatch sb, Rectangle rec, Color col) {
			sb.Draw(Game1.Game.Content.LoadFlyWeight<Texture2D>("textures/pixel"), rec, col);
		}

		public static Color HSVtoRGB(int hue, float sat, float val) {
			var col = new Color();
			col.A = 255;

			float c = sat * val;
			float x = c * (1 - Math.Abs(((hue / 60f) % 2) - 1));
			float m = val - c;

			if (hue >= 0 && hue < 60) {
				col.R = (byte)(c * 255);
				col.G = (byte)(x * 255);
			}
			else if (hue >= 60 && hue < 120) {
				col.G = (byte)(c * 255);
				col.R = (byte)(x * 255);
			}
			else if (hue >= 120 && hue < 180) {
				col.G = (byte)(c * 255);
				col.B = (byte)(x * 255);
			}
			else if (hue >= 180 && hue < 240) {
				col.G = (byte)(x * 255);
				col.B = (byte)(c * 255);
			}
			else if (hue >= 240 && hue < 300) {
				col.R = (byte)(x * 255);
				col.B = (byte)(c * 255);
			}
			else if (hue >= 300 && hue < 360) {
				col.R = (byte)(c * 255);
				col.B = (byte)(x * 255);
			}
			col.R += (byte)(255 * m);
			col.G += (byte)(255 * m);
			col.B += (byte)(255 * m);

			return col;
		}
	}
}
