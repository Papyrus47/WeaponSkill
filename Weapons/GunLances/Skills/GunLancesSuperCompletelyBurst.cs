using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Command;
using WeaponSkill.Dusts.Particles;
using WeaponSkill.Effects;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesSuperCompletelyBurst : BasicGunLancesSkill
    {
        public GunLancesSuperCompletelyBurst(GunLancesProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            #region 炮击
            switch ((int)Projectile.ai[0])
            {
                case 0: // 喷火/判断蓄力
                    #region 移动速度更新
                    if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
                    #endregion
                    swingHelper.Change(Vector2.UnitX, Vector2.One, 0f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
                    swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
                    Fire fire = new(10);
                    fire.SetBasicInfo(null, null, Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.2) * 3, Projectile.Center + Projectile.velocity);
                    Main.ParticleSystem_World_BehindPlayers.Add(fire);
                    if (Projectile.ai[1] < 20)
                    {
                        Projectile.ai[1]++;
                    }
                    else
                    {
                        Projectile.ai[0] = 2;
                        Projectile.ai[1] = 0;
                        SoundEngine.PlaySound(
                                SoundID.Item14.WithVolume(0.8f).WithPitchOffset(-0.2f), // 更低沉
                                player.Center);
                    }
                    break;
                case 1: // 起飞
                    swingHelper.Change_Lerp(-Vector2.UnitX.RotatedBy(-0.3), 0.2f, Vector2.One, 0.2f, 0f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SetRotVel(0);
                    //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
                    swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
                    if ((int)Projectile.ai[1] % 10 == 0)
                    {
                        SoundEngine.PlaySound(
                                SoundID.Item14.WithVolume(0.8f).WithPitchOffset(-0.2f), // 更低沉
                                player.Center);
                        #region 我爱你deepseek
                        for (int i = 0; i < 20; i++) // 生成粒子
                        {
                            // 生成橙色火焰
                            Vector2 velocity = Projectile.velocity;
                            velocity *= 0.1f;
                            Dust fire_Dust = Dust.NewDustDirect(
                                Projectile.Center + Projectile.velocity,
                                1,
                                1,
                                DustID.Torch,
                                SpeedX: Main.rand.NextFloat(-5f, 5f) + velocity.X,
                                SpeedY: Main.rand.NextFloat(-5f, 5f) + velocity.Y,
                                Alpha: 100,
                                new Color(255, 150, 0), // 橙红色
                                3.5f
                            );
                            fire_Dust.noGravity = true;

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
                        #endregion
                        for (int i = 0; i < 2; i++)
                        {
                            Fire fire_Fire = new(50); // Fire = 开火 梗
                            fire_Fire.SetBasicInfo(null, null, Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.9) * 6 * Main.rand.NextFloat(), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire_Fire);
                        }
                    }
                    if (Projectile.ai[1]++ > 90)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                    }
                    else if (Projectile.ai[1] < 20)
                    {
                        player.velocity.X = (-Projectile.velocity.SafeNormalize(default)).X * 20;
                        if (Projectile.ai[1] < 2)
                            player.velocity.Y = -5;
                    }
                    break;
                case 2: // 炮击
                    if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
                    swingHelper.Change(Vector2.UnitX, Vector2.One, 0f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
                    swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);

                    if (Projectile.ai[1]++ > 90)
                    {
                        SkillTimeOut = true;
                    }
                    else if (Projectile.ai[1] < 20)
                    {
                        player.velocity = -Projectile.velocity.SafeNormalize(default) * 30;
                        if ((int)Projectile.ai[1] == 10)
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                Fire fire_Fire = new(20); // Fire = 开火 梗
                                fire_Fire.SetBasicInfo(null, null, Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.2) * 8 * Main.rand.NextFloat(), Projectile.Center + Projectile.velocity);
                                Main.ParticleSystem_World_BehindPlayers.Add(fire_Fire);
                            }
                            #region 我爱你deepseek
                            for (int i = 0; i < 20; i++) // 生成粒子
                            {
                                // 生成橙色火焰
                                Vector2 velocity = Projectile.velocity;
                                velocity *= 0.1f;
                                Dust fire_Dust = Dust.NewDustDirect(
                                    Projectile.Center + Projectile.velocity,
                                    1,
                                    1,
                                    DustID.Torch,
                                    SpeedX: Main.rand.NextFloat(-5f, 5f) + velocity.X,
                                    SpeedY: Main.rand.NextFloat(-5f, 5f) + velocity.Y,
                                    Alpha: 100,
                                    new Color(255, 150, 0), // 橙红色
                                    3.5f
                                );
                                fire_Dust.noGravity = true;

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
                            for (int i = 0; i < 50; i++) // 冲击波
                            {
                                Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
                                Dust.NewDustPerfect(
                                    Projectile.Center + Projectile.velocity,
                                    DustID.GoldFlame,
                                    speed * Main.rand.NextFloat(0.5f, 1.5f) + Projectile.velocity * 0.02f,
                                    100,
                                    Color.Orange,
                                    Main.rand.NextFloat(1f, 2f)
                                ).noGravity = true;
                            }
                            #endregion
                            for (int i = 0; i < GunLancesProj.GunLancesGlobalItem.Ammo; i++)
                            {
                                var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<GunLancesBoomProj>(), (int)(Projectile.damage * 1.5f), 0f, player.whoAmI);
                                proj.Resize(80, 80);
                            }
                            if (GunLancesProj.GunLancesGlobalItem.HasLongHang)
                            {
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 0.5f, Projectile.velocity.SafeNormalize(default), ModContent.ProjectileType<GunLancesDetonateLonghang>(), Projectile.damage * 3, 0f, player.whoAmI);
                                GunLancesProj.GunLancesGlobalItem.HasLongHang = false;
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<GunLancesBoomProj>(), (int)(Projectile.damage * 2.5f), 0f, player.whoAmI);
                                proj.Resize(80, 80);
                            }
                            if (GunLancesProj.GunLancesGlobalItem.DrogueHitTime <= 0)
                            {
                                GunLancesProj.GunLancesGlobalItem.DrogueHitTime = 1800;
                            }
                        }
                    }
                    break;
            }
            #endregion
            #region 盾更新
            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(player.Center, player.direction);
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
        public override void OnSkillActive()
        {
            Projectile.extraUpdates = 2;
            base.OnSkillActive();
            swingHelper.Change(Vector2.UnitX, Vector2.One);
            #region 屏幕缩放shader调用
            ScreenChange.SetScreenScale = 0.8f;
            if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            GunLancesProj.GunLancesGlobalItem.Ammo = 0;
            Projectile.extraUpdates = 0;
            #region 屏幕缩放shader取消调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override void OnSkillDeactivate(ProjSkill_Instantiation changeToSkill)
        {
            base.OnSkillDeactivate(changeToSkill);
        }
    }
}
