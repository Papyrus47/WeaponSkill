using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class LightsKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 23;
            Item.knockBack = 1.8f;
            Item.crit = 15;
            Item.rare = ItemRarityID.Blue;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.DemoniteBar, 15).AddTile(TileID.Anvils).Register();
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Unit(), 974, damageDone / 3, hit.Knockback, player.whoAmI,target.height / 50f);
        }
    }
}
