using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;
using Terraria.ID;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace WeaponSkill.Items.LongSword
{
    public class HarnessingWindLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(48, 98);
            Item.damage = 34;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 10;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/HarnessingWindLongSwordScabbard");
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var proj = Projectile.NewProjectileDirect(source, position, (Main.MouseWorld - position).SafeNormalize(default) * 16, ProjectileID.HarpyFeather, damage / 2, knockback, player.whoAmI);
            proj.friendly = true;
            proj.hostile = false;
            proj.penetrate = 5;
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_OnHit(target), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(default).RotatedBy(MathHelper.PiOver4 * 0.2f * i) * 16, ProjectileID.HarpyFeather, damageDone / 2,hit.Knockback, player.whoAmI);
                proj.friendly = true;
                proj.hostile = false;
                proj.penetrate = 1;
                proj.usesLocalNPCImmunity = true;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.GiantHarpyFeather, 1).AddIngredient(ItemID.Feather,15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
