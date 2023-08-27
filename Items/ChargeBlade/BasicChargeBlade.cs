using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Items.ChargeBlade
{
    public abstract class BasicChargeBlade : ModItem
    {
        public abstract Asset<Texture2D> ShieldTex { get; }
        public abstract void SetShieldData(ref ChargeBladeProj.ShieldData shieldData);
        public sealed override void SetDefaults()
        {
            _ = ShieldTex;
            ChargeBladeGlobalItem.WeaponID ??= new();
            ChargeBladeGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
    }
}
