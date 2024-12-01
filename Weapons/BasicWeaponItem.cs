using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Configs;

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
            if(this is IVanillaWeapon && entity.ModItem == null)
                return lateInstantiation && WeaponID.Contains(entity.type) && NormalConfig.Init.UseWeaponSkill;
            
            return lateInstantiation && WeaponID.Contains(entity.type);
        }
        public override void Unload()
        {
            WeaponID = null;
        }
        public virtual void SetWeaponID(params int[] weaponIDs)
        {
            WeaponID ??= new();
            WeaponID.TryAdd(weaponIDs);
        }
    }
}
