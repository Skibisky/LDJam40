using Game1.Entities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {
    public class EntityManager {

        public Dictionary<long, Entity> Entities = new Dictionary<long, Entity>();
        public List<long> Removals = new List<long>();
        public List<Entity> Additions = new List<Entity>();

        long nextId = 10000;

        ContentManager content;

        public EntityManager(ContentManager c) {
            content = c;
        }

        public long Add<T>(T entity) where T : Entity {
            var cId = nextId++;
            entity.Id = cId;
            entity.Create(content);
            entity.depth = 0;
            Entities.Add(cId, entity);
            return cId;
        }

        public void Update() {
            foreach (var e in Additions) {
                Add<Entity>(e);
            }
            Additions.Clear();
            foreach (var e in Entities) {
                if (!Removals.Contains(e.Key))
                    e.Value.UpdateBase();
            }

            foreach (var i in Entities) {
                if (Removals.Contains(i.Key)) continue;
                foreach (var j in Entities) {
                    if (Removals.Contains(j.Key)) continue;

                    if (i.Key != j.Key && (i.Value.Inside(j.Value.X, j.Value.Y) || j.Value.Inside(i.Value.X, i.Value.Y))) {
                        i.Value.Collide(j.Value);
                        if (!Removals.Contains(i.Key) && !Removals.Contains(j.Key))
                            j.Value.Collide(i.Value);
                    }
                }
            }

            foreach (var i in Removals.Distinct()) {
                Entities[i].OnRemove();
                Entities.Remove(i);
            }
            Removals.Clear();
        }

        public void Render(SpriteBatch sb) {
            foreach (var e in Entities) {
                e.Value.Render(sb);
            }
        }

    }
}
