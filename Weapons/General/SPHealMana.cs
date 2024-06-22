using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.General
{
    /// <summary>
    /// 特殊恢复魔力的判定,ModItem继承
    /// </summary>
    public interface SPHealMana
    {
        public void HealMana(Player player)
        {
            player.manaRegenBonus = -(int)(player.statManaMax2 / 1.6f);
        }
    }
}
