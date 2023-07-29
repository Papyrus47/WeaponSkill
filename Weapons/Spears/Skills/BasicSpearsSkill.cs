using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.Spears.Skills
{
    public class BasicSpearsSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public SpearsProj SpearsProj => modProjectile as SpearsProj;

        protected BasicSpearsSkill(SpearsProj SpearsProj) : base(SpearsProj)
        {
            player = SpearsProj.Player;
            swingHelper = SpearsProj.SwingHelper;
        }
    }
}