using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 手枪
    /// </summary>
    public class Handgun : BasicGunsType
    {
        public int Channel;
        // 按下F进行蓄力,按X进行换弹
        public Handgun(int maxBullet = 0) : base(maxBullet)
        {
            if(maxBullet == 0)
                MaxBullet = 7;
            HasBullet = MaxBullet;
            ResetTime = 40;
        }

        public override void OnHold(Player player, Item item)
        {
            GunsGlobalItem gunsGlobalItem = item.GetGlobalItem<GunsGlobalItem>();
            if (WeaponSkill.SpKeyBind.JustPressed)
            {
                gunsGlobalItem.ResetBullet = true;
            }

            if(!gunsGlobalItem.ResetBullet && WeaponSkill.BowSlidingStep.Current)
            {
                if(Channel < 180)
                    Channel++;

                switch (Channel)
                {
                    case < 60:
                        Dust dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Blue, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        break;
                    case < 120:
                        dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Yellow, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        break;
                    case < 180:
                        dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Red, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        break;
                    case 180:
                        dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Blue, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Yellow, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Red, 1, 2);
                        dust.noGravity = true;
                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                        break;
                }
            }
            else if(gunsGlobalItem.ResetBullet)
            {
                Channel = 0;
            }

            #region Channel的限制
            if (WeaponSkill.BowSlidingStep.Current)
                return;

            switch (Channel)
            {
                case 0:
                    break;
                case < 60:
                    Channel = 0;
                    Dust dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Blue,1,2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    break;
                case < 120:
                    Channel = 60;
                    dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Yellow, 1, 2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    break;
                case < 180:
                    Channel = 120;
                    dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Red, 1, 2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    break;
                case 180:
                    dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Blue, 1, 2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Yellow, 1, 2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 5, 5, DustID.Firework_Red, 1, 2);
                    dust.noGravity = true;
                                        dust.velocity = (player.Center - Main.MouseWorld).SafeNormalize(default).RotatedByRandom(0.7) * 3;
                    break;
            }
            #endregion
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += (Channel / 60) * 0.5f;
        }
        public override void UpdateInventory(Player player, Item item)
        {
            base.UpdateInventory(player, item);
        }
    }
}
