using Microsoft.Xna.Framework.Graphics;
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
            public Vector2[] oldVels;
            public PartSwingHelper Onwer;

            public Part(PartSwingHelper onwer)
            {
                Onwer = onwer;
                oldVels = new Vector2[onwer.oldVels.Length];
            }

            public Asset<Texture2D> DrawTex;
            public virtual void Update()
            {
                Vector2 vel = Onwer.velocity;
                vel = vel.RotatedBy(Rot);
                #region 保存oldXXX
                for (int i = oldVels.Length - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        oldVels[0] = vel;
                    }
                    else
                    {
                        oldVels[i] = oldVels[i - 1];
                    }
                }
                #endregion
            }
            public virtual Vector2 GetOldVel(int i, bool notFilp = true)
            {
                if (i < 0)
                {
                    return oldVels[0].RotatedBy(notFilp.ToDirectionInt() * Onwer.rotation * Onwer.spriteDirection);
                }
                return oldVels[i].RotatedBy(notFilp.ToDirectionInt() * Onwer.rotation * Onwer.spriteDirection);
            }
            public virtual void DrawTrailing(Texture2D tex, Func<float, Color> colorFunc, Effect effect, Func<float, float> SetZ = null)
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
                    Vector2 pos = OffestCenter + Onwer.Center;
                    //if (_drawCorrections)
                    //{
                    //    pos = Center + (pos - Center);
                    //}
                    if (effect == null || Onwer.UseShaderPass == 1)
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
                    effect?.CurrentTechnique.Passes[Onwer.UseShaderPass].Apply();
                    gd.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
                    //gd.RasterizerState = origin;
                }
            }
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

                gd.Textures[0] = DrawTex.Value;
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
        public override void SwingAI(float velLength, int dir, float Rot)
        {
            base.SwingAI(velLength, dir, Rot);
            foreach (var part in Parts.Values)
            {
                part.Update();
            }
        }
        public override void DrawTrailing(Texture2D tex, Func<float, Color> colorFunc, Effect effect, Func<float, float> SetZ = null)
        {
            base.DrawTrailing(tex, colorFunc, effect, SetZ);
            foreach (var part in Parts.Values)
            {
                part.DrawTrailing(tex,colorFunc,effect,SetZ);
            }
        }
        public override void DrawSwingItem(Color drawColor)
        {
            base.DrawSwingItem(drawColor);
            foreach(var part in Parts.Values)
            {
                part.Draw(drawColor);
            }
        }
    }
}
