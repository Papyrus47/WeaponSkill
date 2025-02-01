using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.General.SwingHelper_GeneralSwing;

namespace WeaponSkill.Weapons.SlashAxe.Skills
{
    public class SlashAxe_SwordSwing : BasicSlashAxeSkill
    {
        public SlashAxe_SwordSwing(SlashAxeProj proj, Func<bool> changeCondition) : base(proj)
        {
            ChangeCondition = changeCondition;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        /// <summary>
        /// 切换技能的条件
        /// </summary>
        public Func<bool> ChangeCondition;
        public float SwingRot;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public Action SwingAI;
        public int SwingTimeMax = 30;
        public int PreSwingTimeMax = 15;
        public int TimeoutTimeMax = 15;
        public const float CHANGE_LERP_SPEED = 0.35f;
        public int PowerAdd = 30;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;
        public override void AI()
        {
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
                    if (Projectile.ai[1]++ > PreSwingTimeMax)
                    {
                        SoundEngine.PlaySound(
                           SoundID.Item1.WithPitchOffset(-0.5f),
                           player.Center
                        );
                        SlashAxeProj.SlashAxeGlobalItem.Slash -= 50;
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
                        TheUtility.Player_ItemCheck_Shoot(player, SlashAxeProj.SpawnItem, Projectile.damage);
                        TheUtility.ResetProjHit(Projectile);
                    }
                    break;
                case 1: // 挥舞
                    Projectile.extraUpdates = 2;
                    Projectile.ai[1]++;
                    float Time = SlashAxeProj.TimeChange(Projectile.ai[1] / SwingTimeMax);
                    if (Time > 1)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                    if ((int)Projectile.ai[1] % 3 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var fire = new Dusts.Particles.Fire(25);
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
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 2 && SlashAxeProj.SlashAxeGlobalItem.Slash > 0;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0), null);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Sword", out var sword))
                sword.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Axe", out var axe))
                axe.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += ActionDmg - 1;
            player.SetImmuneTimeForAllTypes(8);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (SlashAxeProj.SlashAxeGlobalItem.Power == SlashAxeProj.SlashAxeGlobalItem.PowerMax)
            {
                for(int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(target.Center, 10, 10, DustID.FireworksRGB, 1, 1, 0, Color.Red);
                    dust.noGravity = true;
                    dust.velocity = dust.velocity.RotatedByRandom(6.28) * 2;
                }
                var proj = Projectile.NewProjectileDirect(target.GetSource_OnHit(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<TransparentProj>(), Projectile.damage, 0f, player.whoAmI);
                proj.penetrate = 1;
            }
            SlashAxeProj.SlashAxeGlobalItem.Power += PowerAdd;
        }
    }
}
