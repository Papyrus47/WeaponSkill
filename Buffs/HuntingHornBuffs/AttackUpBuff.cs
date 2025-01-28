using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Buffs.HuntingHornBuffs
{
    public class AttackUp_SmallBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            if(player.HasBuff<AttackUp_MiddleBuff>() || player.HasBuff<AttackUp_BigBuff>() || player.HasBuff<AttackUp_SuperBuff>())
            {
                player.DelBuff(buffIndex);
            }
        }
    }
    public class AttackUp_MiddleBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            if (player.HasBuff<AttackUp_BigBuff>() || player.HasBuff<AttackUp_SuperBuff>())
            {
                player.DelBuff(buffIndex);
            }
        }
    }
    public class AttackUp_BigBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            if (player.HasBuff<AttackUp_SuperBuff>())
            {
                player.DelBuff(buffIndex);
            }
        }
    }
    public class AttackUp_SuperBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
        }
    }
}
