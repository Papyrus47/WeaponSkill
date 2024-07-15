using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment.Skills
{
    public class FrostBombardmentNotUse : BasicFrostBombardment
    {
        public FrostBombardmentNotUse(FrostBombardment_Proj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.velocity *= 0;
            Projectile.extraUpdates = 0;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            if (frostBombardment.SPMove == 0) Player.ChangeDir((vel.X > 0).ToDirectionInt());

            Projectile.Center = Player.Center + new Vector2(Player.width * Player.direction * -0.8f, 0f);
            Projectile.rotation = 1.8f * Player.direction + Player.fullRotation;
            Projectile.spriteDirection = Player.direction;
            if(frostBombardment.SPMove != 0)
            {
                Projectile.position.Y += Projectile.height;
            }

            if (frostBombardment.IsUseGun && Player.controlUseTile)
            {
                Main.instance.CameraModifiers.Add(new CameraModifierToScreenPos(CameraModifierToScreenPos.GetScreenPos(Player.Center + (Main.MouseWorld - Player.Center) * 0.3f), 2));
            }
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Texture2D getTex = GetTexture();
            sb.Draw(getTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, getTex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
