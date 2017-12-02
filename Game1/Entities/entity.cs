using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1.Entities {
	public abstract class Entity {
		public float X;
		public float Y;
		public Texture2D texture;

		public float VelX;
		public float VelY;

		public Color blend = Color.White;
		public int depth;
		public float rotation;

		public Vector2 origin = Vector2.Zero;
		public Vector2 scale = Vector2.One;

		public long Id;

		public event EventHandler<EventArgs> OnUpdate;

		public bool Inside(float x, float y) {
			if (texture == null)
				return false;
			return (x > X - origin.X &&
				y > Y - origin.Y &&
				x < X - origin.X + texture.Width &&
				y < Y - origin.Y + texture.Height);
		}

		public event EventHandler<Entity> OnCollide;

		public void Collide(Entity other) {
			OnCollide?.Invoke(this, other);
		}

		public virtual void Create(ContentManager Content) {
			if (texture != null) {
				origin = new Vector2(texture.Width / 2, texture.Height / 2);
			}
		}

		public abstract void Update();

		public void OnRemove() {
			var k = this as Killable;
			if (k != null && k.Alive) {
				k.Kill();
			}
		}

		internal protected virtual void UpdateBase() {
			X += VelX;
			Y += VelY;
			this.Update();
			OnUpdate?.Invoke(this, null);
		}

		public virtual void Render(SpriteBatch sb) {
			if (texture != null)
				sb.Draw(texture, new Vector2(X, Y), texture.Bounds, blend, rotation, origin, scale , SpriteEffects.None, (depth + 10000) / 20000f);
		}
	}
}
