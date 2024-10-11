using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    /// <summary>
    /// 没有使用,背负
    /// </summary>
    public class SSB_NoUse : SSB_BasicSkills
    {
        public SSB_NoUse(StarSpinBladeProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            SwingHelper.Change_Lerp(Vector2.UnitY.RotatedBy(0.2), 0.1f, Vector2.One, 0.1f, 0, 0.1f);
            Projectile.spriteDirection = Player.direction;
            SwingHelper.SetSwingActive();
            SwingHelper.ProjFixedPlayerCenter(Player);
            SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, 0);
            Projectile.position.X -= Player.width * Player.direction;
            Projectile.position.Y -= Player.height;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            SwingHelper.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
    }
}
