using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.ResidueSwing
{
    /// <summary>
    /// 刀光消失
    /// </summary>
    public class ResidueSwingRender : IRenderTargetShaderDraw
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
            #region 绘制所有的残影上去
            // 最要命时刻
            // 将所有绘制的刀光全部Begin 与 End
            // 我这边建议,再写一个shader,让刀光同时自己消失与读取颜色
            gd.SetRenderTarget(WeaponSkill.MyRender);
            gd.Clear(Color.Transparent);
            ResidueSwing.Instance.Draw(); // 没错,我自己都不启动Begin与End
            #endregion
            #region 哦天啊,画画布上去了
            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.Draw(WeaponSkill.MyRender, Vector2.Zero, Color.White);
            sb.End();
            #endregion
        }

        public void ResetDrawData()
        {
            ResidueSwing.Instance.Update();
        }
    }
}
