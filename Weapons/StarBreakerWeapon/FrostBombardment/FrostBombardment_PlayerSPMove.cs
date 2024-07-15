using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment
{
    public class FrostBombardment_PlayerSPMove : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_961";
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.Size = new(1);
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 360;
            Main.projFrames[Type] = 5;
            Projectile.frame = Main.rand.Next(5);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool ShouldUpdatePosition() => false;
    }
}
