using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;

namespace WeaponSkill.UI.SlashAxeUI
{
    public class SlashAxeUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(116, 0);
            Height = new(26, 0);
        }
        public override void Update(GameTime gameTime)
        {
            WS_Configs_UI wS_Configs_UI = WS_Configs_UI.Init;
            Left.Percent = wS_Configs_UI.SpiritUI_Pos.X;
            Top.Percent = wS_Configs_UI.SpiritUI_Pos.Y;
            Recalculate();
        }
    }
}
