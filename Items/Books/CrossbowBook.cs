﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;

namespace WeaponSkill.Items.Books
{
    public class CrossbowBook : BasicInstructions
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.ForEach((x) => x.Text = string.Format(x.Text, WeaponSkill.RangeChange.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault(), WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault()));
        }
    }

}
