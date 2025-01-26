
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.SlashAxe.Skills;
using WeaponSkill.Weapons.SlashAxe;
using WeaponSkill.Weapons.InsectStaff.Skills;
using WeaponSkill.Effects;
using Terraria.Graphics.Effects;

namespace WeaponSkill.Weapons.InsectStaff
{
    public class InsectStaffProj : ModProjectile, IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills {get;set;}
        public ProjSkill_Instantiation CurrentSkill {get;set;}
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public FixedOnePosToDrawTrailing_SwingHelper SwingHelper;
        public InsectStaff_Held held;
        public InsectStaff_SkyHeld skyHeld;
        public InsectProj insectProj;
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            (CurrentSkill as BasicInsectStaffSkill).PreAtk = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
            if (hit.Crit)
            {
                TheUtility.CritProj(Projectile, target, Projectile.velocity.RotatedBy(MathHelper.PiOver2 + MathHelper.PiOver4 * Player.direction).SafeNormalize(default));
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D line = TextureAssets.FishingLine.Value;
            float factor = SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime / 1800f;
            Main.spriteBatch.Draw(line, Player.Top - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 0, line.Width, (int)(line.Height)), Color.Gray, MathHelper.PiOver2, line.Size() * 0.5f, 4f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(line, Player.Top - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 0, line.Width, (int)(line.Height * factor)), Color.Lerp(Color.White, Color.Red, factor), MathHelper.PiOver2, line.Size() * 0.5f, 4f, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }

