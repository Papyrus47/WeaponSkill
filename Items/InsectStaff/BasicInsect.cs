using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.InsectStaff
{
    public abstract class BasicInsect : ModItem
    {
        public ModItem Owner;
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem != Owner)
                Owner = null;
        }
    }
}
