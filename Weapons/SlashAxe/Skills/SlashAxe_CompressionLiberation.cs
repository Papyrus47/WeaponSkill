using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using WeaponSkill.Command;
using WeaponSkill.Effects;
using WeaponSkill.Weapons.ChargeBlade.Skills;

namespace WeaponSkill.Weapons.SlashAxe.Skills
{
    public class SlashAxe_CompressionLiberation : SlashAxe_SwordSwing
    {
        public SlashAxe_CompressionLiberation(SlashAxeProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
            SwingRot = MathHelper.TwoPi;
            StartVel = Vector2.UnitX;
            VelScale = new(1, 0.3f);
            SwingTimeMax = 90;
            ActionDmg = 10;
            PreSwingTimeMax = 40;
        }
        public override void AI()
        {
            #region 屏幕缩放shader调用
            ScreenChange.SetScreenScale = 0.8f;
            if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
            #endregion
            #region 玩家更新
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            #endregion
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    PreAtk = true;
                    swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, 0);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 30, 30, DustID.FireworksRGB, 0, 0, 0, Color.OrangeRed * 0.6f);
                        dust.noGravity = true;
                        dust.velocity = -Projectile.velocity * 0.1f;
                    }
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center,Main.rand.NextVector2Unit(), 1, 5, 2));
                    if (Projectile.ai[1]++ > PreSwingTimeMax) // 这里写压缩解放
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 7, 5, 60));
                        #region 压缩解放爆炸
                        for (int i = 0; i < 10; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 30, 30, DustID.FireworksRGB,1,1,0, Color.OrangeRed);
                            dust.noGravity = true;
                            dust.velocity = dust.velocity.RotatedByRandom(6.28) * 2;
                        }
                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Vector2.Zero, ModContent.ProjectileType<TransparentProj>(), Projectile.damage * 10, 0f, player.whoAmI);
                        proj.Size = new Vector2(30);
                        #endregion
                        SlashAxeProj.SlashAxeGlobalItem.Slash -= 400;
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
                        TheUtility.Player_ItemCheck_Shoot(player, SlashAxeProj.SpawnItem, Projectile.damage);
                        TheUtility.ResetProjHit(Projectile);
                        swingHelper.Change(StartVel, VelScale, 0.7f);
                    }
                    break;
                case 1: // 挥舞
                    Projectile.extraUpdates = 4;
                    Projectile.ai[1]++;
                    float Time = SlashAxeProj.TimeChange(Projectile.ai[1] / SwingTimeMax);
                    if (Time > 1)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                    }
                    player.velocity.X = player.direction * 4;
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                    if ((int)Projectile.ai[1] % 3 == 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 30, 30, DustID.FireworksRGB, 0, 0, 0, Color.OrangeRed);
                            dust.noGravity = true;
                            dust.velocity = dust.velocity.RotatedByRandom(6.28) * 2;
                        }
                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Vector2.Zero, ModContent.ProjectileType<TransparentProj>(), Projectile.damage, 0f, player.whoAmI);
                        proj.Size = new Vector2(30);
                        for (int i = 0; i < 2; i++)
                        {
                            var fire = new Particles.Fire(25);
                            fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.02f, 0.05f)).RotatedByRandom(0.6), Projectile.Center + Projectile.velocity);
                            Main.ParticleSystem_World_BehindPlayers.Add(fire);
                        }
                    }
                    break;
                case 2: // 跳出
                    if (Projectile.ai[1]++ > TimeoutTimeMax)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
            }
            #region 剑控制
            if (swingHelper.Parts.TryGetValue("Sword", out var sword))
            {
                sword.SPDir = 1;
                sword.Update();
                sword.Rot = MathHelper.Pi * 0.05f * -Projectile.spriteDirection;
                sword.OffestCenter = Vector2.Lerp(sword.OffestCenter, Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection) * 0.1f + sword.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.4f, 0.9f);
            }
            #endregion
            #region 斧控制
            if (swingHelper.Parts.TryGetValue("Axe", out var axe))
            {
                axe.Update();
                axe.OffestCenter = Vector2.Lerp(axe.OffestCenter, Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection) * 0.1f + axe.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.1f, 0.9f);
            }
            #endregion
        }
        public override bool? CanDamage() => Projectile.ai[0] > 0;
        public override bool ActivationCondition() => player.controlUseItem && player.controlUseTile && SlashAxeProj.SlashAxeGlobalItem.Power == SlashAxeProj.SlashAxeGlobalItem.PowerMax;
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition() && (nowSkill as BasicSlashAxeSkill).PreAtk;
    }
}
