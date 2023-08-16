using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.LongSword.Skills;

namespace WeaponSkill.Weapons.LongSword
{
    public class LongSwordProj : ModProjectile, IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public LongSwordScabbard swordScabbard;
        /// <summary>
        /// 可以更改刀鞘的朝向,返回true则可以,不会自动重置
        /// </summary>
        public bool CanChangeScabbardRot;
        public bool InSpiritAttack;
        public int AddSpiritTime;
        public static List<int> DrawLongSwordSwingShader_Index = new();
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public override string Texture => "Terraria/Images/Item_0";
        public override void Load()
        {
            DrawLongSwordSwingShader_Index = new();
        }
        public override void Unload()
        {
            DrawLongSwordSwingShader_Index = null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 18, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.15f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                LongSwordGlobalItem longSwordGlobalItem = SpawnItem.GetGlobalItem<LongSwordGlobalItem>();
                swordScabbard = new(longSwordGlobalItem.ScabbardTex);
                if(longSwordGlobalItem.ScabbardAction != null)
                {
                    swordScabbard.DrawAction.AddRange(longSwordGlobalItem.ScabbardAction);
                }
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
            Projectile.timeLeft = 2;
            InSpiritAttack = false;
            swordScabbard.projectile = Projectile;
            CurrentSkill.AI();
            Player.itemLocation = Projectile.Center;
            if (AddSpiritTime > 0)
            {
                AddSpiritTime--;
                if (AddSpiritTime % 5 == 0)
                {
                    LongSwordGlobalItem longSwordGlobalItem = SpawnItem.GetGlobalItem<LongSwordGlobalItem>();
                    if (longSwordGlobalItem.Spirit < longSwordGlobalItem.SpiritMax) longSwordGlobalItem.Spirit++;
                }
            }
            if (!CanChangeScabbardRot)
            {
                swordScabbard.Rot = (MathHelper.PiOver2 * 1.82f) * Player.direction;
            }
            CanChangeScabbardRot = false;
            OldSkills.TrimExcess();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time,2.5f);
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel switch
            {
                1 => Color.White,
                2 => Color.Gold,
                3 => Color.Red,
                _ => default
            };
            lightColor = Color.Lerp(lightColor, color, 0.2f);
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            bool flag = CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
            swordScabbard.Draw(Main.spriteBatch, lightColor);
            return flag;
        }
        public override void PostDraw(Color lightColor)
        {
            if(!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is LongSwordSwingRenderDraw))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new LongSwordSwingRenderDraw());
            }
            //Main.spriteBatch.Draw(TextureAssets.FishingLine.Value,Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), default, 4, SpriteEffects.None, 0f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel * 0.5f;
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Type type = Player.GetType();
            type.GetField("_spawnMuramasaCut", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(Player, true);
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }

        public void Init()
        {
            OldSkills = new();
            var notUse = new LongSwordNotUse(this);
            #region 普通攻击
            LongSwordSwing useSlash1 = new LongSwordSwing(this, () => Player.controlUseItem || Player.controlUseTile)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.225f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                OnHit = OnHit_Normhit
            };
            LongSwordSwing useSlash2 = new LongSwordSwing(this, () => true)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false,
                OnHit = OnHit_Normhit
            };
            LongSwordSwing SlashDown = new LongSwordSwing(this, () => Player.controlUseItem)
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.03f),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi,
                OnHit = OnHit_Normhit
            };
            LongSwordSwing SlashUp = new LongSwordSwing(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.6f),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                SwingDirectionChange = false,
                OnHit = OnHit_Normhit
            };
            LongSwordSwing Spurt = new LongSwordSwing(this, () =>
            {
                if (OldSkills[^1] is LongSwordSwing && Player.controlUseItem) return true;
                return Player.controlUseTile;
            })
            {
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.04f),
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 6;
                    }
                    else if ((int)Projectile.ai[0] == 2 && Projectile.ai[2] < 1)
                    {
                        Projectile.extraUpdates = 0;
                        var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(default), Projectile.damage, Projectile.knockBack, Projectile.owner, SwingLength * 1.5f, 80);
                        proj.OnHit = OnHit_Normhit;
                    }
                }
            };
            LongSwordSwing_BackSlash backSlash = new LongSwordSwing_BackSlash(this, () => Player.controlUseItem && Player.controlUseTile)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                OnHit = OnHit_Normhit
            };
            LongSwordSwing_ForesightSlash foresightSlash = new LongSwordSwing_ForesightSlash(this, () => WeaponSkill.BowSlidingStep.Current && Player.controlUseTile)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false
            };
            #endregion
            #region 气刃系攻击
            LongSwordSwing_Spirit longSwordSwing_Spirit = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(0.3f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SP_Spirit = true
            };
            LongSwordSwing_Spirit longSwordSwing_Spirit2 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false
            };
            LongSwordSwing_Spirit longSwordSwing_Spirit3_1 = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 3;
                    }
                }
            };
            LongSwordSwing_Spirit longSwordSwing_Spirit3_2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 3;
                    }
                }
            };
            LongSwordSwing_Spirit longSwordSwing_Spirit3_3 = new(this, () => true)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = Vector2.One,
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
            };
            LongSwordSwing_Spirit longSwordSwing_Spirit_RotSlash = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingRot = MathHelper.TwoPi + MathHelper.PiOver2,
                LevelUp = true,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 2;
                        Player.velocity.X = Player.direction * 20;
                    }
                    else if ((int)Projectile.ai[0] == 2)
                    {
                        Player.velocity.X *= 0.3f;
                        Projectile.ai[2] += 2;
                    }
                    Player.SetImmuneTimeForAllTypes(5);
                }
            };
            LongSwordSwing_Spirit longSwordSwing_AfterBackSlash = new(this, () => WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Player.ChangeDir((Player.velocity.X > 0).ToDirectionInt());
                    }
                    else if ((int)Projectile.ai[0] == 1)
                    {
                        Player.velocity.X = Player.direction * (MathF.Log(10 / (Projectile.ai[1] + 1)) + 3f);
                    }
                    else if (((int)Projectile.ai[0] == 2) && Projectile.ai[2] < 7)
                    {
                        Player.velocity.X *= 0.4f;
                    }
                }
            };
            LongSword_SakuraSlashed longSword_SakuraSlashed = new(this, () => WeaponSkill.BowSlidingStep.Current && Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f
            };
            #endregion
            #region 纳刀系
            LongSword_Naknotsu longSword_Naknotsu = new(this);
            LongSwordSwing NaknotsuSwing1 = new LongSwordSwing(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 0)
                    {
                        Projectile.ai[1] -= 0.5f;
                        Player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash = true;
                    }
                    else if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 3;
                        Player.velocity.X = Player.direction * 5;
                        Player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash = false;
                    }
                }
            };
            LongSwordSwing NaknotsuSwing2 = new LongSwordSwing(this, () => true)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.225f),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                OnHit = OnHit_Naknotsu,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[0] == 1)
                    {
                        Projectile.extraUpdates = 3;
                        Player.velocity.X = Player.direction * 5;
                    }
                    else if ((int)Projectile.ai[0] == 2)
                    {
                        Projectile.ai[2] += 2;
                    }
                }
            };
            LongSword_Naknotsu_RotSlash Naknotsu_RotSlash = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingRot = MathHelper.TwoPi,
            };
            LongSword_SerenePose longSword_SerenePose = new(this, () => WeaponSkill.SpKeyBind.Current) // 水月架势
            {
                StartVel = Vector2.UnitY.RotatedBy(0.225f),
                VelScale = new Vector2(1.5f, 0.9f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2
            };
            #endregion
            #region 普通攻击判定
            notUse.AddSkill(useSlash1).AddSkill(useSlash2);
            useSlash2.AddSkill(SlashDown).AddSkill(Spurt);
            useSlash2.AddSkill(Spurt);
            Spurt.AddSkill(SlashUp);
            SlashUp.AddSkill(SlashDown);
            SlashUp.AddSkill(Spurt);
            #endregion
            #region 纳刀判定
            longSword_Naknotsu.AddBySkill(useSlash2, SlashDown, Spurt, SlashUp, longSwordSwing_Spirit, longSwordSwing_Spirit2, longSwordSwing_Spirit3_3,longSwordSwing_AfterBackSlash,longSwordSwing_Spirit_RotSlash,backSlash);
            longSword_Naknotsu.AddSkill(NaknotsuSwing1).AddSkill(NaknotsuSwing2);
            longSword_Naknotsu.AddSkill(Naknotsu_RotSlash);
            #endregion
            #region 见切判定
            foresightSlash.AddBySkill(useSlash2, SlashDown, Spurt, SlashUp, longSwordSwing_Spirit, longSwordSwing_Spirit2, longSwordSwing_Spirit3_3);
            foresightSlash.AddSkill(longSwordSwing_Spirit_RotSlash);
            #endregion
            #region 气刃攻击判定
            longSwordSwing_Spirit.AddBySkill(SlashDown, Spurt, SlashUp,notUse);
            longSwordSwing_Spirit2.AddBySkill(useSlash2);
            longSwordSwing_Spirit.AddSkill(longSwordSwing_Spirit2).AddSkill(longSwordSwing_Spirit3_1).AddSkill(longSwordSwing_Spirit3_2).AddSkill(longSwordSwing_Spirit3_3).AddSkill(longSwordSwing_Spirit_RotSlash);

            longSwordSwing_Spirit.AddSkill(SlashDown);
            longSwordSwing_Spirit2.AddSkill(Spurt);
            longSword_SakuraSlashed.AddBySkill(SlashDown, Spurt, SlashUp, longSwordSwing_Spirit, longSwordSwing_Spirit2, longSwordSwing_Spirit3_3,notUse);
            #endregion
            #region 袈裟判定
            backSlash.AddBySkill(SlashDown, SlashUp, SlashUp, longSwordSwing_Spirit, longSwordSwing_Spirit2, longSwordSwing_Spirit3_3);
            backSlash.AddSkill(longSwordSwing_AfterBackSlash).AddSkill(longSwordSwing_Spirit3_1);
            #endregion
            #region 水月架势判定
            longSword_SerenePose.AddBySkill(useSlash2, SlashDown, Spurt, SlashUp, longSwordSwing_Spirit, longSwordSwing_Spirit2, longSwordSwing_Spirit3_3, longSwordSwing_AfterBackSlash, longSwordSwing_Spirit_RotSlash, NaknotsuSwing2, longSwordSwing_Spirit_RotSlash, longSword_SakuraSlashed);
            #endregion
            CurrentSkill = notUse;
        }
        public void OnHit_Normhit(NPC npc,NPC.HitInfo hitInfo,int damageDone)
        {
            var item = SpawnItem.GetGlobalItem<LongSwordGlobalItem>();
            item.Spirit += 8;
            if (item.SpiritMax < item.Spirit)
            {
                item.Spirit = item.SpiritMax;
            }
        }
        public void OnHit_Naknotsu(NPC npc, NPC.HitInfo hitInfo, int damageDone)
        {
            AddSpiritTime = 900;
        }
    }
}
