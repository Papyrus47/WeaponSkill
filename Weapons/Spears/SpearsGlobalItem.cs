using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using WeaponSkill.Weapons.Shortsword;
using Terraria.ID;

namespace WeaponSkill.Weapons.Spears
{
    public class SpearsGlobalItem : BasicWeaponItem<SpearsGlobalItem>, IVanillaWeapon
    {
        public Texture2D DrawColorTex;
        public static HashSet<int> WeaponID_SP;
        public override bool InstancePerEntity => true;
        public bool CanShootProj;
        public bool SPItem;
        public override void SetStaticDefaults()
        {
            WeaponID = new()
            {
                280,277,4061,802,2332,274,537,1186,390,1193,406,1200,550,3836,1228,5011,756,2331,1947,3543
            };
            WeaponID_SP = new()
            {
                3543
            };
        }
        public override void Unload()
        {
            base.Unload();
            WeaponID_SP = null;
        }
        public override void SetDefaults(Item entity)
        {
            if (!WeaponID_SP.Contains(entity.type))
            {
                entity.autoReuse = false;
                entity.noUseGraphic = true;
                entity.noMelee = true;
                entity.useStyle = ItemUseStyleID.Shoot;
                entity.UseSound = null;
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (WeaponID_SP.Contains(item.type))
            {
                tooltips.Add(new(Mod, "Spears_SPText", string.Format(Language.GetTextValue("Mods.WeaponSkill.Spears.SP_Text"), WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault())));
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            #region SP特判
            if (WeaponSkill.RangeChange.JustPressed)
            {
                SPItem = !SPItem;
                if (!SPItem)
                {
                    item.noUseGraphic = true;
                    item.noMelee = true;
                    item.useStyle = ItemUseStyleID.Shoot;
                    item.UseSound = null;
                }
                else
                {
                    Item oldItem = new Item(item.type);
                    item.noUseGraphic = oldItem.noUseGraphic;
                    item.noMelee = oldItem.noMelee;
                    item.useStyle = oldItem.useStyle;
                    item.UseSound = oldItem.UseSound;
                }
            }
            #endregion
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SpearsProj>()] <= 0 && !SPItem) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<SpearsProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type], 0.9f, 2.2f);
                if(item.type == ItemID.DayBreak)
                {
                    (Main.projectile[proj].ModProjectile as SpearsProj).RotCorrect = -MathHelper.PiOver4;
                }
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.HeldItem != item && DrawColorTex != null)
            {
                DrawColorTex.Dispose();
                DrawColorTex = null;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CanShootProj || SPItem)
            {
                CanShootProj = false;
                return true;
            }
            return false;
        }
    }
}
