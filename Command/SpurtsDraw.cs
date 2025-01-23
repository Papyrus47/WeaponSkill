using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command
{
    public class SpurtsDraw
    {
        public float Height;
        public float Width;
        public Vector2 DrawPos;
        public Vector2 Direction;
        public Color DrawColor;
        /// <summary>
        /// 如果设置为true,那么调用DrawPos的时候修正
        /// </summary>
        public bool ScreenCorrect;
        public virtual void Draw(SpriteBatch spriteBatch,Effect effect = null)
        {
            Color color = DrawColor;
            List<CustomVertexInfo> vertexInfos = new();
            Vector2 verticalVector = new(Direction.Y, -Direction.X);
            verticalVector.Normalize();
            float max = Width / 6;
            for (int i = 0;i<max; i++)
            {
                if (DrawPos == default) break;
                float factor = i / max;
                float height = Height * (-MathF.Pow((factor * 2 - 1),2) + 1) * 0.25f;
                Vector2 posCorrect = ScreenCorrect ? Main.screenPosition : Vector2.Zero;
                vertexInfos.Add(new CustomVertexInfo(DrawPos + Direction * factor * Width + verticalVector * height - posCorrect,color,new(factor,0.35f,10f)));
                vertexInfos.Add(new CustomVertexInfo(DrawPos + Direction * factor * Width - verticalVector * height - posCorrect, color, new(factor, 0.65f, 10f)));
            }
            if(vertexInfos.Count > 2)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

                effect?.CurrentTechnique.Passes[0].Apply();
                Texture2D texture = TextureAssets.Extra[196].Value;
                //Texture2D texture = TextureAssets.MagicPixel.Value;
                List<CustomVertexInfo> drawVertex = TheUtility.GenerateTriangle(vertexInfos);
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = texture;
                gd.Textures[1] = texture;
                if (gd.Textures[2] == null) gd.Textures[2] = TextureAssets.Extra[179].Value;
                //var raster = gd.RasterizerState;
                //gd.RasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame,
                //};
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
                //gd.RasterizerState = raster;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
        }
    }
}
