using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using WeaponSkill.Effects;
using WeaponSkill.Command;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 大 解
    /// </summary>
    public class ChargeBlade_Axe_Swing_Liberate_Large : ChargeBlade_Axe_Swing_Liberate
    {
        public ChargeBlade_Axe_Swing_Liberate_Large(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade, activationConditionFunc)
        {
            StartVel = -Vector2.UnitX.RotatedBy(0.3);
            VelScale = new Vector2(1, 1f);
            VisualRotation = 0;
            SwingRot = MathHelper.Pi;
            SwingDirectionChange = true;
        }
        public override void AI()
        {
            base.AI();
            player.velocity.X *= 0;
            if ((int)Projectile.ai[0] == 1)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 3, 0.1f, 15, -1));
                #region 屏幕缩放shader调用
                ScreenChange.SetScreenScale = 0.8f;
                if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                    Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
                #endregion
                if (Projectile.ai[1] > SLASH_TIME * 0.6f)
                {
                    Projectile.ai[1] += 0.36f;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage += 4;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0 && UseBottles && !Rehit[target.whoAmI])
            {
                for (int i = 0; i < 2; i++)
                {
                    WeaponSkillGlobalNPC.AddComponent(target, new LiberateOnHit(25 * (i + 1), basicChargeBlade, player));
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe)
            {
                return (player.direction == 1 ? player.controlLeft : player.controlRight) && player.controlUseItem && (nowSkill as BasicChargeBladeSkill).PreAttack;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            #region 屏幕缩放shader调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override bool ActivationCondition()
        {
            return !ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe;
        }
    }
}
