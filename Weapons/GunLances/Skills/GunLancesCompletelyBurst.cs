using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Dusts.Particles;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesCompletelyBurst : BasicGunLancesSkill
    {
        public GunLancesCompletelyBurst(GunLancesProj modProjectile) : base(modProjectile)
        {
        }

        public override void AI()
        {
            #region 限制速度
            if (Math.Abs(player.velocity.X) > 2) 
                player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            #endregion
            Projectile.spriteDirection = player.direction;
            #region 铳枪全蛋发射
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[0]++ >= 30 && Projectile.ai[0] <= 60 && (int)Projectile.ai[0] % (30 / Math.Max(1, GunLancesProj.GunLancesGlobalItem.Ammo)) == 0)
            {
                SoundEngine.PlaySound(
                   SoundID.Item14.WithVolume(0.8f).WithPitchOffset(-0.2f) with
                   {
                       MaxInstances = 20
                   }, // 更低沉
                   player.Center);
                var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<GunLancesBoomProj>(), Projectile.damage, 0f, player.whoAmI);
                proj.Resize(80, 80);
                for (int i = 0; i < 10; i++)
                {
                    Fire fire_Fire = new(20); // Fire = 开火 梗
                    fire_Fire.SetBasicInfo(null, null, Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.8) * 8 * Main.rand.NextFloat(), Projectile.Center + Projectile.velocity);
                    Main.ParticleSystem_World_BehindPlayers.Add(fire_Fire);
                }
                #region 我爱你deepseek
                for (int i = 0; i < 20; i++) // 生成粒子
                {
                    // 生成橙色火焰
                    Vector2 velocity = Projectile.velocity;
                    velocity *= 0.1f;
                    Dust fire = Dust.NewDustDirect(
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
                    fire.noGravity = true;

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
            }
            #endregion
            #region 跳出
            if (Projectile.ai[0] > 90 * Projectile.extraUpdates)
            {
                SkillTimeOut = true;
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
        public override bool ActivationCondition() => player.controlUseTile && GunLancesProj.GunLancesGlobalItem.Ammo > 0;
        public override bool SwitchCondition() => Projectile.ai[0] > 60;
        public override void OnSkillActive()
        {
            Projectile.extraUpdates = 2;
            base.OnSkillActive();
            swingHelper.Change(Vector2.UnitX, Vector2.One);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            GunLancesProj.GunLancesGlobalItem.Ammo = 0;
            Projectile.extraUpdates = 0;
        }
        public override void OnSkillDeactivate(ProjSkill_Instantiation changeToSkill)
        {
            base.OnSkillDeactivate(changeToSkill);
        }
    }
}
