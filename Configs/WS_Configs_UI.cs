using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WeaponSkill.Configs
{
    public class WS_Configs_UI : BasicWeaponSkillConfig<WS_Configs_UI>
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(typeof(Vector2),"0.3,0.02")]
        public Vector2 StaminaUI_Pos;

        [DefaultValue(typeof(Vector2), "0.02,0.3")]
        public Vector2 SpiritUI_Pos;

        [DefaultValue(typeof(Vector2), "0.01,0.15")]
        public Vector2 ChooseAmmoUI_Pos;

        [DefaultValue(typeof(Vector2), "0.9,0.15")]
        public Vector2 SkillsTreeUI_Pos;

        [DefaultValue(typeof(Vector2), "0,0.3")]
        public Vector2 TalkUIPos;
    }
}
