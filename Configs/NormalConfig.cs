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
        [DefaultValue(true)]
        public bool UseWeaponSkill;
        [DefaultValue(false)]
        public bool DarkSword;
        [DefaultValue(false)]
        public bool PickBrokenTile;
        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}
