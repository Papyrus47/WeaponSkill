using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Items.LongSword
{
    public class BloodLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(46,92);
            Item.damage = 23;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 8;
            Item.rare = ItemRarityID.Blue;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/BloodLongSwordScabbard");
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 10).AddTile(TileID.Anvils).Register();
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
