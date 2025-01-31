using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Dusts.Particles;

namespace WeaponSkill.Weapons.GunLances
{
    /// <summary>
    /// 起爆龙杭
    /// </summary>
    public class GunLancesDetonateLonghang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(58);
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 90;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.ai[2] = -1;
            Projectile.frame = 1;
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.ai[2] != -1)
            {
                if (Projectile.timeLeft % 20 == 0)
                {
                    Terraria.Audio.SoundEngine.PlaySound(
                        SoundID.DD2_BallistaTowerShot.WithPitchOffset(1.2f).WithVolume(1.6f), // 加速播放
                        Projectile.Center
                    );
                }
                Projectile.extraUpdates = 0;
                Projectile.frame = 0;
                NPC target = Main.npc[(int)Projectile.ai[2]];
                if (!target.CanBeChasedBy(Projectile))
                {
                    Projectile.ai[2] = -1;
                    Projectile.Kill();
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Fire fire = new(50); // Fire = 开火 梗
                        fire.SetBasicInfo(null, null, -Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.9) * 6 * Main.rand.NextFloat(), Projectile.Center);
                        Main.ParticleSystem_World_BehindPlayers.Add(fire);
                    }
                    Projectile.Center = target.Center - Projectile.velocity.SafeNormalize(default) * Projectile.localAI[0];
                    Projectile.localAI[0] *= 0.99f;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[2] != -1) 
                return target.whoAmI == (int)Projectile.ai[2];
            return null;
        }
        public override void OnKill(int timeLeft)
        {
            #region 我爱你deepseek
            for (int i = 0; i < 20; i++) // 每帧生成3个粒子
            {
                // 生成橙色火焰
                Dust fire = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    SpeedX: Main.rand.NextFloat(-5f, 5f),
                    SpeedY: Main.rand.NextFloat(-5f, 5f),
                    Alpha: 100,
                    new Color(255, 150, 0), // 橙红色
                    3.5f
                );
                fire.noGravity = true;

                // 生成烟雾
                Dust smoke = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke,
                    SpeedX: Main.rand.NextFloat(-3f, 3f),
                    SpeedY: Main.rand.NextFloat(-3f, 3f),
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
                    Projectile.Center,
                    DustID.GoldFlame,
                    speed * Main.rand.NextFloat(0.5f, 1.5f),
                    100,
                    Color.Orange,
                    Main.rand.NextFloat(1f, 2f)
                ).noGravity = true;
            }
            var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GunLancesBoomProj>(), Projectile.damage * 5, 0f, Projectile.owner);
            proj.Resize(80, 80);

            SoundEngine.PlaySound(
                SoundID.Item62.WithPitchOffset(-0.3f).WithVolume(1.5f),
                Projectile.Center
            );

            // 第二段高频爆炸（延迟0.3秒）
            SoundEngine.PlaySound(
                SoundID.DD2_ExplosiveTrapExplode.WithPitchOffset(0.2f),
                Projectile.Center
            );
            #endregion
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == -1)
            {
                Terraria.Audio.SoundEngine.PlaySound(
                    SoundID.DD2_BallistaTowerShot.WithVolume(1.2f).WithPitchOffset(0.5f),
                    target.Center
                );
                Projectile.ai[2] = target.whoAmI;
                Projectile.localAI[0] = (Projectile.Center - target.Center).Length();
                Projectile.velocity = Projectile.velocity.SafeNormalize(default) * 0.2f;
                Projectile.timeLeft = 60;
            }
        }
    }
}
