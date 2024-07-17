using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Weapons.Crossbow;
using WeaponSkill.Weapons.Crossbow.Parts;

namespace WeaponSkill.UI.CrossbowPartUI
{
    public class CrossbowPartUI : UIState
    {
        public CrossbowPartSlot[] crossbowPartSlots = new CrossbowPartSlot[4];
        public override void OnInitialize()
        {
            Vector2 pos = Vector2.Zero;
            for(int i = 0; i < 4; i++)
            {
                CrossbowPartSlot crossbowPartSlot = new((item) =>
                {
                    return item.IsAir || (!item.IsAir && item.TryGetGlobalItem<PartsItemSystem>(out _));
                });

                crossbowPartSlot.Top.Set(i * crossbowPartSlot.Height.Pixels * 0.75f, 0f);
                crossbowPartSlot.ID = i;
                crossbowPartSlots[i] = crossbowPartSlot;
                Append(crossbowPartSlot);
            }
        }
        public override void OnActivate()
        {
            base.OnActivate();
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!CrossbowGlobalItem.ShowTheUI || !Main.playerInventory)
                return;
            if(Main.mouseRight && Main.mouseRightRelease)
            {
                Top.Set(Main.mouseY,0);
                Left.Set(Main.mouseX + 250, 0);
            }
            base.Draw(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
