using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper
{
    /// <summary>
    /// 物理绳子
    /// </summary>
    public class PhysicalRope
    {
        public class Rope_Point
        {
            public Vector2 pos, oldPos;
            /// <summary>
            /// 上锁
            /// </summary>
            public bool locked;
        }
        public class Rope_Line
        {
            public Rope_Point startPoint, endPoint;
            public float Lenght;
            public Rope_Line(Rope_Point startPoint, Rope_Point endPoint, float lenght)
            {
                this.startPoint = startPoint;
                this.endPoint = endPoint;
                Lenght = lenght;
            }
        }
        public Func<float,Color> drawColor;
        public float width;
        public List<Rope_Point> rope_Points;
        public List<Rope_Line> rope_Lines;
        /// <summary>
        /// 绳索的刚性,越大就说明越强
        /// </summary>
        public int RopeRigidity;
        public Vector2 gravity;
        public PhysicalRope()
        {
            rope_Lines = new();
            rope_Points = new();
            drawColor = (_) => Color.White;
        }
        public virtual void Update()
        {
            for (int i = 0; i < rope_Points.Count; i++)
            {
                Rope_Point p = rope_Points[i];
                if (!p.locked)
                {
                    Vector2 vector = p.pos;
                    p.pos += p.pos - p.oldPos + gravity;//自由下落的点
                    p.oldPos = vector;
                }
            }

            for (int i = 0; i < RopeRigidity; i++)
            {
                for (int j = 0; j < rope_Lines.Count; j++)
                {
                    Rope_Line rope = rope_Lines[j];
                    Vector2 endTostart = rope.endPoint.pos - rope.startPoint.pos;
                    float length = endTostart.Length();
                    length = (length - rope.Lenght) / length;//求得位置
                    if (!rope.startPoint.locked)//不是锁着的点
                        rope.startPoint.pos += 0.5f * endTostart * length;
                    if (!rope.endPoint.locked)
                        rope.endPoint.pos -= 0.5f * endTostart * length;
                }
            }
        }
        public virtual void Draw()
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            List<CustomVertexInfo> customs = new(), customVertexInfos = new();
            float length = rope_Points.Count;
            //for(int i = 0; i < length; i++)
            //{
            //    sb.Draw(TextureAssets.MagicPixel.Value, rope_Points[i].pos - Main.screenPosition , null, Color.White, 0, Vector2.Zero, new Vector2(8,0.02f), SpriteEffects.None, 0f);
            //}
            for (int i = 1; i < length; i++)
            {
                float factor = i / length;
                Vector2 vector = rope_Points[i - 1].pos - rope_Points[i].pos;
                vector = new Vector2(-vector.Y, vector.X).SafeNormalize(default) / 2f;
                customs.Add(new(rope_Points[i - 1].pos + vector * width - Main.screenPosition,
                    drawColor(factor), new(factor, 0, 0)));
                customs.Add(new(rope_Points[i - 1].pos + vector * -width - Main.screenPosition,
                    drawColor(factor), new(factor, 1, 0)));
                customs.Add(new(rope_Points[i].pos + vector * width - Main.screenPosition,
                    drawColor(factor), new(factor, 0, 0)));
                customs.Add(new(rope_Points[i].pos + vector * -width - Main.screenPosition,
                    drawColor(factor), new(factor, 1, 0)));
            }
            for (int i = 0; i < customs.Count - 2; i += 2)
            {
                customVertexInfos.Add(customs[i]);
                customVertexInfos.Add(customs[i + 2]);
                customVertexInfos.Add(customs[i + 1]);

                customVertexInfos.Add(customs[i + 1]);
                customVertexInfos.Add(customs[i + 2]);
                customVertexInfos.Add(customs[i + 3]);
            }
            gd.Textures[0] = TextureAssets.MagicPixel.Value;
            gd.SamplerStates[0] = SamplerState.PointClamp;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertexInfos.ToArray(), 0, customVertexInfos.Count / 3);
        }
    }
}
