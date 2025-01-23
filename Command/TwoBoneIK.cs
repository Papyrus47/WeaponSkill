using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command
{
    public class TwoBoneIK
    {
        public TwoBoneIK(BoneData fristBone, BoneData nextBone)
        {
            FristBone = fristBone;
            NextBone = nextBone;
        }
        /// <summary>
        /// 肢体的数据处理
        /// </summary>
        public struct BoneData
        {
            public Texture2D texture2D;
            public float BoneLenght;
            public float Rot;
            public Vector2 origin;
            public float scale;
            public SpriteEffects spriteEffects;
            public Rectangle? frame;

            public BoneData()
            {
                frame = null;
                texture2D = null;
                BoneLenght = 0;
                Rot = 0;
                origin = default;
                spriteEffects = SpriteEffects.None;
                scale = 1;
            }

            public BoneData(Texture2D texture2D, float boneLenght, float rot, Vector2 origin, float scale, SpriteEffects spriteEffects, Rectangle? frame)
            {
                this.texture2D = texture2D;
                BoneLenght = boneLenght;
                Rot = rot;
                this.origin = origin;
                this.scale = scale;
                this.spriteEffects = spriteEffects;
                this.frame = frame;
            }
        }
        /// <summary>
        /// 起点
        /// </summary>
        public Vector2 StartPoint;
        /// <summary>
        /// 终点
        /// </summary>
        public Vector2 EndPoint;
        /// <summary>
        /// 上下颠倒
        /// </summary>
        public bool ChangeDir;
        public BoneData FristBone,NextBone;

        public void Draw(SpriteBatch sb,Color color)
        {
            Vector2 startToEnd = StartPoint - EndPoint;
            float startToEndLenght = startToEnd.Length();
            float fristBoneLenght = FristBone.BoneLenght * FristBone.scale;
            float nextBoneLenght = NextBone.BoneLenght * NextBone.scale;
            float rot = CosineLaw_GetABCRot(Math.Clamp(startToEndLenght, 2, fristBoneLenght + nextBoneLenght - 1), fristBoneLenght, nextBoneLenght);
            rot = rot * (ChangeDir ? 1 : -1) + startToEnd.ToRotation();
            Vector2 vel = rot.ToRotationVector2();

            Texture2D tex1 = FristBone.texture2D;
            Texture2D tex2 = NextBone.texture2D;
            sb.Draw(tex1, StartPoint - Main.screenPosition, FristBone.frame, color, rot + FristBone.Rot, FristBone.origin, FristBone.scale, FristBone.spriteEffects, 0f);
            Vector2 vector2 = StartPoint - vel * fristBoneLenght;
            sb.Draw(tex2, vector2 - Main.screenPosition, NextBone.frame, color, (vector2 - EndPoint).ToRotation() + NextBone.Rot, NextBone.origin, NextBone.scale, NextBone.spriteEffects, 0f);
        }
        /// <summary>
        /// 根据长度ab，bc，ac，获取ABC的旋转角度
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="bc"></param>
        /// <param name="ac"></param>
        /// <returns>弧度的旋转角度</returns>
        public static float CosineLaw_GetABCRot(float ab, float bc, float ac)
        {
            float rot = ((ab * ab) + (bc * bc) - (ac * ac)) / (2 * ab * ac);
            rot = MathF.Acos(rot);

            return rot;
        }
    }
}
