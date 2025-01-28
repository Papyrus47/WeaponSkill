using Terraria.Localization;
using WeaponSkill.Weapons.HuntingHorn.Buffs;

namespace WeaponSkill.Weapons.HuntingHorn
{
    /// <summary>
    /// 狩猎笛的旋律
    /// </summary>
    public abstract class HuntingHornMelody
    {
        public HuntingHornMelody()
        {
        }
        public enum MelodyType : byte
        {
            Left = 1, Right = 2,LeftAndRight = 3,SP = 4,None = 255
        }
        public bool IsRegister;
        public Queue<MelodyType> melodies = new(4);
        public Dictionary<string, HuntingHornBuff> DefHuntingHornBuff = new();

        /// <summary>
        /// 应该按照旋律多到旋律少的地方排序,应该在添加旋律的时候判断(需要重写)
        /// </summary>
        public virtual HuntingHornBuff FindMelody(Player player, Projectile projectile) 
        {
            MelodyType[] findMelodies = melodies.ToArray();

            if (findMelodies.Length >= 4) // 四个旋律效果
            {
                HuntingHornBuff huntingHornBuff = GetFourMelodies(findMelodies, findMelodies.Length);
                if(huntingHornBuff != null) 
                    return huntingHornBuff;
            }
            if (findMelodies.Length >= 3) // 三个旋律效果
            {
                HuntingHornBuff huntingHornBuff = GetThreeMelodies(findMelodies, findMelodies.Length);
                if (huntingHornBuff != null)
                    return huntingHornBuff;
            }
            if (findMelodies.Length >= 2) // 只有两个旋律效果
            {
                HuntingHornBuff huntingHornBuff = GetTwoMelodies(findMelodies, findMelodies.Length);
                if (huntingHornBuff != null)
                    return huntingHornBuff;
            }
            return null;
        }
        ///// <summary>
        ///// UI绘制
        ///// </summary>
        ///// <param name="spriteBatch"></param>
        //public virtual void DrawUI(SpriteBatch spriteBatch,Vector2 drawPos) { }
        /// <summary>
        /// 注册
        /// </summary>
        public virtual void Register() 
        {
            DefHuntingHornBuff.Add("SelfPowerUp", new SelfPowerUp([MelodyType.Left, MelodyType.Left]));
            DefHuntingHornBuff.Add("HitSound", new HitSound([MelodyType.SP, MelodyType.Left]));
        }
        /// <summary>
        /// UI绘制的乐谱颜色
        /// </summary>
        /// <param name="melody"></param>
        /// <returns></returns>
        public virtual Color DrawColor(MelodyType melody) => Color.White;
        public virtual HuntingHornBuff GetFourMelodies(MelodyType[] findMelodies, int Lenght)
        {
            return null;
        }
        public virtual HuntingHornBuff GetThreeMelodies(MelodyType[] findMelodies, int Lenght)
        {
            return null;
        }
        public virtual HuntingHornBuff GetTwoMelodies(MelodyType[] findMelodies, int Lenght)
        {
            switch (findMelodies[Lenght - 1]) // 自我强化旋律
            {
                case MelodyType.Left when findMelodies[Lenght - 2] == MelodyType.Left:
                    return DefHuntingHornBuff["SelfPowerUp"];
                case MelodyType.Left when findMelodies[Lenght - 2] == MelodyType.SP:
                    return DefHuntingHornBuff["HitSound"];
            }
            return null;
        }
        /// <summary>
        /// 在物品的背包里面调用
        /// </summary>
        public virtual void Update()
        {
            if (melodies.Count > 4)
                melodies.Dequeue(); // 取出元素
            if (!IsRegister)
            {
                IsRegister = true;
                Register();
            }
        }
    }
}
