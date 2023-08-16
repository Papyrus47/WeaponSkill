using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;
using Terraria.ID;
using WeaponSkill.Weapons.LongSword.Skills;

namespace WeaponSkill.Items.LongSword
{
    public class TarantulaLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(40, 86);
            Item.damage = 41;
            Item.knockBack = 1;
            Item.rare = ItemRarityID.LightPurple;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 24;
            Item.scale = 1f;
            if(Item.TryGetGlobalItem<LongSwordGlobalItem>(out var globalItem))
            {
                globalItem.ScabbardAction.Add(DrawScabbard);
            }
        }

        public void DrawScabbard(LongSwordScabbard scabbard)
        {
            scabbard.DrawOrigin.X -= 2 * scabbard.Dir;
            scabbard.DrawOrigin.Y -= 6;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/TarantulaLongSwordScabbard");
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 120);
            var longSword = GetLongSwordProj(player);
            if (longSword != null && (longSword.CurrentSkill is LongSword_SakuraSlashed || longSword.CurrentSkill is LongSword_Naknotsu_RotSlash))
            {
                for (int i = 0; i < 2; i++)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center, (-Vector2.UnitY).RotatedByRandom(0.5) * Main.rand.NextFloat(1, 2) * 6, 378, hit.SourceDamage / 3, hit.Knockback, player.whoAmI);
                    Main.projectile[proj].penetrate = -1;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = -1;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SpiderFang, 12).AddTile(TileID.Anvils).Register();
        }
    }
}
