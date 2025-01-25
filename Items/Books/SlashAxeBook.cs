

using System.Linq;
using Terraria.GameInput;

namespace WeaponSkill.Items.Books
{
    public class SlashAxeBook : BasicInstructions
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.ForEach((x) =>
            {
                x.Text = string.Format(x.Text, WeaponSkill.BowSlidingStep.GetAssignedKeys(InputMode.Keyboard).FirstOrDefault());
            });
        }
    }
}
