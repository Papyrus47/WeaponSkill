using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Axes.Skills
{
    public class AxesSpurts : AxesSwing
    {
        public AxesSpurts(AxesProj axeProj, Func<bool> changeCondition) : base(axeProj, changeCondition)
        {
            StartVel = (-Vector2.UnitX).RotatedBy(-0.1);
            VelScale = new Vector2(1, 0.1f);
            VisualRotation = 0;
            SwingRot = MathHelper.Pi * 0.95f;
            AddKn = 2;
        }
        public override void AI()
        {
            base.AI();
            Vector2 projVel = Projectile.velocity * 0.6f;
            Vector2 vel = Collision.TileCollision(Projectile.Center,projVel , (int)(Projectile.width * Projectile.scale), (int)(Projectile.width * Projectile.scale));
            if (Projectile.ai[0] < 0.5f)
            {
                player.velocity.X = player.direction * 15;
            }
            else
            {
                Projectile.ai[0] *= 1.01f;
                player.velocity.X *= 0.6f;
            }
            if (vel != projVel) player.velocity = -vel;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            return Projectile.ai[0] < 0.2f && ActivationCondition(); // 挥舞的ai0小于0.2的时候可以接突刺
        }
        public override bool ActivationCondition() => player.controlUseTile && player.controlUseItem;
    }
}
