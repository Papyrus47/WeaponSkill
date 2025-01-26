using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Items.InsectStaff.Insects
{
    public class EyeOfDark : BasicInsect
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 5;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.shoot = ModContent.ProjectileType<EyeOfDarkProj>();
        }
        public override void AddRecipes() => CreateRecipe().Register();
    }
    public class EyeOfDarkProj : InsectProj
    {
        //public override string Texture => GetType().Namespace.Replace('.', '/') + "/TestInsect";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.Size = new(48, 28);
            Main.projFrames[Type] = 2;
            Setting = new()
            {
                Speed = 25f
            };
        }
        public override void AI()
        {
            base.AI();
            Projectile.spriteDirection = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.frameCounter++ > 6)
            {
                Projectile.frameCounter = 0;
                if(Projectile.frame++ >= 1)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}
