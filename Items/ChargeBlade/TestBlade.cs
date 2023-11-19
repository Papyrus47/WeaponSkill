using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Items.ChargeBlade
{
    public class TestBlade : BasicChargeBlade
    {
        public override Asset<Texture2D> ShieldTex => TextureAssets.Item[3097];
        public override void InitDefaults()
        {
            Item.Size = new(56);
            Item.damage = 25;
            Item.knockBack = 0.3f;
            Item.rare = ItemRarityID.Blue;
            Item.crit = 5;
            Main.instance.LoadItem(3097);
        }

        public override void SetShieldData(ref ChargeBladeProj.ShieldData shieldData)
        {
            shieldData = new()
            {
                Def = 40
            };
        }
    }
}
