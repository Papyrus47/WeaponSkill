using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Sword_ToAxe : BasicChargeBladeSkill
    {
        public ChargeBlade_Sword_ToAxe(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            Projectile.spriteDirection = player.direction;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 插入盾
                    {
                        Vector2 rotVector = Vector2.UnitY.RotatedBy(0.225f);

                        bool flag = false;
                        swingHelper.Change(rotVector, Vector2.One, 0.45f);
                        if ((int)Projectile.ai[1]++ > 40) // 收入完毕
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        Projectile.spriteDirection = player.direction;
                        swingHelper.SetSwingActive();
                        swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.width * player.direction, -player.height * 0.2f), -ChargeBladeProj.SwingLength * 0.6f, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
                        if (flag)
                        {
                            Projectile.Center -= Projectile.velocity * 0.45f;
                        }
                        #region 盾的更新
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.width * player.direction, -player.height * 0.2f), -player.direction);
                        shield.GP = true;
                        shield.InDef = true;
                        shield.VisualRotation = 0.23f;
                        shield.AxeRot = -0.23f;
                        #endregion
                        Projectile.numHits = 0;
                        break;
                    }
                case 1: // 变成斧-准备挥舞
                    {
                        ChargeBladeProj.chargeBladeGlobal.InAxe = true;
                        swingHelper.Change_Lerp((-Vector2.UnitX).RotatedBy(0.2), 0.08f, Vector2.One, 0.4f, 0, 0.1f);;
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(Projectile.Center + Projectile.velocity * 0.95f, 1);
                        shield.GP = true;
                        shield.InDef = true;
                        shield.VisualRotation = 0f;
                        shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;

                        Projectile.ai[1]++;
                        if (Projectile.ai[1] > 36)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 2: // 锤打
                    {
                        ChargeBladeProj.chargeBladeGlobal.InAxe = true;
                        Projectile.extraUpdates = 1;
                        if (Projectile.numHits > 0 && ChargeBladeProj.chargeBladeGlobal.AxeStrengthening && Projectile.ai[1] > 25)
                        {
                            Projectile.numHits = 0;
                            Projectile.ai[2] = 14;
                        }
                        if (Projectile.ai[2] > 0)
                        {
                            if (--Projectile.ai[2] <= 0)
                            {
                                TheUtility.ResetProjHit(Projectile);
                            }
                            Projectile.ai[1] += 0.2f;
                        }
                        else
                        {
                            Projectile.ai[1]++;
                        }
                        if (Projectile.ai[1] < 25 && Projectile.ai[2] <= 0)
                        {
                            Projectile.ai[1] -= 0.6f;
                        }
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / 40);
                        if (Time > 1)
                        {
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * MathHelper.Pi);
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(Projectile.Center + Projectile.velocity * 0.95f, 1);
                        shield.VisualRotation = 0f;
                        shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;
                        break;
                    }
                case 3: // 结束
                    {
                        ChargeBladeProj.chargeBladeGlobal.InAxe = true;
                        Projectile.extraUpdates = 0;
                        Projectile.ai[1] += 0.01f;
                        Projectile.ai[2]++;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / 40);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * MathHelper.Pi);
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(Projectile.Center + Projectile.velocity * 0.95f, 1);
                        shield.VisualRotation = 0f;
                        shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;
                        break;
                    }
            }
        }
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[2] > 20;
        public override bool? CanDamage() => Projectile.ai[0] > 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox) || ChargeBladeProj.shield.swingHelper.GetColliding(targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 0.5f;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            //swingHelper.DrawSwingItem(lightColor);
            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
            //    Main.Rasterizer, null, Main.Transform);
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0));
            return false;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            TheUtility.ResetProjHit(Projectile);
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
    }
}
