using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WeaponSkill.Configs
{
    internal class NormalConfig : BasicWeaponSkillConfig<NormalConfig>
    {
        [DefaultValue(false)]
        public bool DarkSword;
        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}
