using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Buffs.HuntingHornBuffs;

namespace WeaponSkill.Weapons.HuntingHorn.Buffs
{
    public class AttackUp_Small : HuntingHornBuff
    {
        public AttackUp_Small(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }
        public override void OnPlay(Player player, Projectile projectile)
        {
            if(!player.HasBuff<AttackUp_SmallBuff>())
            {
                player.AddBuff(ModContent.BuffType<AttackUp_SmallBuff>(), 3600 * 3);
            }
            else
            {
                player.AddBuff(ModContent.BuffType<AttackUp_MiddleBuff>(), 3600 * 3);
            }

        }
        public override void Register()
        {
        }
    }
}
