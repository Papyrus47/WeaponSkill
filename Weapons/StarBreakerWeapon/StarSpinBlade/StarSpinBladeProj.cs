using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;
using WeaponSkill.Weapons.StarBreakerWeapon.General;
using WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage;
using WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills;
using static WeaponSkill.UI.StarBreakerUI.SkillsTreeUI.SkillsTreeUI;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public partial class StarSpinBladeProj : StarBreakerWeaponProj, IBasicSkillProj
    {
        public override string Texture => (GetType().Namespace + ".StarSpinBlade").Replace('.', '/');
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public SwingHelper SwingHelper;
        public float SwingLenght;
        public SSB_NoUse noUse;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                Player = itemUse.Player;
                Projectile.Size = itemUse.Item.Size;
                SwingHelper = new(Projectile, 36);
                SwingLenght = itemUse.Item.Size.Length();
                Init();
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (Player.HeldItem.ModItem is not StarSpinBlade || !Player.active || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            base.AI();
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            SlashDamage.SlashDamageOnHit();
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SkillsTreeUI.nowSkill = CurrentSkill;
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();
            noUse = new(this);
            RightChannelCombo();
            LeftChannelCombo();
            RightCombo();
            LeftCombo();
            CurrentSkill = noUse;
        }
        /// <summary>
        /// 左键短按攻击注册、连接、技能显示
        /// </summary>
        protected void LeftCombo()
        {
            Func<float, float> timeChange = (time) => MathF.Pow(time, 3);
            #region 技能创建
            #region 左键-左键起步为主
            SSB_Swing LeftStart = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 4,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = true,
                SwingTime = 30,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L1").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing TwoSlash_1 = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 150,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0f,
                SwingDirectionChange = false,
                SwingTime = 20,
                ActionDmg = 1.5f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L2").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing TwoSlash_2 = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 200,
                PreTime = 6,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.4f,
                SwingDirectionChange = false,
                SwingTime = 15,
                OnUse = (skill) =>
                {
                    SwingHelper.SetRotVel(-0.4f);
                },
                ActionDmg = 2,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L2").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing SpinSlash = new(this, () => LeftChick && CanChangeToStopActionSkill, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 6,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = -0f,
                SwingDirectionChange = true,
                SwingTime = 20,
                ActionDmg = 3f,
                OnHit = (target, hit, _) =>
                {
                    if ((Player.HeldItem.ModItem as StarSpinBlade).SpinValue > 10)
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue -= 10;
                        Player.ApplyDamageToNPC(target, hit.SourceDamage / 3 * 2, 0, hit.HitDirection, hit.Crit, hit.DamageType);
                    }
                    else if ((Player.HeldItem.ModItem as StarSpinBlade).SpinValue < -100)
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 100;
                        for (int i = 0; i < 5; i++)
                        {
                            Player.ApplyDamageToNPC(target, (int)(hit.SourceDamage / 3 * 0.7f), 0, hit.HitDirection, hit.Crit, hit.DamageType);
                        }
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L4").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };

            SSB_Swing FallHit = new(this, () => RightChick, timeChange) // 落砸
            {
                IsTrueSlash = false,
                SpinValue = 50,
                PreTime = 6,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.PiOver4 + MathHelper.PiOver2,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 20,
                ActionDmg = 0.3f,
                DmgCounts = 4,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L3").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing BackSlash = new(this, () => RightChick && CanChangeToStopActionSkill, timeChange) // 回击斩
            {
                IsTrueSlash = false,
                SpinValue = 50,
                PreTime = 12,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.8f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.2f,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.2f,
                DmgCounts = 6,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L3").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };

            SSB_Swing_Channel DargBladeSlash = new(this, () => LeftChannel && !RightChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), timeChange, () => LeftChannel)
            {
                IsTrueSlash = true,
                SpinValue = 300,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.2),
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 3f,
                OnChannel = (_) =>
                {
                    Player.velocity.X = Player.direction * 10;
                    Dust dust = Dust.NewDustDirect(Player.Bottom + new Vector2(Player.direction * -120, 0), 10, 10, DustID.Fireworks);
                    dust.velocity.X = -Player.velocity.X * 0.5f;
                    dust.velocity.Y -= Main.rand.NextFloat(4);
                    dust.scale *= 2;
                },
                OnUse = (_) =>
                {
                    SwingHelper.SetRotVel(-0.4f);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L6").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: true,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing_Channel ChannelSlash = new(this, () => LeftChannel && !RightChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)), timeChange, () => LeftChannel)
            {
                IsTrueSlash = true,
                SpinValue = -200,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.TwoPi,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 15,
                ActionDmg = 5f,
                OnChannel = (_) =>
                {
                    (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 50;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L7").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: true,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing SpinSalshUp = new(this, () => LeftChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), timeChange)
            {
                IsTrueSlash = true,
                SpinValue = -100,
                PreTime = 5,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 35,
                ActionDmg = 3f,
                OnUse = (_) =>
                {
                    if ((int)Projectile.ai[1] == 35)
                    {
                        NewWindProj(Player.Bottom + new Vector2(Player.direction * 200, 0), -Vector2.UnitY * 2, Projectile.damage * 3, 0, 0, 0, (proj) =>
                        {
                            if (proj.ai[0] != 1 && proj.ai[1]++ > 200)
                            {
                                proj.ai[0] = 1;
                            }
                        }, (target, _, _) => HitFly(target));
                    }
                },
                OnHit = (target, hit, _) =>
                {
                    HitFly(target);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L8").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: true,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing DodgeSlash = new(this, () => LeftChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)), timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 5,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 35,
                ActionDmg = 2f,
                OnUse = (_) =>
                {
                    Player.velocity.X = Player.direction * -10;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L9").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: true,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            #endregion
            #region 左键-右键起步为主
            SSB_Swing UpSpinSlash1 = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = -100,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0f,
                SwingDirectionChange = false,
                SwingTime = 20,
                ActionDmg = 0.3f,
                DmgCounts = 5,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L11").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing UpSpinSlash2 = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = -30,
                PreTime = 6,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.TwoPi * 1.5f,
                VisualRotation = -0f,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.5f,
                DmgCounts = 4,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L11").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing UpSpinSlash3 = new(this, () => true, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = -30,
                PreTime = 1,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.TwoPi * 1.5f,
                VisualRotation = -0f,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.5f,
                DmgCounts = 4
            };
            SSB_Swing UpSpinSlash4 = new(this, () => true, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = -30,
                PreTime = 1,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.TwoPi * 1.5f,
                VisualRotation = -0f,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.5f,
                DmgCounts = 4,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L11").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing MoveBladeHit = new(this, () => RightChick && CanChangeToStopActionSkill, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 0,
                PreTime = 10,
                StartVel = Vector2.UnitX.RotatedBy(MathHelper.PiOver4),
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.PiOver4,
                VisualRotation = 0f,
                SwingDirectionChange = false,
                SwingTime = 10,
                ActionDmg = 0.3f,
                DmgCounts = 2,
                OnUse = (_) =>
                {
                    Player.velocity.X = Player.direction * 8;
                    SwingHelper.SetNotSaveOldVel();
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L15").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };

            SSB_Throw ThrowTrueSlash = new(this, () => LeftChannel && !RightChick, true)
            {
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L10").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing SlashDown = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 10,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.PiOver2,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 10,
                ActionDmg = 3f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L12").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing AcrossSlash = new(this, () => LeftChick && CanChangeToStopActionSkill, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 10,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 4f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L13").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };
            SSB_Swing StrongAcrossSlash = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 10,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.PiOver2 + MathHelper.Pi,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 6f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L14").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };

            #endregion
            #endregion
            #region 技能连接

            MoveBladeHit.AddSkill(StrongAcrossSlash);
            UpSpinSlash1.AddSkill(AcrossSlash);
            UpSpinSlash1.AddSkill(SlashDown);
            UpSpinSlash1.AddSkill(MoveBladeHit).AddSkill(UpSpinSlash2);
            LeftStart.AddSkill(UpSpinSlash1).AddSkill(UpSpinSlash2).AddSkill(UpSpinSlash3).AddSkill(UpSpinSlash4);

            DodgeSlash.AddBySkill(FallHit, BackSlash, SpinSalshUp, UpSpinSlash1, UpSpinSlash4, LeftStart);
            SpinSalshUp.AddBySkill(FallHit, BackSlash, DodgeSlash, UpSpinSlash1, UpSpinSlash4, LeftStart);

            DargBladeSlash.AddBySkill(LeftStart, TwoSlash_1, TwoSlash_2, SpinSlash, ChannelSlash, StrongAcrossSlash, AcrossSlash, SlashDown);
            ChannelSlash.AddBySkill(LeftStart, TwoSlash_1, TwoSlash_2, SpinSlash, DargBladeSlash, StrongAcrossSlash, AcrossSlash, SlashDown);

            LeftStart.AddSkill(ThrowTrueSlash);

            TwoSlash_1.AddSkill(BackSlash);
            TwoSlash_1.AddSkill(FallHit);
            TwoSlash_1.AddSkill(SpinSlash);
            noUse.AddSkill(LeftStart).AddSkill(TwoSlash_1).AddSkill(TwoSlash_2);
            #endregion
        }
        /// <summary>
        /// 右键短按攻击注册、连接、技能显示
        /// </summary>
        protected void RightCombo()
        {
            Func<float, float> timeChange = (time) => MathF.Pow(time, 3);
            #region 技能创建
            SSB_Swing RightStart = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 100,
                PreTime = 4,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2 + 0.5f,
                VisualRotation = -0.4f,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.5f,
                DmgCounts = 4,
                OnUse = (_) =>
                {
                    SwingHelper.SetRotVel(0.4f);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R0").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            #region 右键-右键
            SSB_Swing SpinCrossSpinSlash1 = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 100,
                PreTime = 4,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = false,
                SwingTime = 20,
                ActionDmg = 0.7f,
                DmgCounts = 4,
                OnUse = (_) =>
                {
                    if ((int)Projectile.ai[1] == 1 || (int)Projectile.ai[1] == 59)
                    {
                        Player.ChangeDir(-Player.direction);
                    }
                    if ((int)Projectile.ai[1] < 30)
                        Player.velocity.X = Player.direction * 22;
                    else
                        Player.velocity.X = 0;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R1").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing SpinCrossSpinSlash2 = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 200,
                PreTime = 4,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = true,
                SwingTime = 20,
                ActionDmg = 1f,
                DmgCounts = 5,
                OnUse = (_) =>
                {
                    //if ((int)Projectile.ai[1] == 1)
                    //{
                    //    Player.ChangeDir(-Player.direction);
                    //}
                    if ((int)Projectile.ai[1] < 30)
                        Player.velocity.X = Player.direction * 22;
                    else
                        Player.velocity.X = 0;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R1").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing TwoStrongSlashUp1 = new(this, () => RightChick && CanChangeToStopActionSkill, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 0,
                PreTime = 13,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + 0.2f,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 30,
                ActionDmg = 0.1f,
                DmgCounts = 15,
                OnHit = (target,_,_) =>
                {
                    HitFly(target);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R2").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };
            SSB_Swing TwoStrongSlashUp2 = new(this, () => true, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 150,
                PreTime = 4,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + 0.2f,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 0.2f,
                DmgCounts = 20,
                OnHit = (target, _, _) =>
                {
                    HitFly(target);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R2").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing BackSlash = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 70,
                PreTime = 4,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 15,
                ActionDmg = 3f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R3").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };

            SSB_Swing RisingSlash = new(this, () => LeftChick && CanChangeToStopActionSkill, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 50,
                PreTime = 4,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 3f,
                OnHit = (target, _, _) =>
                {
                    HitFly(target);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R4").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };
            #endregion
            #region 右键-左键
            SSB_Swing StrongCrossSlash1 = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 4,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 2f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R5").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: true,
                                    inSky: false)
            };
            SSB_Swing StrongCrossSlash2 = new(this, () => LeftChick, timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 150,
                PreTime = 4,
                StartVel = -Vector2.UnitX.RotatedBy(0.4),
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 4f,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R5").Value,
                skillsControl = new(isLeftClick: true,
                        isRightClick: false,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: false,
                        isChannel: false,
                        isStopAtk: true,
                        inSky: false)
            };

            SSB_Swing_Channel DashRisingSlash = new(this, () => RightChick && CanChangeToStopActionSkill, timeChange, () =>
            {
                Projectile.ai[2]++;
                if (Projectile.ai[2] > 30)
                {
                    Projectile.ai[2] = 0;
                    return false;
                }
                return true;
            })
            {
                IsTrueSlash = false,
                SpinValue = 300,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.2),
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 0.5f,
                DmgCounts = 2,
                OnChannel = (_) =>
                {
                    Player.velocity.X = Player.direction * 10;
                    Dust dust = Dust.NewDustDirect(Player.Bottom + new Vector2(Player.direction * -120, 0), 10, 10, DustID.Fireworks);
                    dust.velocity.X = -Player.velocity.X * 0.5f;
                    dust.velocity.Y -= Main.rand.NextFloat(4);
                    dust.scale *= 2;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R6").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            #endregion
            #region 搓招
            SSB_Swing FlashSlashSpinSlash = new(this, () => RightChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), (time) => time)
            {
                IsTrueSlash = false,
                SpinValue = 500,
                PreTime = 4,
                StartVel = -Vector2.UnitX.RotatedBy(0.4),
                VelScale = new Vector2(1, 1),
                SwingRot = MathHelper.TwoPi * 10,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 180,
                ActionDmg = 0.3f,
                DmgCounts = 10,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R8").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: true,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false),
                OnUse = (_) =>
                {
                    if (Projectile.ai[1] % (18 * 3) == 0)
                    {
                        TheUtility.ResetProjHit(Projectile);
                    }
                },
            };
            SSB_Swing SpinMoreAtk = new(this, () => RightChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)), timeChange)
            {
                IsTrueSlash = false,
                SpinValue = -200,
                PreTime = 4,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = false,
                SwingTime = 20,
                ActionDmg = 0.3f,
                DmgCounts = 5,
                OnHit = (NPC target, NPC.HitInfo hit, int _) =>
                {
                    for (int i = 0; i < 8; i++)
                    {
                        SlashDamage.SlashDamageOnHit();
                        Player.ApplyDamageToNPC(target, (int)(Projectile.damage * 0.7f), 0f, hit.HitDirection, hit.Crit, hit.DamageType, false);
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R1").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: true,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Throw ThrowFlashSlash = new(this, () => RightChannel, true)
            {
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.R9").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing_Channel DargBladeSlash = new(this, () => RightChannel && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), timeChange, () => RightChannel)
            {
                IsTrueSlash = false,
                SpinValue = 200,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.2),
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 2f,
                DmgCounts = 2,
                OnChannel = (_) =>
                {
                    Player.velocity.X = Player.direction * 10;
                    Dust dust = Dust.NewDustDirect(Player.Bottom + new Vector2(Player.direction * -120, 0), 10, 10, DustID.Fireworks);
                    dust.velocity.X = -Player.velocity.X * 0.5f;
                    dust.velocity.Y -= Main.rand.NextFloat(4);
                    dust.scale *= 2;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L6").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: true,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            #endregion
            #endregion
            #region 技能注册
            DargBladeSlash.AddBySkill(StrongCrossSlash1, StrongCrossSlash2, BackSlash, RisingSlash);

            SpinMoreAtk.AddBySkill(RightStart, DashRisingSlash, StrongCrossSlash2, SpinCrossSpinSlash1, SpinCrossSpinSlash2);
            FlashSlashSpinSlash.AddBySkill(RightStart, DashRisingSlash, StrongCrossSlash2, SpinCrossSpinSlash1, SpinCrossSpinSlash2);
            RightStart.AddSkill(ThrowFlashSlash);

            StrongCrossSlash1.AddSkill(RisingSlash);
            StrongCrossSlash1.AddSkill(DashRisingSlash);
            RightStart.AddSkill(StrongCrossSlash1).AddSkill(StrongCrossSlash2);

            SpinCrossSpinSlash1.AddSkilles(RisingSlash,BackSlash);
            SpinCrossSpinSlash1.AddSkill(TwoStrongSlashUp1).AddSkill(TwoStrongSlashUp2);
            noUse.AddSkill(RightStart).AddSkill(SpinCrossSpinSlash1).AddSkill(SpinCrossSpinSlash2);
            #endregion
        }
        /// <summary>
        /// 左键长按攻击注册、连接、技能显示
        /// </summary>
        protected void LeftChannelCombo()
        {
            Func<float, float> timeChange = (time) => MathF.Pow(time, 3);
            SSB_Swing_Channel ChannelLeftStart = new(this, () => LeftChannel, timeChange, () => LeftChannel && !RightChick)
            {
                IsTrueSlash = true,
                SpinValue = 300,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.2),
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 25,
                ActionDmg = 3f,
                OnChannel = (_) =>
                {
                    if (Player.CheckMana(2, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 50;
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC0").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing_Channel StrongSpinAcrossSlash1 = new(this, () => LeftChannel, timeChange, () => LeftChannel && !RightChick)
            {
                IsTrueSlash = true,
                SpinValue = 300,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.6),
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 3f,
                OnChannel = (_) =>
                {
                    if (Player.CheckMana(1, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 70;
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC1").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing_Channel StrongSpinAcrossSlash2 = new(this, () => LeftChannel, timeChange, () => LeftChannel)
            {
                IsTrueSlash = true,
                SpinValue = -500,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.4f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                SwingTime = 25,
                ActionDmg = 3f,
                OnChannel = (_) =>
                {
                    if (Player.CheckMana(2, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 100;
                    }
                },
                OnHit = (target, hit, _) =>
                {
                    if ((Player.HeldItem.ModItem as StarSpinBlade).SpinValue > 0)
                    {
                        Player.ApplyDamageToNPC(target, hit.SourceDamage * 8, 0, hit.HitDirection, hit.Crit, hit.DamageType);
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC1").Value,
                skillsControl = new(isLeftClick: true,
                        isRightClick: false,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: false,
                        isChannel: true,
                        isStopAtk: false,
                        inSky: false)
            };
            SSB_Swing SlopeUpBackSlash = new(this, () => RightChick, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 300,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 0.7f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.3f,
                SwingDirectionChange = false,
                SwingTime = 25,
                ActionDmg = 1f,
                DmgCounts = 5,
                OnUse = (_) =>
                {
                    SwingHelper.SetRotVel(-0.5f);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC2").Value,
                skillsControl = new(isLeftClick: false,
                        isRightClick: true,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: false,
                        isChannel: false,
                        isStopAtk: false,
                        inSky: false)
            };
            SSB_Swing StrongFallHit = new(this, () => RightChick && CanChangeToStopActionSkill, timeChange) // 强化落砸
            {
                IsTrueSlash = false,
                SpinValue = 150,
                PreTime = 6,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.PiOver4 + MathHelper.PiOver2,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 20,
                ActionDmg = 0.3f,
                DmgCounts = 8,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC3").Value,
                skillsControl = new(isLeftClick: false,
                        isRightClick: true,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: false,
                        isChannel: false,
                        isStopAtk: true,
                        inSky: false)
            };

            SSB_Swing_Channel ChannelSkyFallSlash = new(this, () => LeftChannel && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), (time) => MathF.Pow(time, 6), () => LeftChannel)
            {
                IsTrueSlash = true,
                SpinValue = 0,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1.5f, 0.4f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                SwingTime = 55,
                ActionDmg = 3f,
                OnChannel = (skill) =>
                {
                    Player.velocity.X *= 0.1f;
                    if (Player.CheckMana(10, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 500;
                        skill.SpinValue = -(Player.HeldItem.ModItem as StarSpinBlade).SpinValue / 10;
                        skill.ActionDmg = -skill.SpinValue / 10;
                    }
                },
                OnUse = (_) =>
                {
                    if (Projectile.ai[1] == 2)
                    {
                        Player.velocity.X = Player.direction * 20;
                        Player.velocity.Y = -20;

                        Player.ChangeDir(-Player.direction);
                    }
                    SwingHelper.SetRotVel(0.5f);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC1").Value,
                skillsControl = new(isLeftClick: true,
                        isRightClick: false,
                        isSP1Click: false,
                        isDoubleForwardMove: true,
                        isDoubleBackwardMove: false,
                        isChannel: true,
                        isStopAtk: false,
                        inSky: false)
            };
            SSB_Swing_Channel ChannelRisingSlash = new(this, () => LeftChannel && !RightChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)), timeChange, () => LeftChannel && !RightChick)
            {
                IsTrueSlash = true,
                SpinValue = 0,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1f, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0f,
                SwingDirectionChange = false,
                SwingTime = 25,
                ActionDmg = 3f,
                OnChannel = (skill) =>
                {
                    Player.velocity.X *= 0.1f;
                    if (Player.CheckMana(10, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 500;
                        skill.SpinValue = -(Player.HeldItem.ModItem as StarSpinBlade).SpinValue / 10;
                        skill.ActionDmg = -skill.SpinValue / 15;
                    }
                },
                OnUse = (_) =>
                {
                    if (Projectile.ai[1] == 2)
                    {
                        Player.velocity.Y = -20;
                    }
                },
                OnHit = (target,_,_) =>
                {
                    HitFly(target,20);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC5").Value,
                skillsControl = new(isLeftClick: true,
                        isRightClick: false,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: true,
                        isChannel: true,
                        isStopAtk: false,
                        inSky: false)
            };

            SSB_Swing_Channel DestroySpinSlash = new(this, () => LeftChannel && CanChangeToStopActionSkill, timeChange, () => LeftChannel)
            {
                IsTrueSlash = true,
                SpinValue = 0,
                PreTime = 6,
                StartVel = -Vector2.UnitX.RotatedBy(0.6),
                VelScale = new Vector2(1f, 1f),
                SwingRot = MathHelper.Pi,
                VisualRotation = 0f,
                SwingDirectionChange = true,
                SwingTime = 25,
                ActionDmg = 10f,
                OnChannel = (skill) =>
                {
                    Player.velocity.X *= 0.1f;
                    if (Player.CheckMana(20, true))
                    {
                        Player.GetModPlayer<WindsPlayer>().UseWindsState = true;
                        Player.GetModPlayer<WindsPlayer>().Time = 0;
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 1000;
                        if (Projectile.ai[2]++ > 4)
                        {
                            Projectile.ai[2] = 0;
                            var proj = NewWindProj(Player.Center, -Vector2.UnitY * 2f, 0, 0, 0, 0);
                            proj.extraUpdates /= 2;
                            proj.timeLeft /= 3;
                        }
                        skill.SpinValue = -(Player.HeldItem.ModItem as StarSpinBlade).SpinValue / 2;
                        
                    }
                },
                OnUse = (skill) =>
                {
                    if (Projectile.ai[1] == 2)
                    {
                        NewWindProj(Player.Center + new Vector2(1000 * Player.direction,0), Vector2.UnitX * 4 * -Player.direction, -skill.SpinValue / 1000, 0, 0, 0);
                    }
                },
                OnHit = (target, _, _) =>
                {
                    HitFly(target, 20);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.LC6").Value,
                skillsControl = new(isLeftClick: true,
                        isRightClick: false,
                        isSP1Click: false,
                        isDoubleForwardMove: false,
                        isDoubleBackwardMove: false,
                        isChannel: true,
                        isStopAtk: true,
                        inSky: false)
            };
            SSB_Swing SpinSalshUp = new(this, () => LeftChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(Player.direction)), timeChange)
            {
                IsTrueSlash = true,
                SpinValue = -100,
                PreTime = 5,
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 35,
                ActionDmg = 3f,
                OnUse = (_) =>
                {
                    if ((int)Projectile.ai[1] == 35)
                    {
                        NewWindProj(Player.Bottom + new Vector2(Player.direction * 200, 0), -Vector2.UnitY * 2, Projectile.damage * 3, 0, 0, 0, (proj) =>
                        {
                            if (proj.ai[0] != 1 && proj.ai[1]++ > 200)
                            {
                                proj.ai[0] = 1;
                            }
                        }, (target, _, _) => HitFly(target));
                    }
                },
                OnHit = (target, hit, _) =>
                {
                    HitFly(target);
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L8").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: true,
                                    isDoubleBackwardMove: false,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_Swing DodgeSlash = new(this, () => LeftChick && GetPlayerDoubleTap(GetPlayerDoubleTapDir(-Player.direction)), timeChange)
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 5,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0,
                SwingDirectionChange = true,
                SwingTime = 35,
                ActionDmg = 2f,
                OnUse = (_) =>
                {
                    Player.velocity.X = Player.direction * -10;
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.L9").Value,
                skillsControl = new(isLeftClick: true,
                                    isRightClick: false,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: true,
                                    isChannel: false,
                                    isStopAtk: false,
                                    inSky: false)
            };
            DodgeSlash.AddBySkill(StrongFallHit, SlopeUpBackSlash);
            SpinSalshUp.AddBySkill(StrongFallHit, SlopeUpBackSlash);

            ChannelRisingSlash.AddBySkill(ChannelLeftStart, StrongSpinAcrossSlash1, StrongSpinAcrossSlash2);
            ChannelSkyFallSlash.AddBySkill(ChannelLeftStart, StrongSpinAcrossSlash1, StrongSpinAcrossSlash2);

            StrongSpinAcrossSlash1.AddSkill(DestroySpinSlash);
            StrongSpinAcrossSlash1.AddSkill(StrongFallHit);
            StrongSpinAcrossSlash1.AddSkill(SlopeUpBackSlash);

            noUse.AddSkill(ChannelLeftStart).AddSkill(StrongSpinAcrossSlash1).AddSkill(StrongSpinAcrossSlash2);
        }
        /// <summary>
        /// 右键长按攻击注册、连接、技能显示
        /// </summary>
        protected void RightChannelCombo()
        {
            Func<float, float> timeChange = (time) => MathF.Pow(time, 3);
            SSB_Swing_Channel ChannelRightStart = new(this, () => RightChannel, timeChange, () => RightChannel && !LeftChick)
            {
                IsTrueSlash = false,
                SpinValue = 50,
                PreTime = 6,
                StartVel = Vector2.UnitX.RotatedBy(MathHelper.PiOver4),
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.PiOver4,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 0.2f,
                DmgCounts = 3,
                OnUse = (_) =>
                {
                    Player.velocity.X = Player.direction * 8;
                    SwingHelper.SetNotSaveOldVel();
                },
                OnChannel = (_) =>
                {
                    if (Player.CheckMana(1, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 50;
                    }
                },
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.RC0").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };
            SSB_SC_WindSpinBlade windSpinBlade = new(this, () => RightChannel, timeChange)
            {
                IsTrueSlash = false,
                SpinValue = 200,
                PreTime = 6,
                StartVel = Vector2.UnitX,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.TwoPi,
                VisualRotation = 0,
                SwingDirectionChange = false,
                SwingTime = 15,
                ActionDmg = 0.2f,
                DmgCounts = 3,
                Name = Language.GetOrRegister("Mods.WeaponSkill.StarSpinBladeProj.Skill.RC1").Value,
                skillsControl = new(isLeftClick: false,
                                    isRightClick: true,
                                    isSP1Click: false,
                                    isDoubleForwardMove: false,
                                    isDoubleBackwardMove: false,
                                    isChannel: true,
                                    isStopAtk: false,
                                    inSky: false)
            };

            noUse.AddSkill(ChannelRightStart).AddSkill(windSpinBlade);
        }

        /// <summary>
        /// 击飞函数
        /// </summary>
        /// <param name="target"></param>
        public static void HitFly(NPC target,float velY = 10)
        {
            NPC n = target;
            if (n.knockBackResist != 0)
            {
                n.velocity.Y -= velY;
                n.velocity.X = 0;
            }
        }
        public Projectile NewWindProj(Vector2 pos,Vector2 vel,int dmg,float ai0,float ai1,float ai2,Action<Projectile> UseAI = null,WindsProj.OnHit onHit = null)
        {
            var proj = Projectile.NewProjectileDirect(Projectile.GetItemSource_FromThis(),pos,vel,ModContent.ProjectileType<WindsProj>(),dmg,0f,Player.whoAmI,ai0,ai1,ai2);
            (proj.ModProjectile as WindsProj).UseAI = UseAI;
            (proj.ModProjectile as WindsProj).onHit = onHit;
            return proj;
        }
    }
}
