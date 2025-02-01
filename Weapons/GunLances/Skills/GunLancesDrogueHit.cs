using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Dusts.Particles;
using WeaponSkill.Effects;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesDrogueHit : BasicGunLancesSkill
    {
        public GunLancesDrogueHit(GunLancesProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            #region 移动速度更新
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            #endregion
            swingHelper.Change(Vector2.UnitX, Vector2.One, 0f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);

            if (Projectile.ai[0]++ < 120)
            {
                float scale = 1 + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.2f) * 0.3f;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = Projectile.Center + Projectile.velocity + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 6 * i + Main.GlobalTimeWrappedHourly * 4) * 20;
                    Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, Projectile.velocity * 0.1f, 0, Color.OrangeRed, scale).noGravity = true;
                }
                Fire fire = new(10);
                fire.SetBasicInfo(null, null, Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.2) * 3, Projectile.Center + Projectile.velocity);
                Main.ParticleSystem_World_BehindPlayers.Add(fire);
                Dust smoke = Dust.NewDustDirect(
                                Projectile.Center + Projectile.velocity,
                                1,
                                1,
                                DustID.Smoke,
                                SpeedX: Main.rand.NextFloat(-2f, 2f),
                                SpeedY: Main.rand.NextFloat(-2f, 2f),
                                Alpha: 100,
                                new Color(80, 80, 80), // 深灰色
                                4.2f
                            );
                smoke.noGravity = true;
            }
            else
            {
                //GunLancesProj.GunLancesGlobalItem.Ammo--;
                if ((int)Projectile.ai[0] == 121)
                {
                    SoundEngine.PlaySound(
                            SoundID.Item14.WithVolume(0.8f).WithPitchOffset(-0.2f), // 更低沉
                            player.Center);
                    for (int j = 0;j < 10; j++)
                    {
                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * (1f + j * 0.5f), Projectile.velocity * 0.1f, ModContent.ProjectileType<GunLancesBoomProj>(), (int)(Projectile.damage * 10.5f), Projectile.knockBack, player.whoAmI);
                        proj.Resize(120, 120);
                        #region 我爱你deepseek
                        for (int i = 0; i < 20; i++) // 生成粒子
                        {
                            // 生成橙色火焰
                            Vector2 velocity = Projectile.velocity;
                            velocity *= 0.1f;
                            Dust fire = Dust.NewDustDirect(
                                Projectile.Center + Projectile.velocity * j * 0.5f,
                                1,
                                1,
                                DustID.Torch,
                                SpeedX: Main.rand.NextFloat(-5f, 5f) + velocity.X,
                                SpeedY: Main.rand.NextFloat(-5f, 5f) + velocity.Y,
                                Alpha: 100,
                                new Color(255, 150, 0), // 橙红色
                                3.5f
                            );
                            fire.noGravity = true;

                            // 生成烟雾
                            Dust smoke = Dust.NewDustDirect(
                                Projectile.Center + Projectile.velocity * j * 0.5f,
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
                        for (int i = 0; i < 50; i++) // 冲击波
                        {
                            Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
                            Dust.NewDustPerfect(
                                Projectile.Center + Projectile.velocity * j * 0.5f,
                                DustID.GoldFlame,
                                speed * Main.rand.NextFloat(0.5f, 1.5f) + Projectile.velocity * 0.02f,
                                100,
                                Color.Orange,
                                Main.rand.NextFloat(1f, 2f)
                            ).noGravity = true;
                        }
                        #endregion
                    }
                }
                else if (Projectile.ai[0] < 140)
                {
                    player.velocity.X = -player.direction * 6;
                }
                if (Projectile.ai[0] > 190)
                {
                    SkillTimeOut = true;
                }
            }

            #region 盾的更新

            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(Projectile.Center, player.direction);
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
                gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel, 1f);
                gun.Update();
                gun.Rot = MathHelper.Lerp(gun.Rot, 0, 0.2f);
            }
            #endregion
        }
        public override bool ActivationCondition() => WeaponSkill.RangeChange.Current && GunLancesProj.GunLancesGlobalItem.DrogueHitTime <= 0;
        public override bool SwitchCondition() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            #region 屏幕缩放shader调用
            ScreenChange.SetScreenScale = 0.8f;
            if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            //swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
                gun.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
                handle.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override bool? CanDamage() => false;
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            GunLancesProj.GunLancesGlobalItem.DrogueHitTime = 600;
            #region 屏幕缩放shader取消调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
    }
}
