using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class CADKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(44,32);
            Item.damage = 20;
            Item.knockBack = 2;
            Item.crit = 7;
            Item.scale = 0.7f;
        }
    }
}
