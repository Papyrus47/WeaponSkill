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
                return DefHuntingHornBuff["AttackUp_Small"];
            }
            else if (findMelodies[Lenght - 1] == MelodyType.Right && findMelodies[Lenght - 2] == MelodyType.Right && findMelodies[Lenght - 3] == MelodyType.Left && findMelodies[Lenght - 4] == MelodyType.LeftAndRight)
            {
                return DefHuntingHornBuff["HealthBuff_Middle"];
            }
            return base.GetFourMelodies(findMelodies, Lenght);
        }
        public override HuntingHornBuff GetThreeMelodies(MelodyType[] findMelodies, int Lenght)
        {
            if (findMelodies[Lenght - 1] == MelodyType.Left && findMelodies[Lenght - 2] == MelodyType.LeftAndRight && findMelodies[Lenght - 3] == MelodyType.Right)
            {
                return DefHuntingHornBuff["HealthBuff_Small"];
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
        public override void Register()
        {
            base.Register();
            DefHuntingHornBuff.Add("HealthBuff_Small", new HealthBuff_Small([MelodyType.Right, MelodyType.LeftAndRight, MelodyType.Left]));
            DefHuntingHornBuff.Add("HealthBuff_Middle", new HealthBuff_Middle([MelodyType.LeftAndRight, MelodyType.Left, MelodyType.Right, MelodyType.Right]));
            DefHuntingHornBuff.Add("AttackUp_Small", new AttackUp_Small([MelodyType.Left, MelodyType.Right, MelodyType.Right, MelodyType.LeftAndRight]));
        }
    }
}
