﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.Localization;

namespace WeaponSkill.Items.Books
{
    public class GunsBook : BasicInstructions
    {
        public int Show = 1;
        public override bool CanRightClick()
        {
            if (Main.mouseRightRelease && ++Show > 5)
            {
                Show = 1;
            }
            return base.CanRightClick();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.ForEach((x) =>
            {
                x.Text = string.Format(x.Text, WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.SpKeyBind.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            });
            tooltips.Add(new TooltipLine(Mod, "Guns Tooltip", Language.GetTextValue(Tooltip.Key + Show.ToString(), WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(),WeaponSkill.SpKeyBind.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault())));
        }
    }
}
