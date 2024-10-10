using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon
{
    public abstract class StarBreakerWeaponProj : ModProjectile
    {
        public Player Player;
        public int GetPlayerDoubleTapDir(int Dir)
        {
            if (Dir == 1)
                return 2; // 朝向为正-右边
            else
                return 3;
        }
        public bool GetPlayerDoubleTap(int Dir) => Player.GetModPlayer<WeaponSkillPlayer>().DashDir == Dir;
    }
}
