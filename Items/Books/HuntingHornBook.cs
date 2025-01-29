using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace WeaponSkill.Items.Books
{
    public class HuntingHornBook : BasicInstructions
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                tooltip.Text = string.Format(tooltip.Text, WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.SpKeyBind.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            }
        }
    }
}
