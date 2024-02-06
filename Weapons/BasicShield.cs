namespace WeaponSkill.Weapons
{
    public class BasicShield
    {
        public enum KNLevelEnum : byte
        {
            Small = 1,
            Medium = 2,
            Big = 3
        }

        public int height;
        /// <summary>
        /// 击退级别
        /// </summary>
        public KNLevelEnum KNLevel;
        public float VisualRotation;
        public int width;
        public Vector2 Size
        {
            get => new(width, height);
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }
        /// <summary>
        /// 防御强度
        /// </summary>
        public int Defence;
        /// <summary>
        /// 正在防御
        /// </summary>
        public bool InDef;
        /// <summary>
        /// 防御成功攻击
        /// </summary>
        public bool DefSucceeded;
        public virtual float GetDefence() => Defence;
        /// <summary>
        /// 用于你的盾伤害计算用的
        /// </summary>
        /// <param name="hurtModifiers"></param>
        public virtual void ModifyHit(ref Player.HurtModifiers hurtModifiers)
        {
            hurtModifiers.ModifyHurtInfo += HurtModifiers_ModifyHurtInfo;
        }

        protected virtual void HurtModifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            info.Damage -= (int)GetDefence();
        }
        public virtual bool GetDefSucced(Rectangle hitbox) => false;

        /// <summary>
        /// 防御成功后函数
        /// </summary>
        public virtual void OnDefSucceeded() { }
    }
}