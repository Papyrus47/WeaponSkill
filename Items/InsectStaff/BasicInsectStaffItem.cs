using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.InsectStaff;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.Items.InsectStaff
{
    public abstract class BasicInsectStaffItem : ModItem
    {
        public sealed override void SetDefaults()
        {
            InsectStaffGlobalItem.WeaponID ??= new();
            InsectStaffGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults()
        {
        }
    }
}
