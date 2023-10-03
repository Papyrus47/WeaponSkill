using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.Localization;

namespace WeaponSkill.Items.Books
{
    public class AxesBook : BasicInstructions
    {
        public bool Show = false;
        public override bool CanRightClick()
        {
            if (Main.mouseRightRelease)Show = !Show;
            return base.CanRightClick();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Show)
            {
                int index = tooltips.FindIndex(x => x.Name == "Tooltip1");
                tooltips.RemoveRange(index, tooltips.Count - 1 - index);
                tooltips.Add(new TooltipLine(Mod, "AxeTooltip", Language.GetTextValue(Tooltip.Key + "1")));
            }
            tooltips.ForEach((x) =>
            {
                x.Text = string.Format(x.Text, WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            });
        }
    }
}
