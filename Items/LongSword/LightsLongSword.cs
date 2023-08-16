using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Items.LongSword
{
    public class LightsLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(28, 66);
            Item.damage = 23;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 8;
            Item.rare = ItemRarityID.Green;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/LightsLongSwordScabbard");
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            float size = target.Size.Length();
            Vector2 vel = Main.rand.NextVector2Unit();
            SpurtsProj proj = SpurtsProj.NewSpurtsProj(player.GetSource_OnHit(target), target.Center - (vel * size * 3), vel, hit.Damage / 2, hit.Knockback / 5, player.whoAmI, size * 6,20, TextureAssets.Extra[192].Value);
            proj.FixedPos = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}
