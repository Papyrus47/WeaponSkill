using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.BroadSword.Skills
{
    public abstract class BasicBroadSwordSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public BroadSwordProj broadSword => modProjectile as BroadSwordProj;

        protected BasicBroadSwordSkill(BroadSwordProj broadSwordProj) : base(broadSwordProj)
        {
            player = broadSwordProj.Player;
            swingHelper = broadSwordProj.SwingHelper;
        }
    }
}
