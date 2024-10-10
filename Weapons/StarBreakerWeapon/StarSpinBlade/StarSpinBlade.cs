using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class StarSpinBlade : ModItem, StarBreakerMoreItemPart
    {
        public LocalizedText PartText => Language.GetOrRegister("Mods.WeaponSkill.Items.StarSpinBlade.SPText");

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 0;
            Item.useTime = Item.useAnimation = 1;
            Item.Size = new(32, 54);
            Item.rare = ModContent.RarityType<SSB_Rarity>();
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<FrostFistProj>();
            Item.knockBack = 9999999999;
            Item.noMelee = true;
            Item.value = 15234960;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; ++i)
            {
                if (tooltips[i].Name == "Speed")
                {
                    tooltips[i].Text = "???速度";
                    tooltips[i].OverrideColor = Color.Purple;
                }
            }
        }
    }
}
