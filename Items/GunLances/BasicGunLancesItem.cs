using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.GunLances;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Items.GunLances
{
    public abstract class BasicGunLancesItem : ModItem
    {
        public abstract Asset<Texture2D> ShieldTex { get; }
        public abstract Asset<Texture2D> ProjTex1 { get; }
        public abstract Asset<Texture2D> ProjTex2 { get; }
        public Asset<Texture2D> GetShieldTex => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Shield");
        public Asset<Texture2D> GetProjTex1 => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Proj1");
        public Asset<Texture2D> GetProjTex2 => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Proj2");
        public Vector2 Proj1Size;
        public Vector2 Proj2Size;
        public sealed override void SetDefaults()
        {
            _ = ShieldTex;
            _ = ProjTex1;
            _ = ProjTex2;
            GunLancesGlobalItem.WeaponID ??= new();
            GunLancesGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
    }
}
