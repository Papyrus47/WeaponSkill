using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Staffs.Skills;

namespace WeaponSkill.Weapons.Staffs
{
    /// <summary>
    /// 魔法杖的设置
    /// </summary>
    public class MagicStaffsSetting_Swing : MagicStaffsSetting
    {
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float SwingRot;
        public bool SwingDirectionChange;
        public float ChangeLerpSpeed;
        public float VisualRotation;
        /// <summary>
        /// 挥舞所需要时间
        /// </summary>
        public float SwingTime;
        /// <summary>
        /// 挥舞变化方式
        /// </summary>
        public Func<float, float> TimeChange;
        /// <summary>
        /// 如何使用这个技能
        /// </summary>
        public Func<Player,bool> ChangeCondition;
        /// <summary>
        /// 在这里调用额外Shoot
        /// </summary>
        public Action<MagicStaffsSwing> Shoot;
        /// <summary>
        /// 在这里使用特定AI
        /// </summary>
        public Action<MagicStaffsSwing> OnUse;
    }
}
