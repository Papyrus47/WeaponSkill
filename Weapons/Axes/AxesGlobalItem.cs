using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.Pickaxe;

namespace WeaponSkill.Weapons.Axes
{
    public class AxesGlobalItem : BasicWeaponItem<AxesGlobalItem>, IVanillaWeapon
    {
        public Texture2D DrawColorTex;
        public Texture2D DrawSwingColorTex;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            WeaponID = new()
            {
                3506,3500,10,3494,3512,3488,3518,3482,991,45,799,1222,992,1223,993,1224,1223,217,4317,1507,3522,3523,3524,3525,1305,1233,990
            };
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.TryGetGlobalItem<PickaxeGlobalItem>(out var pickaxeGlobalItem) && pickaxeGlobalItem.SP_PickaxeMode == 0)
            {
                return;
            }
            base.SetDefaults(entity);
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.UseSound = null;
            entity.useTurn = false;
            switch(entity.type)
            {
                case 992:
                case 1222:
                case 993:
                case 1224:
                case 991:
                case 1223:
                    {
                        entity.scale = 2f;
                        break;
                    }
                case 1233:
                    {
                        entity.scale = 3f;
                        break;
                    }
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if(item.TryGetGlobalItem<PickaxeGlobalItem>(out var pickaxeGlobalItem) && pickaxeGlobalItem.SP_PickaxeMode == 0)
            {
                return;
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AxesProj>()] <= 0) // 生成手持弹幕
            {
                item.autoReuse = false;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.useStyle = ItemUseStyleID.Rapier;
                item.UseSound = null;
                item.useTurn = false;
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<AxesProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                DrawSwingColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type],0.6f,2.5f);
                TheUtility.GetWeaponDrawColor(DrawSwingColorTex, TextureAssets.Item[item.type], 0.95f, 1.5f);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.HeldItem != item && DrawColorTex != null)
            {
                DrawColorTex.Dispose();
                DrawColorTex = null;
                DrawSwingColorTex.Dispose();
                DrawSwingColorTex = null;
            }
        }
    }
}
