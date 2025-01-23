using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public abstract class BasicDualBladesSkill : ProjSkill_Instantiation
    {
        public Player player;
        public string ID;
        public Action<BasicDualBladesSkill> AIAction;
        public DualBladesProj bladesProj => modProjectile as DualBladesProj;
        public DualBladesProj.DualBlades HeldBlade => bladesProj.HeldBlades;
        public DualBladesProj.DualBlades BackBlade => bladesProj.BackBlades;
        public DualBladesGlobalItem bladesGlobalItem => bladesProj.SpawnItem.GetGlobalItem<DualBladesGlobalItem>();
        /// <summary>
        /// 命中加成修正
        /// </summary>
        public float AddCorrection => bladesGlobalItem.AddCorrection;
        public BasicDualBladesSkill(DualBladesProj dualBladesProj) : base(dualBladesProj) 
        {
            player = dualBladesProj.Player;
        }
        public virtual void BackDraw(SpriteBatch spriteBatch,Color color) { }
    }
}
