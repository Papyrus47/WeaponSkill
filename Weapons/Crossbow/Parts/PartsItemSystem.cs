using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Crossbow.Parts
{
    public class PartsItemSystem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public delegate void UpdateParts(Player player);
        public UpdateParts updateParts;
        public bool IsPartsItem = true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => lateInstantiation && IsPartsItem;
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
        }
    }
}
