using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class WhetfishSabers : BasicDualBlades
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true; // All vanilla fish can be placed in a weapon rack.
        }
        public override void InitDefault()
        {
            Item.Size = new(44, 32);
            Item.damage = 24;
            Item.knockBack = 2;
            Item.crit = 40;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 90);
        }
    }
}
