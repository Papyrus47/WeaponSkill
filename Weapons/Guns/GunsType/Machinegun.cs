using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 机枪
    /// </summary>
    public class Machinegun : BasicGunsType
    {
        // 按X换弹
        // 按F切换扫射模式和普通模式,扫射模式下伤害为250%,发射时间*50%
        public bool InNormal = true;
        public Machinegun(int maxBullet = 0) : base(maxBullet)
        {
            if (maxBullet == 0)
                MaxBullet = 60;
            HasBullet = MaxBullet;
            ResetTime = 90;
        }
        public override SoundStyle ResetSound => base.ResetSound with {  Pitch = -0.5f };
        public override void OnHold(Player player, Item item)
        {
            GunsGlobalItem gunsGlobalItem = item.GetGlobalItem<GunsGlobalItem>();
            if (WeaponSkill.SpKeyBind.Current)
                gunsGlobalItem.ResetBullet = true;
            

            if (WeaponSkill.BowSlidingStep.JustPressed)
                InNormal = !InNormal;

            if (!InNormal)
            {
                if(player.itemAnimation > 0 && player.itemTime > 0)
                {
                    player.itemAnimation--;
                    player.itemTime--;
                }
                player.velocity.X *= 0.95f;
            }

            if (gunsGlobalItem.ResetBullet)
                InNormal = true;
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (!InNormal)
            {
                damage += 2.5f;
            }
        }
    }
}
