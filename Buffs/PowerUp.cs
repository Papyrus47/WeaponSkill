using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Buffs
{
    public class PowerUp : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetDamage(DamageClass.Generic) *= 1.15f;
        }
    }
}
