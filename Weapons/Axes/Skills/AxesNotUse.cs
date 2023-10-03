using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Weapons.BroadSword.Skills;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.Axes.Skills
{
    public class AxesNotUse : BasicAxesSkill
    {
        public AxesNotUse(AxesProj axeProj) : base(axeProj)
        {
        }

        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0);
            Projectile.rotation = MathHelper.Pi * 0.85f * player.direction;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.2f * player.direction);
            Projectile.spriteDirection = player.direction;
            SkillTimeOut = false;

            if (WeaponSkill.BowSlidingStep.JustPressed)
            {
                AxesProj.InFighting = !AxesProj.InFighting;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(AxesProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale - 1.6f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.rotation = 0;
        }
    }
}
