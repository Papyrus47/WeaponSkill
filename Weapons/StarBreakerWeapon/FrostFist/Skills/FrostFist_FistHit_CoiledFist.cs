using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit_CoiledFist : FrostFist_FistHit
    {
        /// <summary>
        /// 连击数
        /// </summary>
        public int HitCounst = 20;
        public FrostFist_FistHit_CoiledFist(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
            ActionDamage = 0.2f;
        }
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
            if (Projectile.ai[1]-- < 0 && Projectile.ai[0]++ < HitCounst)
            {
                Projectile.ai[1] = 1;
                Vector2 shootVel = vel.SafeNormalize(default).RotatedByRandom(0.4) * Main.rand.NextFloat(4,8);
                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj>(), (int)(Projectile.damage * ActionDamage), Projectile.knockBack, Player.whoAmI,Main.rand.NextFloat(40));
                proj.alpha = 200;
                FistHitProjChange?.Invoke(proj);
                for (int i = 0; i < 32; i++)
                {
                    Dust dust = FrostFistDust();
                    dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2) * i * 0.3f;
                    dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                }
                //SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, shootVel, Projectile.damage, Projectile.knockBack, Player.whoAmI, 60, 200);

            }
            if (Projectile.ai[0] < 5)
            {
                PreAtk = true;
            }
            else
            {
                PreAtk = false;
            }
            if (Projectile.ai[0] > HitCounst + 20)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > HitCounst)
            {
                CanChangeToStopActionSkill = true;
            }
            ExtraAction?.Invoke();
        }
        public override bool SwitchCondition() => Projectile.ai[0] > HitCounst;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[1] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[1] = 0;
        }
    }
}
