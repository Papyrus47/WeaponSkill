using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.WorldGen;

namespace WeaponSkill.Items.GunLances
{
    public class CopperGunLances : BasicGunLancesItem
    {
        public override Asset<Texture2D> ShieldTex => GetShieldTex;

        public override Asset<Texture2D> ProjTex1 => GetProjTex1;

        public override Asset<Texture2D> ProjTex2 => GetProjTex2;
        public override void InitDefaults()
        {
            Item.damage = 8;
            Item.knockBack = 3.5f;
            Item.Size = new(90,82);

            Proj1Size = new(40);
            Proj2Size = new(58,50);
        }
    }
}
