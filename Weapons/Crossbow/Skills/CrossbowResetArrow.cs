using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Crossbow.Skills
{
    public class CrossbowResetArrow : BasicCrossbowSkill
    {
        public CrossbowResetArrow(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.extraUpdates = 0;
            player.itemTime = player.itemAnimation = 5;
            player.heldProj = Projectile.whoAmI;
            Projectile.ai[1]++;
            Projectile.spriteDirection = player.direction;
            if (Projectile.ai[1] < 10) // 旋转武器,让其进入装弹动作
            {
                Projectile.Center += (player.Center - Projectile.Center) * 0.8f + Projectile.velocity * 15 * player.direction;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0.2f * player.direction, 0.5f);
            }
            else
            {
                Projectile.Center = player.Center + Projectile.velocity * 15 * player.direction;
                int time = (int)(Projectile.ai[1] - 10);
                player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) + (time - 10) * -0.1f * player.direction;
                if (time > 12)
                {
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.3f }, player.position);
                    WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
                    Item item = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
                    if (CrossProj.SpawnItem.GetGlobalItem<CrossbowGlobalItem>().CrossbowLoadArrow.TryGetValue(item, out var @ref))
                    {
                        @ref.Value = 10;
                    }
                    SkillTimeOut = true;
                }
            }
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
        }
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = 0;
            SkillTimeOut = false;
        }
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition() => false;
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = Projectile.ai[1] = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(CrossProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
