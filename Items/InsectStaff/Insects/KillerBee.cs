using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Items.InsectStaff.Insects
{
    public class KillerBee : BasicInsect
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.shoot = ModContent.ProjectileType<KillerBeeProj>();
        }
        public override void AddRecipes() => CreateRecipe().Register();
    }
    public class KillerBeeProj : InsectProj
    {
        public override string Texture => GetType().Namespace.Replace('.', '/') + "/KillerBee";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.Size = new(48, 28);
            Main.projFrames[Type] = 4;
            Setting = new()
            {
                Speed = 6f
            };
        }
        public override void AI()
        {
            base.AI();
            //Projectile.spriteDirection = 1;
            Projectile.rotation = Projectile.velocity.X * 0.1f;
            if (Projectile.frameCounter++ > 6)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= Main.projFrames[Type] - 1)
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Poisoned,60);
        }
    }
}
