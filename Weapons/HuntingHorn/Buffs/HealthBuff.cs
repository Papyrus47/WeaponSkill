using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.HuntingHorn.Buffs
{
    public class HealthBuff_Small : HuntingHornBuff
    {
        public HealthBuff_Small(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            player.Heal(10);
        }
        public override void Register()
        {
        }
    }
    public class HealthBuff_Middle : HuntingHornBuff
    {
        public HealthBuff_Middle(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            player.Heal(40);
        }
        public override void Register()
        {
        }
    }
    public class HealthBuff_Big : HuntingHornBuff
    {
        public HealthBuff_Big(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            player.Heal(80);
        }

        public override void Register()
        {
        }
    }
}
