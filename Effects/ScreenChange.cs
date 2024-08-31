using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace WeaponSkill.Effects
{
    public class ScreenChange : ScreenShaderData
    {
        /// <summary>
        /// 这个是缩放
        /// </summary>
        public static float SetScreenScale;
        public ScreenChange(string passName) : base(passName)
        {
        }
        public ScreenChange(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Apply()
        {
            //switch (_passName)
            //{
            //    case "ScaleScreen": // 启用缩放shader
            //        UseIntensity(1); // 这个处理屏幕缩放大小
            //        break;
            //}
            base.Apply();
        }
        public override void Update(GameTime gameTime)
        {
            switch (_passName)
            {
                case "ScaleScreen": // 启用缩放shader
                    if (!Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                        UseIntensity(MathHelper.Lerp(Intensity, 1, 0.3f)); // 这个处理屏幕缩放大小
                    else
                        UseIntensity(MathHelper.Lerp(Intensity, SetScreenScale, 0.3f)); // 这个处理屏幕缩放大小
                    break;
            }
        }
    }
}
