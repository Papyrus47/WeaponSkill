using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit_MoveFist : FrostFist_FistHit
    {
        public FrostFist_FistHit_MoveFist(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
        }
        public Action<Projectile> FistMoveAI;
        public override void AI()
        {
            Projectile.Center = Player.Center;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Player.velocity.X *= 0.2f;
            Player.velocity.Y = 0;
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            Projectile.direction = Player.direction;
            Player.itemTime = Player.itemAnimation = 2;
            Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction, vel.X * Projectile.direction);
            if (Projectile.ai[0]++ == 0)
            {
                Vector2 shootVel = vel.SafeNormalize(default) * 6;
                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj_MoveFist>(), (int)(Projectile.damage * ActionDamage), Projectile.knockBack, Player.whoAmI);
                proj.alpha = 0;
                FistHitProjChange?.Invoke(proj);
                (proj.ModProjectile as FrostFist_Proj_FistHitProj_MoveFist).MoveFistAI = FistMoveAI;
                //SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, shootVel, Projectile.damage, Projectile.knockBack, Player.whoAmI, 60, 200);

            }
            else if (Projectile.ai[0] < 7)
            {
                PreAtk = true;
            }
            else
            {
                PreAtk = false;
            }
            if (Projectile.ai[0] > 50)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > 30)
            {
                CanChangeToStopActionSkill = true;
            }
            ExtraAction?.Invoke();
        }
        public override bool SwitchCondition() => Projectile.ai[0] > 15;
    }
}
