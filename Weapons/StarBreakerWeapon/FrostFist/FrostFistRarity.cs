using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFistRarity : ModRarity
    {
        public override Color RarityColor => new Color((byte)(Main.DiscoR * 0.5f), (byte)(Main.DiscoG * 0.2f), (byte)(Main.DiscoB * 0.7f)) * 2f;
        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}
