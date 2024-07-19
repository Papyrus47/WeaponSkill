using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.DualBlades.Skills;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;

namespace WeaponSkill.Weapons.DualBlades
{
    public class DualBladesProj : ModProjectile, IBasicSkillProj
    {
        public class DualBlades
        {
            public DualBladesProj dualBladesProj;
            public SwingHelper SwingHelper;
            public Vector2 StartVel;
            public Vector2 VelScale;
            public float VisualRotation;
            public float SwingRot;
            public bool SwingDirectionChange;
            public Ref<Vector2> vel;
            public int spDir;
            public bool[] NPCHit;
            public DualBlades(DualBladesProj dualBladesProj,SwingHelper swingHelper)
            {
                SwingHelper = swingHelper;
                vel = new();
                if(swingHelper is DualBladesSwingHelper helper)
                {
                    helper.vel = vel;
                }
                this.dualBladesProj = dualBladesProj;
                SwingDirectionChange = true;
                NPCHit = new bool[Main.npc.Length];
            }
            public virtual void AI(float Time)
            {
                Time = Math.Clamp(Time, 0f, 1.2f);
                if (Time > 1)
                {
                    SwingHelper.SetNotSaveOldVel();
                }
                if (SwingHelper is DualBladesSwingHelper helper)
                {
                    helper.spriteDir = spDir;
                }
                SwingHelper.ProjFixedPlayerCenter(dualBladesProj.Player, 0, true);
                SwingHelper.SwingAI(dualBladesProj.SwingLength, dualBladesProj.Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
            }
            public virtual void Draw(SpriteBatch sb,Color drawColor)
            {
                Effect effect = WeaponSkill.SwingEffect.Value;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["uColorChange"].SetValue(0.95f);
                Main.graphics.GraphicsDevice.Textures[1] = dualBladesProj.SpawnItem.GetGlobalItem<DualBladesGlobalItem>().DrawColorTex;
                if (dualBladesProj.InArchdemonMode || dualBladesProj.InDemonMode) drawColor = Color.Lerp(Color.Red, drawColor, 0.9f);
                SwingHelper.Swing_Draw_ItemAndTrailling(drawColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0),effect);
            }
            public virtual void ResetHit()
            {
                for(int i = NPCHit.Length - 1; i >= 0; i--)
                {
                    NPCHit[i] = false;
                }
            }
        }
        /// <summary>
        /// 玩家手持的刀
        /// </summary>
        public DualBlades HeldBlades;
        /// <summary>
        /// 在玩家底下拿着的刀
        /// </summary>
        public DualBlades BackBlades;
        public Asset<Texture2D> DrawProjTex;
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public bool BackDraw;
        public static List<int> DrawBackBlades = new();
        public bool InDemonMode => SpawnItem.GetGlobalItem<DualBladesGlobalItem>().DemonMode;
        public bool InArchdemonMode => SpawnItem.GetGlobalItem<DualBladesGlobalItem>().ArchdemonMode;
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
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.8f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void Load()
        {
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
        }
        public override void Unload()
        {
            DrawBackBlades = null;
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
        }
        public override void SetDefaults()
        {
            Projectile.scale = 1.5f;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;

        }
        private static void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig.Invoke(self);
            if (DrawBackBlades.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                foreach (var item in DrawBackBlades)
                {
                    Projectile proj = Main.projectile[item];
                    if (proj == null || proj.ModProjectile is not DualBladesProj) break;
                    ((proj.ModProjectile as DualBladesProj).CurrentSkill as BasicDualBladesSkill).BackDraw(Main.spriteBatch, Lighting.GetColor((proj.Center / 16).ToPoint()));
                }
                DrawBackBlades.Clear();
                Main.spriteBatch.End();
            }
        }

        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead || DrawProjTex == null) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Player.heldProj = Projectile.whoAmI;
            if(HeldBlades == null || BackBlades == null)
            {
                HeldBlades = new(this, new DualBladesSwingHelper(this, 18, DrawProjTex));
                BackBlades = new(this, new DualBladesSwingHelper(this, 18, DrawProjTex));
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            OldSkills.TrimExcess();
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool? CanHitNPC(NPC target) => !BackBlades.NPCHit[target.whoAmI] || !HeldBlades.NPCHit[target.whoAmI];

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathHelper.SmoothStep(0, 2f, time);
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBackBlades.Add(Projectile.whoAmI);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
            ItemLoader.ModifyHitNPC(SpawnItem,Player, target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }

        public void Init()
        {
            OldSkills = new();
            DualBladesNotUse dualBladesNotUse = new DualBladesNotUse(this);
            #region 普通模式下
            DualBladesSwing normSwing_TwoSlash = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseItem && !InDemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                ID = "NormSwing1"
            };
            DualBladesSwing normSwing_TwoSlashBack = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.BackLow, () => Player.controlUseItem && !InDemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = (swing) =>
                {
                    Player.velocity.X = Player.direction * 5;
                    if (swing is DualBladesSwing bladesSwing && bladesSwing.CanChangeSkill)
                    {
                        Player.velocity.X *= 0.3f;
                    }
                },
                ID = "NormSwing2"
            };
            DualBladesSwing normSwing_CarSlash1 = new(this, SwingSet.HeldBlades, DoubleSwingSpeed.Same, () => Player.controlUseItem && !InDemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                DefaultVel = -Vector2.UnitY.RotatedBy(-0.6),
                ID = "NormSwing3"
            };
            DualBladesSwing normSwing_CarSlash_End = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => true)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.Pi * 1.25f,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1,1),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                ID = "NormSwing4"
            };
            DualBladesSwing normSwing_RotSlash = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseTile && !InDemonMode && !InArchdemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                AIAction = (swing) =>
                {
                    Player.velocity.X = Player.direction * 9;
                    if (swing is DualBladesSwing bladesSwing && bladesSwing.CanChangeSkill)
                    {
                        Player.velocity.X *= 0.3f;
                        TheUtility.SetPlayerImmune(Player);
                    }
                },
                ID = "NormSwing5"
            };
            DualBladesSwing normSwing_RotSlash_BackSlash = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseTile && !InDemonMode && !InArchdemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.TwoPi * 1.25f,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = (swing) =>
                {
                    Player.velocity.Y = (Projectile.ai[0] - 0.5f) * 9f;
                },
                ID = "NormSwing6"
            };
            DualBladesSwing normSwing_RotSlash_ToLeft_SlashUp = new(this, SwingSet.BackBlades, DoubleSwingSpeed.BackLow, () => Player.controlUseItem && !InDemonMode && !InArchdemonMode)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.Pi,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                DefaultVel = -Vector2.UnitX.RotatedBy(-0.2),
                ID = "NormSwing7"
            };
            #endregion
            #region 鬼人模式
            DualBlades_DemonStart dualBlades_DemonStart = new(this);
            DualBladesSwing_InDemon demonSlash_Left1 = new(this, SwingSet.BackBlades, DoubleSwingSpeed.BackLow, () => Player.controlUseItem)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                DefaultVel = -Vector2.UnitX.RotatedBy(-0.2),
                ID = "demonSlash_Left1"
            };
            DualBladesSwing_InDemon demonSlash_Left2 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseItem)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                ID = "demonSlash_Left2"
            };

            DualBladesSwing_InDemon demonSlash_Left3_1 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseItem)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = false,
                ID = "demonSlash_Left3"
            };
            DualBladesSwing_InDemon demonSlash_Left3_2 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.Pi,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                ID = "demonSlash_Left3"
            };
            DualBladesSwing_InDemon demonSlash_Left3_3 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingDirectionChange = true,
                ID = "demonSlash_Left3",
                DemonMode_AddCorrection = 1f
            };

            DualBladesSwing_InDemon demonSlash_Move = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseTile)
            {
                AITimeChange = () => 1 / 180f,
                SwingRot = MathHelper.TwoPi * 2,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                AIAction = (swing) =>
                {
                    Player.velocity.X = Player.direction * 16;
                    TheUtility.SetPlayerImmune(Player,2);
                    Projectile.rotation = 0;
                    Player.immuneAlpha = 0;
                    Projectile.extraUpdates = 6;
                    if (swing is DualBladesSwing bladesSwing && bladesSwing.CanChangeSkill)
                    {
                        Player.velocity.X *= 0.1f;
                    }
                    Projectile.ai[2]++;
                    if ((int)Projectile.ai[2] % 30 == 0)
                    {
                        swing.HeldBlade.ResetHit();
                        swing.BackBlade.ResetHit();
                    }
                },
                ID = "demonSlash_Move"
            };
            DualBladesSwing_InDemon demonSlash_Move_BackSlash1 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseTile)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.TwoPi * 1.25f,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = (swing) =>
                {
                    Player.velocity.Y = (Projectile.ai[0] - 0.3f) * 9f;
                },
                ID = "demonSlash_Move_BackSlash1"
            };
            DualBladesSwing_InDemon demonSlash_Move_BackSlash2 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseTile)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.TwoPi * 1.25f,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = (swing) =>
                {
                    Player.velocity.Y = (Projectile.ai[0] - 0.5f) * 9f;
                },
                ID = "demonSlash_Move_BackSlash2"
            };
            DualBladesSwing_InDemon demonSlash_SlashUP = new(this, SwingSet.BackBlades, DoubleSwingSpeed.BackLow, () => Player.controlUseItem)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.Pi,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                DefaultVel = -Vector2.UnitX.RotatedBy(-0.2),
                ID = "demonSlash_SlashUP"
            };
            DualBladesSwing_POWER dualBladesSwing_POWER = new(this); // 搓背

            #region 乱舞
            DualBladesSwing_InDemon demonSlash_Dance1 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseItem && Player.controlUseTile)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                IsDemonDance = true,
                ID = "demonSlash_Dance"
            };
            DualBladesSwing_InDemon demonSlash_Dance2 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                ID = "demonSlash_Dance"
            };
            DualBladesSwing_InDemon demonSlash_Dance3 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                ID = "demonSlash_Dance"
            };
            DualBladesSwing_InDemon demonSlash_Dance4 = new(this, SwingSet.BackBlades, DoubleSwingSpeed.BackLow, () => true)
            {
                AITimeChange = () => 1 / 40f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.7f),
                VisualRotation = 0.3f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                DefaultVel = (-Vector2.UnitX).RotatedBy(-0.3),
                ID = "demonSlash_Dance_1"
            };
            DualBladesSwing_InDemon demonSlash_Dance5 = new(this, SwingSet.HeldBlades, DoubleSwingSpeed.HeldLow, () => true)
            {
                AITimeChange = () => 1 / 40f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = true,
                AIAction = SalshDancesAI,
                DefaultVel = (-Vector2.UnitX).RotatedBy(0.3),
                ID = "demonSlash_Dance_2"
            };
            DualBladesSwing_InDemon demonSlash_Dance6 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.BackLow, () => true)
            {
                AITimeChange = () => 1 / 40f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY.RotatedBy(-0.9),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingDirectionChange = true,
                AIAction = SalshDancesAI,
                ID = "demonSlash_Dance_3"
            };
            DualBladesSwing_InDemon demonSlash_Dance7 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => true)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY.RotatedBy(-0.9),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingDirectionChange = false,
                AIAction = (swing) => 
                {
                    SalshDancesAI(swing);
                    Player.velocity.Y = (Projectile.ai[0] - 0.5f) * 9f;
                },
                ID = "demonSlash_Dance_4"
            };
            DualBladesSwing_InDemon demonSlash_Dance8 = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1,1f),
                VisualRotation = 0f,
                SwingDirectionChange = true,
                AIAction = SalshDancesAI,
                DemonMode_AddCorrection = 2,
                ID = "demonSlash_Dance_End"
            };
            #endregion

            #endregion
            #region 鬼人强化模式
            DualBladesSwing_InArchdemon ArchdemonSlash_Move = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseTile)
            {
                AITimeChange = () => 1 / 180f,
                SwingRot = MathHelper.TwoPi * 2,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                AIAction = (swing) =>
                {
                    Player.velocity.X = Player.direction * 16;
                    TheUtility.SetPlayerImmune(Player, 2);
                    Projectile.rotation = 0;
                    Player.immuneAlpha = 0;
                    Projectile.extraUpdates = 6;
                    if (swing is DualBladesSwing bladesSwing && bladesSwing.CanChangeSkill)
                    {
                        Player.velocity.X *= 0.1f;
                    }
                    Projectile.ai[2]++;
                    if ((int)Projectile.ai[2] % 30 == 0)
                    {
                        swing.HeldBlade.ResetHit();
                        swing.BackBlade.ResetHit();
                    }
                },
                DelDemonGauge = 60,
                ID = "demonSlash_Move"
            };
            DualBladesSwing_InArchdemon ArchdemonSlash_Move_BackSlash = new(this, SwingSet.TwoBlades, DoubleSwingSpeed.HeldLow, () => Player.controlUseTile)
            {
                AITimeChange = () => 1 / 70f,
                SwingRot = MathHelper.TwoPi * 1.25f,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = (swing) =>
                {
                    Player.velocity.Y = (Projectile.ai[0] - 0.3f) * 9f;
                },
                ID = "ArchdemonSlash_Move_BackSlash"
            };
            #region 鬼人强化舞
            DualBladesSwing_InArchdemon ArchdemonSlash_Dance1 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => Player.controlUseItem && Player.controlUseTile)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                IsDemonDance = true,
                DelDemonGauge = 150,
                ID = "ArchdemonSlash_Dance"
            };
            DualBladesSwing_InArchdemon ArchdemonSlash_Dance2 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                ID = "ArchdemonSlash_Dance"
            };
            DualBladesSwing_InArchdemon ArchdemonSlash_Dance3 = new(this, SwingSet.IntersectBlades, DoubleSwingSpeed.Same, () => true)
            {
                AITimeChange = () => 1 / 50f,
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AIAction = SalshDancesAI,
                ID = "ArchdemonSlash_Dance"
            };
            #endregion
            #endregion

            #region 普通攻击模式
            dualBladesNotUse.AddSkill(normSwing_RotSlash).AddSkill(normSwing_RotSlash_BackSlash); // 右键攻击

            dualBladesNotUse.AddSkill(normSwing_TwoSlash).AddSkill(normSwing_TwoSlashBack).AddSkill(normSwing_CarSlash1).AddSkill(normSwing_CarSlash_End); // 左键攻击

            normSwing_RotSlash.AddSkill(normSwing_RotSlash_ToLeft_SlashUp).AddSkill(normSwing_TwoSlash); // 上撩斩

            normSwing_RotSlash.AddBySkill(normSwing_TwoSlash, normSwing_TwoSlashBack, normSwing_CarSlash_End); // 回旋斩可以接的派生
            #endregion
            #region 鬼人攻击模式
            dualBladesNotUse.AddSkill(dualBlades_DemonStart);
            dualBladesNotUse.AddSkill(demonSlash_Left1).AddSkill(demonSlash_Left2).AddSkill(demonSlash_Left3_1).AddSkill(demonSlash_Left3_2).AddSkill(demonSlash_Left3_3); // 鬼人6连斩系
            dualBladesNotUse.AddSkill(demonSlash_Move).AddSkill(demonSlash_Move_BackSlash1).AddSkill(demonSlash_Move_BackSlash2); // 陀螺
            demonSlash_Move.AddSkill(demonSlash_SlashUP).AddSkill(demonSlash_Left1); // 陀螺转鬼人6连

            dualBladesSwing_POWER.AddBySkill(dualBladesNotUse, demonSlash_Left1, demonSlash_Left2, demonSlash_Left3_3, demonSlash_Move, demonSlash_Move_BackSlash1, demonSlash_Move_BackSlash2); // 搓背

            #region 乱舞
            demonSlash_Dance1.AddSkill(demonSlash_Dance2).AddSkill(demonSlash_Dance3).AddSkill(demonSlash_Dance4).AddSkill(demonSlash_Dance5).AddSkill(demonSlash_Dance6).AddSkill(demonSlash_Dance7).AddSkill(demonSlash_Dance8);
            demonSlash_Dance1.AddBySkill(dualBladesNotUse, demonSlash_Left1, demonSlash_Left2, demonSlash_Left3_3, demonSlash_Move, demonSlash_Move_BackSlash1, demonSlash_Move_BackSlash2);
            #endregion

            #endregion
            #region 鬼人强化攻击模式
            dualBladesNotUse.AddSkill(ArchdemonSlash_Move).AddSkill(ArchdemonSlash_Move_BackSlash);
            ArchdemonSlash_Move.AddSkill(normSwing_RotSlash_ToLeft_SlashUp);
            ArchdemonSlash_Dance1.AddSkill(ArchdemonSlash_Dance2).AddSkill(ArchdemonSlash_Dance3);
            ArchdemonSlash_Dance1.AddBySkill(dualBladesNotUse, normSwing_TwoSlash, normSwing_TwoSlashBack, normSwing_CarSlash_End, normSwing_CarSlash_End, normSwing_RotSlash_ToLeft_SlashUp, ArchdemonSlash_Move, ArchdemonSlash_Move_BackSlash);
            #endregion
            CurrentSkill = dualBladesNotUse;
        }
        public virtual void SalshDancesAI(BasicDualBladesSkill skill)
        {
            Player.velocity *= 0;
        }
    }
}
