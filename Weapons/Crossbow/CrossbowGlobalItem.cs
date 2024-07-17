using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using WeaponSkill.Weapons.Crossbow.Parts;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.Crossbow
{
    public class CrossbowGlobalItem : BasicWeaponItem<CrossbowGlobalItem>
    {
        /// <summary>
        /// 消耗子弹用的玩意
        /// </summary>
        public bool CosumeAmmo;
        public static bool ShowTheUI;
        /// <summary>
        /// 开启的物品
        /// </summary>
        public static Item OpenItem;
        public Item[] Crossbow_Parts;
        public Dictionary<Item,Ref<int>> CrossbowLoadArrow;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            WeaponID = new() { 1229,1194,578,436,481,1201,1187,435 };
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag == null)
                return;
            if(tag.TryGet<Item[]>(nameof(Crossbow_Parts),out var items))
            {
                item.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts = items;
            }
        }
        public override void SaveData(Item item, TagCompound tag)
        {
            if (tag != null && Crossbow_Parts != null)
            {
                tag.Add(nameof(Crossbow_Parts), Crossbow_Parts);
            }
        }
        public override void SetDefaults(Item entity)
        {
            Crossbow_Parts = new Item[4];
            for(int i = 0;i< Crossbow_Parts.Length;i++)
            {
                Crossbow_Parts[i] = new();
                Crossbow_Parts[i].SetDefaults(0);
            }
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Shoot;
            //entity.UseSound = null;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
            Item shootItem = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
            if (item.consumable && !player.IsAmmoFreeThisShot(player.HeldItem, item, item.shoot))
            {
                CombinedHooks.OnConsumeAmmo(player, player.HeldItem, shootItem);
                if (item.stack-- <= 0)
                {
                    item.active = false;
                    item.TurnToAir();
                }
            }
            type = shootItem.shoot;
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            return CosumeAmmo;
        }
        public override void HoldItem(Item item, Player player)
        {
            WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
            CrossbowLoadArrow ??= new();
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheRangeChangeUI = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CrossbowProj>()] <= 0) // 生成手持弹幕
            {
                for (int i = 0;i < weaponSkillPlayer.AmmoItems.Count; i++)
                {
                    CrossbowLoadArrow.TryAdd(weaponSkillPlayer.AmmoItems[i],new(10));
                }
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<CrossbowProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }

            for (int i = 0; i < weaponSkillPlayer.AmmoItems.Count; i++)
            {
                if (!CrossbowLoadArrow.Keys.Contains(weaponSkillPlayer.AmmoItems[i]))
                {
                    CrossbowLoadArrow.TryAdd(weaponSkillPlayer.AmmoItems[i], new(10));
                }
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (!Main.playerInventory)
            {
                ShowTheUI = false;
                OpenItem = null;
            }

        }
        public override bool CanRightClick(Item item)
        {
            ShowTheUI = true;
            OpenItem = item;
            return base.CanRightClick(item);
        }
    }
}
