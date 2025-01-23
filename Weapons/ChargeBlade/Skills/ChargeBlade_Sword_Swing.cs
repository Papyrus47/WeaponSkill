using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Command;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Sword_Swing : ChargeBlade_Sword_Basic
    {
        public ChargeBlade_Sword_Swing(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade)
        {
            ChangeCondition = activationConditionFunc;
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
        /// <summary>
        /// 弹刀
        /// </summary>
        public bool AttackSwap;
        /// <summary>
        /// 格挡后攻击
        /// </summary>
        public bool DefAttack;
        public Action SwingAI;
        public Action<NPC, NPC.HitInfo, int> OnHit;

        public const int SLASH_TIME = 36;
        public const int PREATTACK_TIME = 12;
        public const int TIMEOUT_TIME = 120;
        public const float CHANGE_LERP_SPEED = 0.35f;
        public override void AI()
        {
            base.AI();
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
                        if (Projectile.ai[1]++ > 8)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 1;
                            TheUtility.Player_ItemCheck_Shoot(player, ChargeBladeProj.SpawnItem, Projectile.damage);
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 挥舞进行
                    {
                        PreAttack = false;
                        if (AttackSwap)
                        {
                            Projectile.ai[1]--;
                            swingHelper.SetNotSaveOldVel();
                        }
                        else Projectile.ai[1]++;
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME);
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
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > TIMEOUT_TIME)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (DefAttack && ChargeBladeProj.shield.DefSucceeded && ChargeBladeProj.shield.KNLevel != ChargeBladeShield.KNLevelEnum.Big)
            {
                return ActivationCondition();
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            OnHit?.Invoke(target, hit, damageDone);
            TheUtility.SetPlayerImmune(player,35);
            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemTimeMax;
            ChargeBladeProj.chargeBladeGlobal.StatCharge += 0.5f;
            if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 23 && ChargeBladeProj.chargeBladeGlobal.SwordStrengthening <= 0)
            {
                AttackSwap = true;
                swingHelper.Change(StartVel, Vector2.One, 0);
            }
            if(ChargeBladeProj.chargeBladeGlobal.SwordStrengthening > 0) // 剑强化模式
            {
                (ChargeBladeProj.SpawnItem.ModItem as BasicChargeBlade).LiberateHit(target, player);
            }
            TheUtility.VillagesItemOnHit(ChargeBladeProj.SpawnItem, player, Projectile.Hitbox, hit.Damage, hit.Knockback, target.whoAmI, damageDone, damageDone);
            //ItemLoader.OnHitNPC(LongSword.SpawnItem, player,target, hit, damageDone);
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

            if (ChargeBladeProj.chargeBladeGlobal.SwordStrengthening > 0) // 剑强化模式
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

                swingHelper.DrawTrailing(TextureAssets.Extra[209].Value, (_) =>
                {
                    Color color = (ChargeBladeProj.SpawnItem.ModItem as BasicChargeBlade).LiberateColor;
                    color.A = 0;
                    return color;
                }, null);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
            else
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

                swingHelper.DrawTrailing(TextureAssets.Extra[209].Value, (_) => new Color(255,255,255,0), null);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override bool ActivationCondition()
        {
            if (DefAttack && ChargeBladeProj.DefSucceededTime <= 0) return false;
            return ChangeCondition.Invoke() /*&& (!DefAttack ? true : !WeaponSkill.BowSlidingStep.Current)*/;
        }

        public override bool SwitchCondition() => (int)Projectile.ai[0] == 2 && Projectile.ai[2] > 9;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            AttackSwap = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.extraUpdates = 0;
            AttackSwap = false;
        }
    }
}
