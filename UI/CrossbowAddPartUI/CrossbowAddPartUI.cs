using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace WeaponSkill.UI.CrossbowAddPartUI
{
    public class CrossbowAddPartUI : UIState
    {
        public override void OnInitialize()
        {
            Left = new(0, 0.5f);
            Top = new(0, 0.5f);
            CrossbowPartItemSlot crossbowAddPartUI = new CrossbowPartItemSlot(null,null);
            Append(crossbowAddPartUI);
        }
    }
}
