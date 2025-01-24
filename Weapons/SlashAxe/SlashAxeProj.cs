using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Items.SlashAxe;
using WeaponSkill.Weapons.ChargeBlade.Skills;
using WeaponSkill.Weapons.Shortsword;
using WeaponSkill.Weapons.SlashAxe.Skills;

namespace WeaponSkill.Weapons.SlashAxe
{
    public class SlashAxeProj : ModProjectile,IBasicSkillProj
    {
        public class Part_Axe : PartSwingHelper.Part
        {
            public SlashAxeProj proj;
            public Part_Axe(PartSwingHelper onwer, SlashAxeProj proj) : base(onwer)
            {
                this.proj = proj;
            }
            public override void DrawSwingItem(Color drawColor)
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                //if (Onwer.projectile != null)
                //{
                //    Onwer.SwingItemTex ??= TextureAssets.Projectile[Onwer.projectile.type];
                //}
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;

                Vector2 velocity = GetOldVel(-1, true);
                //velocity = velocity.RotatedBy(Rot);
                Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(Onwer.VisualRotation * Onwer.spriteDirection * SPDir).SafeNormalize(default)
                    * Size.Length() * 0.5f * Onwer.spriteDirection * SPDir;

                Vector2 center = Onwer.GetDrawCenter();
                center += OffestCenter;
                if (Onwer._drawCorrections)
                {
                    center = Onwer.Center + (center - Onwer.Center);
                }
                Vector2 halfVelPos = center + velocity * 0.5f;
                Vector2[] pos = new Vector2[4]
                {
                    center - Main.screenPosition,
                    halfVelPos - halfLength - Main.screenPosition,
                    center + velocity - Main.screenPosition,
                    halfVelPos + halfLength  - Main.screenPosition
                };
                Vector2 rotPos = (pos[0] + pos[1] + pos[2] + pos[3]) / 4;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 v = (pos[i] - rotPos).RotatedBy(Rot);
                    pos[i] = rotPos + v;
                }

                float factor = (Onwer.frame + 1f) / Onwer.frameMax;
                if (SPDir == -1)
                    factor = 1 - factor;
                CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
                customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
                customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
                customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
                customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

