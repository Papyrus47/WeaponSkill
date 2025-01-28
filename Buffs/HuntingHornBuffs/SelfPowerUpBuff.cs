using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Buffs.HuntingHornBuffs
{
    public class SelfPowerUpBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.3f;
            player.statDefense.AdditiveBonus += 0.2f;
        }
    }
}
