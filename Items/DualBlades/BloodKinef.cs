using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace WeaponSkill.Items.DualBlades
{
    public class BloodKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(50);
            Item.damage = 22;
            Item.knockBack = 2;
            Item.crit = 7;
            Item.rare = ItemRarityID.Blue;
            Item.scale = 0.7f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 15).AddTile(TileID.Anvils).Register();
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.CanBeChasedBy() || player.ownedProjectileCounts[975] > 3)
                return;
            //target.AddBuff(344, 5);
            Vector2 v = target.Center - player.MountedCenter;
            v = v.SafeNormalize(Vector2.Zero);
            Vector2 vector = target.Hitbox.ClosestPointInRect(player.MountedCenter) + v;
            Vector2 spinningpoint = (target.Center - vector) * 0.8f;
            spinningpoint = spinningpoint.RotatedBy(Main.rand.NextFloatDirection() * (float)Math.PI * 0.25f);
            int num = Projectile.NewProjectile(player.GetSource_OnHit(target), vector.X, vector.Y, spinningpoint.X, spinningpoint.Y, 975, (int)damageDone, hit.Knockback, player.whoAmI, 1f, target.whoAmI);
            Main.projectile[num].StatusNPC(target.whoAmI);
        }
    }
}
