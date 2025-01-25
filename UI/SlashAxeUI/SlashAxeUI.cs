using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.UI.SlashAxeUI
{
    public class SlashAxeUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(92 * 2, 0);
            Height = new(110 / 2, 0);
        }
        public override void Update(GameTime gameTime)
        {
            WS_Configs_UI wS_Configs_UI = WS_Configs_UI.Init;
            Left.Percent = wS_Configs_UI.SpiritUI_Pos.X;
            Top.Percent = wS_Configs_UI.SpiritUI_Pos.Y;
            Recalculate();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.active && !Main.playerInventory && Main.LocalPlayer.HeldItem.TryGetGlobalItem<SlashAxeGlobalItem>(out _))
            {
                base.Draw(spriteBatch);
            }
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            var tex = ModContent.Request<Texture2D>("WeaponSkill/UI/SlashAxeUI/SlashAxeUI").Value;
            var item = Main.LocalPlayer.HeldItem.GetGlobalItem<SlashAxeGlobalItem>();
            Rectangle rect = GetDimensions().ToRectangle();
            int height = tex.Height / 4;
            Rectangle drawRect = new Rectangle(0, 0, tex.Width, height); // 绘制用的rect
            #region 绘制外壳
            if (item.Power > 0)
            {
                drawRect = new Rectangle(0, 0, tex.Width, height); // 绘制用的rect
                drawRect.Y = height * 3;
                spriteBatch.Draw(tex, rect.Center(), drawRect, Color.White, 0, rect.Size() * 0.25f + new Vector2(-2, 2f), 2.2f, SpriteEffects.None, 0f); // 绘制普通条
                drawRect.Width = (int)(tex.Width * Math.Clamp((float)item.Power / item.PowerMax, 0, 1));
                spriteBatch.Draw(tex, rect.Center(), drawRect, Color.Blue with { A = 100 }, 0, rect.Size() * 0.25f + new Vector2(-2, 2), 2.2f, SpriteEffects.None, 0f); // 绘制普通条
            }
            #endregion
            drawRect = new Rectangle(0, 0, tex.Width, height); // 绘制用的rect
            #region 绘制本体
            spriteBatch.Draw(tex, rect, drawRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f); // 绘制普通条
            #region 红斧模式
            if (item.AxeStrength > 0)
            {
                drawRect.Y = height * 2;
                spriteBatch.Draw(tex, rect, drawRect, Color.OrangeRed * (item.AxeStrength * 0.01f % 1f), 0, Vector2.Zero, SpriteEffects.None, 0f); // 绘制红斧
            }
            #endregion
            #endregion
            #region 绘制能量条
            drawRect.Y = height;
            drawRect.Width = (int)(tex.Width * ((float)item.Slash / item.SlashMax));
            spriteBatch.Draw(tex, rect.Center(), drawRect, Color.White, 0, rect.Size() * 0.25f, 2f, SpriteEffects.None, 0f); // 绘制能量条
            #endregion
        }
    }
}
