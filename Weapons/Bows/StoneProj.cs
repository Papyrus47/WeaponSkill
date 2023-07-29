using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Bows
{
    public class StoneProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.Size = new(32);
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.frame = Main.rand.Next(3);
        }
        public override bool PreAI()
        {
            Projectile.velocity.X *= 0.9f;
            Projectile.velocity.Y += 0.05f;
            return base.PreAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle rectangle = new(34 * Projectile.frame, 0, 34, 32);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rectangle, lightColor, 0f, rectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false; 
        }
    }
}
