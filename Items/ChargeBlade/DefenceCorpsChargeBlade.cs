using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Items.ChargeBlade
{
    public class DefenceCorpsChargeBlade : BasicChargeBlade
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "_Shield");
        public override void InitDefaults()
        {
            Item.Size = new(92,88);
            Item.damage = 70;
            Item.knockBack = 3.3f;
            Item.rare = ItemRarityID.LightRed;
            Item.crit = 20;
        }

        public override void SetShieldData(ref ChargeBladeProj.ShieldData shieldData)
        {
            shieldData = new()
            {
                MaxDmg = 240,
                MaxReduction = 0.3f
            };
        }
    }
}
