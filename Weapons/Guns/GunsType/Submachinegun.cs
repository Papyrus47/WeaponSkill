using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 冲锋枪 或者 步枪
    /// </summary>
    public class Submachinegun : BasicGunsType
    {
        public Submachinegun(int maxBullet = 0) : base(maxBullet)
        {
            if (maxBullet == 0)
                MaxBullet = 25;
            HasBullet = MaxBullet;
            ResetTime = 35;
        }
        /// <summary>
        /// 技能CD
        /// </summary>
        public int SkillCD;
        // 按X换弹,按F进行高速射击(CD:5s)
        public override void OnHold(Player player, Item item)
        {
            GunsGlobalItem gunsGlobalItem = item.GetGlobalItem<GunsGlobalItem>();
            if (WeaponSkill.SpKeyBind.Current)
            {
                gunsGlobalItem.ResetBullet = true;
            }

            if(SkillCD == 0 && WeaponSkill.BowSlidingStep.Current)
            {
                if (HasBullet == 0)
                {
                    HasBullet = MaxBullet;
                    gunsGlobalItem.ResetBullet = false;
                }
                player.itemAnimation = player.itemTime = 2;
                TheUtility.Player_ItemCheck_Shoot(player, item, player.GetWeaponDamage(item));
                if(HasBullet <= 0)
                {
                    SkillCD = 300;
                }
            }
            for (int i = 0; i < 30; i++)
            {
                if (SkillCD <= i * 10)
                    continue;
                Dust dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 1, 1, DustID.GoldFlame);
                dust.noGravity = true;
                dust.velocity *= 0;
                dust.velocity += player.velocity;
                dust.position += Vector2.UnitX.RotatedBy(i / 30f * MathHelper.TwoPi) * 50;
            }
        }
        public override void UpdateInventory(Player player, Item item)
        {
            if (SkillCD > 0)
            {
                SkillCD--;
            }
        }
    }
}
