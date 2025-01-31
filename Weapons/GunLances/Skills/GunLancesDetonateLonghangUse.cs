using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Dusts.Particles;
using WeaponSkill.Effects;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesDetonateLonghangUse : BasicGunLancesSkill
    {
        public Func<bool> ActivationConditionFunc;
        public GunLancesDetonateLonghangUse(GunLancesProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile)
        {
            ActivationConditionFunc = activationConditionFunc;
        }
        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            #region 炮击
            //swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.3f);
            Projectile.spriteDirection = player.direction;
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, MathHelper.Pi * Math.Min(1,Projectile.ai[1] / 15));
            if (Projectile.ai[1]++ > 60)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[1] == 20) // 产生起爆龙杭
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 0.5f, Projectile.velocity.SafeNormalize(default), ModContent.ProjectileType<GunLancesDetonateLonghang>(), Projectile.damage * 3, 0f, player.whoAmI);
            }
            #endregion
            #region 盾更新
            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(player.Center, player.direction);
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
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke() && GunLancesProj.GunLancesGlobalItem.HasLongHang;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.Change(-Vector2.UnitX, new Vector2(1,0.2f));
            GunLancesProj.GunLancesGlobalItem.HasLongHang = false;
            #region 屏幕缩放shader调用
            ScreenChange.SetScreenScale = 0.8f;
            if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Activate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            swingHelper.SetRotVel(0);
            #region 屏幕缩放shader取消调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
    }
}
