using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.Localization;

namespace WeaponSkill.Items.Books
{
    public class BowBook : BasicInstructions
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var toolTip = tooltips.Find(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
            toolTip.Text = string.Format(toolTip.Text, WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
        }
    }
}
