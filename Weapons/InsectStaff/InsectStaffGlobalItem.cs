using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff
{
    public class InsectStaffGlobalItem : BasicWeaponItem<InsectStaffGlobalItem>
    {
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.SummonMeleeSpeed;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.useTurn = false;
            entity.useAnimation = entity.useTime = 10;
            entity.noMelee = true;
        }
    }
}
