﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_OnDef : ChargeBlade_Sword_Held
    {
        public ChargeBlade_OnDef(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(0.225f + Math.Sin(Projectile.ai[1]) * 0.05f);

            player.heldProj = Projectile.whoAmI;
            PreAttack = false;
            Projectile.ai[0]++;
            Projectile.ai[1] += player.velocity.X * 0.02f + 0.03f;
            if (!ActivationCondition() && SwitchCondition()) // 用于跳出技能
            {
                Projectile.ai[1] = 0;
                SkillTimeOut = true;
                return;
            }
            Projectile.extraUpdates = 0;
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -ChargeBladeProj.SwingLength * 0.45f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);

            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width, 0), player.direction);
            shield.VisualRotation = 0.3f;
            shield.AxeRot = -0.2f;
            shield.InDef = true;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.direction * -MathHelper.PiOver2);

            if (ChargeBladeProj.DefSucceededTime <= 0 && !ChargeBladeProj.shield.DefSucceeded && Projectile.ai[2] <= 0)
            {
                if (Math.Abs(player.velocity.X) > 0.5f) player.velocity.X = 0.5f * (player.velocity.X > 0).ToDirectionInt();
            }
            else
            {
                if (ChargeBladeProj.shield.DefSucceeded)
                {
                    Projectile.ai[2] = ChargeBladeProj.shield.KNLevel switch
                    {
                        ChargeBladeShield.KNLevelEnum.Small => 12,
                        ChargeBladeShield.KNLevelEnum.Medium => 25,
                        ChargeBladeShield.KNLevelEnum.Big => 40,
                        _ => 1
                    };
                    ChargeBladeProj.DefSucceededTime = 60;
                    player.immuneTime += 15;
                    Projectile.ai[0] = 0;
                }
                if(Projectile.ai[2] > 0) Projectile.ai[2]--;
                float kn = ChargeBladeProj.shield.KNLevel switch
                {
                    ChargeBladeShield.KNLevelEnum.Small => 0.4f,
                    ChargeBladeShield.KNLevelEnum.Medium => 0.4f,
                    ChargeBladeShield.KNLevelEnum.Big => 0.2f,
                    _ => 1f
                };
                player.velocity.X = Projectile.ai[2] * -player.direction * kn;
            }

            Projectile.numHits = 0;
            SkillTimeOut = false;
        }
        public override bool SwitchCondition()
        {
            return base.SwitchCondition() && ChargeBladeProj.shield.KNLevel != BasicShield.KNLevelEnum.Big;
        }
        public override bool ActivationCondition() => WeaponSkill.BowSlidingStep.Current;
    }
}
