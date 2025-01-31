using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Effects;
using WeaponSkill.Command;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;
using WeaponSkill.Dusts.Particles;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 盾斧盾锯+限制移除
    /// </summary>
    public class ChargeBlade_ShieldsRotSlash_LimitRemoval : BasicChargeBladeSkill
    {
        public int CD;
        public ChargeBlade_ShieldsRotSlash_LimitRemoval(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            player.velocity.X *= 0f;
            switch ((int)Projectile.ai[2])
            {
                case 0: // 盾锯前摇
                    {
                        PreAttack = true;
                        Vector2 rotVector = (-Vector2.UnitX).RotatedBy(0.35f).RotatedBy(Projectile.ai[0] * 0.001);

                        swingHelper.Change_Lerp(rotVector, 0.1f,Vector2.One, 0.1f, 0, 0.1f);
                        Projectile.spriteDirection = player.direction;
                        swingHelper.SetSwingActive();
                        swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.SafeNormalize(default) * (player.height * -2.6f + Math.Min(Projectile.ai[0] - 4, 15) * 2), -ChargeBladeProj.SwingLength * 0.6f, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
                        #region 盾的更新
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.2f), -player.direction);
                        shield.VisualRotation = 0.1f;
                        shield.AxeRot = -2f - Projectile.ai[0] * 0.0015f;
                        shield.Fixed = false;
                        ChargeBladeProj.shieldCanDraw = false;
                        #endregion
                        #region 屏幕缩放shader调用
                        ScreenChange.SetScreenScale = 0.5f;
                        if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                            Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                        #endregion

                        if (Projectile.ai[0]++ > 70)
                        {
                            Projectile.ai[2]++;
                            Projectile.ai[0] = 0;
                            swingHelper.Change(rotVector, new Vector2(1, 0.4f), 0);
                        }
                        else if (Projectile.ai[0] > 40)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity * 0.5f, 3, 3, DustID.Smoke);
                                dust.velocity = Projectile.velocity.RotatedByRandom(0.2) * 0.05f * Main.rand.NextFloat();
                                dust.color = Color.White;
                                dust.alpha = 150;
                                dust.scale = 1.5f;

                                //dust = Dust.NewDustDirect(center, 3, 3, DustID.Smoke);
                                //dust.velocity = Projectile.velocity.RotatedByRandom(0.2) * -0.05f * Main.rand.NextFloat();
                                //dust.color = Color.White;
                                //dust.alpha = 150;
                                //dust.scale = 1.5f;

                                //dust = Dust.NewDustDirect(Projectile.Center, 5, 5, DustID.Smoke);
                                //dust.velocity = Projectile.velocity.RotatedByRandom(0.1) * -0.05f;
                                //dust.color = Color.Red;
                            }
                        }
                        break;
                    }
                case 1: // 挥舞
                    {
                        PreAttack = false;
                        Projectile.ai[1]++;
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / 60);
                        if (Projectile.ai[0]-- < 0) // 重置命中次数
                        {
                            TheUtility.ResetProjHit(Projectile);
                        }
                        if(Time > 2.5f)
                        {
                            Projectile.ai[0] = 0;
                            Projectile.ai[2]++;
                            Projectile.ai[1] = 0;
                            break;
                        }
                        else if(Time > 1)
                        {
                            Time = 1 + Time * 0.05f;
                        }
                        
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, -Time * (MathHelper.Pi + 0.3f));

                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(Projectile.Center + Projectile.velocity * 0.65f, 1);
                        shield.VisualRotation = 0;
                        shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;
                        if(Time > 0.3f) shield.Fixed = true;
                        shield.GP = true;
                        shield.InDef = true;
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 1.05f * player.direction);
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 0.46f * player.direction);
                        break;
                    }
                case 2: // 回去
                    {
                        PreAttack = false;
                        Projectile.ai[1]++;
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / 30);
                        if (Projectile.ai[0]-- < 0) // 重置命中次数
                        {
                            TheUtility.ResetProjHit(Projectile);
                        }
                        if (Time > 2.5f)
                        {
                            Projectile.ai[0] = 0;
                            Projectile.ai[2]++;
                            Projectile.ai[1] = 0;
                            ChargeBladeProj.chargeBladeGlobal.BottleLimitRemovalTime = 3600;
                            break;
                        }
                        else if (Time > 1)
                        {
                            Time = 1 + Time * 0.05f;
                        }

                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, -player.direction, -Time * (MathHelper.Pi + 0.3f));

                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Update(Projectile.Center + Projectile.velocity * 0.65f, 1);
                        shield.VisualRotation = 0;
                        shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;
                        if (Time > 0.3f) shield.Fixed = true;
                        shield.GP = true;
                        shield.InDef = true;
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 1.05f * player.direction);
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 0.46f * player.direction);
                        break;
                    }
                case 3: // 后摇
                    {
                        swingHelper.SwingAI(0, 0, 0);
                        ChargeBladeShield shield = ChargeBladeProj.shield;
                        shield.Fixed = false;
                        shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.9f, 0), player.direction);
                        shield.GP = false;
                        shield.InDef = false;
                        //shield.AxeRot = (shield.swingHelper.center - Projectile.Center).Length() * 0.02f;
                        Projectile.ai[0]++;
                        if (Projectile.ai[0] > 15)
                        {
                            Projectile.ai[0] = 0;
                            Projectile.ai[2]++;
                            if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 10)
                            {
                                ChargeBladeProj.chargeBladeGlobal.StatChargeBottleMax = 10;
                                ChargeBladeProj.chargeBladeGlobal.StatChargeBottle += 3;
                                if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 16)
                                {
                                    ChargeBladeProj.chargeBladeGlobal.StatChargeBottle += 2;
                                }
                                ChargeBladeProj.chargeBladeGlobal.StatCharge = 0;
                            }
                        }
                        break;
                    }
                case 4: // 退出
                    {
                        SkillTimeOut = true;
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            switch ((int)Projectile.ai[2])
            {
                case 0:
                    {
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
                        swingHelper.DrawSwingItem(lightColor);
                        ChargeBladeProj.shield.Draw(sb, lightColor);
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                            Main.Rasterizer, null, Main.Transform);
                        break;
                    }
                case >= 1:
                    {
                        swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0));
                        break;
                    }
            }
            return base.PreDraw(sb, ref lightColor);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage += 0.8f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ChargeBladeProj.chargeBladeGlobal.StatCharge += 2f;
            Projectile.ai[0] = 4;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox) || ChargeBladeProj.shield.swingHelper.GetColliding(targetHitbox);
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            CD = 0;
            if (CD > 0)
            {
                if (--CD <= 0)
                {
                    SpearsStar spearsStar = new(player, new Vector2(0.4f, 0.8f) * 3f)
                    {
                        ScaleVelocity = new Vector2(0.1f, 0.2f) * -0.7f,
                        TimeLeft = 12
                    };
                    Main.ParticleSystem_World_OverPlayers.Add(spearsStar);
                }
                return false;
            }
            if (nowSkill is not ChargeBlade_Axe_Swing_Liberate_SP_PreAttack && nowSkill is not ChargeBlade_Axe_Swing_Liberate_Super) return false;
            if (ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe) // 盾强化下
            {
                return (player.direction == 1 ? player.controlLeft : player.controlRight) && player.controlUseTile && (nowSkill as BasicChargeBladeSkill).PreAttack;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override bool ActivationCondition() => false;
        public override bool? CanDamage() => (int)Projectile.ai[2] == 1 || (int)Projectile.ai[2] == 2;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            Projectile.ai[2] = 0;
            Projectile.ai[1] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
            CD = 3600 * 2;
            //CD = 10;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.ai[2] = 0;
            Projectile.ai[1] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
            #region 屏幕缩放shader调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
            //Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
