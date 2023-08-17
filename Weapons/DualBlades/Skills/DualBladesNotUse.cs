using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBladesNotUse : BasicDualBladesSkill
    {
        public DualBladesNotUse(DualBladesProj dualBladesProj) : base(dualBladesProj)
        {
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(-player.direction * Projectile.width * 0.3f,0);
            Projectile.spriteDirection = player.direction;
            Projectile.rotation = (MathHelper.PiOver4 + 1.2f)* Projectile.spriteDirection;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Texture2D tex = bladesProj.DrawProjTex.Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), lightColor, Projectile.rotation + (0.5f * Projectile.spriteDirection), tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
