using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment.Skills
{
    /// <summary>
    /// 瞄准,射击
    /// </summary>
    public class FrostBombardment_Aim_Shoot : BasicFrostBombardment
    {
        public NPC Target = null;
        public FrostBombardment_Aim_Shoot(FrostBombardment_Proj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Player.itemTime = Player.itemAnimation = 5;
            Player.heldProj = Projectile.whoAmI;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 装填子弹
                    {
                        Projectile.ai[1]++;
                        Projectile.spriteDirection = Player.direction;
                        if (Projectile.ai[1] < 10) // 旋转武器,让其进入装弹动作
                        {
                            Projectile.Center += (Player.Center - Projectile.Center) * 0.8f + Projectile.velocity * 15 * Player.direction;
                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0.2f * Player.direction, 0.5f);
                        }
                        else
                        {
                            Projectile.Center = Player.Center + Projectile.velocity * 15 * Player.direction;
                            int time = (int)(Projectile.ai[1] - 10);
                            Player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) + (time - 10) * -0.1f * Player.direction;
                            if (time > 12)
                            {
                                SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.3f }, Player.position);
                                Projectile.ai[0]++;
                                Projectile.ai[1] = 0;
                            }
                        }
                        Projectile.velocity = Projectile.rotation.ToRotationVector2();
                        break;
                    }
                case 1: // 瞄准
                    {
                        Projectile.ai[2] = 0;
                        #region 霜星的特殊瞄准
                        if (Player.controlUseTile)
                        {
                            Projectile.ai[2] = 1;
                            Vector2 toNextScreenPosVel = Main.MouseWorld - Player.Center;
                            Vector2 toTargetScreenPosVel = Target != null ? (Main.MouseWorld - Target.Center) * -0.5f : Vector2.Zero;
                            Vector2 ScreenCenter = Main.ScreenSize.ToVector2() * 0.5f;
                            ScreenCenter -= toTargetScreenPosVel;
                            Vector2 nextScreenPos = CameraModifierToScreenPos.GetScreenPos(Player.Center + toNextScreenPosVel * 0.95f + toTargetScreenPosVel);
                            Main.instance.CameraModifiers.Add(new CameraModifierToScreenPos(Vector2.Lerp(Main.screenPosition,nextScreenPos,(Main.MouseScreen - ScreenCenter).LengthSquared() * 0.000001f), 10));
                        }
                        #endregion
                        if (Player.controlUseItem)
                        {
                            Vector2 vel = (Main.MouseWorld - Projectile.Center);
                            #region 搜寻距离鼠标最近的敌人
                            float maxDir = 300f;
                            foreach (NPC n in Main.npc)
                            {
                                float dir = (n.Center - Main.MouseWorld).Length();
                                if (dir < maxDir && n.active && !n.dontTakeDamage)
                                {
                                    maxDir = dir;
                                    Target = n;
                                }
                            }
                            #endregion
                            if (Target != null)
                            {
                                if (Target.whoAmI == -1 || (Target.Center - Main.MouseWorld).LengthSquared() > 600 * 600)
                                {
                                    Target = null;
                                }
                                else if(!Target.friendly && !frostBombardment.SourceItem.InBomMode)
                                {
                                    for(int time = 0;time < 120;time++)
                                    {
                                        Vector2 NPC_pos = Target.Center + Collision.TileCollision(Target.Center,Target.velocity * time * 0.85f,Target.width,Target.height,true);
                                        float flyTime = (NPC_pos - Projectile.Center).Length() / 16;
                                        if(flyTime < time)
                                        {
                                            NPC newTarget = Target.Clone() as NPC;
                                            //newTarget.Size = Target.Size;
                                            newTarget.whoAmI = -1;
                                            newTarget.realLife = Target.whoAmI;
                                            newTarget.position = NPC_pos;
                                            vel = NPC_pos - Projectile.Center;
                                            Target = newTarget;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    vel = Target.Center - Projectile.Center;
                                }
                            }
                            Player.ChangeDir((vel.X > 0).ToDirectionInt());
                            Projectile.rotation = vel.ToRotation() - (Player.direction == 1 ? 0 : MathHelper.Pi);
                            Projectile.velocity = Projectile.rotation.ToRotationVector2() * Player.direction;
                            Projectile.Center = Player.Center + Projectile.velocity * 15;
                            Projectile.spriteDirection = Player.direction;
                            Player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                        }
                        else
                        {
                            int changeLevel = frostBombardment.SourceItem.ChangeLevel;
                            frostBombardment.SourceItem.ChangeLevel = 0;
                            int aimTarget = Target == null ? -1 : (Target.whoAmI == -1 ? Target.realLife : Target.whoAmI);
                            if (aimTarget > -1 && (Target.friendly || Target.townNPC)) aimTarget = -1;
                            int boom = frostBombardment.SourceItem.InBomMode.ToInt();
                            if (boom == 0)
                                SkillTimeOut = true;
                            else
                            {
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity, 20, 2,10));
                                SoundEngine.PlaySound(SoundID.Item45 with { Pitch = -0.45f }, Player.position);
                            }
                            Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Projectile.Center, Projectile.velocity * 16f, ModContent.ProjectileType<EnergyBullet_FrostBombardment>(), Projectile.damage * (changeLevel + 1), Projectile.knockBack, Player.whoAmI, changeLevel, aimTarget,boom);
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    }
                case 2: // 激光炮的后摇
                    {
                        Projectile.ai[1] += MathHelper.SmoothStep(0f, 3f, (Projectile.ai[1] + 1f) / 15f);
                        Projectile.Center = Player.Center + Projectile.velocity * 15;

                        if (Projectile.ai[1] >= 1)
                        {
                            Player.velocity.X += Player.direction * 0.5f;
                            if (Projectile.ai[1] > 3)
                            {
                                SkillTimeOut = true;
                            }
                        }
                        else if (Projectile.ai[1] < 1)
                        {
                            Player.velocity.X -= Player.direction;
                            Projectile.rotation += Projectile.ai[1] / MathHelper.TwoPi * -Player.direction;
                        }
                        break;
                    }
            }
        }
        public override bool SwitchCondition()
        {
            return false;
        }
        public override bool ActivationCondition()
        {
            return Player.controlUseItem && frostBombardment.IsUseGun;
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Target = null;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Texture2D getTex = GetTexture();
            sb.Draw(getTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, getTex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            gd.Textures[0] = TextureAssets.MagicPixel.Value;

            #region 如果搜寻到目标时,则框住NPC的边缘,并且中心与弹幕连接一条直线
            if ((int)Projectile.ai[0] == 1 && Target != null)
            {
                Color drawColor = Color.SkyBlue;
                if (Target.friendly || Target.townNPC)
                {
                    drawColor = Color.Green;
                }
                if (Target.boss)
                {
                    drawColor = Color.Lerp(drawColor, Color.Red, Main.DiscoB / 255f);
                }
                List<CustomVertexInfo> list = new()
                {
                    new(Target.TopLeft - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                    new(Target.TopRight - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.BottomRight - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.BottomLeft - Main.screenPosition, drawColor, new Vector3(1, 0, 0)),
                    new(Target.TopLeft - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                };

                gd.DrawUserPrimitives(PrimitiveType.LineStrip, list.ToArray(), 0, list.Count - 1);
                list = new()
                {
                    new(Target.Left - new Vector2(Target.width * 0.3f,0) - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.Left + new Vector2(Target.width * 0.3f, 0) - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                    new(Target.Right - new Vector2(Target.width * 0.3f, 0) - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.Right + new Vector2(Target.width * 0.3f, 0) - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                    new(Target.Top - new Vector2(0,Target.height * 0.3f) - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.Top + new Vector2(0,Target.height * 0.3f) - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                    new(Target.Bottom - new Vector2(0,Target.height * 0.3f) - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Target.Bottom + new Vector2(0,Target.height * 0.3f) - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                    new(Target.Center - Main.screenPosition, drawColor, new Vector3(0, 1, 0)),
                    new(Projectile.Center + (Projectile.velocity * Projectile.width * 0.42f) - Main.screenPosition, drawColor, new Vector3(0, 0, 0)),
                };

                gd.DrawUserPrimitives(PrimitiveType.LineList, list.ToArray(), 0, list.Count - 1);
            }
            #endregion

            #region 瞄准
            if (Player.controlUseTile)
            {
                CustomVertexInfo[] list =
                {
                    new(Main.MouseScreen, Color.IndianRed, new Vector3(0, 0, 0)),
                    new(Projectile.Center + (Projectile.velocity * Projectile.width * 0.42f) - Main.screenPosition, Color.IndianRed, new Vector3(0, 1, 0)),
                };
                gd.DrawUserPrimitives(PrimitiveType.LineList, list, 0, list.Length);

                float dis = (list[0].Position - list[1].Position).Length();
                int Change = frostBombardment.SourceItem.ChangeLevel;
                float time = dis / 16 / 60;
                if (frostBombardment.SourceItem.InBomMode)
                    time = 0f;
                ReLogic.Graphics.DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = "Dis:" + (int)(dis / 16) + "\nChange Level:" + Change + "\nTime:" + time.ToString("#0.00") + "s";
                Color textColor = Color.White;
                if (Target?.friendly == true)
                {
                    text += "\nNo Target.";
                    textColor = Color.Lerp(textColor, Color.LightGreen, Main.DiscoB / 255f);
                }
                else if(Target?.boss == true)
                {
                    text += "\nWarning!";
                    textColor = Color.Lerp(textColor, Color.Red, Main.DiscoB / 255f);
                }

                if (Target?.dontTakeDamage == true)
                {
                    text += "\nCannot Hit!";
                    textColor = Color.Lerp(textColor, Color.Red, Math.Clamp(Main.DiscoB / 255f,0.5f,1f));
                }
                Vector2 origin = font.MeasureString(text);
                Vector2 textPos = Main.MouseScreen + new Vector2(0, -origin.Y * 0.6f * (Main.mouseY > Main.screenHeight * 0.5f).ToDirectionInt());
                Utils.DrawBorderStringFourWay(sb, font, text, textPos.X, textPos.Y, textColor, Color.Black, origin * 0.5f, 0.8f);
            }
            #endregion
            return false;
        }
    }
}
