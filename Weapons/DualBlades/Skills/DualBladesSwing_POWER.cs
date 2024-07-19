using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBladesSwing_POWER : DualBladesSwing_InDemon
    {
        public bool HitNPC;
        public int HitTime;
        public DualBladesSwing_POWER(DualBladesProj dualBladesProj) : base(dualBladesProj, SwingSet.TwoBlades, DoubleSwingSpeed.BackLow,null)
        {
            ChangeCondition = () => WeaponSkill.SpKeyBind.JustPressed && bladesGlobalItem.DemonGauge == bladesGlobalItem.DemonGaugeMax;
            SwingRot = MathHelper.TwoPi * 2;
            AITimeChange = () => 1 / 50f;
            StartVel = Vector2.One;
            VelScale = new Vector2(1, 0.8f);
            VisualRotation = 0.7f;
            ID = "POWER";
            SwingDirectionChange = true;
        }
        public override void AI()
        {
            base.AI();
            player.velocity.X = player.direction * 25;
            if (Projectile.ai[0] >= 1f)
            {
                Projectile.extraUpdates = 0;
                player.velocity.X = 0;
                player.fullRotation = 0;
                TheUtility.SetPlayerImmune(player, 6);
            }
            else
            {
                player.fullRotationOrigin = player.Size * 0.5f;
                if (HitNPC)
                {
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina--;
                    if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina <= 0)
                    {
                        HitNPC = false;
                        return;
                    }
                    if (Projectile.ai[0] > 0.4f)
                    {
                        Projectile.ai[0] = 0f;
                        Projectile.ai[1] = 0f;
                    }
                    player.velocity.Y = 5;
                    player.velocity.X = player.direction * Projectile.ai[2];
                    TheUtility.SetPlayerImmune(player);
                    if (HitTime-- == 7)
                    {
                        HeldBlade.ResetHit();
                        BackBlade.ResetHit();
                    }
                    else if (HitTime <= 0)
                    {
                        HitNPC = false;
                        Projectile.ai[0] = 1.5f;
                        Projectile.ai[1] = 1.5f;
                        player.fullRotation = 0;
                    }
                    player.fullRotation = Main.GlobalTimeWrappedHourly * 20 % MathHelper.TwoPi;
                }
                else
                {
                    Projectile.rotation = 0;
                }
            }
        }
        public override void TimeChange(out float Time, out float Time1)
        {
            base.TimeChange(out Time, out Time1);
            //if (End) Time = Time1 = 1 + Time * 0.01f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitNPC = true;
            bladesGlobalItem.AddCorrection = 0;
            player.position.Y = target.position.Y - target.height * 0.4f;
            Projectile.rotation = (target.position - player.position).ToRotation() + MathHelper.PiOver2;
            HitTime = 9;
            Projectile.ai[2] = Math.Max(target.width * 0.2f,8);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            bladesGlobalItem.DemonGauge = 0;
            player.fullRotation = 0;
            HitTime = 0;
            HitNPC = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            player.fullRotation = 0;
            player.velocity *= 0.3f;
            HitTime = 0;
            HitNPC = false;
            Projectile.ai[2] = 0;
        }
    }
}
