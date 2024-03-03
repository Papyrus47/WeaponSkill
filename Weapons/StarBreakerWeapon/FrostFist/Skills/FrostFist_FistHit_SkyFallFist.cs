using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit_SkyFallFist : FrostFist_FistHit
    {
        public FrostFist_FistHit_SkyFallFist(FrostFistProj modProjectile) : base(modProjectile,null)
        {
            ActionDamage = 2.5f;
        }
        public override void AI()
        {
            Projectile.Center = Player.Center;
            Vector2 vel = Vector2.UnitY;
            Player.velocity.X *= 0.2f;
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            Projectile.direction = Player.direction;
            Player.itemTime = Player.itemAnimation = 2;
            Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction, vel.X * Projectile.direction);
            if (Projectile.ai[0]++ == 0)
            {
                Vector2 shootVel = vel.SafeNormalize(default) * 6;
                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj_SkyFallFist>(), (int)(Projectile.damage * ActionDamage), Projectile.knockBack,Player.whoAmI);
                proj.alpha = 0;
                FistHitProjChange?.Invoke(proj);
                for (int i = 0; i < 32; i++)
                {
                    Dust dust = FrostFistDust();
                    dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2) * i * 0.3f;
                    dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                }
                //SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, shootVel, Projectile.damage, Projectile.knockBack, Player.whoAmI, 60, 200);

            }
            else if (Projectile.ai[0] < 5)
            {
                PreAtk = true;
            }
            else
            {
                PreAtk = false;
                Player.fullRotation = MathHelper.Lerp(Player.fullRotation, 0, 0.5f);
            }
            if (Projectile.ai[0] > 40)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > 20)
            {
                CanChangeToStopActionSkill = true;
            }
            #region 持续下落
            if (!GetPlayerStandTile() && Projectile.ai[0] < 4)
            {
                Projectile.ai[0] = 2;
                #region 下落判定
                Player.fullRotationOrigin = Player.Size * 0.5f;
                Player.velocity.Y = 0;
                Player.fullRotation = MathHelper.Lerp(Player.fullRotation, MathHelper.Pi, 0.5f);
                #endregion
            }
            else if(Projectile.ai[0] < 4)
            {
                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center,Vector2.UnitX, ModContent.ProjectileType<ShockProj>(), (int)(Projectile.damage * ActionDamage), Projectile.knockBack, Player.whoAmI);
                proj.timeLeft = 120;
                Player.velocity.Y = -5;
            }
            #endregion
            ExtraAction?.Invoke();
        }
        public override bool ActivationCondition()
        {
            return (Player.controlUseItem || Player.controlUseTile) && !GetPlayerStandTile();
        }
    }
}
