using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class SSB_Rarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(16,11,38,100),new Color(114,49,74,255),MathF.Sin(Main.GlobalTimeWrappedHourly));
        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}
