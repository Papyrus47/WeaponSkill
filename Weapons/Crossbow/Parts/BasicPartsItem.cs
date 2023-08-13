using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Crossbow.Parts
{
    public class BasicPartsItem : GlobalItem
    {
        public delegate void UpdateParts(Player player);
        public UpdateParts updateParts;
        public bool IsPartsItem;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => lateInstantiation && IsPartsItem;
    }
}
