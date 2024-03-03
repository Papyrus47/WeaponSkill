using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader.IO;
using WeaponSkill.Weapons;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.LongSword.Skills;

namespace WeaponSkill
{
    public class WeaponSkillPlayer : ModPlayer
    {
        public bool InBlocking;
        public bool IsBlockAttack;
        public bool Player_BowSidingStep;
        public byte BowChannelLeave;
        /// <summary>
        /// 太刀 见切
        /// </summary>
        public bool InForesightSlash;
        /// <summary>
        /// 见切成功
        /// </summary>
        public bool ForesightSlash_OnHit;
        /// <summary>
        /// 居合无敌
        /// </summary>
        public bool Naknotsu_Slash;
        /// <summary>
        /// 居合被命中
        /// </summary>
        public bool Naknotsu_Slash_OnHit;
        /// <summary>
        /// 拥有的弹药,用于切换
        /// </summary>
        public List<Item> AmmoItems;
        /// <summary>
        /// 展示远程攻击弹药切换UI
        /// </summary>
        public bool ShowTheRangeChangeUI;
        /// <summary>
        /// 展示耐力槽UI
        /// </summary>
        public bool ShowTheStamina;
        /// <summary>
        /// 非物品栏索引,一种独特的索引
        /// </summary>
        public int UseAmmoIndex;
        /// <summary>
        /// 耐力槽
        /// </summary>
        public int StatStamina;
        /// <summary>
        /// 耐力槽最大值
        /// </summary>
        public int StatStaminaMax;
        /// <summary>
        /// 根据这个确定是否回复耐力槽
        /// </summary>
        public int StatStaminaAddTime;
        /// <summary>
        /// 旧的耐力槽
        /// </summary>
        public int OldStatStamina;
        /// <summary>
        /// 太刀水月架势
        /// </summary>
        public bool SerenePose;
        /// <summary>
        /// 水月架势被命中
        /// </summary>
        public bool SerenePoseOnHit;
        /// <summary>
        /// 大锤水面击
        /// </summary>
        public bool WaterStrike;
        /// <summary>
        /// 大锤水面击成功
        /// </summary>
        /// </summary>
        public bool WaterStrike_OnHit;
        /// <summary>
        /// 神圣反击判定
        /// </summary>
        public bool HolyStrikesBack;
        /// <summary>
        /// 神圣反击被命中
        /// </summary>
        public bool HolyStrikesBack_OnHit;

        public BasicShield HeldShield;

        public const int DashRight = 2;
        public const int DashLeft = 3;
        public int DashTimer;
        public int DashDir = -1;
        public override void ResetEffects()
        {
            ShowTheRangeChangeUI = false; 
            ShowTheStamina = false;
            AmmoItems ??= new();
            BowChannelLeave = 0;
            if (DashTimer > 0) DashTimer--;

            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
                DashTimer = 15;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
                DashTimer = 15;
            }
            else if(DashTimer <= 0)
            {
                DashDir = -1;
            }
        }
        public override void PostUpdate()
        {
            if (StatStamina < OldStatStamina)
            {
                StatStaminaAddTime = 0;
            }
            OldStatStamina = StatStamina;
            if(StatStaminaAddTime < 90)
            {
                StatStaminaAddTime++;
            }
            else
            {
                StatStamina++;
                if(Player.velocity.LengthSquared() < 1f)
                {
                    StatStamina += 2;
                }
            }
            if (StatStamina > StatStaminaMax)
            {
                StatStamina = StatStaminaMax;
            }
        }
        public override void OnEnterWorld()
        {
            if(StatStaminaMax <= 0) StatStaminaMax = 600;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ShowTheRangeChangeUI && WeaponSkill.RangeChange.JustPressed)
            {
                UseAmmoIndex++;
            }
            if (WeaponSkill.BowSlidingStep.JustPressed)
            {
                Player_BowSidingStep = true;
            }
            else
            {
                Player_BowSidingStep = false;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (InBlocking)
            {
                modifiers.ModifyHurtInfo += BlockingDamage;
                IsBlockAttack = true;
            }
            //modifiers.FinalDamage *= 0;
        }
        public override bool? CanMeleeAttackCollideWithNPC(Item item, Rectangle meleeAttackHitbox, NPC target)
        {
            return base.CanMeleeAttackCollideWithNPC(item, meleeAttackHitbox, target);
        }
        private void BlockingDamage(ref Player.HurtInfo info)
        {
            info.Damage -= Player.statDefense * 10;
        }
        public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
        {
            //var crossbowPlayerDraw = new CrossbowPlayerDrawLayer();
            //positions.Add(crossbowPlayerDraw, crossbowPlayerDraw.GetDefaultPosition());
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            switch (BowChannelLeave)
            {
                case 1:
                    {
                        r = 0.7f;
                        b = 0.4f;
                        g = 0.4f;
                        a = 0;
                        break;
                    }
                case 2:
                    {
                        r = 0.9f;
                        b = 0.6f;
                        g = 0.6f;
                        a = 0;
                        break;
                    }
                case 3:
                    {
                        r = 1;
                        b = 0.5f;
                        g = 0.5f;
                        a = 0;
                        break;
                    }
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if(HeldShield != null && HeldShield.InDef && HeldShield.GetDefSucced(npc.Hitbox))
            {
                HeldShield.DefSucceeded = true;
                HeldShield.ModifyHit(ref modifiers);
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (HeldShield != null && HeldShield.InDef && HeldShield.GetDefSucced(proj.Hitbox))
            {
                HeldShield.DefSucceeded = true;
                HeldShield.ModifyHit(ref modifiers);
            }
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            #region 大剑防御
            if (InBlocking && info.Damage <= 1)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Main.rand.NextVector2Unit(), 4, 3, 1));
                var sound = SoundID.Item106;
                sound.MaxInstances = 2;
                SoundEngine.PlaySound(sound.WithVolumeScale(5).WithPitchOffset(-0.8f), Player.position);
                SoundEngine.PlaySound(sound.WithVolumeScale(0.5f).WithPitchOffset(0.9f), Player.position);
                Player.SetImmuneTimeForAllTypes(20);
                return true;
            }
            #endregion
            #region 太刀的
            if (InForesightSlash)
            {
                ForesightSlash_OnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }

            if (Naknotsu_Slash)
            {
                Naknotsu_Slash_OnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }
            if (SerenePose)
            {
                SerenePoseOnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                return true;
            }
            #endregion
            #region 大锤水面击
            if (WaterStrike)
            {
                WaterStrike_OnHit = true;
                Player.SetImmuneTimeForAllTypes(60);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }
            #endregion
            #region 霜拳神圣反击
            if (HolyStrikesBack)
            {
                HolyStrikesBack_OnHit = true;
                Player.SetImmuneTimeForAllTypes(60);
                return true;
            }
            #endregion
            #region 盾系列防御
            if (HeldShield != null)
            {
                if (HeldShield.InDef)
                {
                    float prosses = info.Damage / Player.statLifeMax2;
                    if(prosses < 0.1f)
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Small;
                    }
                    else if(prosses < 0.5f)
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Medium;
                    }
                    else
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Big;
                    }
                    if(info.Damage <= Player.statLifeMax2 * 0.01f)
                    {
                        return false;
                    }
                }
                HeldShield = null;
            }
            #endregion
            return base.FreeDodge(info);
        }
    }
}
