using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.Hammer.Skills;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.Hammer
{
    /// <summary>
    /// 勇气锤子
    /// </summary>
    public class HammerProj : ModProjectile,IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public int ChannelLevel;
        public static List<int> DrawHammerSwingShader_Index;
        public HammerHeld hammerHeld;
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
        public override void Load()
        {
            DrawHammerSwingShader_Index = new();
        }
        public override void Unload()
        {
            DrawHammerSwingShader_Index = null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 18, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 2.15f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead || !Player.HeldItem.GetGlobalItem<HammerGlobalItem>().InAttackMode) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is HammerSwingRenderDraw))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new HammerSwingRenderDraw());
            }
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            CurrentSkill.ModifyHitNPC(target,ref modifiers);
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

            HammerNotUse notUse = new HammerNotUse(this);
            hammerHeld = new HammerHeld(this);

            #region 攻击技能 
            HammerSwing hammerSwing_ToHeld_SideBlow = new(this,() => Player.controlUseItem) // 起手攻击
            {
                StartVel = Vector2.UnitY.RotatedBy(0.8),
                VelScale = new Vector2(1,0.5f),
                SwingRot = MathHelper.Pi + 0.3f,
                VisualRotation = 0.5f,
                SwingDirectionChange = false,
                TimeChangeMax = 30, // 30帧结束动作
                TimeChangeFunc = HammerSwingTimeChange
            };
            HammerSwing hammerSwing_ToHeld_SideBlow_Back = new(this, () => Player.controlUseItem) // 起手攻击-返挥
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.4),
                VelScale = new Vector2(1, 0.85f),
                SwingRot = MathHelper.Pi + 0.3f,
                VisualRotation = 0.15f,
                TimeChangeMax = 35,
                TimeChangeFunc = HammerSwingTimeChange
            };
            
            HammerSwing OverheadSmash_1 = new(this, () => Player.controlUseItem) // 纵挥
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.3),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + 0.3f,
                VisualRotation = 0f,
                TimeChangeMax = 25,
                TimeChangeFunc = HammerSwingTimeChange
            };
            HammerSwing OverheadSmash_2 = new(this, () => Player.controlUseItem) // 纵挥
            {
                StartVel = -Vector2.UnitY,
                VelScale = Vector2.One,
                SwingRot = MathHelper.PiOver2 + 0.2f,
                VisualRotation = 0f,
                TimeChangeMax = 30,
                TimeChangeFunc = HammerSwingTimeChange
            };
            HammerSwing Upswing = new(this, () => Player.controlUseItem) // 压迫
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1f,0.6f),
                SwingRot = MathHelper.TwoPi + 0.5f,
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AddDamage = 4,
                TimeChangeMax = 40,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    if (Projectile.ai[0] > 0) Projectile.rotation = 0.3f;
                    if (Projectile.ai[0] == 1 && Projectile.ai[1] > 35)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 6f, 1f, 15, 2));
                    }
                }
            };

            HammerSwing_WaterStrike hammerSwing_WaterStrike = new(this); // 水面击
            HammerSwing Upswing_AfterWaterStrike = new(this, () => Player.GetModPlayer<WeaponSkillPlayer>().WaterStrike_OnHit && (Player.controlUseItem || Player.controlUseTile)) // 压迫
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1f, 0.6f),
                SwingRot = MathHelper.TwoPi + 0.5f,
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                AddDamage = 4,
                TimeChangeMax = 40,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    if (Projectile.ai[0] > 0) Projectile.rotation = 0.3f;
                    if (Projectile.ai[0] == 1 && Projectile.ai[1] > 35)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 6f, 1f, 15, 2));
                    }
                }
            };
            HammerSwing_BigBang hammerSwing_BigBang = new(this); // 敲打
            HammerSwing hammerSwing_BigBangFinisher_1 = new(this, () => Player.controlUseTile) // 敲打终结
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1f, 0.6f),
                SwingRot = MathHelper.TwoPi + 0.5f,
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                TimeChangeMax = 40,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    Projectile.extraUpdates = 5;
                    if (Projectile.ai[0] > 1) Projectile.ai[2] = 14;
                }
            };
            HammerSwing hammerSwing_BigBangFinisher_2 = new(this, () => true) // 敲打终结
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1f, 0.6f),
                SwingRot = MathHelper.Pi + 0.5f,
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                TimeChangeMax = 40,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    Projectile.extraUpdates = 5;
                    if (Projectile.ai[0] > 1) Projectile.ai[2] = 14;
                    else if (Projectile.ai[0] > 0) Projectile.rotation = -0.5f;
                }
            };
            HammerSwing hammerSwing_BigBangFinisher_3 = new(this, () => true) // 敲打终结
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.3),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + 0.3f,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                AddDamage = 4,
                TimeChangeMax = 30,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    Projectile.extraUpdates = 5;
                    if (Projectile.ai[0] == 1 && Projectile.ai[1] > 25)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 6f, 1f, 15, 2));
                    }
                }
            };

            HammerSwing_Channel hammerSwing_ChargedSideBlow = new(this, () => WeaponSkill.BowSlidingStep.Current) // 蓄力返挥
            {
                StartVel = Vector2.UnitY.RotatedBy(0.8),
                VelScale = new Vector2(1, 0.5f),
                SwingRot = MathHelper.Pi + 0.3f,
                VisualRotation = 0.5f,
                SwingDirectionChange = false,
                TimeChangeMax = 30, // 30帧结束动作
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    if (Projectile.ai[0] == 1)
                    {
                        if (Projectile.ai[1] < 15)
                        {
                            Player.velocity.X *= 1.2f;
                            if (Player.velocity.X > 20 || Player.velocity.X < -20) Player.velocity.X = 20 * (Player.velocity.X > 0).ToDirectionInt();
                        }
                        else
                        {
                            Player.velocity.X *= 0.5f;
                        }
                    }
                }
            };
            HammerSwing_Channel hammerSwing_ChargedRisingSlash = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.8),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + 0.6f,
                SwingDirectionChange = false,
                VisualRotation = 0f,
                AddDamage = 0.5f,
                TimeChangeMax = 30, // 30帧结束动作
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    if (Projectile.ai[0] == 1 && Projectile.ai[1] < 15)
                    {
                        Player.velocity.X = Player.direction * (30 + ((Player.velocity.X < 1 && Player.velocity.X > -1) ? 20 : 0));
                    }
                    else if (Projectile.ai[0] == 1)
                    {
                        Player.velocity.X = 0;
                    }
                    ChannelLevel = 2;
                }
            };
            HammerSwing_Channel hammerSwing_StrongChargedSlash1 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.8),
                VelScale = new Vector2(1,0.5f),
                SwingRot = MathHelper.TwoPi + 0.2f,
                SwingDirectionChange = false,
                VisualRotation = 0.5f,
                TimeChangeMax = 45, // 30帧结束动作
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    if (Projectile.ai[0] > 1) Projectile.extraUpdates = 5;
                    ChannelLevel = 2;
                }
            };
            HammerSwing hammerSwing_StrongChargedSlash2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitX,
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + 0.2f,
                SwingDirectionChange = true,
                VisualRotation = 0f,
                TimeChangeMax = 20, // 30帧结束动作
                AddDamage = 5,
                TimeChangeFunc = HammerSwingTimeChange,
                SwingAI = () =>
                {
                    Projectile.extraUpdates = 5;
                    ChannelLevel = 2;
                    if (Projectile.ai[0] > 1)
                    {
                        Projectile.ai[2] -= 0.5f;
                    }
                    else if (Projectile.ai[0] > 0)
                    {
                        if (Projectile.ai[1] > 15) Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 6f, 1f, 15, 2));
                    }
                }
            };
            #endregion

            #region 技能组合
            notUse.AddSkill(hammerSwing_ToHeld_SideBlow).AddSkill(hammerSwing_ToHeld_SideBlow_Back).AddSkill(OverheadSmash_1).AddSkill(OverheadSmash_2).AddSkill(Upswing);
            hammerHeld.AddSkill(OverheadSmash_1);

            hammerSwing_WaterStrike.AddBySkill(hammerSwing_ToHeld_SideBlow, hammerSwing_ToHeld_SideBlow_Back, OverheadSmash_1, OverheadSmash_2, hammerHeld, hammerSwing_ChargedSideBlow);
            hammerSwing_WaterStrike.AddSkill(Upswing_AfterWaterStrike);
            hammerSwing_WaterStrike.AddSkill(hammerSwing_BigBang).AddSkill(hammerSwing_BigBangFinisher_1).AddSkill(hammerSwing_BigBangFinisher_2).AddSkill(hammerSwing_BigBangFinisher_3);
            hammerSwing_WaterStrike.AddSkill(OverheadSmash_2);

            hammerSwing_ChargedSideBlow.AddBySkill(hammerSwing_WaterStrike,
                                                   hammerSwing_ToHeld_SideBlow,
                                                   hammerSwing_ToHeld_SideBlow_Back,
                                                   OverheadSmash_1,
                                                   OverheadSmash_2,
                                                   hammerHeld,
                                                   notUse,
                                                   hammerSwing_BigBang,
                                                   hammerSwing_BigBangFinisher_3,
                                                   Upswing_AfterWaterStrike);
            hammerSwing_ChargedSideBlow.AddSkill(hammerSwing_ChargedRisingSlash).AddSkill(hammerSwing_StrongChargedSlash1).AddSkill(hammerSwing_StrongChargedSlash2);
            hammerSwing_BigBangFinisher_3.AddSkill(hammerSwing_ChargedSideBlow);
            Upswing.AddSkill(hammerSwing_ChargedRisingSlash);
            #endregion
            CurrentSkill = notUse;
        }
        public bool PreSkillTimeOut()
        {
            if(CurrentSkill is HammerSwing)
            {
                CurrentSkill.OnSkillDeactivate();
                hammerHeld.OnSkillActive();
                CurrentSkill = hammerHeld;
                return false;
            }
            return true;
        }
        public Color GetDrawColor()
        {
            switch (ChannelLevel)
            {
                case 1: // 一级蓄力
                    {
                        return new Color(128, 40, 160, 0);
                    }
                case 2: // 二级蓄力
                    {
                        return new Color(50, 100, 200, 0);
                    }
                default:
                    {
                        return new Color(100, 100, 100, 0);
                    }
            }
        }
        public float HammerSwingTimeChange(float ai1)
        {
            //float c1 = 1.3f;
            //float c2 = c1 + 1;
            //return 1 + c2 * MathF.Pow(ai1 - 1, 3) + c1 * MathF.Pow(ai1 - 1, 2);

            return MathF.Pow(ai1, 3f);
        }
    }
}
