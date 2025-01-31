using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesNoUse : BasicGunLancesSkill
    {
        public GunLancesNoUse(GunLancesProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = -Vector2.UnitY;
            Projectile.spriteDirection = player.direction;
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -GunLancesProj.SwingLength * 0.6f, true);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
            #region 盾的更新

            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(Projectile.Center, -player.direction);
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
                gun.OffestCenter = Vector2.Lerp(gun.OffestCenter,Projectile.velocity.SafeNormalize(default) * vel * gun.Size.Length() * 0.02f + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * -player.direction).SafeNormalize(default) * vel * 0.4f,0.2f);
                gun.Update();
                gun.Rot = MathHelper.Lerp(gun.Rot,MathHelper.Pi * 1,0.2f);
            }
            #endregion
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
    }
}
