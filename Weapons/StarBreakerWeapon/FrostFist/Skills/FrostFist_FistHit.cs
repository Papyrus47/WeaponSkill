using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit : BasicFrostFistSkill
    {
        public class FrostFistHit_ProjEntitySource : EntitySource_Parent
        {
            public FrostFist_FistHit frostFist_FistHit;

            public FrostFistHit_ProjEntitySource(Projectile proj, string context = null) : base(proj, context)
            {
                if(proj.ModProjectile is FrostFistProj frostFistProj)
                {
                    frostFist_FistHit = frostFistProj.CurrentSkill as FrostFist_FistHit;
                }
            }
        }
        public FrostFist_FistHit(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile)
        {
            ActivationConditionFunc = activationConditionFunc;
            ActionDamage = 1;
        }
        public Func<bool> ActivationConditionFunc;
        /// <summary>
        /// 额外动作
        /// </summary>
        public Action ExtraAction;
        /// <summary>
        /// 更改生成弹幕
        /// </summary>
        public Action<Projectile> FistHitProjChange;
        /// <summary>
        /// 命中调用
        /// </summary>
        public Action<NPC, NPC.HitInfo, int> OnHit;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDamage;
        public override void AI()
        {
            Projectile.Center = Player.Center;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Player.velocity.X *= 0.2f;
            Player.velocity.Y = 0;
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            Projectile.direction = Player.direction;
            Player.itemTime = Player.itemAnimation = 2;
            Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction,vel.X * Projectile.direction);
            if (Projectile.ai[0]++ == 1)
            {
                Vector2 shootVel = vel.SafeNormalize(default) * 6;
                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj>(),(int)(Projectile.damage * ActionDamage), Projectile.knockBack, Player.whoAmI);
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
            }
            if (Projectile.ai[0] > 40)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > 20)
            {
                CanChangeToStopActionSkill = true;
            }
            ExtraAction?.Invoke();
        }
        public FrostFistHit_ProjEntitySource GetSource() => new(Projectile);
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] > 10;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            PreAtk = false;
            CanChangeToStopActionSkill = false;
        }
    }
}
