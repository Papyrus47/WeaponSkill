using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SlashAxe
{
    public class SlashAxeGlobalItem : BasicWeaponItem<SlashAxeGlobalItem>
    {
        /// <summary>
        /// 斩斧的觉醒条
        /// </summary>
        public int Power;
        /// <summary>
        /// 斩斧的觉醒条上限
        /// </summary>
        public int PowerMax = 300;
        /// <summary>
        /// 斩击条
        /// </summary>
        public int Slash;
        /// <summary>
        /// 斩击条上限
        /// </summary>
        public int SlashMax = 300;
        /// <summary>
        /// 斧强化
        /// </summary>
        public int AxeStrength;
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.noUseGraphic = true;
            entity.noMelee = true;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            Power = Math.Clamp(Power, 0, PowerMax);
            Slash = Math.Clamp(Slash, 0, SlashMax);
        }
    }
}
