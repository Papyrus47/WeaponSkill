using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.HuntingHorn;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Items.HuntingHorn
{
    public abstract class BasicHuntingHornItem : ModItem
    {
        public HuntingHornGlobalItem globalItem
        {
            get
            {
                if (Item.TryGetGlobalItem<HuntingHornGlobalItem>(out var result))
                    return result;
                return null;
            }
        }

        public sealed override void SetDefaults()
        {
            HuntingHornGlobalItem.WeaponID ??= new();
            HuntingHornGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults()
        {
        }
    }
}