                if (proj.SlashAxeGlobalItem.AxeStrength > 0 && (proj.CurrentSkill is SlashAxe_AxeHeld || proj.CurrentSkill is SlashAxe_AxeSwing))
                {
                    Effect effect = ModAsset.SlashAxe_AxeShader.Value;
                    effect.Parameters["uTime"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.1f));
                    effect.Parameters["uColor"].SetValue(((new Color(250, 59, 20)) with { A = 255 }).ToVector4() * 5);
                    effect.Parameters["tex"].SetValue(ModAsset.Perlin.Value);
                    effect.CurrentTechnique.Passes[0].Apply();
                }
                gd.Textures[0] = DrawTex.Value;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
                //gd.RasterizerState = origin;
            }
        }
        public class Part_Sword : PartSwingHelper.Part
        {
            public SlashAxeProj proj;
            public Part_Sword(PartSwingHelper onwer, SlashAxeProj proj) : base(onwer)
            {
                this.proj = proj;
            }
            public override void DrawSwingItem(Color drawColor)
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                //if (Onwer.projectile != null)
                //{
                //    Onwer.SwingItemTex ??= TextureAssets.Projectile[Onwer.projectile.type];
                //}
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;

                Vector2 velocity = GetOldVel(-1, true);
                //velocity = velocity.RotatedBy(Rot);
                Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(Onwer.VisualRotation * Onwer.spriteDirection * SPDir).SafeNormalize(default)
                    *Size.Length() * 0.5f * Onwer.spriteDirection * SPDir;

                Vector2 center = Onwer.GetDrawCenter();
                center += OffestCenter;
                if (Onwer._drawCorrections)
                {
                    center = Onwer.Center + (center - Onwer.Center);
                }
                Vector2 halfVelPos = center + velocity * 0.5f;
                Vector2[] pos = new Vector2[4]
                {
                    center - Main.screenPosition,
                    halfVelPos - halfLength - Main.screenPosition,
                    center + velocity - Main.screenPosition,
                    halfVelPos + halfLength  - Main.screenPosition
                };
                Vector2 rotPos = (pos[0] + pos[1] + pos[2] + pos[3]) / 4;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 v = (pos[i] - rotPos).RotatedBy(Rot);
                    pos[i] = rotPos + v;
                }

                float factor = (Onwer.frame + 1f) / Onwer.frameMax;
                if (SPDir == -1)
                    factor = 1 - factor;
                CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
                customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
                customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
                customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
                customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

                if (proj.SlashAxeGlobalItem.Power == proj.SlashAxeGlobalItem.PowerMax && (proj.CurrentSkill is SlashAxe_SwordSwing || proj.CurrentSkill is SlashAxe_SwordHeld))
                {
                    Effect effect = ModAsset.SlashAxe_AxeShader.Value;
                    effect.Parameters["uTime"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.1f));
                    effect.Parameters["uColor"].SetValue(((new Color(250, 59, 20)) with { A = 255 }).ToVector4() * 5);
                    effect.Parameters["tex"].SetValue(ModAsset.Perlin.Value);
                    effect.CurrentTechnique.Passes[0].Apply();
                }
                gd.Textures[0] = DrawTex.Value;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
                //gd.RasterizerState = origin;
            }
        }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public PartSwingHelper SwingHelper;
        public SlashAxeGlobalItem SlashAxeGlobalItem => SpawnItem.GetGlobalItem<SlashAxeGlobalItem>();
        ///// <summary>
        ///// 是否处于斧模式
        ///// </summary>
        //public bool IsAxeMode;
        public SlashAxe_AxeHeld AxeHeld;
        public SlashAxe_SwordHeld SwordHeld;
        public override string Texture => "Terraria/Images/Item_0";

        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                BasicSlashAxe basicSlashAxe = (SpawnItem.ModItem as BasicSlashAxe);
                SwingHelper = new(Projectile, 25, basicSlashAxe.DefTex);
                //SwingHelper.Parts.Add("Sword", new(Projectile, 16, (SpawnItem.ModItem as BasicSlashAxe).SwordTex));
                //SwingHelper.Parts.Add("Axe", new(Projectile, 16, (SpawnItem.ModItem as BasicSlashAxe).AxeTex));
                SwingHelper.Parts.Add("Sword", new Part_Sword(SwingHelper,this)
                {
                    DrawTex = basicSlashAxe.SwordTex,
                    Size = basicSlashAxe.SwordSize
                });
                SwingHelper.Parts.Add("Axe", new Part_Axe(SwingHelper,this)
                {
                    DrawTex = basicSlashAxe.AxeTex,
                    Size = basicSlashAxe.AxeSize
                });
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }

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
            (CurrentSkill as BasicSlashAxeSkill).PreAtk = false;

            if(CurrentSkill is not SlashAxe_SwordHeld && CurrentSkill is not SlashAxe_SwordSwing)
            {
                SlashAxeGlobalItem.Slash++;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time)
        {
            return MathF.Pow(time, 3.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            bool flag = CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
            return flag;
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
        public void Init()
        {
            OldSkills = new();
            #region 技能创建
            SlashAxe_NoUse slashAxe_NoUse = new(this);

            AxeHeld = new(this);

            SlashAxe_AxeSwing AcrossSlash1 = new(this, () => Player.controlUseTile)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1,0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                ActionDmg = 1.2f,
                SwingTimeMax = 45,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[1] % 2 == 0 && (int)Projectile.ai[0] == 1)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            var fire = new Particles.Fire(25);
                            fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.02f, 0.05f)).RotatedByRandom(0.6), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire);
                        }
                    }
                }
            };
            SlashAxe_AxeSwing AcrossSlash2 = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 1.3f,
                SwingTimeMax = 40,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[1] % 2 == 0 && (int)Projectile.ai[0] == 1)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            var fire = new Particles.Fire(25);
                            fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.02f, 0.05f)).RotatedByRandom(0.6), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire);
                        }
                    }
                }
            };

            SlashAxe_AxeSwing SpurtSlash = new(this, () => Player.controlUseItem && (Player.direction == -1 ? Player.controlLeft : Player.controlRight))
            {
                StartVel = Vector2.UnitX.RotatedBy(-0.3),
                SwingRot = 0.6f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 0.8f,
                SwingTimeMax = 20,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 4;
                    }
                }
            };
            SlashAxe_AxeSwing SlashDown = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 1.1f,
                SwingTimeMax = 30,
            };
            SlashAxe_AxeSwing SlashCross = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY,
                SwingRot = MathHelper.PiOver2,
                VelScale = new(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                ActionDmg = 1.1f,
                SwingTimeMax = 30,
            };
            SlashAxe_AxeSwing SlashUp = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = false,
                ActionDmg = 1.1f,
                SwingTimeMax = 30,
            };

            SlashAxe_AxeSwing StrongSlashDown1 = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitX,
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 1.4f,
                SwingTimeMax = 30,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[1] % 2 == 0 && (int)Projectile.ai[0] == 1)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var fire = new Particles.Fire(25);
                            fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.02f, 0.05f)).RotatedByRandom(0.6), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire);
                        }
                    }
                }
            };
            SlashAxe_AxeSwing StrongSlashDown2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitX,
                SwingRot = MathHelper.Pi,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 2f,
                SwingTimeMax = 30,
                PreSwingTimeMax = 3,
                SwingAI = () =>
                {
                    SlashAxeGlobalItem.AxeStrength = 3600;
                    if ((int)Projectile.ai[1] % 2 == 0 && (int)Projectile.ai[0] == 1)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var fire = new Particles.Fire(25);
                            fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.02f, 0.05f)).RotatedByRandom(0.6), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire);
                        }
                    }
                }
            };

            SwordHeld = new(this);

            SlashAxe_SwordSwing slashAxe_Sword_Down = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 1.4f,
                SwingTimeMax = 30,
            };
            SlashAxe_SwordSwing slashAxe_SwordSwing_Left = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                SwingTimeMax = 30,
                ActionDmg = 2

            };
            SlashAxe_SwordSwing slashAxe_SwordSwing_Right = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                SwingTimeMax = 30,
                ActionDmg = 2.5f
            };

            SlashAxe_AxeSwing slashAxe_Change1 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                SwingTimeMax = 30,
                ActionDmg = 2.5f
            };
            SlashAxe_SwordSwing slashAxe_Change2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                SwingTimeMax = 30,
                ActionDmg = 2.5f,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 4;
                    }
                }
            };

            SlashAxe_AxeSwing slashAxe_Sword_Change = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ActionDmg = 1.4f,
                SwingTimeMax = 30,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0 && Projectile.ai[1] < 6)
                    {
                        SlashAxeGlobalItem.Slash += 100;
                    }
                },
            };

            SlashAxe_SwordSwing slashAxe_SwordSwing_TwoSlash1 = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                SwingTimeMax = 30,
                ActionDmg = 3,
                PreSwingTimeMax = 20,
                PowerAdd = 60
            };
            SlashAxe_SwordSwing slashAxe_SwordSwing_TwoSlash2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                SwingTimeMax = 30,
                PreSwingTimeMax = 3,
                ActionDmg = 3.5f,
                PowerAdd = 60
            };

            SlashAxe_SwordSwing slashAxe_SwordSwing_SlashDown_Change = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTimeMax = 30,
                ActionDmg = 2
            };

            SlashAxe_SwordSwing slashAxe_SwordSwing_FlySlash1 = new(this, () => Player.controlUseTile)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = false,
                SwingTimeMax = 30,
                ActionDmg = 3,
                PreSwingTimeMax = 15,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0 && Projectile.ai[1] < 6)
                    {
                        Player.velocity.Y = -4;
                    }
                },
                PowerAdd = 75
            };
            SlashAxe_SwordSwing slashAxe_SwordSwing_FlySlash2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitY.RotatedBy(-0.4),
                SwingRot = MathHelper.Pi + 0.8f,
                VelScale = new(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTimeMax = 30,
                ActionDmg = 4,
                PreSwingTimeMax = 5,
                PowerAdd = 75
            };
            //SlashAxe_SwordSwing slashAxe_SwordSwing_SlashDown = new(this, () => WeaponSkill.BowSlidingStep.Current)
            //{
            //    StartVel = -Vector2.UnitY.RotatedBy(-0.4),
            //    SwingRot = MathHelper.Pi + 0.8f,
            //    VelScale = new(1, 1f),
            //    VisualRotation = 0f,
            //    SwingDirectionChange = true,
            //    SwingTimeMax = 30,
            //    ActionDmg = 3
            //};
            SlashAxe_CompressionLiberation slashAxe_CompressionLiberation = new(this, null);
            #endregion
            #region 技能连接
            slashAxe_NoUse.AddSkill(AxeHeld);
            slashAxe_NoUse.AddSkill(slashAxe_SwordSwing_SlashDown_Change);
            slashAxe_SwordSwing_SlashDown_Change.AddBySkill(AxeHeld);

            slashAxe_CompressionLiberation.AddBySkill(slashAxe_SwordSwing_TwoSlash2, SwordHeld, slashAxe_Sword_Down);

            slashAxe_SwordSwing_TwoSlash2.AddSkill(slashAxe_SwordSwing_FlySlash1).AddSkill(slashAxe_SwordSwing_FlySlash2);

            slashAxe_SwordSwing_TwoSlash1.AddBySkill(SwordHeld, slashAxe_Sword_Down, slashAxe_SwordSwing_Left, slashAxe_SwordSwing_Right);
            slashAxe_SwordSwing_TwoSlash1.AddSkill(slashAxe_SwordSwing_TwoSlash2);

            slashAxe_Sword_Change.AddBySkill(SwordHeld, slashAxe_SwordSwing_Left, slashAxe_SwordSwing_Right);

            SwordHeld.AddSkill(slashAxe_Sword_Down).AddSkill(slashAxe_SwordSwing_Left).AddSkill(slashAxe_SwordSwing_Right).AddSkill(slashAxe_Sword_Down);

            slashAxe_Change1.AddSkill(slashAxe_Change2);
            slashAxe_Change1.AddBySkill(AcrossSlash1, AcrossSlash2);

            SpurtSlash.AddSkill(SlashDown).AddSkill(SlashCross).AddSkill(SlashUp);
            AxeHeld.AddSkill(SpurtSlash).AddSkill(AcrossSlash1);

            AxeHeld.AddSkill(SlashDown);
            AcrossSlash2.AddSkill(StrongSlashDown1).AddSkill(StrongSlashDown2);
            AcrossSlash1.AddSkill(StrongSlashDown1);
            AxeHeld.AddSkill(AcrossSlash1).AddSkill(AcrossSlash2).AddSkill(AcrossSlash1);
            #endregion
            CurrentSkill = slashAxe_NoUse;
        }
        public bool PreSkillTimeOut()
        {
            if (CurrentSkill is SlashAxe_SwordSwing) // 如果是剑挥舞类
            {
                if (SlashAxeGlobalItem.Slash > 0)
                {
                    CurrentSkill.OnSkillDeactivate();
                    SwordHeld.OnSkillActive();
                    CurrentSkill = SwordHeld;
                }
                else
                {
                    CurrentSkill.OnSkillDeactivate();
                    AxeHeld.OnSkillActive();
                    CurrentSkill = AxeHeld;
                }
                return false;
            }

            if (OldSkills.Count <= 1) return true;

            if (CurrentSkill is SlashAxe_AxeSwing) // 如果是斧挥舞类
            {
                CurrentSkill.OnSkillDeactivate();
                AxeHeld.OnSkillActive();
                CurrentSkill = AxeHeld;
                return false;
            }
            return true;
        }
    }
}
