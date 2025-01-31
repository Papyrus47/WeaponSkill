using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLances_SwingReset : GunLancesSwing
    {
        public GunLances_SwingReset(GunLancesProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(StartVel, VelScale, VisualRotation);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
                SoundEngine.PlaySound(
                   SoundID.Item1.WithPitchOffset(-0.3f), // 降低音调更显沉重
                   player.Center
                );
                //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            }
            if (Projectile.ai[0] < 20)
            {
                player.velocity.X = 2 * player.direction;
                PreAttack = true;
            }
            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(player.Center, player.direction);

            Projectile.spriteDirection = Projectile.direction = player.direction * SwingDir;
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            float factor = Projectile.ai[0] / 80f;
            factor = Math.Min(1, factor);
            factor = MathF.Pow(factor, 6);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, factor * Rot * SwingDir);
            if (Projectile.ai[0] > 190)
            {
                Projectile.ai[0] = 0;
                Projectile.extraUpdates = 0;
                SkillTimeOut = true;
            }
            #region 部位更新
            Vector2 vel = Vector2.Zero;
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
            {
                handle.Update();
                vel = handle.Size;
            }
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
            {
                gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel, 0.2f);
                gun.Update();
                gun.Rot = MathHelper.Lerp(gun.Rot, 0, 0.04f);
            }
            #endregion
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            GunLancesProj.GunLancesGlobalItem.Ammo = GunLancesProj.GunLancesGlobalItem.MaxAmmo;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            swingHelper.SetRotVel(0);
            Vector2 vel = Vector2.Zero;
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
            {
                vel = handle.Size;
            }
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
            {
                gun.OffestCenter = Projectile.velocity.SafeNormalize(default) * vel * gun.Size.Length() * 0.02f + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * -player.direction).SafeNormalize(default) * vel * 0.4f;
                gun.Rot = MathHelper.Pi;
            }
        }
    }
}
