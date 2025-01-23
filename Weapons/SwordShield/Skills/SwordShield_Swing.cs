using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade.Skills;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Effects;
using Terraria.Graphics.Effects;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    public class SwordShield_Swing : BasicSwordShieldSkill
    {
        public SwordShield_Swing(SwordShieldProj proj, Func<bool> changeCondition) : base(proj)
        {
            ChangeCondition = changeCondition;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        public bool StrongSlash;
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
        public const int SLASH_TIME = 30;
        public const int PREATTACK_TIME = 12;
        public const int TIMEOUT_TIME = 15;
        public const float CHANGE_LERP_SPEED = 0.35f;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            if (StrongSlash)
            {
                #region 屏幕缩放shader调用
                ScreenChange.SetScreenScale = 0.9f;
                if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                    Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                #endregion
            }
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    {
                        swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, 0);
                        if((int)Projectile.ai[1] == 10)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                var dust = Dust.NewDustDirect(player.Center, 1, 1, DustID.Fireworks);
                                dust.scale = 1.5f;
                                dust.color = Color.Gold;
                                dust.fadeIn = 0.1f;
                                dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                                dust.noGravity = true;
                            }
                        }
                        if (Projectile.ai[1]++ > 6 && (!StrongSlash || (StrongSlash && Projectile.ai[1] > 10 && player.controlUseItem)))
                        {
                            if (Projectile.ai[1] < 50 && Projectile.ai[1] > 15)
                            {
                                Projectile.damage *= 2;
                            }
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 1;
                            TheUtility.Player_ItemCheck_Shoot(player, SwordShieldProj.SpawnItem, Projectile.damage);
                            TheUtility.ResetProjHit(Projectile);
                        }
                        if (Projectile.ai[1]++ > TIMEOUT_TIME * 5)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
                case 1: // 挥舞进行
                    {
                        Projectile.extraUpdates = 2;
                        Projectile.ai[1]++;
                        float Time = SwordShieldProj.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        if (Time > 1)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        break;
                    }
                case 2: // 后摇
                    {
                        if (Projectile.ai[1]++ > TIMEOUT_TIME * (StrongSlash ? 5 : 1))
                        {
                            SkillTimeOut = true;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, SwingRot * SwingDirectionChange.ToDirectionInt());
                        break;
                    }
            }
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction));
            #region 盾的更新
            player.itemRotation = (player.velocity.X * 0.1f);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            SwordShieldProj.swordShield_Shield.Update(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.itemRotation), player.direction, 0);

            #endregion
        }
        public override bool? CanDamage() => true;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += ActionDmg - 1;
            player.SetImmuneTimeForAllTypes(15);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override void SwordDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor,TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0), null);
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //swingHelper.DrawTrailing(

            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
            //    Main.Rasterizer, null, Main.Transform);
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
        public override void ShieldDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwordShieldProj.swordShield_Shield.Draw(sb, lightColor);
        }
    }
}