        public virtual float TimeChange(float time)
        {
            return MathF.Pow(time, 2.5f);
        }
        public void Init()
        {
            OldSkills = new();
            Func<bool> isStrongAtk = () => SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime > 0;
            #region 技能创建
            InsectStaffNoUse insectStaffNoUse = new(this);
            skyHeld = new InsectStaff_SkyHeld(this);

            held = new(this);

            #region 普通攻击
            InsectStaff_Swing Spurt = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(-0.3),
                SwingRot = MathHelper.PiOver2,
                VelScale= new Vector2(1.3f,0.1f),
                VisualRotation = 0,
                SwingDirectionChange = false,
                ActionDmg = 0.8f,
            };
            InsectStaff_Swing JiaShaSwing = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                ActionDmg = 1f,
            };
            InsectStaff_Swing AcrossSwing = new(this,() => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = true,
                ActionDmg = 1.1f,
            };
            InsectStaff_Swing KnockDown = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 2;
                    }
                }
            };
            InsectStaff_Swing TwoSlash1 = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = true,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 2;
                    }
                }
            };
            InsectStaff_Swing TwoSlash2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 5;
                    }
                }
            };

            InsectStaff_Swing FlyMoveSwing = new(this, () => (Player.direction == -1 ? Player.controlLeft : Player.controlRight) && Player.controlUseTile)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 8,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 5;
                        Player.velocity.Y = -4;
                    }
                }
            };
            #endregion
            #region 操虫
            InsectStaff_ControlInsect ControlAtk = new(this, () => Player.controlUseItem && WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                PreSwingTimeMax = 8,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        int i = 1000;
                        NPC nPC = Projectile.FindTargetWithinRange(i);
                        if(nPC != null)
                            Player.MinionAttackTargetNPC = nPC.whoAmI;
                    }
                }
            };
            InsectStaff_ControlInsect ControlBack = new(this, () => Player.controlUseTile && WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                PreSwingTimeMax = 8,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.MinionAttackTargetNPC = -1;
                    }
                }
            };
            InsectStaff_ControlInsect ControlDustBoom = new(this, () => WeaponSkill.RangeChange.Current && WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                PreSwingTimeMax = 8,
                SwingAI = () =>
                {
                    insectProj.Projectile.ai[0] = 6;
                    insectProj.Projectile.ai[1] = 0;
                }
            };
            #endregion
            #region 飞行攻击
            InsectStaff_Swing Fly = new(this, () => WeaponSkill.RangeChange.Current && Player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 50)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.3f),
                SwingRot = 0.3f,
                VelScale = new Vector2(1.3f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = false,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 8,
                IsSkyAtk = true,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0 && (int)Projectile.ai[1] == 0)
                    {
                        Player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50;
                        Player.velocity.Y = -15;
                        if (SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime > 0)
                        {
                            Player.velocity.Y = -20;
                        }
                    }
                }
            };
            InsectStaff_Swing SkySlashDown = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.TwoPi + 0.3f,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                ActionDmg = 0.5f,
                SwingTimeMax = 40,
                SwingAI = () =>
                {
                    Player.GetModPlayer<WeaponSkillPlayer>().playerFallSpeed = 30;
                    if (Player.velocity.Y != 0)
                    {
                        if ((int)Projectile.ai[0] == 1 && (int)Projectile.ai[1] == 39)
                        {
                            TheUtility.ResetProjHit(Projectile);
                            Projectile.ai[1] = 0;
                        }
                        Player.velocity.Y++;
                    }
                }
            };
            InsectStaff_Swing FlyAgain = new(this, () => Player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 100 && Projectile.numHits > 0) // 舞踏跳跃
            {
                StartVel = Vector2.UnitY.RotatedBy(0.3f),
                SwingRot = 0.3f,
                VelScale = new Vector2(1.3f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = false,
                ActionDmg = 1.2f,
                PreSwingTimeMax = 8,
                IsSkyAtk = true,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0 && (int)Projectile.ai[1] == 0)
                    {
                        Player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 100;
                        Player.velocity.Y = -15;
                        if(SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime > 0)
                        {
                            Player.velocity.Y = -20;
                        }
                    }
                }
            };
            InsectStaff_ControlInsectDash insectStaff_ControlInsectDash = new(this, () => Player.controlUseTile)
            {
                ActionDmg = 2f,
                SwingAI = ()=>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        insectProj.Projectile.ai[0] = 2;
                    }
                }
            };
            InsectStaff_ControlInsectDash insectStaff_ControlInsectDash1 = new(this, () => Player.controlUseTile && Projectile.numHits > 0)
            {
                ActionDmg = 2f,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        insectProj.Projectile.ai[0] = 2;
                    }
                }
            };
            InsectStaff_DownInsect insectStaff_DownInsect = new(this, () => WeaponSkill.RangeChange.Current)
            {
                SwingTimeMax = 40,
                SwingAI = () =>
                {
                    Player.GetModPlayer<WeaponSkillPlayer>().playerFallSpeed = 50;
                    if (Player.velocity.Y != 0)
                    {
                        if ((int)Projectile.ai[0] == 1 && (int)Projectile.ai[1] == 39)
                        {
                            Projectile.ai[1]--;
                        }
                        Player.velocity.Y += 2;
                    }
                    insectProj.Projectile.ai[0] = 3;
                },
                ActionDmg = 5
            };
            #endregion
            #region 强化攻击
            InsectStaff_Swing StrongSpurt1 = new(this, () => Player.controlUseItem && isStrongAtk.Invoke())
            {
                StartVel = Vector2.UnitY,
                SwingRot = MathHelper.PiOver2,
                VelScale = new Vector2(1.3f, 0.1f),
                VisualRotation = 0,
                SwingDirectionChange = false,
                ActionDmg = 0.8f,
            };
            InsectStaff_Swing StrongSpurt2 = new(this, () => true)
            {
                StartVel = Vector2.UnitX,
                SwingRot = MathHelper.PiOver2,
                VelScale = new Vector2(1.3f, 0.1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                ActionDmg = 0.8f,
            };

            InsectStaff_Swing StrongJiaShaSwing1 = new(this, () => Player.controlUseItem && isStrongAtk.Invoke())
            {
                StartVel = -Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                ActionDmg = 1f,
            };
            InsectStaff_Swing StrongJiaShaSwing2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                ActionDmg = 1f,
            };

            InsectStaff_Swing StrongTwoSlash1 = new(this, () => Player.controlUseItem && isStrongAtk.Invoke())
            {
                StartVel = Vector2.UnitY.RotatedBy(0.3),
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                ActionDmg = 1f,
            };
            InsectStaff_Swing StrongTwoSlash2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.3),
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 1f,
            };
            InsectStaff_Swing StrongTwoSlash3 = new(this, () => true)
            {
                StartVel = Vector2.UnitX,
                SwingRot = MathHelper.TwoPi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingTimeMax = 45,
                SwingDirectionChange = true,
                ActionDmg = 2.5f,
            };

            InsectStaff_Swing StrongAcrossSlash1 = new(this, () => Player.controlUseTile && isStrongAtk.Invoke())
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.3),
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 1f,
            };
            InsectStaff_Swing StrongAcrossSlash2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY.RotatedBy(-0.3),
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 1f,
            };

            InsectStaff_Swing FlyingRoundSlash1 = new(this, () => Player.controlUseTile && isStrongAtk.Invoke())
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 1f),
                VisualRotation = 0,
                SwingDirectionChange = true,
                ActionDmg = 3.5f,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 4;
                        Player.velocity.Y = -2;
                    }
                }
            };
            InsectStaff_Swing FlyingRoundSlash2 = new(this, () => true)
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                TimeoutTimeMax = 180,
                PreSwingTimeMax = 2,
                ActionDmg = 5.5f,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 6;
                    }
                    #region 屏幕缩放shader调用
                    ScreenChange.SetScreenScale = 0.9f;
                    if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                        Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                    #endregion
                }
            };
            InsectStaff_Swing ThrowBug1 = new(this, () => WeaponSkill.SpKeyBind.Current && isStrongAtk.Invoke()) // 梗
            {
                StartVel = Vector2.UnitX,
                SwingRot = MathHelper.TwoPi + MathHelper.Pi,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 3.5f,
                SwingTimeMax = 90,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 1, 1, DustID.FireworkFountain_Red);
                        dust.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2).RotatedByRandom(0.2) * 0.02f;
                    }
                    #region 屏幕缩放shader调用
                    ScreenChange.SetScreenScale = 0.6f;
                    if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                        Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                    #endregion
                }
            };
            InsectStaff_Swing ThrowBug2 = new(this, () => true) // 梗
            {
                StartVel = Vector2.UnitX,
                SwingRot = MathHelper.TwoPi * 2 + MathHelper.Pi,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                ActionDmg = 3.5f,
                SwingTimeMax = 90,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 1, 1, DustID.FireworkFountain_Red);
                        dust.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2).RotatedByRandom(0.2) * 0.02f;
                    }
                    #region 屏幕缩放shader调用
                    ScreenChange.SetScreenScale = 0.6f;
                    if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                        Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                    #endregion
                }
            };
            InsectStaff_Swing ThrowBug3 = new(this, () => true) // 梗
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.Pi + 0.3f,
                VelScale = new Vector2(1f, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                ActionDmg = 3.5f,
                PreSwingTimeMax = 5,
                SwingAI = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 1, 1, DustID.FireworkFountain_Red);
                        dust.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2).RotatedByRandom(0.2) * 0.02f;
                    }
                    #region 屏幕缩放shader调用
                    ScreenChange.SetScreenScale = 0.6f;
                    if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                        Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                    #endregion
                    insectProj.Projectile.ai[1] = 0;
                    insectProj.Projectile.ai[0] = 4;
                    insectProj.Projectile.Center = Projectile.velocity * 1.5f + Player.Center;
                    insectProj.Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default) * 16;
                    Projectile.extraUpdates = 2;
                    SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime = 0;
                }
            };
            #endregion
            #endregion

            #region 技能连接
            insectStaffNoUse.AddSkill(held);

            ControlDustBoom.AddBySkill(held, FlyMoveSwing, AcrossSwing, KnockDown, JiaShaSwing, TwoSlash2, Spurt, StrongAcrossSlash2, FlyingRoundSlash2, StrongTwoSlash3, StrongJiaShaSwing2, StrongSpurt2);

            held.AddSkill(ThrowBug1).AddSkill(ThrowBug2).AddSkill(ThrowBug3);
            ControlBack.AddBySkill(held, FlyMoveSwing, AcrossSwing, KnockDown, JiaShaSwing, TwoSlash2, Spurt, StrongAcrossSlash2, FlyingRoundSlash2, StrongTwoSlash3, StrongJiaShaSwing2, StrongSpurt2);
            ControlAtk.AddBySkill(held, FlyMoveSwing, AcrossSwing, KnockDown, JiaShaSwing, TwoSlash2, Spurt, StrongAcrossSlash2, FlyingRoundSlash2, StrongTwoSlash3, StrongJiaShaSwing2, StrongSpurt2);

            held.AddSkill(FlyMoveSwing);
            FlyMoveSwing.AddSkill(FlyingRoundSlash1);
            StrongTwoSlash3.AddSkill(FlyingRoundSlash1);
            StrongJiaShaSwing2.AddSkill(StrongAcrossSlash1);
            StrongSpurt2.AddSkill(StrongAcrossSlash1);
            held.AddSkill(StrongAcrossSlash1).AddSkill(StrongAcrossSlash2).AddSkill(FlyingRoundSlash1).AddSkill(FlyingRoundSlash2);
            held.AddSkill(StrongSpurt1).AddSkill(StrongSpurt2).AddSkill(StrongJiaShaSwing1).AddSkill(StrongJiaShaSwing2).AddSkill(StrongTwoSlash1).AddSkill(StrongTwoSlash2).AddSkill(StrongTwoSlash3);

            skyHeld.AddSkill(insectStaff_DownInsect);
            skyHeld.AddSkill(insectStaff_ControlInsectDash).AddSkill(FlyAgain).AddSkill(insectStaff_ControlInsectDash1).AddSkill(FlyAgain);
            skyHeld.AddSkill(SkySlashDown);

            Fly.AddBySkill(held, FlyMoveSwing, AcrossSwing, KnockDown, JiaShaSwing, TwoSlash2, Spurt, StrongAcrossSlash2, FlyingRoundSlash2, StrongTwoSlash3, StrongJiaShaSwing2, StrongSpurt2);

            FlyMoveSwing.AddSkill(AcrossSwing);
            held.AddSkill(AcrossSwing);
            JiaShaSwing.AddSkill(TwoSlash1).AddSkill(TwoSlash2).AddSkill(KnockDown);
            Spurt.AddSkill(JiaShaSwing).AddSkill(AcrossSwing);
            held.AddSkill(Spurt).AddSkill(AcrossSwing).AddSkill(KnockDown);
            #endregion
            CurrentSkill = insectStaffNoUse;
        }
        public bool PreSkillTimeOut()
        {
            if (OldSkills.Count <= 1) return true;
            if (CurrentSkill is InsectStaff_Swing insectStaff_Swing) // 如果挥舞类
            {
                CurrentSkill.OnSkillDeactivate();
                if (insectStaff_Swing.IsSkyAtk)
                {
                    if(CurrentSkill is InsectStaff_ControlInsectDash)
                    {
                        return true;
                    }
                    skyHeld.OnSkillActive();
                    CurrentSkill = skyHeld;
                    return false;
                }
                held.OnSkillActive();
                CurrentSkill = held;
                return false;
            }
            if(CurrentSkill is InsectStaff_SkyHeld)
            {
                CurrentSkill.OnSkillDeactivate();
                held.OnSkillActive();
                CurrentSkill = held;
                return false;
            }
            return true;
        }
    }
}