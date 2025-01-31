using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesHeld : BasicGunLancesSkill
    {
        public GunLancesHeld(GunLancesProj modProjectile) : base(modProjectile)
        {
        }

        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(-0.5);
            Projectile.spriteDirection = player.direction;
            //swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[1] <= 0)
            {
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
            }
            #region 盾的更新

            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(Projectile.Center, player.direction);
            #endregion
            #region 部位更新
            Vector2 vel = Vector2.Zero;
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
            {
                handle.Update();
                vel = handle.Size;
            }
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
            {
                gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel, 1f);
                gun.Update();
                gun.Rot = MathHelper.Lerp(gun.Rot, 0, 0.2f);
            }
            #endregion
            Projectile.ai[0]++;

            if (Projectile.ai[0] > 120)
            {
                Projectile.ai[0] = 0;
                SkillTimeOut = true;
            }
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                player.velocity.X = 10 * -player.direction;
            }
            if (WeaponSkill.SpKeyBind.JustPressed && Projectile.ai[1] <= 0)
            {
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50;
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 15;
                player.velocity.Y = -5;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            //swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
                gun.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
                handle.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[0] >= 30 && Projectile.ai[1] <= 0;
    }
}
