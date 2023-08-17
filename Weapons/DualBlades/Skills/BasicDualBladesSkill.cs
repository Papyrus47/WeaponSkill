using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public abstract class BasicDualBladesSkill : ProjSkill_Instantiation
    {
        public Player player;
        public DualBladesProj bladesProj => modProjectile as DualBladesProj;
        public DualBladesGlobalItem bladesGlobalItem => bladesProj.SpawnItem.GetGlobalItem<DualBladesGlobalItem>();
        /// <summary>
        /// 命中加成修正
        /// </summary>
        public float AddCorrection => bladesGlobalItem.AddCorrection;
        public BasicDualBladesSkill(DualBladesProj dualBladesProj) : base(dualBladesProj) 
        {
            player = dualBladesProj.Player;
        }
    }
}
