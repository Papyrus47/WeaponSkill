using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.LongSword
{
    public class LongSwordScabbard
    {
        public Asset<Texture2D> ScabbardTex;

        public LongSwordScabbard(Asset<Texture2D> scabbardTex)
        {
            ScabbardTex = scabbardTex;
            ScabbardTex ??= ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/DefaultLongSwordScabbard");
            Lenght = ScabbardTex.Size().Length();
            DrawAction = (da) =>
            {
                da.DrawOrigin = new Vector2(da.ScabbardTex.Width() * (da.Dir == -1 ? 0.7f : 0.3f), da.ScabbardTex.Height() * 0.7f);
            };
        }

        public Projectile projectile;
        public Vector2 DrawPos;
        public float Lenght;
        public float Rot;
        public float VisualRotation;
        public Vector2 DrawOrigin;
        public int Dir;
        public int FrameMax, Frame;
        public Action<LongSwordScabbard> DrawAction;
        public virtual void Draw(SpriteBatch spriteBatch,Color drawColor)
        {
            //GraphicsDevice gd = Main.graphics.GraphicsDevice;
            //Vector2 center = projectile.Center;
            //Vector2 vel = Rot.ToRotationVector2() * Lenght;
            //Vector2 vel1 = new Vector2(vel.Y, -vel.X).RotatedBy(VisualRotation) * 0.5f * Dir;
            //Vector2[] poses = new Vector2[4]
            //{
            //    center - Main.screenPosition, // 起点(左下角)
            //    center + vel1 - Main.screenPosition, // 左上角
            //    center + vel - Main.screenPosition, // 顶点(右上角)
            //    center - vel1 - Main.screenPosition  // 右下角
            //};
            //if (FrameMax == 0) FrameMax = 1;
            //float factor = (Frame + 1f) / FrameMax;
            //CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            //customVertices[0] = customVertices[5] = new(poses[0], drawColor, new Vector3(0f, factor, 0)); // 柄
            //customVertices[1] = new(poses[1], drawColor, new Vector3(0f, factor - 1f, 0)); // 左上角
            //customVertices[2] = customVertices[3] = new(poses[2], drawColor, new Vector3(1f, factor - 1f, 0)); // 头
            //customVertices[4] = new(poses[3], drawColor, new Vector3(1f, factor, 0)); // 右下角

            //gd.Textures[0] = ScabbardTex.Value;
            //gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
            if (ScabbardTex == null) return;
            DrawAction?.Invoke(this);
            if (FrameMax == 0) FrameMax = 1;
            spriteBatch.Draw(ScabbardTex.Value, DrawPos - Main.screenPosition, ScabbardTex.Frame(1,FrameMax,0,Frame), Color.White, Rot, DrawOrigin, projectile.scale + 0.2f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
