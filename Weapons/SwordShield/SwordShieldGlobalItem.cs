using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SwordShield
{
    public class SwordShieldGlobalItem : BasicWeaponItem<SwordShieldGlobalItem>
    {
        public override void SetStaticDefaults()
        {
            WeaponID ??= new();
        }
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useTime = entity.useAnimation = 10;
            entity.noMelee = true;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
        }
        public override void HoldItem(Item item, Player player)
        {
            base.HoldItem(item, player);
        }
    }
}
