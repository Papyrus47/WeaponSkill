using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit_HolyStrikesBack : FrostFist_FistHit_MoveFist
    {
        public FrostFist_FistHit_HolyStrikesBack(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
        }
        public override void AI()
        {
            Player.manaRegenDelay = 0;
            if (Player.GetModPlayer<WeaponSkillPlayer>().HolyStrikesBack_OnHit && Projectile.ai[0] < 0)
            {
                Projectile.ai[0] = 0;
            }
            if (Projectile.ai[0] >= 0)
            {
                base.AI();
            }
            else
            {
                if (++Projectile.ai[0] >= 0)
                {
                    SkillTimeOut = true;
                }
                Projectile.Center = Player.Center;
                Vector2 vel = (Main.MouseWorld - Projectile.Center);
                Player.velocity.X *= 0.2f;
                Player.velocity.Y = 0;
                Player.ChangeDir((vel.X > 0).ToDirectionInt());
                Projectile.direction = Player.direction;
                Player.itemTime = Player.itemAnimation = 2;
                Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction, vel.X * Projectile.direction);
                Player.GetModPlayer<WeaponSkillPlayer>().HolyStrikesBack = true;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            if (Projectile.ai[0] < 0)
            {
                Texture2D tex = TextureAssets.Extra[60].Value;
                sb.Draw(tex, Player.Bottom - Main.screenPosition, null, new Color(0.3f, 0.5f, 1f, 0), 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                sb.Draw(tex, Player.Bottom - Main.screenPosition, null, new Color(0.3f, 0.5f, 1f, 0), 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }
            return base.PreDraw(sb, ref lightColor);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = -120;
            Player.GetModPlayer<WeaponSkillPlayer>().HolyStrikesBack_OnHit = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Player.GetModPlayer<WeaponSkillPlayer>().HolyStrikesBack_OnHit = false;
            Player.GetModPlayer<WeaponSkillPlayer>().HolyStrikesBack = false;
            FrostFist.frostFistItem.ChangeLevel += 2;
        }
    }
}
