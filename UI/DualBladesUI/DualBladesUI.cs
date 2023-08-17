using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.UI.DualBladesUI
{
    public class DualBladesUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(122, 0);
            Height = new(28, 0);
        }
        public override void Update(GameTime gameTime)
        {
            WS_Configs_UI wS_Configs_UI = WS_Configs_UI.Init;
            Left.Percent = wS_Configs_UI.SpiritUI_Pos.X;
            Top.Percent = wS_Configs_UI.SpiritUI_Pos.Y;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (DualBladesGlobalItem.ShowTheDualUI && Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.active && !Main.playerInventory && Main.LocalPlayer.HeldItem.TryGetGlobalItem<DualBladesGlobalItem>(out _))
            {
                DualBladesGlobalItem.ShowTheDualUI = false;
                base.Draw(spriteBatch);
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var tex = ModContent.Request<Texture2D>("WeaponSkill/UI/DualBladesUI/DualBladesUI").Value;
            var item = Main.LocalPlayer.HeldItem.GetGlobalItem<DualBladesGlobalItem>();
            Rectangle rect = GetDimensions().ToRectangle();
            Rectangle drawBladeRect = new Rectangle(0, 0, 61, 14); // 鬼人条-未使用鬼人模式的Rect
            if(item.DemonMode || item.ArchdemonMode)
            {
                drawBladeRect.Y = 14;
            }
            spriteBatch.Draw(tex, rect, drawBladeRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f); // 绘制普通条
            if (item.DemonMode)
            {
                drawBladeRect.Y = 28;
                spriteBatch.Draw(tex, rect, drawBladeRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f); // 绘制鬼人条外壳
            }
            if(item.DemonGauge > 0)
            {
                drawBladeRect.Y = 42;
                drawBladeRect.X = 17;
                drawBladeRect.Width = (int)(44 * Math.Clamp(item.DemonGauge / item.DemonGaugeMax, 0, 1));
                spriteBatch.Draw(tex, rect.Center(), drawBladeRect,Color.White, 0f,new Vector2(30.5f - 17,7), 2f, SpriteEffects.None, 0f);
            }

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.hoverItemName = item.DemonGauge.ToString() + "/" + item.DemonGaugeMax.ToString();
            }
        }
    }
}
