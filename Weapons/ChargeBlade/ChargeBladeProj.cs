using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.Weapons.ChargeBlade.Skills;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.ChargeBlade
{
    // 盾斧充能机制
    // 左键攻击按一点算
    public class ChargeBladeProj : ModProjectile, IBasicSkillProj
    {
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public ChargeBladeShield shield;
        public bool shieldCanDraw;
        public ChargeBladeGlobalItem chargeBladeGlobal => SpawnItem.GetGlobalItem<ChargeBladeGlobalItem>();
        public Asset<Texture2D> ShieldTex => chargeBladeGlobal.ShieldTex;
        /// <summary>
        /// 防御成功的时间
        /// </summary>
        public int DefSucceededTime;

        public ChargeBlade_Sword_Held SwordHeld;
        public ChargeBlade_Axe_Held AxeHeld;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.3f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                //if(SpawnItem.ModItem is BasicChargeBlade blade)
                //{
                //    blade.SetShieldData(ref shieldData);
                //}
                //if(shieldData == default)
                //{
                //    shieldData = new()
                //    {
                //        Def = 10
                //    };
                //}
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
            shield ??= new(this,ShieldTex);

            Projectile.timeLeft = 2;
            shieldCanDraw = true;
            chargeBladeGlobal.InAxe = false;
            Player.GetModPlayer<WeaponSkillPlayer>().HeldShield = shield;
            //shield.Defence = shieldData.Def;
            shield.Defence = SpawnItem.defense;
            if (DefSucceededTime > 0) DefSucceededTime--;
            if (OldSkills.Count > 10) OldSkills.RemoveAt(1);
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            shield.DefSucceeded = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time)
        {
            if (chargeBladeGlobal.InAxe)
            {
                //return MathF.Pow(time, 4f);
                return MathHelper.SmoothStep(0, 2f, MathF.Pow(time * 0.6f, 2f));
            }
            return MathF.Pow(time, 3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            DrawShieldAndWeaponSystem.DrawShieldAndWeapon.Add(Projectile.whoAmI);
            if (chargeBladeGlobal.InAxe)
            {
                CurrentSkill.PreDraw(Main.spriteBatch,ref lightColor); // 斧模式专门的绘制
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
            CurrentSkill.ModifyHitNPC(target,ref modifiers);
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
            OldSkills = new List<ProjSkill_Instantiation>();
            #region 技能创建
            ChargeBladeNotUse chargeBladeNotUse = new ChargeBladeNotUse(this);

            #region 剑形态下的
            SwordHeld = new ChargeBlade_Sword_Held(this); // 手持

            ChargeBlade_Sword_Swing Sword_SlashDown = new(this, () => Player.controlUseItem) // 直斩
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = true
            };

            ChargeBlade_Sword_Swing Sword_SlashUP = new(this, () => Player.controlUseItem) // 上捞斩
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false
            };

            ChargeBlade_Sword_Swing_RotSlash Sword_RotSlash = new(this, () => Player.controlUseItem) // 回旋斩
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.6),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true
            };

            ChargeBlade_Sword_Swing_Dash Sword_Dash_SlashDown = new(this, () => Player.controlUseItem && Player.controlUseTile && !shield.DefSucceeded); // 突进斩

            ChargeBlade_OnDef Sword_Def = new(this); // 防御攻击

            ChargeBlade_AddBottles chargeBlade_AddBottles = new(this); // 填瓶子

            ChargeBlade_AddBottles_InChannel chargeBlade_AddBottles_InChannel = new(this); // 填瓶子后蓄力

            ChargeBlade_Sword_Swing_ChannelSwing chargeBlade_Sword_Swing_ChannelSwing = new(this, () => Player.controlUseTile); // 蓄力斩

            ChargeBlade_Sword_Swing_RotSlash Sword_MoveSlash = new(this, () => Player.controlUseTile) // 移动斩
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.6),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
            };

            //ChargeBlade_Sword_Swing Sword_SlashUP_AfterAddBottles = new(this, () => Player.controlUseItem) // 上捞斩
            //{
            //    StartVel = Vector2.UnitY.RotatedBy(0.5f),
            //    VelScale = new Vector2(1, 1f),
            //    VisualRotation = 0f,
            //    SwingRot = MathHelper.Pi + MathHelper.PiOver2,
            //    SwingDirectionChange = false
            //};

            ChargeBlade_AfterAddBottles_Swing chargeBlade_AfterAddBottles_Swing = new(this, () => Player.controlUseItem, chargeBlade_AddBottles_InChannel); // 开红剑用的斩

            ChargeBlade_Def_GP chargeBlade_Def_GP = new(this);

            ChargeBlade_Sword_ToAxe chargeBlade_Sword_ToAxe = new(this); // 变形斩

            ChargeBlade_Sword_ShieldSpurts chargeBlade_Sword_ShieldSpurts = new(this);

            ChargeBlade_Sword_Swing Sword_SlashDown_OnDef = new(this, () => Player.controlUseItem) // 直斩-格挡成功
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = true,
                DefAttack = true
            };
            #endregion

            #region 斧形态下
            AxeHeld = new(this);

            ChargeBlade_Axe_Swing Axe_SlashUp = new(this, () => Player.controlUseItem) // 斧上捞
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = Vector2.One,
                SwingRot = MathHelper.PiOver2 * 1.45f,
                SwingDirectionChange = false
            };

            ChargeBlade_Axe_Swing Axe_SlashDown = new(this, () => Player.controlUseItem)  // 斧下砸
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.3),
                VelScale = Vector2.One,
                SwingRot = MathHelper.PiOver2 * 1.45f,
                SwingDirectionChange = true
            };

            ChargeBlade_Sword_Swing_RotSlash Axe_ToSword = new(this, () => WeaponSkill.BowSlidingStep.Current) // 回旋斩
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.6),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true,
                SwingAI = () =>
                {
                    shield.Fixed = true;
                }
            };

            ChargeBlade_Axe_Swing_Liberate Axe_Liberate_Slash1 = new(this, () => Player.controlUseTile) // 斧解1
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1,0.4f),
                VisualRotation = 0.6f,
                SwingRot = MathHelper.PiOver2,
                SwingDirectionChange = false,
                SwingAI = () =>
                {
                    if (Projectile.ai[1] > ChargeBlade_Axe_Swing.SLASH_TIME / 2 && (int)Projectile.ai[0] == 1)
                    {
                        Projectile.ai[1] += 0.8f;
                    }
                }
            };
            ChargeBlade_Axe_Swing_Liberate Axe_Liberate_Slash2 = new(this, () => Player.controlUseTile) // 斧解2
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1, 0.25f),
                VisualRotation = 0.75f,
                SwingRot = MathHelper.TwoPi * 2,
                SwingDirectionChange = false,
                SwingAI = () =>
                {
                    if ((int)Projectile.ai[1] == ChargeBlade_Axe_Swing.SLASH_TIME / 2)
                    {
                        TheUtility.ResetProjHit(Projectile);
                    }
                    if (Projectile.ai[1] > ChargeBlade_Axe_Swing.SLASH_TIME / 1.8f && Projectile.ai[1] < ChargeBlade_Axe_Swing.SLASH_TIME / 2.2f)
                    {
                        Projectile.ai[1] -= 0.4f;
                    }
                }
            };

            ChargeBlade_Axe_Swing_Liberate Axe_Liberate_Slash1_OnDef = new(this, () => Player.controlUseTile) // 斧解1-格挡成功
            {
                StartVel = Vector2.UnitX.RotatedBy(0.8),
                VelScale = new Vector2(1, 0.25f),
                VisualRotation = 0.75f,
                SwingRot = MathHelper.PiOver2 * 1.25f,
                SwingDirectionChange = false,
                DefAttack = true
            };

            ChargeBlade_Axe_Swing_Liberate_SP_PreAttack chargeBlade_Axe_Swing_Liberate_SP_PreAttack = new(this); // 大超解动作

            ChargeBlade_Axe_Swing_Liberate_Large chargeBlade_Axe_Swing_Liberate_Large = new(this, () => true); // 大解

            ChargeBlade_Axe_Swing_Liberate_Super chargeBlade_Axe_Swing_Liberate_Super = new(this); // 超解

            ChargeBlade_Axe_Swing_ShieldStrength chargeBlade_Axe_Swing_ShieldStrength = new(this); // 盾强化

            ChargeBlade_Axe_Swing_AxeStrength chargeBlade_Axe_Swing_AxeStrength = new(this); // 斧强化

            //ChargeBlade_LimitRemoval chargeBlade_LimitRemoval = new(this); // 瓶子限制移除

            ChargeBlade_ShieldsRotSlash_LimitRemoval chargeBlade_ShieldsRotSlash_LimitRemoval = new(this);

            ChargeBlade_EnergyBlade chargeBlade_EnergyBlade = new(this); // 光剑
            #endregion

            #endregion
            #region 技能添加
            chargeBladeNotUse.AddSkill(SwordHeld).AddSkill(Sword_Def);
            #region 盾锯特殊全部添加,用于CD,实际只有特殊动作时可以切换
            chargeBlade_ShieldsRotSlash_LimitRemoval.AddBySkill(Sword_MoveSlash,
                Sword_Def,
                SwordHeld,
                chargeBladeNotUse,
                AxeHeld,
                Axe_Liberate_Slash1,
                Axe_Liberate_Slash1_OnDef,
                Axe_Liberate_Slash2,
                Axe_SlashDown,
                Axe_SlashUp,
                Axe_ToSword,
                Sword_RotSlash,
                Sword_SlashDown,
                Sword_SlashDown_OnDef,
                Sword_SlashUP,
                chargeBlade_AddBottles,
                chargeBlade_AddBottles_InChannel,
                chargeBlade_AfterAddBottles_Swing,
                chargeBlade_Axe_Swing_AxeStrength,
                chargeBlade_Axe_Swing_Liberate_Large,
                chargeBlade_Axe_Swing_Liberate_SP_PreAttack,
                chargeBlade_Axe_Swing_Liberate_Super,
                chargeBlade_Axe_Swing_ShieldStrength,
                chargeBlade_Def_GP,
                chargeBlade_EnergyBlade);
            chargeBlade_ShieldsRotSlash_LimitRemoval.AddSkill(chargeBlade_Def_GP);
            #endregion
            #region 格挡技能添加
            Axe_Liberate_Slash1_OnDef.AddBySkill(chargeBlade_Def_GP, Sword_Def);
            Sword_SlashDown_OnDef.AddBySkill(chargeBlade_Def_GP, Sword_Def);

            Sword_SlashDown_OnDef.AddSkill(Sword_SlashUP);
            Axe_Liberate_Slash1_OnDef.AddSkilles(Axe_SlashDown, Axe_Liberate_Slash2);
            #endregion
            #region 剑形态技能添加
            SwordHeld.AddSkill(Sword_SlashDown).AddSkill(Sword_SlashUP).AddSkill(Sword_RotSlash).AddSkill(chargeBlade_Def_GP);
            Sword_Def.AddBySkill(Sword_SlashDown, Sword_SlashUP, Sword_RotSlash);
            Sword_Def.AddSkill(chargeBlade_AddBottles);
            chargeBlade_AddBottles.AddBySkill(Sword_SlashDown, Sword_SlashUP, Sword_RotSlash, chargeBlade_Sword_Swing_ChannelSwing,Sword_Dash_SlashDown,Sword_MoveSlash,Sword_SlashDown_OnDef, Axe_ToSword);
            Sword_Def.AddSkill(chargeBlade_Sword_ToAxe);
            chargeBlade_AddBottles.AddSkill(chargeBlade_AddBottles_InChannel).AddSkill(chargeBlade_AfterAddBottles_Swing);

            chargeBlade_Sword_ShieldSpurts.AddBySkill(Sword_Dash_SlashDown, Sword_SlashUP, Sword_RotSlash, chargeBlade_AddBottles, chargeBlade_Sword_Swing_ChannelSwing);
            Sword_Dash_SlashDown.AddBySkill(chargeBladeNotUse, SwordHeld, Sword_SlashDown, Sword_SlashUP);

            SwordHeld.AddSkill(chargeBlade_Sword_Swing_ChannelSwing).AddSkill(Sword_RotSlash);
            chargeBlade_Def_GP.AddSkill(chargeBlade_AddBottles);
            chargeBlade_Def_GP.AddSkill(chargeBlade_Sword_ToAxe);

            chargeBlade_Sword_ToAxe.AddSkill(chargeBlade_Def_GP);

            Sword_MoveSlash.AddBySkill(Sword_SlashUP, Sword_SlashDown, chargeBlade_Sword_Swing_ChannelSwing, Sword_SlashDown_OnDef, Sword_RotSlash);
            Sword_MoveSlash.AddSkill(Sword_Def);

            chargeBlade_Sword_ShieldSpurts.AddSkill(chargeBlade_Axe_Swing_Liberate_SP_PreAttack);
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddChangeSkill(chargeBlade_Sword_ShieldSpurts, () => Player.controlUseTile && Player.controlUseItem && chargeBlade_Sword_ShieldSpurts.SwitchCondition());
            //Sword_Def.AddBySkill(chargeBladeNotUse, chargeBlade_OnHeld_Sword);
            #endregion
            #region 斧形态技能添加
            chargeBlade_Sword_ToAxe.AddSkill(AxeHeld);

            AxeHeld.AddSkill(Axe_SlashUp).AddSkill(Axe_SlashDown).AddSkill(Axe_SlashUp);
            AxeHeld.AddSkill(Axe_Liberate_Slash1).AddSkill(Axe_Liberate_Slash2);
            Axe_SlashUp.AddSkill(Axe_Liberate_Slash2).AddSkill(Axe_SlashUp);
            Axe_SlashDown.AddSkill(Axe_Liberate_Slash1).AddSkill(Axe_SlashDown);

            #region 超解
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddSkill(chargeBlade_Axe_Swing_Liberate_Super);
            chargeBlade_Axe_Swing_Liberate_Super.AddSkilles(chargeBlade_EnergyBlade,chargeBlade_Axe_Swing_Liberate_Large/*, chargeBlade_LimitRemoval*/);
            #endregion
            #region 大解特殊动作
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddSkill(chargeBlade_Axe_Swing_Liberate_Large);
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddBySkill(AxeHeld, Axe_Liberate_Slash1, Axe_Liberate_Slash2, Axe_SlashUp, Axe_SlashDown, Sword_Def, chargeBlade_Def_GP,Sword_SlashDown_OnDef, Axe_Liberate_Slash1_OnDef,chargeBlade_AddBottles);
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddChangeSkill(Axe_Liberate_Slash2, () => (Player.controlUseTile && Projectile.ai[0] > 1) || (Player.controlUseTile && Player.controlUseItem));
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddChangeSkill(Axe_Liberate_Slash1_OnDef, () =>
            {
                return Player.controlUseTile && Player.controlUseItem;
            });
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddChangeSkill(Sword_SlashDown_OnDef, () =>
            {
                return Player.controlUseTile;
            });
            chargeBlade_Axe_Swing_Liberate_SP_PreAttack.AddSkilles(chargeBlade_Axe_Swing_ShieldStrength, chargeBlade_Axe_Swing_AxeStrength);
            #endregion

            Axe_ToSword.AddBySkill(AxeHeld, Axe_SlashUp, Axe_SlashDown);
            #endregion
            #endregion
            CurrentSkill = chargeBladeNotUse;
        }
        public bool PreSkillTimeOut()
        {
            if (OldSkills.Count <= 1) return true;

            if (CurrentSkill is ChargeBlade_Sword_Swing || CurrentSkill.GetType().IsSubclassOf(typeof(ChargeBlade_Sword_Held)) || CurrentSkill is ChargeBlade_AddBottles || CurrentSkill is ChargeBlade_Axe_Swing_Liberate_Large || CurrentSkill is ChargeBlade_Sword_ShieldSpurts || CurrentSkill is ChargeBlade_Axe_Swing_Liberate_Super || CurrentSkill is ChargeBlade_ShieldsRotSlash_LimitRemoval || CurrentSkill is ChargeBlade_EnergyBlade) // 如果是剑挥舞类,或者防御类,或者装瓶
            {
                //var targetSkill = OldSkills.Find(x => x is ChargeBlade_Sword_Held && !x.GetType().IsSubclassOf(typeof(ChargeBlade_Sword_Held)));
                //if(targetSkill != null)
                //{
                //    CurrentSkill.OnSkillDeactivate();
                //    targetSkill.OnSkillActive();
                //    CurrentSkill = targetSkill;
                //    return false;
                //}
                CurrentSkill.OnSkillDeactivate();
                SwordHeld.OnSkillActive();
                CurrentSkill = SwordHeld;
                return false;
            }
            else if(CurrentSkill is ChargeBlade_Axe_Basic)
            {
                CurrentSkill.OnSkillDeactivate();
                AxeHeld.OnSkillActive();
                CurrentSkill = AxeHeld;
                return false;
                //var targetSkill = OldSkills.Find(x => x is ChargeBlade_Axe_Held && !x.GetType().IsSubclassOf(typeof(ChargeBlade_Axe_Held)));
                //if (targetSkill != null)
                //{
                //    CurrentSkill.OnSkillDeactivate();
                //    targetSkill.OnSkillActive();
                //    CurrentSkill = targetSkill;
                //    return false;
                //}
                //else
                //{
                //    var skill = OldSkills[1].switchToSkill.Find(x => x is ChargeBlade_OnDef).switchToSkill.Find(x => x is ChargeBlade_Sword_ToAxe).switchToSkill.Find(x => x is ChargeBlade_Axe_Held);
                //    if(skill != null)
                //    {
                //        CurrentSkill.OnSkillDeactivate();
                //        skill.OnSkillActive();
                //        CurrentSkill = skill;
                //        return false;
                //    }
                //}
            }
            return true;
        }
    }
}
