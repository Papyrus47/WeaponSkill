using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
            ChargeBladeProj.chargeBladeGlobal.AxeStrengthening = false;
            base.AI();
            ChargeBladeProj.chargeBladeGlobal.AxeStrengthening = AxeRot;
            player.velocity.X *= 0f;
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
                SwingDirectionChange = true;
            }
            if (SwingTwo)
            {
                PreAttack = false;
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 9, 0.1f, 15, -1));
                if (Projectile.ai[1] < SLASH_TIME * 0.2f)
                {
                    Projectile.ai[1] -= 0.7f - Projectile.ai[1] * 0.04f;
                    if (Projectile.ai[1] < 0) Projectile.ai[1] = 0;
                    if (Projectile.ai[1] < SLASH_TIME * 0.02f) player.velocity.X = player.direction * 15f;
                }
                if ((int)Projectile.ai[0] == 2 && !End)
                {
                    End = true;
                    if (ChargeBladeProj.chargeBladeGlobal.StatChargeBottle > 0) Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Projectile.velocity - Vector2.UnitY * 10, Vector2.UnitX * player.direction * 140, ModContent.ProjectileType<ChargeBlade_SuperLiberateProj>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, ChargeBladeProj.chargeBladeGlobal.StatChargeBottle);
                    ChargeBladeProj.chargeBladeGlobal.StatChargeBottle = 0;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 25, 10, 30));

                    Vector2 shieldCenter = ChargeBladeProj.shield.swingHelper.center;
                    int width = ChargeBladeProj.shield.width / 2;
                    for (int i = -width; i <= width; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            //Collision.HitTiles(new Vector2(shieldCenter.X + i, shieldCenter.Y + j * 16), Vector2.UnitY * -20,5,5);
                            int dustIndex = WorldGen.KillTile_MakeTileDust((int)((shieldCenter.X / 16) + i / 16f), (int)((shieldCenter.Y / 16) + j / 8f), Main.tile[new Point((int)((shieldCenter.X / 16) + i), (int)((shieldCenter.Y / 16) + j))]);
                            Dust dust = Main.dust[dustIndex];
                            //dust.noGravity = true;
                            dust.velocity.Y = -Main.rand.NextFloat(1,6);
                            dust.velocity.X *= 0.7f;
                        }
                    }
                }
                //else if ((int)Projectile.ai[0] == 2)
                //{

                //}
                for(int i = 0; i < Rehit.Length; i++)
                {
                    Rehit[i] = false;
                }
            }
            else
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 6f, 1f, 15, 2));
            }

        }
        public override bool? CanDamage()
        {
            if (SwingTwo && Projectile.ai[1] < SLASH_TIME * 0.02f)
            {
                return false;
            }
            return base.CanDamage();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (SwingTwo)
            {
                modifiers.SourceDamage += 5;
            }
        }
        public override bool ActivationCondition() => ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            StartVel = Vector2.UnitY.RotatedBy(-0.6);
            VelScale = new Vector2(1, 0.3f);
            SwingRot = MathHelper.TwoPi * 0.75f;
            VisualRotation = 0.7f;
            SwingDirectionChange = false;
            SwingTwo = false;
            End = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
    }
}
