using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.LongSword;

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
        public static DualBladesProj GetDualBlades(Player player,bool isForeach = false)
        {
            if (isForeach)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if(projectile.ModProjectile is DualBladesProj bladesProj && projectile.active && projectile.owner == player.whoAmI)
                        return bladesProj;
                }
            }
            if (player.heldProj >= 0 && Main.projectile[player.heldProj].ModProjectile is DualBladesProj proj)
                return proj;
            return null;
        }
    }
}
