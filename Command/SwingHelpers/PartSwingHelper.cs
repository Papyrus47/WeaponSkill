using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SwingHelpers
{
    /// <summary>
    /// 携带部件的挥舞帮助类>
    /// <para>直接调用原函数是全部一起运动</para>
    /// </summary>
    public partial class PartSwingHelper : SwingHelper
    {
        /// <summary>
        /// 部件
        /// </summary>
        public class Part
        {
            /// <summary>
            /// 中心偏移
            /// </summary>
            public Vector2 OffestCenter;
            /// <summary>
            /// 旋转角度
            /// </summary>
            public float Rot;
            public PartSwingHelper Onwer;
            public virtual void Draw(Color drawColor)
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (Onwer.projectile != null)
                {
                    Onwer.SwingItemTex ??= TextureAssets.Projectile[Onwer.projectile.type];
                }
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;

                Vector2 velocity = Onwer.GetOldVel(-1, true);
                velocity = velocity.RotatedBy(Rot);
                Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(Onwer.VisualRotation * Onwer.spriteDirection).SafeNormalize(default)
                    * Onwer._halfSizeLength * Onwer.spriteDirection;

                Vector2 center = Onwer.GetDrawCenter();
                center += OffestCenter;
                if (Onwer._drawCorrections)
                {
                    center = Onwer.Center + (center - Onwer.Center);
                }
                Vector2 halfVelPos = center + velocity * 0.5f;
                Vector2[] pos = new Vector2[4]
                {
                    center - Main.screenPosition,
                    halfVelPos - halfLength - Main.screenPosition,
                    center + velocity - Main.screenPosition,
                    halfVelPos + halfLength  - Main.screenPosition
                };

                float factor = (Onwer.frame + 1f) / Onwer.frameMax;
                CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
                customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
                customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
                customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
                customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

                gd.Textures[0] = Onwer.SwingItemTex.Value;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
                //gd.RasterizerState = origin;
            }
        }
        public PartSwingHelper(object spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(spawnEntity, oldVelLength, swingItemTex)
        {
        }
        /// <summary>
        /// 储存部件的字典,使用字符串ID的形式
        /// </summary>
        public Dictionary<string, Part> Parts = new();
        public override void DrawSwingItem(Color drawColor)
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (projectile != null)
            {
                SwingItemTex ??= TextureAssets.Projectile[projectile.type];
            }
            //var origin = gd.RasterizerState;
            //RasterizerState rasterizerState = new()
            //{
            //    CullMode = CullMode.None,
            //    FillMode = FillMode.WireFrame
            //};
            //gd.RasterizerState = rasterizerState;

            Vector2 velocity = GetOldVel(-1, true);
            Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(VisualRotation * spriteDirection).SafeNormalize(default)
                * _halfSizeLength * spriteDirection;

            Vector2 center = GetDrawCenter();
            if (_drawCorrections)
            {
                center = Center + (center - Center);
            }
            Vector2 halfVelPos = center + velocity * 0.5f;
            Vector2[] pos = new Vector2[4]
            {
                 center - Main.screenPosition,
                 halfVelPos - halfLength - Main.screenPosition,
                 center + velocity - Main.screenPosition,
                 halfVelPos + halfLength  - Main.screenPosition
            };

            float factor = (frame + 1f) / frameMax;
            CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
            customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
            customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
            customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

            gd.Textures[0] = SwingItemTex.Value;
            //gd.Textures[0] = TextureAssets.MagicPixel.Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
            //gd.RasterizerState = origin;
            foreach(var part in Parts.Values)
            {
                part.Draw(drawColor);
            }
        }
    }
}
