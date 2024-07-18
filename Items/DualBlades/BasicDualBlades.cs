using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.DualBlades;

namespace WeaponSkill.Items.DualBlades
{
    public abstract class BasicDualBlades : ModItem
    {
        public override void SetStaticDefaults()
        {
            DualBladesGlobalItem.WeaponID ??= new();
            DualBladesGlobalItem.WeaponID.Add(Type);
        }
        public sealed override void SetDefaults()
        {
            if(Item.TryGetGlobalItem<DualBladesGlobalItem>(out var result))
            {
                result.ProjTex = GetName() + "Proj";
            }
            InitDefault();
            Item.damage /= 2;
        }
        public string GetName() => GetType().Namespace.Replace('.', '/') + "/" + GetType().Name;
        public virtual void InitDefault() { }
    }
}
