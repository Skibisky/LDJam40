using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Entities {
    public abstract class Killable : Entity {
        public int healthMax = 1;
        public int health = 1;

        public bool Alive = true;

        public event EventHandler<EventArgs> OnDeath;

        protected virtual void Death() { }

        internal void DeathBase() {
            OnDeath?.Invoke(this, null);
            Death();
        }

        public void Kill() {
            Alive = false;
            DeathBase();
        }

        protected internal override void UpdateBase() {
            base.UpdateBase();
            if (health <= 0) {
                Kill();
            }
            if (!Alive)
                Game1.Game.manager.Removals.Add(this.Id);
        }
    }
}
