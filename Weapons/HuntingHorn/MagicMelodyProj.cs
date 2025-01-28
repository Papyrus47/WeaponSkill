using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.HuntingHorn
{
    public class MagicMelodyProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 1800;
            Projectile.Size = new(78 / 3, 46);
            Main.projFrames[Type] = 3;
            Projectile.frame = Main.rand.Next(3);
        }
        public override Color? GetAlpha(Color lightColor) => Color.SkyBlue with { A = 50 };
        public override void AI()
        {
            NPC target = Projectile.FindTargetWithinRange(1000);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = MathHelper.PiOver2 * -Projectile.spriteDirection;
            if(target != null)
            {
                Projectile.velocity = (Projectile.velocity * 10 + (target.Center - Projectile.Center) * 0.1f) / 11f;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }
        }
    }
}
