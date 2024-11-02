using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Talisman
{
    public abstract class BasicTalisman : ModItem
    {
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) => incomingItem.ModItem is not BasicTalisman;
    }
}
