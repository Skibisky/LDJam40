using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1.Entities {
    public class Enemy : Killable {
        public Texture2D wings;
        public Texture2D arms;
        public Texture2D bits;

        Texture2D arrow;

        public float wingRotation;
        public float armRotation;
        public float bitRotation;

        public Color wingBlend = Color.White;
        public Color armBlend = Color.White;
        public Color bitBlend = Color.White;

        static Random rand = new Random();

        int behaviour;

        int charge = 0;
        int chargeMax = 200;
        bool wasCharged;
        int refire = 0;
        bool alternate;

        float destX;
        float destY;
        float destDir;

        public Enemy() : base() {
            OnCollide += Enemy_OnCollide;
            OnDeath += Enemy_OnDeath;
            healthMax = EnemyHealth();
            health = healthMax;

            behaviour = rand.Next(0, 4); // 13
        }

        public static int EnemyHealth() {
            return 5 + Player.Current.dollars / 10 + Player.Current.powerupHealth + Player.Current.distTravelled / 1000;
        }

        public static int EnemyDamage() {
            return 1 + Player.Current.powerupDamage / 5 + Player.Current.dollars / 10 + Player.Current.distTravelled / 1000;
        }

        private void Enemy_OnDeath(object sender, EventArgs e) {
            Player.Current.kills++;
            Game1.Game.manager.Additions.Add(new Powerup() {
                X = X,
                Y = Y,
                type = 0,
                VelX = -3,
            });
        }

        private void Enemy_OnCollide(object sender, Entity e) {

        }

        public override void Create(ContentManager Content) {
            this.texture = Content.LoadFlyWeight<Texture2D>("textures/body");
            this.wings = Content.LoadFlyWeight<Texture2D>("textures/wings");
            this.arms = Content.LoadFlyWeight<Texture2D>("textures/arms");
            this.bits = Content.LoadFlyWeight<Texture2D>("textures/bits");
            this.arrow = Content.LoadFlyWeight<Texture2D>("textures/arrow");
            base.Create(Content);
            //armBlend = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);
            blend = Extensions.HSVtoRGB(rand.Next(0, 360), 0.8f, 0.8f);
            bitBlend = Extensions.HSVtoRGB(30 * behaviour, 0.8f, 0.8f);
        }

        public override void Update() {

            armRotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X) + Math.PI / 2f);
            wingRotation += (float)(Math.Sin(bitRotation) / 10);
            bitRotation += 0.1f;

            wingBlend = Extensions.HSVtoRGB((health * 120 / healthMax), 0.8f, 0.8f);
            armBlend = Extensions.HSVtoRGB(300, (charge / (float)chargeMax), 0.8f);

            // move and charge up
            switch (behaviour) {
                case 3:

                    destX = 400 + 200 * (float)Math.Sin(charge / 50f);
                    destY = 300 + 150 * (float)Math.Cos(charge / 50f);

                    destX += 64 * (float)Math.Sin(Id);
                    destY += 64 * (float)Math.Cos(Id);

                    VelX = (destX - X) / 50f;
                    VelY = (destY - Y) / 50f;

                    charge++;

                    break;
                case 2:
                    if (refire <= 0) {
                        destDir = destDir + (float)((rand.NextDouble() - 0.5) * Math.PI / 1.5);
                        destX = Player.Current.X + (float)(192 * Math.Sin(destDir));
                        destY = Player.Current.Y + (float)(192 * Math.Cos(destDir));
                        refire = 60;
                    }

                    VelX = (destX - X) / 50f;
                    VelY = (destY - Y) / 50f;

                    refire--;
                    break;
                case 1:
                    charge++;
                    destX = 600;
                    destY = 100;
                    if (Player.Current.Y < 300) {
                        destX = 500;
                    }

                    destX += 64 * (float)Math.Sin(Id);
                    destY += 64 * (float)Math.Cos(Id);

                    VelX = (destX - X) / 50f;
                    VelY = (destY - Y) / 50f;
                    
                    break;
                case 0:
                default:
                    VelX = (300 - X) / 100f;
                    if (X < 350) {
                        charge++;
                    }
                    break;
            }

            if (charge >= chargeMax) {
                wasCharged = true;
            }
            if (charge <= 0) {
                wasCharged = false;
            }

            float bvelX = (Player.Current.X - X);
            float bvelY = (Player.Current.Y - Y);
            float bdist = (float)Math.Sqrt(bvelX * bvelX + bvelY * bvelY);
            bvelX = 10 * bvelX / bdist;
            bvelY = 10 * bvelY / bdist;

            // fire!
            switch (behaviour) {
                case 1:
                    if (wasCharged) {
                        refire--;
                        if (refire <= 0) {
                          charge -= 30;
                            health -= 1;
                            alternate = !alternate;
                            if (alternate) {
                                Game1.Game.manager.Additions.Add(new Bullet() {
                                    blend = Color.Red,
                                    damage = EnemyDamage(),
                                    X = X + (float)(32 * Math.Sin(armRotation)),
                                    Y = Y + (float)(32 * Math.Cos(armRotation)),
                                    rotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X)),
                                    VelX = bvelX,
                                    VelY = bvelY,
                                    Owner = this,
                                });
                            }
                            else {
                                Game1.Game.manager.Additions.Add(new Bullet() {
                                    blend = Color.Red,
                                    damage = EnemyDamage(),
                                    X = X - (float)(32 * Math.Sin(armRotation)),
                                    Y = Y - (float)(32 * Math.Cos(armRotation)),
                                    rotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X)),
                                    VelX = bvelX,
                                    VelY = bvelY,
                                    Owner = this,
                                });
                            }
                            refire = 30;
                        }
                    }
                    break;
                case 0:
                default:
                    if (wasCharged) {
                        charge = 0;
                        wasCharged = false;
                        health -= 1;
                        Game1.Game.manager.Additions.Add(new Bullet() {
                            blend = Color.Red,
                            damage = EnemyDamage(),
                            X = X + (float)(32 * Math.Sin(armRotation)),
                            Y = Y + (float)(32 * Math.Cos(armRotation)),
                            rotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X)),
                            VelX = bvelX,
                            VelY = bvelY,
                            Owner = this,
                        });
                        health -= 1;
                        Game1.Game.manager.Additions.Add(new Bullet() {
                            blend = Color.Red,
                            damage = EnemyDamage(),
                            X = X - (float)(32 * Math.Sin(armRotation)),
                            Y = Y - (float)(32 * Math.Cos(armRotation)),
                            rotation = (float)(Math.Atan2(Player.Current.Y - Y, Player.Current.X - X)),
                            VelX = bvelX,
                            VelY = bvelY,
                            Owner = this,
                        });
                    }
                    break;
            }
        }

        public override void Render(SpriteBatch sb) {
            sb.Draw(texture, new Vector2(X, Y), texture.Bounds, blend, rotation, origin, scale, SpriteEffects.None, (depth - 1 + 10000) / 20000f);
            sb.Draw(bits, new Vector2(X, Y), texture.Bounds, bitBlend, bitRotation, origin, scale, SpriteEffects.None, (depth + 10000) / 20000f);
            sb.Draw(arms, new Vector2(X, Y), texture.Bounds, armBlend, armRotation, origin, scale, SpriteEffects.None, (depth + 1 + 10000) / 20000f);
            sb.Draw(wings, new Vector2(X, Y), texture.Bounds, wingBlend, wingRotation, origin, scale, SpriteEffects.None, (depth + 2 + 10000) / 20000f);
            if (X > 800) {
                sb.Draw(arrow, new Vector2(800 - (X - 800) / 10, Y), arrow.Bounds, Color.Red, 0, new Vector2(arrow.Width / 2, arrow.Height / 2), 1, SpriteEffects.None, 0);
            }
            else if (X < 0) {
                sb.Draw(arrow, new Vector2(- X / 10, Y), arrow.Bounds, Color.Red, 0, new Vector2(arrow.Width / 2, arrow.Height / 2), 1, SpriteEffects.FlipVertically, 0);
            }
        }

    }
}
