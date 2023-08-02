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
        public override void SetStaticDefaults()
        {
            LongSwordGlobalItem.WeaponID ??= new();
            LongSwordGlobalItem.WeaponID.Add(Type);
        }
    }
}
