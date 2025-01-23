using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFist_Proj_FistHitProj_Render : IRenderTargetShaderDraw
    {
        public bool Remove { get; set; }
        public void Draw()
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            #region 保存原图片
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();
            #endregion
            #region 绘制扭曲图片到自定义render上
            gd.SetRenderTarget(WeaponSkill.MyRender);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform); // 即刻绘制需要的图片到对应的Render上
            //swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (_) => new(0.3f, 0.3f, 0.3f, 0f), null);
            for (int i = 0; i < FrostFist_Proj_FistHitProj.FistHitProjs.Count; i++)
            {
                FrostFist_Proj_FistHitProj frostFist_Proj_FistHitProj = Main.projectile[FrostFist_Proj_FistHitProj.FistHitProjs[i]].ModProjectile as FrostFist_Proj_FistHitProj;
                if(frostFist_Proj_FistHitProj == null)
                {
                    break;
                }
                frostFist_Proj_FistHitProj.FistHitProjDraw_Offset(GetDrawOffsetColor(frostFist_Proj_FistHitProj.Projectile.velocity));
            }
            //sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White); // 原始图片
            sb.End();
            #endregion
            #region 绘制扭曲
            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //sb.Draw(Main.screenTargetSwap,Vector2.Zero, Color.White); // 原始图片
            //sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone); // 绘制扭曲
            //gd.Textures[1] = WeaponSkill.MyRender;
            Effect effect = WeaponSkill.OffsetShader.Value;
            effect.Parameters["uOffset"].SetValue(0.06f);
            effect.Parameters["tex"].SetValue(WeaponSkill.MyRender);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            //sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            sb.End();
            #endregion
            #region 绘制拳击
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform); // 绘制

            for (int i = 0; i < FrostFist_Proj_FistHitProj.FistHitProjs.Count; i++)
            {
                FrostFist_Proj_FistHitProj frostFist_Proj_FistHitProj = Main.projectile[FrostFist_Proj_FistHitProj.FistHitProjs[i]].ModProjectile as FrostFist_Proj_FistHitProj;
                if (frostFist_Proj_FistHitProj == null)
                {
                    break;
                }
                frostFist_Proj_FistHitProj.FistHitProjDraw_Proj();
            }
            sb.End();
            #endregion
        }
        public void ResetDrawData()
        {
            if(FrostFist_Proj_FistHitProj.FistHitProjs.Count <= 0)
            {
                Remove = true;
            }
            FrostFist_Proj_FistHitProj.FistHitProjs.Clear();
        }
        public Color GetDrawOffsetColor(Vector2 vel)
        {
            vel = new Vector2(vel.Y, -vel.X);
            Color color = new(vel.ToRotation() / MathHelper.TwoPi,0.2f, 0, 0);
            return color;
        }
    }
}
