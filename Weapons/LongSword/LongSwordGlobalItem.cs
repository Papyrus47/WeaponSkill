using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.LongSword
{
    public class LongSwordGlobalItem : BasicWeaponItem<LongSwordGlobalItem>
    {
        public override bool InstancePerEntity => true;
        public static bool ShowTheSpirit;
        public int SpiritMax = 500;
        public int Spirit;
        public byte SpiritLevel,OldSpiritLevel;
        public byte Time;
        public Asset<Texture2D> ScabbardTex;
        public List<Action<LongSwordScabbard>> ScabbardAction = new();
        public override void SetStaticDefaults()
        {
            WeaponID ??= new();
            WeaponID.Add(ItemID.Katana);
            WeaponID.Add(ItemID.Muramasa);
            WeaponID.Add(ItemID.CobaltSword);
            WeaponID.Add(ItemID.OrichalcumSword);
        }
        public override void SetDefaults(Item entity)
        {
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.UseSound = null;
            entity.useTurn = false;
            SpiritMax = 150;
            switch (entity.type)
            {
                case ItemID.Katana:
                case ItemID.Muramasa:
                    {
                        entity.scale = 1.8f;
                        break;
                    }
                case ItemID.CobaltSword:
                case ItemID.OrichalcumSword:
                    {
                        entity.scale = 1.95f;
                        break;
                    }
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LongSwordProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<LongSwordProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
            ShowTheSpirit = true;
            if (Spirit < 0) Spirit = 0;
            if (OldSpiritLevel != SpiritLevel)
            {
                Time = 255;
            }
            if(SpiritLevel > 0 && (int)Main.GlobalTimeWrappedHourly % 20 == 0 && Time-- <= 0)
            {
                SpiritLevel--;
            }
            if (SpiritLevel > 3) SpiritLevel = 3;
            OldSpiritLevel = SpiritLevel;
        }
    }
}
