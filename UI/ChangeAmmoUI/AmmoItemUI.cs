using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace WeaponSkill.UI.ChangeAmmoUI
{
    public class AmmoItemUI : UIElement
    {
        public AmmoItemUI()
        {
        }
        public Item AmmoItem;
        public bool IsChoose;
        public override void OnInitialize()
        {
            Width = new(32, 0);
            Height = new(32, 0);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (AmmoItem == null) return;
            Texture2D uiTex = WeaponSkill.ChooseAmmoUITex.Value;
            Main.instance.LoadItem(AmmoItem.type);
            Texture2D itemTex = TextureAssets.Item[AmmoItem.type].Value;

            Rectangle DrawRect = GetDimensions().ToRectangle();
            Rectangle rectangle = new(0, 0, 30, 30);
            if (IsChoose) rectangle.Y += rectangle.Height;
            spriteBatch.Draw(uiTex, DrawRect, rectangle, Color.White);

            Rectangle itemRect;

            if (Main.itemAnimations[AmmoItem.type] != null)
            {
                itemRect = Main.itemAnimations[AmmoItem.type].GetFrame(itemTex);
            }
            else
            {
                itemRect = itemTex.Frame(1, 1, 0, 0);
            }
            spriteBatch.Draw(itemTex, DrawRect.Center(), itemRect, Color.White,0,itemRect.Size() * 0.5f,1f,SpriteEffects.None,0f);
        }
    }
}
