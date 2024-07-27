using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.UI;
using WeaponSkill.Weapons.Crossbow;

namespace WeaponSkill.UI.CrossbowPartUI
{
    public class CrossbowPartSlot : UIElement
    {
        ///// <summary>
        ///// 装备的物品
        ///// </summary>
        //public Item PartItem;
        /// <summary>
        /// 可以使用这个Part的条件
        /// </summary>
        public Func<Item,bool> CanUseThisPart;
        /// <summary>
        /// ID位置,决定读取的物品位置
        /// </summary>
        public int ID;
        public CrossbowPartSlot(Func<Item,bool> canUseThisPart)
        {
            //PartItem = new Item();
            //PartItem.SetDefaults(0);
            CanUseThisPart = canUseThisPart;

            Width.Set(TextureAssets.InventoryBack9.Width(), 0f);
            Height.Set(TextureAssets.InventoryBack9.Height(), 0f);
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (CrossbowGlobalItem.OpenItem == null)
                return;
            Rectangle rectangle = GetDimensions().ToRectangle();
            Main.inventoryScale = 0.75f;
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface && CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID] != null)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (CanUseThisPart == null || CanUseThisPart(Main.mouseItem))
                {
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID], ItemSlot.Context.BankItem);
                }
            }
            //if (CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID] != null && PartItem == null)
            //{
            //    PartItem = CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID];
            //}

            //CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID] = PartItem;

            // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
            ItemSlot.Draw(spriteBatch, ref CrossbowGlobalItem.OpenItem.GetGlobalItem<CrossbowGlobalItem>().Crossbow_Parts[ID], ItemSlot.Context.BankItem, rectangle.TopLeft());
        }
    }
}
