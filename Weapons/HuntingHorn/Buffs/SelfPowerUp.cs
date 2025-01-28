using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Buffs.HuntingHornBuffs;

namespace WeaponSkill.Weapons.HuntingHorn.Buffs
{
    public class SelfPowerUp : HuntingHornBuff
    {
        public SelfPowerUp(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            player.AddBuff(ModContent.BuffType<SelfPowerUpBuff>(), 3600 * 5);
        }
        public override void Register()
        {
        }
    }
}
