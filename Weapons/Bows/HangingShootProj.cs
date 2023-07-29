using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Bows
{
    public class HangingShootProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Projectile.Size = ContentSamples.ProjectilesByType[ProjectileID.WoodenArrowFriendly].Size;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.scale = 1;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ai[0]++;
            if(Projectile.ai[0] >= 15)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloatDirection() * 15,0), ModContent.ProjectileType<StoneProj>(), Projectile.damage / 15, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool ShouldUpdatePosition() => Projectile.ai[0] < 15;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 15)
            {
                Main.instance.LoadProjectile(ProjectileID.WoodenArrowFriendly);
                Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileID.WoodenArrowFriendly].Value, Projectile.Center - Main.screenPosition, null, lightColor,Projectile.rotation, TextureAssets.Projectile[ProjectileID.WoodenArrowFriendly].Size() * 0.5f,2,SpriteEffects.None,0f);
            }
            return false;
        }
    }
}
