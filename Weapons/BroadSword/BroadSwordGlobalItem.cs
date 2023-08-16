using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.BroadSword
{
    public class BroadSwordGlobalItem : BasicWeaponItem<BroadSwordGlobalItem>
    {
        public bool ProjCanShoot;
        public Texture2D DrawColorTex;
        public override void SetStaticDefaults()
        {
            WeaponID = new()
            {
                ItemID.WoodenSword,ItemID.BorealWoodSword,ItemID.CopperBroadsword,ItemID.PalmWoodSword,121,ItemID.NightsEdge,426,659,3258,675,3065,3063,1185,484,3764,3765,3766,3767,
                3768,3769,4259,482,723,2517,656,881,3502,653,4,921,3496,3514,5284,3520,5129,3772,1304,3352,3490,1909,724,46,795,1123,190,1166,5094,5097,2330,989,65,676,1199,672,3211,
                5096,368,5382,674,1227,671,1226,1826,1928,3018,ItemID.TerraBlade,2880,3484,3827
            };
        }
        public override void SetDefaults(Item entity)
        {
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.UseSound = null;
            switch (entity.type)
            {
                case 121:
                    {
                        entity.Size = new(50);
                        break;
                    }
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BroadSwordProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<BroadSwordProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type]);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if(player.HeldItem != item && DrawColorTex != null)
            {
                DrawColorTex.Dispose();
                DrawColorTex = null;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ProjCanShoot)
            {
                ProjCanShoot = false;
                return true;
            }
            return false;
        }
        public override void Unload()
        {
            WeaponID.Clear();
            WeaponID = null;
        }
        public override bool InstancePerEntity => true;
    }
}
