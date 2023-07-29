using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
namespace WeaponSkill.Weapons.Shortsword.Skills
{
    public abstract class BasicShortswordSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public ShortswordProj Shortsword => modProjectile as ShortswordProj;

        protected BasicShortswordSkill(ShortswordProj ShortswordProj) : base(ShortswordProj)
        {
            player = ShortswordProj.Player;
            swingHelper = ShortswordProj.SwingHelper;
        }
    }
}
