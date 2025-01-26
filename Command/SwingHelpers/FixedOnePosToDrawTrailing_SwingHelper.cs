using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SwingHelpers
{
    public class FixedOnePosToDrawTrailing_SwingHelper : SwingHelper
    {
        public FixedOnePosToDrawTrailing_SwingHelper(object spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(spawnEntity, oldVelLength, swingItemTex)
        {
        }
        public Vector2 Pos;
        public override void ProjFixedPlayerCenter(Player player, float length = 0, bool isUseSwing = false, bool drawCorrections = false)
        {
            base.ProjFixedPlayerCenter(player, length, isUseSwing, drawCorrections);
            Pos = player.Center;
        }
        public override void ProjFixedPos(Vector2 pos, float length = 0, bool drawCorrections = false)
        {
            base.ProjFixedPos(pos, length, drawCorrections);
            Pos = pos;
        }
        public override void DrawTrailing(Texture2D tex, Func<float, Color> colorFunc, Effect effect, Func<float, float> SetZ = null)
        {
            List<CustomVertexInfo> customVertices = new();
            if (colorFunc == null)
                return;
            int length = oldVels.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 vel = GetOldVel(i, true);
                if (vel == default)
                {
                    break;
                }

                //float factor2 = EaseFunction.EaseOutQuint(i, 0f, length - 1, 0, 0.2f);
                float factor = (float)i / length;
                Color drawColor = colorFunc.Invoke(factor); // 获取绘制颜色
                //Main.NewText(drawColor);
                Vector2 pos = GetDrawCenter(i);
                if (_drawCorrections)
                {
                    pos = Center + (pos - Center);
                }
                if (Pos != default)
                    pos = Pos;
                if (effect == null || UseShaderPass == 1)
                {
                    pos -= Main.screenPosition;
                }
                float z = 0;
                if (SetZ != null) z = SetZ(factor);
                customVertices.Add(new(pos, drawColor, new Vector3(factor, 0, z)));
                customVertices.Add(new(pos + vel, drawColor, new Vector3(factor, 1, z)));
            }
            if (customVertices.Count > 4)
            {
                List<CustomVertexInfo> vertices = TheUtility.GenerateTriangle(customVertices);
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;
                //gd.Textures[0] = tex;

                gd.Textures[0] = tex;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                effect?.CurrentTechnique.Passes[UseShaderPass].Apply();
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
                //gd.RasterizerState = origin;
            }
        }
    }
}
