using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Dusts.Particles;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    /// <summary>
    /// 炮击,兼职起飞
    /// </summary>
    public class GunLancesBombardment : BasicGunLancesSkill
    {
        public Func<bool> ActivationConditionFunc;
        /// <summary>
        /// 发射次数
        /// </summary>
        public int Counts;
        public GunLancesBombardment(GunLancesProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile)
        {
            ActivationConditionFunc = activationConditionFunc;
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
                    if (ActivationCondition() && Projectile.ai[1] < 60)
                    {
                        Projectile.ai[1]++;
                    }
                    else
                    {
                        //GunLancesProj.GunLancesGlobalItem.Ammo--;
                        if (Projectile.ai[1] < 60) // 炮击
                        {
                            Projectile.ai[0] = 1;
                        }
                        else
                        {
                            Projectile.ai[0] = 2; // 起飞
                        }
                        Projectile.ai[1] = 0;
                        SoundEngine.PlaySound(
                                SoundID.Item14.WithVolume(0.8f).WithPitchOffset(-0.2f), // 更低沉
                                player.Center);
                    }
                    break;
                case 1: // 炮击
                    swingHelper.Change(Vector2.UnitX, Vector2.One, 0f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
                    swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);

                    if(Projectile.ai[1]++ > 90)
                    {
                        SkillTimeOut = true;
                    }
                    else if (Projectile.ai[1] < 20)
                    {
                        player.velocity = -Projectile.velocity.SafeNormalize(default) * 4;
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
                            var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<GunLancesBoomProj>(), Projectile.damage, 0f, player.whoAmI);
                            proj.Resize(80, 80);
                        }
                    }
                    break;
                case 2: // 起飞
                    swingHelper.Change_Lerp(-Vector2.UnitX.RotatedBy(-0.3),0.2f, Vector2.One,0.2f, 0f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SetRotVel(0);
                    //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
                    swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
                    if ((int)Projectile.ai[1] % 10 == 0)
                    {
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
                        SkillTimeOut = true;
                    }
                    else if (Projectile.ai[1] < 20)
                    {
                        player.velocity.X = (-Projectile.velocity.SafeNormalize(default)).X * 30;
                        if (Projectile.ai[1] < 2)
                            player.velocity.Y = -5;
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
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke() && GunLancesProj.GunLancesGlobalItem.Ammo > 0;
        public override bool SwitchCondition() => (Projectile.ai[0] == 1 && Projectile.ai[1] > 30) || (Projectile.ai[0] == 2 && Projectile.ai[1] > 60);
        public override bool SwitchCondition(ProjSkill_Instantiation changeToSkill)
        {
            bool canChangeSkill = true;
            if (Projectile.ai[0] == 1 && changeToSkill is GunLancesBombardment && Counts >= 1) // 正常火炮
            {
                canChangeSkill = false;
            }
            else if(Projectile.ai[0] == 2)
            {
                canChangeSkill = true;
            }
            if (changeToSkill is GunLancesDetonateLonghangUse && (int)Projectile.ai[0] == 2)
                canChangeSkill = false;
            if (changeToSkill is GunLancesSwing && (int)Projectile.ai[0] == 1)
                canChangeSkill = false;
            return base.SwitchCondition(changeToSkill) && canChangeSkill;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.extraUpdates = 4;
        }
        public override void OnSkillDeactivate(ProjSkill_Instantiation changeToSkill)
        {
            if (changeToSkill is GunLancesBombardment && (int)Projectile.ai[0] == 1)
            {
                Counts++;
            }
            else
                Counts = 0;
            bool falg = false;
            if (changeToSkill is GunLancesBombardment && (int)Projectile.ai[0] == 2)
            {
                falg = true;
            }
            base.OnSkillDeactivate(changeToSkill);
            if (falg)
            {
                Projectile.ai[0] = 2;
            }
        }
        public override void OnSkillDeactivate()
        {
            if (SkillTimeOut)
            {
                Counts = 0;
            }
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            GunLancesProj.GunLancesGlobalItem.Ammo--;
        }
    }
}
