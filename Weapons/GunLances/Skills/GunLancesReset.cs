using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesReset : BasicGunLancesSkill
    {
        public GunLancesReset(GunLancesProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 50)
            {
                SkillTimeOut = true;
            }
            Vector2 rotVector = -Vector2.UnitY;
            Projectile.spriteDirection = player.direction;
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -GunLancesProj.SwingLength * 0.6f, true);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
            #region 盾的更新

            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(Projectile.Center, -player.direction);
            #endregion
            #region 部位更新
            Vector2 vel = Vector2.Zero;
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
            {
                handle.Update();
                vel = handle.Size;
            }
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
            {
                gun.Update();
                if (Projectile.ai[0] < 20)
                {
                    gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel * gun.Size.Length() * 0.02f + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * -player.direction).SafeNormalize(default) * vel * 0.4f, 0.2f);
                    gun.Rot = MathHelper.Lerp(gun.Rot, MathHelper.Pi * 1, 0.2f);
                }
                else
                {
                    if ((int)Projectile.ai[0] == 25)
                    {
                        for (int i = 0; i < 20; i++) // 生成粒子
                        {
                            // 生成橙色火焰
                            Vector2 velocity = Projectile.velocity;
                            velocity *= 0.1f;

                            // 生成烟雾
                            Dust smoke = Dust.NewDustDirect(
                                Projectile.Center + Projectile.velocity,
                                1,
                                1,
                                DustID.Smoke,
                                SpeedX: Main.rand.NextFloat(-2f, 2f) + velocity.X,
                                SpeedY: Main.rand.NextFloat(-2f, 2f) + velocity.Y,
                                Alpha: 100,
                                new Color(80, 80, 80), // 深灰色
                                4.2f
                            );
                            smoke.noGravity = true;
                        }
                    }

                    gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel, 1f);
                    gun.Update();
                    gun.Rot = MathHelper.Lerp(gun.Rot, 0, 0.2f);
                }
            }
            if (swingHelper.Parts.TryGetValue("Reset", out var reset))
            {
                reset.Update();
                if (Projectile.ai[0] < 20)
                    reset.OffestCenter = Projectile.velocity.SafeNormalize(default) * reset.Size.Length() * 0.5f * (Projectile.ai[0] / 20f);
                else
                    reset.OffestCenter = Projectile.velocity.SafeNormalize(default) * reset.Size.Length() * 0.5f * (2 - Projectile.ai[0] / 20f);
            }
            #endregion
        }
        public override bool SwitchCondition() => false;
        public override bool ActivationCondition() => player.controlUseTile;
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.extraUpdates = 0;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            GunLancesProj.GunLancesGlobalItem.Ammo = GunLancesProj.GunLancesGlobalItem.MaxAmmo;
            GunLancesProj.GunLancesGlobalItem.HasLongHang = true;
            Projectile.extraUpdates = 1;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            //swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
                gun.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Reset", out var reset))
                reset.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
                handle.DrawSwingItem(lightColor);


            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
    }
}
