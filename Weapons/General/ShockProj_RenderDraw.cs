using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;

namespace WeaponSkill.Weapons.General
{
    public class ShockProj_RenderDraw : IRenderTargetShaderDraw
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
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null); // 即刻绘制需要的图片到对应的Render上
            //swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (_) => new(0.3f, 0.3f, 0.3f, 0f), null);
            for(int i =0;i< ShockProj.ShockProjIndex.Count; i++)
            {
                ShockProj shock = Main.projectile[ShockProj.ShockProjIndex[i]].ModProjectile as ShockProj;
                if (shock == null)
                {
                    break;
                }
                shock.DrawShock();
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

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null); // 绘制扭曲
            //gd.Textures[1] = WeaponSkill.MyRender;
            Effect effect = WeaponSkill.OffsetShader.Value;
            effect.Parameters["uOffset"].SetValue(0.06f);
            effect.Parameters["tex"].SetValue(WeaponSkill.MyRender);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            //sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            sb.End();
            #endregion
        }

        public void ResetDrawData()
        {
            if (ShockProj.ShockProjIndex.Count == 0)
            {
                Remove = true;
            }
            ShockProj.ShockProjIndex.Clear();
        }
        public static Color GetDrawOffsetColor(Vector2 vel,float g)
        {
            vel = new Vector2(vel.Y, -vel.X);
            Color color = new(vel.ToRotation() / MathHelper.TwoPi, g, 0, 0);
            return color;
        }
    }
}
