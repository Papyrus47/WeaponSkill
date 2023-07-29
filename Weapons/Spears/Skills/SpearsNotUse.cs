using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Spears.Skills
{
    public class SpearsNotUse : BasicSpearsSkill
    {
        public SpearsNotUse(SpearsProj SpearsProj) : base(SpearsProj)
        {
        }

        public override void AI()
        {
            Projectile.Center = player.Center + new Vector2(player.direction * -6, 0);
            Projectile.rotation += Math.Min(MathHelper.WrapAngle((MathHelper.Pi * 0.35f) * player.direction - Projectile.rotation) * 0.3f,MathHelper.PiOver4 * 0.25f);
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.2f * player.direction);
            Projectile.spriteDirection = player.direction;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.instance.LoadProjectile(SpearsProj.SpawnItem_OriginShootProj);
            var tex = TextureAssets.Projectile[SpearsProj.SpawnItem_OriginShootProj].Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

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
