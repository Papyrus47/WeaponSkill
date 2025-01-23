using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.Hammer
{
    public class HammerSwingRenderDraw : IRenderTargetShaderDraw
    {
        ~HammerSwingRenderDraw()
        {
            CanUseHammerSwingRender = null;
        }
        public static List<int> CanUseHammerSwingRender = new();
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
            for (int i = 0; i < HammerProj.DrawHammerSwingShader_Index.Count; i++)
            {
                SwingHelper swingHelper = (Main.projectile[HammerProj.DrawHammerSwingShader_Index[i]].ModProjectile as HammerProj).SwingHelper;
                //swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (i) => GetDrawOffsetColor(swingHelper, i), null);
                swingHelper.DrawTrailing(TextureAssets.Extra[193].Value, (i) => GetDrawOffsetColor(swingHelper, i), null);
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

            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform); // 绘制刀光
            //for (int i = 0; i < HammerProj.DrawHammerSwingShader_Index.Count; i++)
            //{
            //    HammerProj hammer = Main.projectile[HammerProj.DrawHammerSwingShader_Index[i]].ModProjectile as HammerProj;
            //    SwingHelper swingHelper = hammer.SwingHelper;
            //    swingHelper.DrawTrailing(WeaponSkill.SwingTex.Value, (_) =>
            //    {
            //        return hammer.GetDrawColor();
            //    }, null);
            //}
            ////sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            //sb.End();
            #endregion
        }
        public void ResetDrawData()
        {
            HammerProj.DrawHammerSwingShader_Index.Clear();
            if (!Main.LocalPlayer.HeldItem.TryGetGlobalItem<HammerGlobalItem>(out _) && !CanUseHammerSwingRender.Contains(Main.LocalPlayer.HeldItem.type))
            {
                Remove = true;
            }
        }
        public Color GetDrawOffsetColor(SwingHelper swingHelper, float factor)
        {
            Vector2 vel = swingHelper.oldVels[(int)(factor * swingHelper.oldVels.Length)];
            vel = new Vector2(vel.Y, -vel.X);
            Color color = new(vel.ToRotation() / MathHelper.TwoPi, factor * 0.3f, 0, 0);
            return color;
        }
    }
}
