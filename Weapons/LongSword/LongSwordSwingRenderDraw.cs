using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.LongSword
{
    public class LongSwordSwingRenderDraw : IRenderTargetShaderDraw
    {
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
            sb.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend,SamplerState.AnisotropicWrap,DepthStencilState.None,RasterizerState.CullNone, null); // 即刻绘制需要的图片到对应的Render上
            //swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (_) => new(0.3f, 0.3f, 0.3f, 0f), null);
            for (int i = 0; i < LongSwordProj.DrawLongSwordSwingShader_Index.Count; i++)
            {
                SwingHelper swingHelper = (Main.projectile[LongSwordProj.DrawLongSwordSwingShader_Index[i]].ModProjectile as LongSwordProj).SwingHelper;
                swingHelper.DrawTrailing(WeaponSkill.SwingTex_Offset.Value, (i) => GetDrawOffsetColor(swingHelper, i), null);
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

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,null); // 绘制扭曲
            //gd.Textures[1] = WeaponSkill.MyRender;
            Effect effect = WeaponSkill.OffsetShader.Value;
            effect.Parameters["uOffset"].SetValue(0.06f);
            effect.Parameters["tex"].SetValue(WeaponSkill.MyRender);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            //sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone); // 绘制刀光
            for (int i = 0; i < LongSwordProj.DrawLongSwordSwingShader_Index.Count; i++)
            {
                LongSwordProj longSwordProj = (Main.projectile[LongSwordProj.DrawLongSwordSwingShader_Index[i]].ModProjectile as LongSwordProj);
                SwingHelper swingHelper = longSwordProj.SwingHelper;
                swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (_) =>
                {
                    if (longSwordProj.InSpiritAttack)
                    {
                        return new Color(255,0,0,0);
                    }
                    return new Color(100,100,100,0);
                }, null);
            }
            //sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            sb.End();
            #endregion
        }
        public void ResetDrawData()
        {
            LongSwordProj.DrawLongSwordSwingShader_Index.Clear();
        }
        public Color GetDrawOffsetColor(SwingHelper swingHelper,float factor)
        {
            Vector2 vel = swingHelper.oldVels[(int)(factor * swingHelper.oldVels.Length)];
            vel = new Vector2(vel.Y, -vel.X);
            Color color = new(vel.ToRotation() / MathHelper.TwoPi,factor * 0.2f, 0, 0);
            return color;
        }
    }
}
