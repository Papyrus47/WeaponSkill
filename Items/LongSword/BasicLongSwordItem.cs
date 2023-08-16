using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Items.LongSword
{
    public abstract class BasicLongSwordItem : ModItem
    {
        public LongSwordProj GetLongSwordProj(Player player)
        {
            if(player.heldProj >= 0 && Main.projectile[player.heldProj].ModProjectile is LongSwordProj longSwordProj)
                return longSwordProj;
            return null;
        }
        public override void SetStaticDefaults()
        {
            LongSwordGlobalItem.WeaponID ??= new();
            LongSwordGlobalItem.WeaponID.Add(Type);
        }
    }
}
