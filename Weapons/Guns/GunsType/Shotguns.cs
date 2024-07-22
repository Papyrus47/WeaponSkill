using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 散弹枪
    /// </summary>
    public class Shotguns : BasicGunsType
    {
        // X进行翻滚换弹(上一发),F进行宣泄(发射所有子弹)
        // 正常装填只能装填一发,直到装满为止
        // 特殊散弹枪可以装满

        /// <summary>
        /// 特殊散弹种类
        /// </summary>
        public bool SPShotgun;
        /// <summary>
        /// 翻滚CD
        /// </summary>
        public int RollingCD;
        /// <summary>
        /// 翻滚时间
        /// </summary>
        public int RollingTime;
        public byte RollingDir;
        public Shotguns(int maxBullet = 0) : base(maxBullet)
        {
            if (maxBullet == 0)
                MaxBullet = 7;
            HasBullet = MaxBullet;
            ResetTime = 15;
        }
        public override void OnHold(Player player, Item item)
        {
            GunsGlobalItem gunsGlobalItem = item.GetGlobalItem<GunsGlobalItem>();
            if (WeaponSkill.SpKeyBind.Current && HasBullet < MaxBullet && RollingCD <= 0)
            {
                gunsGlobalItem.ResetBullet = true;
                RollingCD = ResetTime;
                RollingTime = ResetTime;
                if (player.direction == 1)
                    RollingDir = 3;
                else
                    RollingDir = 1;
            }

            if (WeaponSkill.BowSlidingStep.JustPressed)
            {
                if (HasBullet == 0)
                    return;
                player.itemAnimation = player.itemTime = 60;
                SoundEngine.PlaySound(item.UseSound,player.position);
                for (int i = 0; i < HasBullet; i++)
                {
                    TheUtility.Player_ItemCheck_Shoot(player, item, player.GetWeaponDamage(item));
                }
                HasBullet = 0;
            }
        }
        public override void UpdateInventory(Player player, Item item)
        {
            if(RollingTime == 0)
            {
                if (RollingCD > 0)
                    RollingCD--;
            }
            else
            {
                RollingTime--;
                player.SetImmuneTimeForAllTypes(8);
                player.immuneAlpha = 0;
                player.ChangeDir(RollingDir - 2);
                player.velocity.X = (RollingDir - 2) * 15f;
                player.itemTime = player.itemAnimation = 2;
                player.fullRotation = MathHelper.SmoothStep(0, MathHelper.TwoPi * -player.direction, (float)RollingTime / ResetTime);
                player.fullRotationOrigin = player.Size * 0.5f;
            }
        }
        public override void OnResetBullet(Player player, Item item)
        {
            if (player.itemAnimation == 0 || player.itemTime == 0)
                player.itemAnimation = player.itemTime = ResetTime;
            else if (player.itemAnimation == 2 || player.itemTime == 2)
            {
                item.GetGlobalItem<GunsGlobalItem>().ResetBullet = false;
                if (++HasBullet < MaxBullet)
                {
                    SoundEngine.PlaySound(ResetSound, player.position);
                    if (SPShotgun)
                        HasBullet = MaxBullet;
                    else if(RollingCD <= 0)
                    {
                        item.GetGlobalItem<GunsGlobalItem>().ResetBullet = true;
                        player.itemAnimation = player.itemTime = ResetTime; // 继续上弹
                    }
                }
                Item item1 = new Item(item.type);
                item1.SetDefaults(item.type);
                item.UseSound = item1.UseSound;
            }
            //else if (player.itemAnimation == ResetTime / 2 || player.itemTime == ResetTime / 2)
        }
    }
}
