using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Items.InsectStaff.Insects
{
    public class TestInsect : BasicInsect
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ModContent.ProjectileType<TestInsectProj>();
        }
    }
    public class TestInsectProj : InsectProj
    {
        public override string Texture => GetType().Namespace.Replace('.','/') + "/TestInsect";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.Size = new(48, 28);
        }
    }
}
