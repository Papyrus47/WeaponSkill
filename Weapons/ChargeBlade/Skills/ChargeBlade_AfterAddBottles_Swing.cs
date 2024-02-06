using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_AfterAddBottles_Swing : ChargeBlade_Sword_Swing
    {
        public ChargeBlade_AddBottles_InChannel blade_AddBottles_InChannel;

        public ChargeBlade_AfterAddBottles_Swing(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc,ChargeBlade_AddBottles_InChannel blade_AddBottles_InChannel) : base(chargeBlade, activationConditionFunc)
        {
            StartVel = Vector2.UnitY.RotatedBy(0.5);
            SwingRot = MathHelper.Pi + MathHelper.PiOver2;
            VelScale = new Vector2(1, 1f);
            VisualRotation = 0f;
            this.blade_AddBottles_InChannel = blade_AddBottles_InChannel;
        }
        public override void AI()
        {
            base.AI();
            if (blade_AddBottles_InChannel.IsChannelMax)
            {
                if ((int)Projectile.ai[0] == 1 && Projectile.ai[1] < SLASH_TIME * 0.3f && ChargeBladeProj.chargeBladeGlobal.ShieldStrengthening > 0)
                {
                    Projectile.ai[1] -= 0.8f;
                    ChargeBladeProj.chargeBladeGlobal.SwordStrengthening = 2700;
                }
                else if((int)Projectile.ai[0] == 2 && Projectile.ai[2] < 3)
                {
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 6, 5, 2));
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (blade_AddBottles_InChannel.IsChannelMax)
            {
                modifiers.SourceDamage += 3.5f;
            }
        }
        public override bool ActivationCondition() => true;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            if (blade_AddBottles_InChannel.IsChannelMax)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3);
                SwingRot = MathHelper.Pi + 0.5f;
                VelScale = new Vector2(1, 1f);
                VisualRotation = 0f;
                SwingDirectionChange = true;
            }
            else
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5);
                SwingRot = MathHelper.Pi + MathHelper.PiOver2;
                VelScale = new Vector2(1, 1f);
                VisualRotation = 0f;
                SwingDirectionChange = false;
            }
        }
    }
}
