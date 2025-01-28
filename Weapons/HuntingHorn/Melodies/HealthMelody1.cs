using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.HuntingHorn.Buffs;

namespace WeaponSkill.Weapons.HuntingHorn.Melodies
{
    /// <summary>
    /// 回复1旋律
    /// </summary>
    public class HealthMelody1 : HuntingHornMelody
    {
        public override HuntingHornBuff GetFourMelodies(MelodyType[] findMelodies, int Lenght)
        {
            if (findMelodies[Lenght - 1] == MelodyType.LeftAndRight && findMelodies[Lenght - 2] == MelodyType.Right && findMelodies[Lenght - 3] == MelodyType.Right && findMelodies[Lenght - 4] == MelodyType.Left)
            {
                return new AttackUp_Small([MelodyType.Left,MelodyType.Right, MelodyType.Right, MelodyType.LeftAndRight]);
            }
            return base.GetFourMelodies(findMelodies, Lenght);
        }
        public override HuntingHornBuff GetThreeMelodies(MelodyType[] findMelodies, int Lenght)
        {
            if (findMelodies[Lenght - 1] == MelodyType.Left && findMelodies[Lenght - 2] == MelodyType.LeftAndRight && findMelodies[Lenght - 3] == MelodyType.Right)
            {
                return new HealthBuff_Small([MelodyType.Right, MelodyType.LeftAndRight, MelodyType.Left]);
            }
            return base.GetThreeMelodies(findMelodies, Lenght);
        }
        public override Color DrawColor(MelodyType melody)
        {
            switch (melody)
            {
                case MelodyType.Left:
                    return Color.White;
                case MelodyType.Right:
                    return Color.LightGreen;
                case MelodyType.LeftAndRight:
                    return Color.OrangeRed;
            }
            return base.DrawColor(melody);
        }
    }
}
