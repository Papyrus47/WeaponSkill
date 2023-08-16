using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace WeaponSkill.UI.CrossbowAddPartUI
{
    public class CrossbowPartItemSlot : UIElement
    {
        public Item PartItem;
        public Func<Item,bool> PartItemFunc;
        public CrossbowPartItemSlot(Item partItem, Func<Item, bool> partItemFunc)
        {
            PartItem = partItem;
            PartItemFunc = partItemFunc;
            Width = new(30,0);
            Height = new(30,0);
            PartItem = ContentSamples.ItemsByType[ItemID.Zenith];
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //ItemSlot.DrawItemIcon(PartItem, 0, Main.spriteBatch, GetDimensions().Center(), 1f,GetDimensions().Width * GetDimensions().Height, Color.White);
            //if (ContainsPoint(Main.MouseScreen))
            //{
            //    Main.LocalPlayer.mouseInterface = true;
            //    ItemSlot.MouseHover(ref PartItem);
            //}
        }
    }
}
