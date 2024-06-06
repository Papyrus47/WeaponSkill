using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Configs;
using WeaponSkill.Helper;
using WeaponSkill.NPCs;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.Weapons.Spears;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills;
using static WeaponSkill.UI.StarBreakerUI.SkillsTreeUI.SkillsTreeUI;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFistProj : ModProjectile, IBasicSkillProj
    {
        public override string Texture => (GetType().Namespace + "." + "FrostFist").Replace('.', '/');
        public Player Player;
        public bool AddSwordRender;
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        /// <summary>
        /// 可以用于切换停止技能
        /// </summary>
        public bool CanChangeToStopActionSkill;
        /// <summary>
        /// 剑的挥舞用的
        /// </summary>
        public SwingHelper SwordSwingHelper;
        /// <summary>
        /// 剑的长度
        /// </summary>
        public float SwordLength;
        public FrostFistItem frostFistItem => Player.HeldItem.ModItem as FrostFistItem;
        public static List<int> FrostFistRenderIndex = new();
        public override void Load()
        {
            FrostFistRenderIndex = new();
        }
        public override void Unload()
        {
            FrostFistRenderIndex = null;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                Player = itemUse.Player;
                Projectile.Size = itemUse.Item.Size;
                SwordSwingHelper = new(Projectile, 20, ModContent.Request<Texture2D>(Texture + "_LongSword"));
                SwordLength = 120;
                Init();
            }
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (Player.HeldItem.ModItem is not FrostFistItem || !Player.active || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => CurrentSkill.OnHitNPC(target, hit, damageDone);
        public override bool PreDraw(ref Color lightColor)
        {
            SkillsTreeUI.nowSkill = CurrentSkill;
            CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor); 
            if (AddSwordRender)
            {
                if (!FrostFistRenderIndex.Contains(Projectile.whoAmI)) FrostFistRenderIndex.Add(Projectile.whoAmI);
                AddSwordRender = false;
                if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is FrostFist_Proj_Render))
                {
                    WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new FrostFist_Proj_Render());
                }
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D UITex = ModContent.Request<Texture2D>(Texture + "_ChangedUI").Value;
            Rectangle rect = new(0, 0, 76, 28);
            Vector2 position = Main.ScreenSize.ToVector2() * WS_Configs_UI.Init.SpiritUI_Pos;
            position.X += 300;
            position.Y -= 70;
            Main.spriteBatch.Draw(UITex, position, rect, Color.White,0, new Vector2(0,rect.Height / 2),1.5f, SpriteEffects.None,0f);
            rect = new(12, 28, (int)(62 * (frostFistItem.ChangeLevel / 20f)), 28);
            Main.spriteBatch.Draw(UITex, position, rect, Color.White, 0, new Vector2(-12, rect.Height / 2), 1.5f, SpriteEffects.None, 0f);
        }
        public void Init()
        {
            OldSkills = new();

            #region 技能创建
            FrostFistNotUse frostFistNotUse = new(this);

            #region 左键派生/速攻拳连段
            FrostFist_FistHit fistHit_SpeedAtk_Hit1 = new(this, () => Player.controlUseItem);
            FrostFist_FistHit fistHit_SpeedAtk_Hit2 = new(this, () => Player.controlUseItem);

            FrostFist_FistHit_MoveFist fistHit_SpeedAtk_Hit3 = new(this, () => Player.controlUseItem)
            {
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if(proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_SpeedAtk_Hit4 = new(this, () => Player.controlUseItem)
            {
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = -Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = (center - proj.Center);
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit fistHit_SpeedAtk_Hit5 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 1.2f
            };

            FrostFist_FistHit fistHit_SpeedAtk2 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 2f,
                ExtraAction = () =>
                {
                    Player.SetImmuneTimeForAllTypes(2);
                    if (Projectile.ai[0] < 10)
                    {
                        Player.velocity.X = Player.direction * 40;
                        for (int i = 0; i < 80; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue);
                            dust.position = Player.Center;
                            dust.scale = 0.3f;
                            Vector2 vel = Player.velocity.RotatedBy(i / 80f * MathHelper.TwoPi).SafeNormalize(default) * 3.5f;
                            vel.X *= 0.2f;
                            vel = vel.RotatedBy(Player.velocity.ToRotation());
                            dust.velocity = vel;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            //dust.velocity += Projectile.velocity.RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f) * 1.2f;
                        }
                    }
                    else
                    {
                        Player.velocity.X *= 0.1f;
                    }
                },
                FistHitProjChange = (Projectile proj) =>
                {
                    proj.extraUpdates -= 1;
                    proj.velocity.Y = 0;
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = Player.velocity.X * 1.2f;
                }
            };
            FrostFist_FistHit_MoveFist fistHit_SpeedAtk3 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.2f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * MathF.Sqrt(proj.ai[0] / 240f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                ExtraAction = () =>
                {
                    Player.SetImmuneTimeForAllTypes(2);
                    if (Projectile.ai[0] < 10)
                    {
                        Player.velocity.Y = -20;
                    }
                    else
                    {
                        Player.velocity.Y = 0;
                    }
                },
                FistHitProjChange = (proj) =>
                {
                    proj.timeLeft = 240;
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = Player.velocity.Y;
                }
            };
            FrostFist_FistHit_SkyFallFist frostFist_FistHit_SkyFallFist = new(this)
            {
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = Main.rand.Next(new int[] { 1, -1 }) * 30;
                }
            };
            #endregion
            #region 右键派生/通用
            FrostFist_FistHit fistHit_GeneralAtk1 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.3f,
            };
            FrostFist_FistHit_MoveFist fistHit_GeneralAtk2 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.5f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY.RotatedBy(0.6 * Player.direction) * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 1.4f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_GeneralAtk3 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.5f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = -9;
                }
            };
            #endregion
            #region 拳的搓招
            #region 连续拳
            FrostFist_FistHit_CoiledFist frostFist_FistHit_CoiledFist = new(this, () => GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)) && Player.controlUseItem);
            FrostFist_FistHit frostFist_FistHit_CoiledFist_AfterFist = new(this, () => Player.controlUseTile || Player.controlUseItem)
            {
                ExtraAction = () =>
                {
                    Player.SetImmuneTimeForAllTypes(2);
                    if (Projectile.ai[0] < 10)
                    {
                        Player.velocity.X = Player.direction * 30;
                        for (int i = 0; i < 80; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue);
                            dust.position = Player.Center;
                            dust.scale = 0.3f;
                            Vector2 vel = Player.velocity.RotatedBy(i / 80f * MathHelper.TwoPi).SafeNormalize(default) * 3.5f;
                            vel.X *= 0.2f;
                            vel = vel.RotatedBy(Player.velocity.ToRotation());
                            dust.velocity = vel;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            //dust.velocity += Projectile.velocity.RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f) * 1.2f;
                        }
                    }
                    else
                    {
                        Player.velocity.X *= 0.1f;
                    }
                },
                FistHitProjChange = (Projectile proj) =>
                {
                    proj.extraUpdates -= 1;
                    proj.velocity.Y = 0;
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = Player.velocity.X * 0.6f;
                }
            };
            #endregion
            #region 双摆拳
            FrostFist_FistHit_MoveFist fistHit_DoubleHit = new(this, () => GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)) && Player.controlUseTile)
            {
                ActionDamage = 2f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY.RotatedBy(0.6 * Player.direction) * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 1.4f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_DoubleHit2 = new(this, () => true)
            {
                ActionDamage = 2.5f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = (-Vector2.UnitY).RotatedBy(0.6 * -Player.direction) * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 1.4f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_DoubleHit3 = new(this, () => Player.controlUseTile || Player.controlUseItem)
            {
                ActionDamage = 2f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * MathF.Sqrt(proj.ai[0] / 120f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 3)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                ExtraAction = () =>
                {
                    Player.SetImmuneTimeForAllTypes(2);
                    if (Projectile.ai[0] < 10)
                    {
                        Player.velocity.Y = -20;
                    }
                    else
                    {
                        Player.velocity.Y = 0;
                    }
                },
                FistHitProjChange = (proj) =>
                {
                    proj.timeLeft = 120;
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = Player.velocity.Y;
                }
            };
            #endregion
            #region 蓄力升龙
            FrostFist_FistHit_ChangeRisingFist frostFist_FistHit_ChangeRisingFist = new(this,() => GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)) && Player.controlUseItem);
            #endregion
            #region 蓄力冲拳
            FrostFist_FistHit_ChangeDashFist frostFist_FistHit_ChangeDashFist = new(this, () => GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)) && Player.controlUseTile);
            #endregion
            #endregion
            #region 刀的连段

            Func<float, float> SwordChangeTime = (factor) =>
            {
                return MathF.Pow(factor, 2.5f);
            };

            FrostFist_SwordSwing frostFist_SwordSwing_Start = new(this, () => !WeaponSkill.BowSlidingStep.Current && WeaponSkill.BowSlidingStep.JustReleased)
            {
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                SwingRot = MathHelper.TwoPi * 0.65f,
                SwingDirectionChange = false,
                AddDmg = 0.5f,
                PreAtkTime = 6,
                AtkTime = 30,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };

            FrostFist_SwordSwing_SkyFall frostFist_SwordSwing_SkyFall = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = (-Vector2.UnitY).RotatedBy(-0.5),
                SwingRot = MathHelper.PiOver2 + 0.6f,
                SwingDirectionChange = true,
                AddDmg = 1.5f,
                PreAtkTime = 10,
                AtkTime = 25,
                PostAtkTime = 60,
                TimeChange = SwordChangeTime,
            };

            #region 霜拳舞刃式·速
            FrostFist_SwordSwing frostFist_SwordSwing_Speed_Start = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = -Vector2.UnitY.RotatedBy(-0.6),
                SwingRot = MathHelper.TwoPi * 0.65f,
                SwingDirectionChange = true,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 25,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 30;
                    }
                    else
                    {
                        Player.velocity.X = 0;
                    }
                }
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Speed_3 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = true,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 25,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Speed_1 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                SwingRot = MathHelper.TwoPi * 0.65f,
                SwingDirectionChange = false,
                AddDmg = 0.5f,
                PreAtkTime = 6,
                AtkTime = 25,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Speed_2 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = -Vector2.UnitY.RotatedBy(-0.6),
                SwingRot = MathHelper.TwoPi * 0.65f,
                SwingDirectionChange = true,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 25,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 30;
                    }
                    else
                    {
                        Player.velocity.X = 0;
                    }
                }
            };

            #region 速动冲拳派生
            FrostFist_FistHit fistHit_SpeedAtk_SpeedFrozen1 = new(this, () => Player.controlUseItem);
            FrostFist_FistHit_MoveFist fistHit_SpeedAtk_SpeedFrozen2 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.5f,
                ExtraAction = () =>
                {
                    if (Projectile.ai[0] < 5)
                    {
                        Player.velocity.X = Player.direction * 30;
                    }
                    else
                    {
                        Player.velocity.X *= 0.3f;
                    }
                },
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitX * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 0.8f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit fistHit_SpeedAtk_SpeedFrozen3 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.8f,
                ExtraAction = () =>
                {
                    if (Projectile.ai[0] < 5 && Projectile.ai[1] < 1 && Player.CheckMana(100,true))
                    {
                        Projectile.ai[1] = 1;
                    }
                    else
                    {
                        Projectile.ai[1] = 0;
                    }
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (Projectile.ai[1] < 1) return;
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * 3))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #region 击退横扫拳派生
            FrostFist_FistHit_MoveFist fistHit_SpeedAtk_KnockbackFist = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 2.5f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitX * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 0.8f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist != 0) npc.velocity.X += hitInfo.HitDirection * 15;
                    FrostFist_FistBoom frostFist_FistBoom = new(60, Player, new Vector2(hitInfo.HitDirection * 6, 0), (int)(Projectile.damage * 3));
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #region 乱斩派生
            FrostFist_SwordSwing_SpeedSlash frostFist_SwordSwing_SpeedSlash = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = -0.5f,
                PreAtkTime = 6,
                AtkTime = 40,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 30;
                    }
                    else
                    {
                        Player.velocity.X = 0;
                    }
                }
            };
            #endregion
            #region 冻击连续拳
            FrostFist_FistHit fistHit_SpeedAtk_FrozenCoiledFist = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.8f,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            FrostFist_FistHit_CoiledFist fistHit_SpeedAtk_FrozenCoiledFist2 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 0.2f,
                HitCounst = 30
            };
            #endregion
            #region 冻结斩
            FrostFist_SwordSwing frostFist_SwordSwing_Speed_FrozenSlash = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                SwingRot = MathHelper.TwoPi * 0.65f,
                SwingDirectionChange = false,
                AddDmg = 1.5f,
                PreAtkTime = 6,
                AtkTime = 50,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 180;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #region 最 速 连 击
            FrostFist_SpeedAtk_FastetHit frostFist_SpeedAtk_FastetHit = new(this, () => Player.controlUseTile);
            #endregion
            #endregion
            #region 霜拳舞刃式·力
            FrostFist_SwordSwing frostFist_SwordSwing_Strong_1 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = true,
                AddDmg = 1.8f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Strong_2 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitY,
                SwingRot = MathHelper.Pi * 0.8f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Strong_3 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = -Vector2.UnitX.RotatedBy(0.6),
                SwingRot = MathHelper.PiOver2 * 1.6f,
                SwingDirectionChange = true,
                AddDmg = 2f,
                PreAtkTime = 10,
                AtkTime = 35,
                PostAtkTime = 60,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * 3))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            FrostFist_SwordSwing frostFist_SwordSwing_Strong_4 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitX.RotatedBy(0.8),
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 2f,
                PreAtkTime = 4,
                AtkTime = 35,
                PostAtkTime = 60,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * 3))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #region 猛拳派生
            FrostFist_FistHit_MoveFist fistHit_StrongFistAtk1 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.8f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = -Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = (center - proj.Center);
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_StrongFistAtk2 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.8f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = -Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = (center - proj.Center);
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            }; 
            FrostFist_FistHit fistHit_StrongFistAtk3 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.9f,
                OnHit = (npc, hit, _) =>
                {
                    frostFistItem.ChangeLevel += 3;
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = hit.HitDirection * 10;
                }
            };
            #endregion
            #region 冰刺派生
            FrostFist_FistHit_MoveFist fistHit_IceSpurts = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.5f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY.RotatedBy(0.6 * Player.direction) * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 1.4f);
                    vel.Y *= 0.5f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            if (Player.CheckMana(200, true))
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(Player.HeldItem), Projectile.Center + new Vector2(Player.direction * 100 + i * Player.direction * 40, 30), Vector2.UnitX.RotatedBy(-0.6 * Player.direction) * Player.direction, 961, Projectile.damage * 5, 0f, Player.whoAmI, 0f, 0.8f);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.penetrate = -1;
                                    projectile.usesLocalNPCImmunity = true;
                                    projectile.localNPCHitCooldown = -1;
                                    projectile.ArmorPenetration = int.MaxValue;
                                }
                            }
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                }
            };
            #endregion
            #region 冲刺派生
            FrostFist_SwordSwing_Dash frostFist_SwordSwing_Dash = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 45,
                PostAtkTime = 30,
                TimeChange = SwordChangeTime
            };
            #endregion
            #region 神圣反击派生
            FrostFist_FistHit_HolyStrikesBack frostFist_FistHit_HolyStrikesBack = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY.RotatedBy(0.6 * Player.direction) * 40;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player,new Vector2(hitInfo.HitDirection) * 10, (int)(Projectile.damage * 2));
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #region 充能拳派生
            FrostFist_FistHit fistHit_ChangedFist = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 2f,
                ExtraAction = () =>
                {
                    if (Projectile.ai[0] == 2)
                    {
                        Vector2 vel = Main.MouseWorld - Projectile.Center;
                        for (int i = 0; i < 32; i++)
                        {
                            Vector2 center = Player.HandPosition.Value - Player.velocity;
                            Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FrostStaff, Player.direction * 2, 0, 150, default, 1.3f);
                            dust.position = center;
                            dust.velocity *= 0;
                            dust.noGravity = true;
                            dust.fadeIn = 1;
                            dust.velocity += Player.velocity * 0.1f;
                            dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2).RotatedBy(MathF.Sin(i / 3f * MathHelper.TwoPi) * 0.5f) * i * 0.3f;
                            dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                        }
                    }
                },
                OnHit = (npc, hit, _) =>
                {
                    frostFistItem.ChangeLevel += 10;
                }
            };
            #endregion
            #endregion
            #region 霜拳舞刃式·瞬
            FrostFist_SwordSwing frostFist_MomentSwordSwing1 = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 20,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    Player.SetImmuneTimeForAllTypes(3);
                    switch (Projectile.ai[0])
                    {
                        case 0:
                            Player.velocity.X = Player.direction * 35;
                            break;
                        case 1:
                            Projectile.extraUpdates = 4;
                            Player.velocity.X = 0;
                            break;
                    }
                }
            };
            FrostFist_SwordSwing frostFist_MomentSwordSwing2 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 20,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    Player.SetImmuneTimeForAllTypes(3);
                    switch (Projectile.ai[0])
                    {
                        case 0:
                            if ((int)Projectile.ai[1] == 1) Player.direction = -Player.direction;
                            Player.velocity.X = Player.direction * 40;
                            break;
                        case 1:
                            Projectile.extraUpdates = 4;
                            Player.velocity.X = 0;
                            break;
                    }
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = -9;
                }
            };
            FrostFist_SwordSwing frostFist_MomentSwordSwing3 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 0.4f) * 2,
                VisualRotation = 0f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1.5f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 20,
                TimeChange = SwordChangeTime,
                SwingAI = () =>
                {
                    Player.SetImmuneTimeForAllTypes(3);
                    switch (Projectile.ai[0])
                    {
                        case 0:
                            if ((int)Projectile.ai[1] == 2) Player.direction = -Player.direction;
                            break;
                        case 1:
                            (CurrentSkill as FrostFist_SwordSwing).swingHelper.SetRotVel(-0.4f);
                            Projectile.extraUpdates = 4;
                            Player.velocity.X = 0;
                            break;
                    }
                }
            };

            #region 猛砸冻斩派生
            FrostFist_SwordSwing frostFist_MomentSwor_StrongSwing = new(this, () => WeaponSkill.BowSlidingStep.Current && CanChangeToStopActionSkill)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitX.RotatedBy(0.8),
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1.5f,
                PreAtkTime = 10,
                AtkTime = 40,
                PostAtkTime = 60,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (!Player.CheckMana(200, true)) return;
                    FrostFist_FistBoom frostFist_FistBoom = new(10, Player, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * 3))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 300;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #region 瞬·上勾拳派生
            FrostFist_FistHit frostFist_FistHit_Moment_RisingFist1 = new(this, () => Player.controlUseItem)
            {
                ExtraAction = () =>
                {
                    Player.SetImmuneTimeForAllTypes(2);
                    if (Projectile.ai[0] < 10)
                    {
                        Player.velocity.X = Player.direction * 30;
                        for (int i = 0; i < 80; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue);
                            dust.position = Player.Center;
                            dust.scale = 0.3f;
                            Vector2 vel = Player.velocity.RotatedBy(i / 80f * MathHelper.TwoPi).SafeNormalize(default) * 3.5f;
                            vel.X *= 0.2f;
                            vel = vel.RotatedBy(Player.velocity.ToRotation());
                            dust.velocity = vel;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            //dust.velocity += Projectile.velocity.RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f) * 1.2f;
                        }
                    }
                    else
                    {
                        Player.velocity.X *= 0.1f;
                    }
                },
                FistHitProjChange = (Projectile proj) =>
                {
                    proj.extraUpdates -= 1;
                    proj.velocity.Y = 0;
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = Player.velocity.X * 0.6f;
                }
            };
            FrostFist_FistHit_MoveFist frostFist_FistHit_Moment_RisingFist2 = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2f,
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player, new Vector2(0,-5), (int)(Projectile.damage * 2));
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = -9;
                }
            };
            #endregion
            #region 霜拳拳法·冻
            FrostFist_FistHit fistHit_Moment_Hit1 = new(this, () => Player.controlUseTile)
            {
                OnHit = (npc,_,_) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player,Vector2.Zero, (int)(Projectile.damage))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            FrostFist_FistHit fistHit_Moment_Hit2 = new(this, () => Player.controlUseTile)
            {
                OnHit = (npc, _, _) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player, Vector2.Zero, (int)(Projectile.damage))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 60;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #region 封印术
            FrostFist_FistHit_MoveFist fistHit_Moment_SealFist = new(this, () => Player.controlUseItem)
            {
                ActionDamage = 2.5f,
                ExtraAction = () =>
                {
                    if (Projectile.ai[0] < 5)
                    {
                        Player.velocity.X = Player.direction * 30;
                    }
                    else
                    {
                        Player.velocity.X *= 0.3f;
                    }
                },
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitX * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi * 0.8f);
                    vel.Y *= 0.6f;
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (npc, _, _) =>
                {
                    npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal = 3600;
                }
            };
            #endregion

            FrostFist_FistHit_MoveFist fistHit_Moment_Hit3 = new(this, () => Player.controlUseTile)
            {
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(-Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = center - proj.Center;
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (npc, _, _) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player, Vector2.Zero, (int)(Projectile.damage * 2))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 120;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            FrostFist_FistHit_MoveFist fistHit_Moment_Hit4 = new(this, () => Player.controlUseTile)
            {
                FistMoveAI = (Projectile proj) =>
                {
                    Vector2 StartVel = -Vector2.UnitY * 60;
                    Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                    Vector2 center = Player.Center + vel;
                    proj.velocity = (center - proj.Center);
                    if (proj.ai[0] > 59)
                    {
                        proj.ai[0] = 60;
                        proj.extraUpdates = 0;
                        proj.timeLeft = 5;
                        Projectile.ai[0] = 14;
                        if (proj.ai[1]++ > 10)
                        {
                            proj.Kill();
                        }
                    }
                    //proj.velocity = proj.velocity.RotatedBy(0.3);
                },
                OnHit = (npc, _, _) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player, Vector2.Zero, (int)(Projectile.damage * 2))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 180;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            FrostFist_FistHit fistHit_Moment_Hit5 = new(this, () => Player.controlUseTile)
            {
                ActionDamage = 1.2f,
                OnHit = (npc, _, _) =>
                {
                    FrostFist_FistBoom frostFist_FistBoom = new(30, Player, Vector2.Zero, (int)(Projectile.damage * 2))
                    {
                        ExtraAI = (NPC npc) =>
                        {
                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 360;
                        }
                    };
                    WeaponSkillGlobalNPC.AddComponent(npc, frostFist_FistBoom);
                }
            };
            #endregion
            #endregion
            #region 大招·极速乱刃
            FrostFist_SwordSwing_CrazySpeedSlash frostFist_SwordSwing_CrazySpeedSlash1 = new(this)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = -0.9f,
                PreAtkTime = 6,
                AtkTime = 40,
                PostAtkTime = 120,
                TimeChange = SwordChangeTime
            };
            FrostFist_SwordSwing frostFist_SwordSwing_CrazySpeedSlash2 = new(this, () => true)
            {
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 6,
                AtkTime = 35,
                PostAtkTime = 20,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.X = hitInfo.HitDirection * 20;
                }
            };
            #endregion
            #region 刀的搓招
            FrostFist_SwordSwing_CutItInTwo frostFist_SwordSwing_CutItInTwo = new(this, () => GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)) && WeaponSkill.BowSlidingStep.Current)
            {
                TimeChange = SwordChangeTime
            };

            FrostFist_SwordSwing frostFist_SwordSwing_RisingSlash = new(this, () => GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)) && WeaponSkill.BowSlidingStep.Current)
            {
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                StartVel = Vector2.UnitX.RotatedBy(0.8),
                SwingRot = MathHelper.TwoPi * 0.75f,
                SwingDirectionChange = false,
                AddDmg = 1f,
                PreAtkTime = 4,
                AtkTime = 35,
                PostAtkTime = 60,
                TimeChange = SwordChangeTime,
                OnHit = (NPC npc, NPC.HitInfo hitInfo, int dmg) =>
                {
                    if (npc.knockBackResist == 0) return;
                    npc.velocity.Y = -20;
                }
            };
            #endregion
            #endregion
            #endregion
            #region 技能表添加
            #region 技能表的说明添加
            SkillsTreeUI.TryAddSkillTree(frostFistNotUse, new()
            {
                (new SkillsControl(true,false,false,false,false,false,false,true),"空中落砸"),
                (new SkillsControl(false,true,false,false,false,false,false,true),"空中落落"),
                (new SkillsControl(false,false,true,false,false,false,false,true),"剑-天落"),
                (new SkillsControl(false,false,true,false,false,true,false,false),"极速乱刃"),
                (new SkillsControl(false,false,true,true,false,false,false,false),"一刀两断"),
                (new SkillsControl(true,false,false,true,false,false,false,false),"连续拳"),
                (new SkillsControl(false,true,false,true,false,false,false,false),"双摆拳"),
                (new SkillsControl(true,false,false,false,true,true,false,false),"蓄力升龙"),
                (new SkillsControl(false,true,false,false,true,true,false,false),"蓄力冲拳"),
                (new SkillsControl(false,false,true,false,true,false,false,false),"剑-上挑"),
                (new SkillsControl(true,false,false,false,false,false,false,false),"速攻拳连段"),
                (new SkillsControl(false,true,false,false,false,false,false,false),"通用拳连段"),
                (new SkillsControl(false,false,true,false,false,false,false,false),"霜拳舞刃式-力"),
            });
            #region 拳
            SkillsTreeUI.TryAddSkillTree(fistHit_SpeedAtk_Hit1, new()
            {
                (new SkillsControl(){IsLeftClick = true},"速攻拳连段-2"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_SpeedAtk_Hit2, new()
            {
                (new SkillsControl(){IsLeftClick = true},"速攻拳连段-3"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_SpeedAtk_Hit3, new()
            {
                (new SkillsControl(){IsLeftClick = true},"速攻拳连段-4"),
                (new SkillsControl(){IsRightClick = true},"速攻拳连段3"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_SpeedAtk3, new()
            {
                (new SkillsControl(){IsLeftClick = true},"空中落砸"),
                (new SkillsControl(){IsRightClick = true},"空中落砸"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_SpeedAtk_Hit4, new()
            {
                (new SkillsControl(){IsLeftClick = true},"速攻拳连段-5"),
                (new SkillsControl(){IsRightClick = true},"速攻拳连段2"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_DoubleHit2, new()
            {                
                (new SkillsControl(){IsLeftClick = true},"上勾拳"),
                (new SkillsControl(){IsRightClick = true},"上勾拳")
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_DoubleHit3, new()
            {
                (new SkillsControl(){IsLeftClick = true},"空中落砸"),
                (new SkillsControl(){IsRightClick = true},"空中落砸"),
            });

            SkillsTreeUI.TryAddSkillTree(fistHit_GeneralAtk1, new()
            {
                (new SkillsControl(){IsRightClick = true},"通用拳连段-2"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_GeneralAtk2, new()
            {
                (new SkillsControl(){IsRightClick = true},"通用拳连段-3"),
            });
            #endregion
            #region 刀
            #region 力
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Strong_1, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-力-3"),
                (new SkillsControl(){IsLeftClick = true},"猛拳"),
                (new SkillsControl(){IsRightClick = true},"冰刺"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"冲刺"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Strong_2, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-力-4"),
                (new SkillsControl(){IsLeftClick = true},"神圣反击"),
                (new SkillsControl(){IsRightClick = true},"充能拳"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"霜拳舞刃式-瞬"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Strong_3, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-力-5"),
            });

            SkillsTreeUI.TryAddSkillTree(fistHit_StrongFistAtk1, new()
            {
                (new SkillsControl(){IsSP1Click = true},"猛拳-2"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_StrongFistAtk2, new()
            {
                (new SkillsControl(){IsSP1Click = true},"猛拳-3"),
            });
            #region 瞬
            SkillsTreeUI.TryAddSkillTree(frostFist_MomentSwordSwing1, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-瞬-2"),
                (new SkillsControl(){IsLeftClick = true},"冲刺上勾拳"),
                (new SkillsControl(){IsRightClick = true},"霜拳拳法·冻"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"强冰冻斩"),
            });
            SkillsTreeUI.TryAddSkillTree(fistHit_Moment_Hit2, new()
            {
                (new SkillsControl(){IsLeftClick = true},"霜拳封印术"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_MomentSwordSwing2, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-瞬-3"),
            });
            #endregion
            #endregion
            #region 速
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Start, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-力-2"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"霜拳舞刃式-速"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Speed_3, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-速-1"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"冻结斩"),
                (new SkillsControl(){IsLeftClick = true},"冻击连续拳"),
                (new SkillsControl(){IsRightClick = true},"最速连击"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Speed_1, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-速-2"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Speed_2, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-速-3"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"乱斩"),
                (new SkillsControl(){IsLeftClick = true},"速冻冲拳"),
                (new SkillsControl(){IsRightClick = true},"击退横扫拳"),
            });
            SkillsTreeUI.TryAddSkillTree(frostFist_SwordSwing_Speed_Start, new()
            {
                (new SkillsControl(){IsSP1Click = true},"霜拳舞刃式-速-3"),
                (new SkillsControl(){IsSP1Click = true,IsStopAtk = true},"乱斩"),
                (new SkillsControl(){IsLeftClick = true},"速冻冲拳"),
                (new SkillsControl(){IsRightClick = true},"击退横扫拳"),
            });
            #endregion
            #endregion
            #endregion
            frostFistNotUse.AddSkill(frostFist_FistHit_SkyFallFist);
            frostFistNotUse.AddSkill(frostFist_SwordSwing_SkyFall);
            ProjSkill_Instantiation[] changeSkills = new ProjSkill_Instantiation[]
            {
                fistHit_SpeedAtk_Hit1,
                fistHit_SpeedAtk_Hit2,
                fistHit_SpeedAtk_Hit3,
                fistHit_SpeedAtk_Hit4,
                fistHit_SpeedAtk_Hit5,
                fistHit_SpeedAtk2,
                fistHit_SpeedAtk3,
                frostFist_FistHit_SkyFallFist,
                fistHit_GeneralAtk1,
                fistHit_GeneralAtk2,
                fistHit_GeneralAtk3,
                fistHit_DoubleHit3,
                frostFist_FistHit_CoiledFist_AfterFist,
                frostFist_FistHit_ChangeRisingFist,
                frostFist_FistHit_ChangeDashFist,
                fistHit_SpeedAtk_FrozenCoiledFist2,
                frostFist_SwordSwing_Speed_FrozenSlash,
                fistHit_StrongFistAtk3,
                frostFist_SwordSwing_Dash,
                frostFist_FistHit_HolyStrikesBack,
                fistHit_ChangedFist,
                frostFist_FistHit_Moment_RisingFist2,
                fistHit_Moment_Hit5,
                fistHit_Moment_SealFist,
                frostFist_SwordSwing_SkyFall,
                frostFist_SwordSwing_CutItInTwo,
                frostFist_SwordSwing_RisingSlash,
                frostFist_MomentSwor_StrongSwing
            };
            #region 搓招添加
            frostFistNotUse.AddSkill(frostFist_FistHit_CoiledFist).AddSkill(frostFist_FistHit_CoiledFist_AfterFist);
            frostFist_FistHit_CoiledFist.AddBySkill(changeSkills);

            frostFistNotUse.AddSkill(fistHit_DoubleHit).AddSkill(fistHit_DoubleHit2).AddSkill(fistHit_DoubleHit3);
            fistHit_DoubleHit.AddBySkill(changeSkills);

            frostFistNotUse.AddSkill(frostFist_FistHit_ChangeRisingFist);
            frostFist_FistHit_ChangeRisingFist.AddBySkill(changeSkills);

            frostFistNotUse.AddSkill(frostFist_FistHit_ChangeDashFist);
            frostFist_FistHit_ChangeDashFist.AddBySkill(changeSkills);
            #endregion

            #region 刀的添加
            #region 大招
            frostFistNotUse.AddSkill(frostFist_SwordSwing_CrazySpeedSlash1).AddSkill(frostFist_SwordSwing_CrazySpeedSlash2);
            #endregion
            #region 搓招技能
            frostFistNotUse.AddSkill(frostFist_SwordSwing_CutItInTwo);
            frostFist_SwordSwing_CutItInTwo.AddBySkill(changeSkills);

            frostFistNotUse.AddSkill(frostFist_SwordSwing_RisingSlash);
            frostFist_SwordSwing_RisingSlash.AddBySkill(changeSkills);
            #endregion
            #region 速
            fistHit_SpeedAtk_SpeedFrozen1.AddSkill(fistHit_SpeedAtk_SpeedFrozen2).AddSkill(fistHit_SpeedAtk_SpeedFrozen3);
            fistHit_SpeedAtk_SpeedFrozen1.AddBySkill(frostFist_SwordSwing_Speed_Start, frostFist_SwordSwing_Speed_2);

            fistHit_SpeedAtk_KnockbackFist.AddBySkill(frostFist_SwordSwing_Speed_Start, frostFist_SwordSwing_Speed_2);

            frostFist_SwordSwing_SpeedSlash.AddBySkill(frostFist_SwordSwing_Speed_Start, frostFist_SwordSwing_Speed_2);

            frostFist_SwordSwing_Speed_3.AddSkill(fistHit_SpeedAtk_FrozenCoiledFist).AddSkill(fistHit_SpeedAtk_FrozenCoiledFist2);

            frostFist_SwordSwing_Speed_3.AddSkill(frostFist_SwordSwing_Speed_FrozenSlash);

            frostFist_SwordSwing_Speed_3.AddSkill(frostFist_SpeedAtk_FastetHit);

            frostFistNotUse.AddSkill(frostFist_SwordSwing_Start).AddSkill(frostFist_SwordSwing_Speed_Start).AddSkill(frostFist_SwordSwing_Speed_3).AddSkill(frostFist_SwordSwing_Speed_1).AddSkill(frostFist_SwordSwing_Speed_2).AddSkill(frostFist_SwordSwing_Speed_3);
            frostFist_SwordSwing_Start.AddBySkill(changeSkills);
            #endregion
            #region 力

            frostFist_SwordSwing_Strong_1.AddSkill(fistHit_StrongFistAtk1).AddSkill(fistHit_StrongFistAtk2).AddSkill(fistHit_StrongFistAtk3);

            frostFist_SwordSwing_Strong_1.AddSkill(fistHit_IceSpurts);

            frostFist_SwordSwing_Strong_1.AddSkill(frostFist_SwordSwing_Dash);

            frostFist_SwordSwing_Strong_2.AddSkill(frostFist_FistHit_HolyStrikesBack);

            frostFist_SwordSwing_Strong_2.AddSkill(fistHit_ChangedFist);

            #region 瞬
            frostFist_MomentSwordSwing1.AddSkill(frostFist_MomentSwor_StrongSwing);

            frostFist_MomentSwordSwing1.AddSkill(frostFist_FistHit_Moment_RisingFist1).AddSkill(frostFist_FistHit_Moment_RisingFist2).AddSkill(frostFist_MomentSwordSwing3);

            frostFist_MomentSwordSwing1.AddSkill(fistHit_Moment_Hit1).AddSkill(fistHit_Moment_Hit2).AddSkill(fistHit_Moment_Hit3).AddSkill(fistHit_Moment_Hit4).AddSkill(fistHit_Moment_Hit5);
            fistHit_Moment_Hit2.AddSkill(fistHit_Moment_SealFist);

            frostFist_SwordSwing_Strong_2.AddSkill(frostFist_MomentSwordSwing1).AddSkill(frostFist_MomentSwordSwing2).AddSkill(frostFist_MomentSwordSwing3);
            #endregion

            frostFist_SwordSwing_Start.AddSkill(frostFist_SwordSwing_Strong_1).AddSkill(frostFist_SwordSwing_Strong_2).AddSkill(frostFist_SwordSwing_Strong_3).AddSkill(frostFist_SwordSwing_Strong_4);
            #endregion
            #endregion

            #region 拳的添加
            frostFistNotUse.AddSkill(fistHit_SpeedAtk_Hit1).AddSkill(fistHit_SpeedAtk_Hit2).AddSkill(fistHit_SpeedAtk_Hit3).AddSkill(fistHit_SpeedAtk_Hit4).AddSkill(fistHit_SpeedAtk_Hit5);
            fistHit_SpeedAtk_Hit4.AddSkill(fistHit_SpeedAtk2);
            fistHit_SpeedAtk_Hit3.AddSkill(fistHit_SpeedAtk3).AddSkill(frostFist_FistHit_SkyFallFist);

            frostFistNotUse.AddSkill(fistHit_GeneralAtk1).AddSkill(fistHit_GeneralAtk2).AddSkill(fistHit_GeneralAtk3);
            #endregion
            #endregion

            CurrentSkill = frostFistNotUse;
        }
        public int GetPlayerDoubleTapDir(int Dir)
        {
            if (Dir == 1) return 2; // 朝向为正-右边
            else return 3;
        }
        public bool GetPlayerDoubleTap(int Dir) => Player.GetModPlayer<WeaponSkillPlayer>().DashDir == Dir;
    }
}
