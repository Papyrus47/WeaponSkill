using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.SlashAxe
{
    public class TestSlashAxe : BasicSlashAxe
    {
        public override Asset<Texture2D> SwordTex => GetSwordTex;

        public override Asset<Texture2D> AxeTex => GetAxeTex;

        public override Asset<Texture2D> DefTex => GetDefTex;
        public override void InitDefaults()
        {
            Item.damage = 12;
            Item.knockBack = 3.5f;
            Item.Size = new(72, 82);

            SwordSize = new(40, 48);
            AxeSize = new(26, 30);
        }
    }
}
