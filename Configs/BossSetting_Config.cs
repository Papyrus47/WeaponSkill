using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WeaponSkill.Configs
{
    public class BossSetting_Config : BasicWeaponSkillConfig<BossSetting_Config>
    {
        [DefaultValue(false)]
        public bool ResetBossAI;
        [DefaultValue(false)]
        public bool WoShiHuangLeiLong;

        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}
