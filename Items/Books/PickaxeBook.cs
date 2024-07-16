using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace WeaponSkill.Items.Books
{
    public class PickaxeBook : BasicInstructions
    {
        //public bool Show = false;
        //public override bool CanRightClick()
        //{
        //    if (Main.mouseRightRelease)Show = !Show;
        //    return base.CanRightClick();
        //}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //tooltips.Add(new TooltipLine(Mod, "AxeTooltip", "未实装"));
            //if (Show)
            //{
            //    int index = tooltips.FindIndex(x => x.Name == "Tooltip1");
            //    tooltips.RemoveRange(index, tooltips.Count - 1 - index);
            //    tooltips.Add(new TooltipLine(Mod, "AxeTooltip", Language.GetTextValue(Tooltip.Key + "1")));
            //}
            //tooltips.ForEach((x) =>
            //{
            //    x.Text = string.Format(x.Text, WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            //});
            tooltips.ForEach((x) =>
            {
                x.Text = string.Format(x.Text, WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.SpKeyBind.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            });
        }
    }
}
