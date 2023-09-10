using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_AddBottles_InChannel : ChargeBlade_AddBottles
    {
        public ChargeBlade_AddBottles_InChannel(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        /// <summary>
        /// 蓄力完成
        /// </summary>
        public bool IsChannelMax;
        public override void AI()
        {
            float lenght = ChargeBladeProj.SwingLength;
            if (IsChannelMax)
            {
                ChargeBladeProj.SwingLength /= 1.4f;
            }
            base.AI();
            ChargeBladeProj.SwingLength = lenght;

            Projectile.ai[0] -= 0.9f;
            if (Projectile.ai[0] > 15 && Projectile.ai[0] < 20)
            {
                ChargeBladeProj.Projectile.position.Y += ChargeBladeProj.SwingLength * 0.15f;
                IsChannelMax = true;
                var dust = Dust.NewDustDirect(player.Center, 5, 5, DustID.Clentaminator_Red);
                dust.velocity = Projectile.velocity * 0.05f;
                dust.position = player.Center + new Vector2(-Projectile.velocity.Y, Projectile.velocity.X).SafeNormalize(default) * Main.rand.NextFloatDirection() * Projectile.width * 0.1f + Projectile.velocity * 0.45f;
                dust.noGravity = true;
                dust.color = new Color(245, 254, 106, 255);
            }
            else
            {
                IsChannelMax = false;
            }
            if (SkillTimeOut)
            {
                Projectile.ai[2] = -1;
                SkillTimeOut = false;
            }
        }
        public override bool ActivationCondition() => player.controlUseItem && !ChargeBladeProj.shield.DefSucceeded;
        public override bool SwitchCondition()
        {
            return (base.SwitchCondition() && !player.controlUseItem) || (int)Projectile.ai[2] == -1;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            HasBottles = true;
            IsChannelMax = false;
            Projectile.ai[0] = 9;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[2] = 0;
        }
    }
}
