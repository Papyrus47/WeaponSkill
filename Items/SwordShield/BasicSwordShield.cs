using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.SwordShield;

namespace WeaponSkill.Items.SwordShield
{
    public abstract class BasicSwordShield : ModItem
    {
        public abstract Asset<Texture2D> ShieldTex { get; }
        public Asset<Texture2D> GetShieldTex => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Shield");
        public sealed override void SetDefaults()
        {
            _ = ShieldTex;
            SwordShieldGlobalItem.WeaponID ??= new();
            SwordShieldGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
    }
}
