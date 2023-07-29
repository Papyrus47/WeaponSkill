using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.Shortsword.Skills
{
    public class ShortswordNotUse : BasicShortswordSkill
    {
        public ShortswordNotUse(ShortswordProj broadSwordProj) : base(broadSwordProj)
        {
        }

        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * 5, 0);
            Projectile.rotation = MathHelper.Pi * 0.35f * player.direction;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.2f * player.direction);
            Projectile.spriteDirection = player.direction;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            var tex = TextureAssets.Item[Shortsword.SpawnItem.type].Value;
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
