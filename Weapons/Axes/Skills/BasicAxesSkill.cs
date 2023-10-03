using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.Axes.Skills
{
    public abstract class BasicAxesSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public AxesProj AxesProj => modProjectile as AxesProj;

        protected BasicAxesSkill(AxesProj axeProj) : base(axeProj)
        {
            player = axeProj.Player;
            swingHelper = axeProj.SwingHelper;
        }
    }
}
