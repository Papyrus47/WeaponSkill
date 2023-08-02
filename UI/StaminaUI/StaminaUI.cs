using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;

namespace WeaponSkill.UI.StaminaUI
{
    public class StaminaUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(200, 0);
            Height = new(12, 0);
            Left = new(0, 0.3f);
            Top = new(0, 0.02f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            WS_Configs_UI wS_Configs_UI = WS_Configs_UI.Init;
            Left.Percent = wS_Configs_UI.StaminaUI_Pos.X;
            Top.Percent = wS_Configs_UI.StaminaUI_Pos.Y;
            if (Main.LocalPlayer.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina && !Main.playerInventory) base.Draw(spriteBatch);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var tex = WeaponSkill.StaminaUITex.Value;
            Rectangle drawRect = GetDimensions().ToRectangle();
            Vector2 center = drawRect.Center();
            WeaponSkillPlayer skillPlayer = Main.LocalPlayer.GetModPlayer<WeaponSkillPlayer>();
            float factor = (float)skillPlayer.StatStamina / skillPlayer.StatStaminaMax;

            Rectangle edgeRect = new(0, 0, 22, 36);
            Rectangle edgeRect1 = new(56, 0, 22, 36);
            Rectangle lineRect = new(18, 0, 42, 36);
            //Rectangle lineRect1 = new(20,42, 38, 10);

            spriteBatch.Draw(TextureAssets.BlackTile.Value, center - new Vector2(100 - edgeRect.Width, 0), null, Color.Black, 0, new Vector2(0, -edgeRect.Height * 0.4f), new Vector2(480f / lineRect.Width, 0.9f), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, center - new Vector2(100 - edgeRect.Width, 0), null, Color.Gold, 0, new Vector2(0, -edgeRect.Height * 0.4f), new Vector2(480f / lineRect.Width * factor, 0.9f), SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, center, lineRect, Color.White,0,lineRect.TopLeft(),new Vector2(200f / lineRect.Width,1),SpriteEffects.FlipHorizontally,0f);

            spriteBatch.Draw(tex, center - new Vector2(100, 0), edgeRect, Color.White);
            spriteBatch.Draw(tex, center + new Vector2(100, 0), edgeRect1, Color.White);
        }
    }
}
