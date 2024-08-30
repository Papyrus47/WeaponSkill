using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons
{
    public abstract class BasicWeaponItem<T> : GlobalItem where T : BasicWeaponItem<T>
    {
        public override bool InstancePerEntity => true;
        public static HashSet<int> WeaponID;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if (WeaponID == null) 
                return false;
            return lateInstantiation && WeaponID.Contains(entity.type);
        }
        public override void Unload()
        {
            WeaponID = null;
        }
    }
}
