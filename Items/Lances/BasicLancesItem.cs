using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Items.Lances
{
    public abstract class BasicLancesItem : ModItem
    {
        public abstract Asset<Texture2D> ShieldTex { get; }
        public abstract Asset<Texture2D> ProjTex { get; }
        public sealed override void SetDefaults()
        {
            _ = ShieldTex;
            LancesGlobalItem.WeaponID ??= new();
            LancesGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
    }
}
