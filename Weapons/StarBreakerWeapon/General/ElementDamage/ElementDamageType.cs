namespace WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage
{
    /// <summary>
    /// 属性伤害
    /// </summary>
    public abstract class ElementDamageType
    {
        public abstract string DamageName { get; }
        public virtual Color HitColor => Color.White;
        /// <summary>
        /// 伤害的处理
        /// </summary>
        public StatModifier statModifier = StatModifier.Default;
        public virtual void SetDefaults()
        {
            statModifier = StatModifier.Default;
        }
        /// <summary>
        /// 基础伤害
        /// </summary>
        public int baseDamage;
        /// <summary>
        /// 被应用的时候
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="dmgDone">最终伤害</param>
        public virtual void OnApply(NPC npc,int dmgDone) { }
        /// <summary>
        /// 被应用的时候
        /// </summary>
        /// <param name="dmgDone">最终伤害</param>
        public virtual void OnApply(Player player, int dmgDone) { }
        /// <summary>
        /// 获取应该的元素伤害
        /// </summary>
        /// <returns></returns>
        public virtual float GetElementDamage() => statModifier.ApplyTo(baseDamage);
    }
}
