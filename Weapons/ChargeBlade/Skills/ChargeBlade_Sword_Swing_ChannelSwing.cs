using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Sword_Swing_ChannelSwing : ChargeBlade_Sword_Swing
    {
        public ChargeBlade_Sword_Swing_ChannelSwing(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade, activationConditionFunc)
        {
            StartVel = Vector2.UnitY.RotatedBy(0.5);
            SwingRot = MathHelper.Pi + MathHelper.PiOver2;
            VelScale = new Vector2(1, 0.6f);
            VisualRotation = 0.4f;
        }
        public bool IsSlashTwo;
        public override void AI()
        {
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.6f, 0), player.direction);
            shield.VisualRotation = 0.3f;
            shield.AxeRot = 0.05f;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.direction * -MathHelper.PiOver2);
            #endregion

            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    {
                        PreAttack = true;
                        swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
                        player.velocity.X = 0;
                        Projectile.extraUpdates = 0;
                        if (Projectile.ai[1]++ > 8)
                        {
                            if (ActivationCondition() && !SwingDirectionChange && Projectile.ai[2] < 50)
                            {
                                Projectile.ai[1] = 8;
                                Projectile.ai[2]++;
                                IsSlashTwo = false;
                                if (Projectile.ai[2] > 15 && Projectile.ai[2] < 40)
                                {
                                    var dust = Dust.NewDustDirect(player.Center, 5, 5, DustID.Clentaminator_Red);
                                    dust.velocity = Projectile.velocity * 0.05f;
                                    dust.position = player.Center + new Vector2(-Projectile.velocity.Y,Projectile.velocity.X).SafeNormalize(default) * Main.rand.NextFloatDirection() * Projectile.width * 0.1f;
                                    dust.noGravity = true;
                                    dust.color = new Color(245, 254, 106, 255);
                                    IsSlashTwo = true;
                                }
                            }
                            else
                            {
                                Projectile.ai[0]++;
                                Projectile.ai[1] = 0;
                            }
                            Projectile.extraUpdates = 1;
                            TheUtility.Player_ItemCheck_Shoot(player, ChargeBladeProj.SpawnItem, Projectile.damage);
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 挥舞进行
                    {
                        if (AttackSwap)
                        {
                            Projectile.ai[1]--;
                            swingHelper.SetNotSaveOldVel();
                        }
                        else Projectile.ai[1]++;
                        if (!SwingDirectionChange || IsSlashTwo) Projectile.extraUpdates = 2;
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME * 0.8f);
                        if (Time > 1 || (Time <= 0f && AttackSwap))
                        {
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        break;
                    }
                case 2: // 后摇
                    {
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME * 0.8f);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (IsSlashTwo && !AttackSwap)
                        {
                            StartVel = -Vector2.UnitY.RotatedBy(-0.5);
                            SwingDirectionChange = true;
                            IsSlashTwo = false;
                            Projectile.ai[0] = 0;
                            Projectile.ai[1] = 0;
                            Projectile.ai[2] = 0;
                            SkillTimeOut = false;
                        }
                        if (Projectile.ai[2] > TIMEOUT_TIME)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 23)
            {
                var effect = WeaponSkill.SwordHot.Value;
                effect.CurrentTechnique.Passes[0].Apply();
                effect.Parameters["tex"].SetValue(WeaponSkill.HotTex.Value);
                effect.Parameters["uTime"].SetValue(10f);
            }
            swingHelper.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

            swingHelper.DrawTrailing(TextureAssets.Extra[209].Value, (_) => new Color(255, 0, 0, 0), null);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            ChargeBladeProj.chargeBladeGlobal.StatCharge += 1.5f;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            IsSlashTwo = false;
            SkillTimeOut = false;
            StartVel = Vector2.UnitY.RotatedBy(0.5);
            SwingDirectionChange = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            IsSlashTwo = false;
        }
    }
}
