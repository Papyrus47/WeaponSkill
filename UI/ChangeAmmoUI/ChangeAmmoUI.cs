using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace WeaponSkill.UI.ChangeAmmoUI
{
    public class ChangeAmmoUI : UIState
    {
        public List<Item> AmmoItems;
        public AmmoItemUI Now, Next, Last;
        public override void OnInitialize()
        {
            AmmoItems = new List<Item>();
            Now = new()
            {
                IsChoose = true
            };
            Next = new();
            Last = new();

            Width = new(100, 0);
            Height = new(30, 0);
            Top = new(0, 0.15f);
            Left = new(50, 0);


            Now.Top = new(0, 0.5f);
            Now.Left = new(Width.Pixels / 3, 0);

            Next.Top = new(0, 0.5f);
            Next.Left = new(0, 0);

            Last.Top = new(0, 0.5f);
            Last.Left = new(Width.Pixels * 2 / 3, 0);
            Append(Now);
            Append(Next);
            Append(Last);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player localPlayer = Main.LocalPlayer;
            if (!localPlayer.GetModPlayer<WeaponSkillPlayer>().ShowTheRangeChangeUI) return;
            ref int index = ref localPlayer.GetModPlayer<WeaponSkillPlayer>().UseAmmoIndex;
            if (AmmoItems != localPlayer.GetModPlayer<WeaponSkillPlayer>().AmmoItems) AmmoItems = localPlayer.GetModPlayer<WeaponSkillPlayer>().AmmoItems;
            AmmoItems.Clear();
            #region 获取数量
            for (int i = 0; i < 58; i++)
            {
                if (localPlayer.inventory[i].stack > 0 && ItemLoader.CanChooseAmmo(localPlayer.HeldItem, localPlayer.inventory[i],localPlayer))
                {
                    AmmoItems.Add(localPlayer.inventory[i]);
                }
            }
            #endregion
            if (index >= AmmoItems.Count)
            {
                index = 0;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<WeaponSkillPlayer>().ShowTheRangeChangeUI && !Main.playerInventory) base.Draw(spriteBatch);
        }
        protected override void DrawChildren(SpriteBatch spriteBatch) { }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int index = Main.LocalPlayer.GetModPlayer<WeaponSkillPlayer>().UseAmmoIndex;
            if (AmmoItems.Count == 0) return;
            if (index >= AmmoItems.Count) index = 0;

            Now.AmmoItem = AmmoItems[index];
            Now.Draw(spriteBatch);
            if (AmmoItems.Count > 1)
            {
                int index1 = index + 1;
                if(index1 >= AmmoItems.Count)
                {
                    index1 = 0;
                }
                Next.AmmoItem = AmmoItems[index1];
                Next.Draw(spriteBatch);
            }
            if (AmmoItems.Count > 2)
            {
                int index1 = index - 1;
                if (index1 < 0)
                {
                    index1 = AmmoItems.Count - 1;
                }
                Last.AmmoItem = AmmoItems[index1];
                Last.Draw(spriteBatch);
            }
        }
    }
}
