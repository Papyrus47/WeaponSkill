using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Pickaxe;

namespace WeaponSkill.Items.Pickaxes
{
    public class CropperLevyPickaxe : ModItem
    {
        /// <summary>
        /// 时间征收
        /// </summary>
        public int TimeLevy;
        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.knockBack = 7;
            Item.Size = new(48);
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Red;

            PickaxeGlobalItem.WeaponID?.Add(Type);
        }
        public override void UpdateInventory(Player player)
        {
            player.statLifeMax2 += TimeLevy;
            if (TimeLevy > 0)
            {
                TimeLevy -= 2;
                if (TimeLevy <= 0)
                    TimeLevy -= (int)(player.statLifeMax2 / 1.1f);
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.CanBeChasedBy())
                return;
            TimeLevy += damageDone / 10;
            target.lifeMax -= damageDone / 10;

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center,target.width,target.height, DustID.YellowStarDust);
                dust.velocity = (dust.position - player.Center) * -0.06f;
            }
        }
    }
}
