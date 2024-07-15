using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFist_Proj_FistHitProj_SkyFallFist : FrostFist_Proj_FistHitProj
    {
        public override string Texture => (GetType().Namespace + "/FrostFist_Proj_FistHitProj").Replace('.', '/');
        public Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            if(frostFist_FistHit is FrostFist_FistHit_SkyFallFist fistHit && fistHit.GetPlayerStandTile())
            {
                Projectile.Kill();
                return;
            }
            Projectile.extraUpdates = 0;
            Projectile.width = 60;
            if (!FistHitProjs.Contains(Projectile.whoAmI)) FistHitProjs.Add(Projectile.whoAmI);
            if(Projectile.ai[0] < 90) Projectile.ai[0] += MathF.Pow(Projectile.timeLeft / 30f, 2);
            if (Projectile.velocity.LengthSquared() < 40) Projectile.velocity *= 1.01f;
            player.Center = Projectile.Center;
            Projectile.Center = player.Center + Projectile.velocity.SafeNormalize(default) * Projectile.ai[0];
            if (Projectile.timeLeft < 10) Projectile.timeLeft = 10;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue, 1.6f);
                dust.position = Projectile.Center;
                //dust.velocity *= 0;
                dust.noGravity = true;
                dust.fadeIn = 1;
            }
        }
    }
}
