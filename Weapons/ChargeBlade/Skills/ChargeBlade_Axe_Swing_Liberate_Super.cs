using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Axe_Swing_Liberate_Super : ChargeBlade_Axe_Swing
    {
        public ChargeBlade_Axe_Swing_Liberate_Super(ChargeBladeProj chargeBlade) : base(chargeBlade, null)
        {
        }
        public bool SwingTwo;
        public bool End;
        public override void AI()
        {
            bool AxeRot = ChargeBladeProj.chargeBladeGlobal.AxeStrengthening;
            ChargeBladeProj.chargeBladeGlobal.AxeStrengthening  = false;
            base.AI();
            ChargeBladeProj.chargeBladeGlobal.AxeStrengthening = AxeRot;
            player.velocity.X *= 0;
            if ((int)Projectile.ai[0] == 2 && !SwingTwo)
            {
                SwingTwo = true;
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;
                Projectile.ai[2] = 0;
                StartVel = -Vector2.UnitX;
                SwingRot = MathHelper.Pi * 1.03f;
                VelScale = new Vector2(1, 1f);
                VisualRotation = 0;
            }
            if (SwingTwo)
            {
                PreAttack = false;
                if (Projectile.ai[1] < SLASH_TIME * 0.2f)
                {
                    Projectile.ai[1] -= 0.4f;
                }
                if ((int)Projectile.ai[0] == 2 && !End)
                {
                    End = true;
                    if(ChargeBladeProj.chargeBladeGlobal.StatChargeBottle > 0) Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Projectile.velocity - Vector2.UnitY * 10, Vector2.UnitX * player.direction * 140, ModContent.ProjectileType<ChargeBlade_SuperLiberateProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ChargeBladeProj.chargeBladeGlobal.StatChargeBottle);
                    ChargeBladeProj.chargeBladeGlobal.StatChargeBottle = 0;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 7, 20, 10));
                }
            }
            else
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 1.2f * Main.rand.Next(-1,2), 0.1f, 3,-1));
            }
        }
        public override bool ActivationCondition() => ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            StartVel = Vector2.UnitY.RotatedBy(0.6);
            VelScale = new Vector2(1, 0.3f);
            SwingRot = MathHelper.TwoPi * 0.85f;
            VisualRotation = 0.7f;
            SwingTwo = false;
            End = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
    }
}
