using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.UI.ChangeAmmoUI;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.UI.SpiritUI
{
    public class SpiritUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(116, 0);
            Height = new(26, 0);
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
            if (LongSwordGlobalItem.ShowTheSpirit && Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.active && !Main.playerInventory && Main.LocalPlayer.HeldItem.TryGetGlobalItem<LongSwordGlobalItem>(out _))
            {
                LongSwordGlobalItem.ShowTheSpirit = false;
                base.Draw(spriteBatch);
            }
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            var tex = WeaponSkill.SpiritUITex.Value;
            Rectangle rect = GetDimensions().ToRectangle();
            //rect.Width *= 2;
            //rect.Height *= 2;

            LongSwordGlobalItem longSwordGlobalItem = Main.LocalPlayer.HeldItem.GetGlobalItem<LongSwordGlobalItem>();
            int SpiritLevel = longSwordGlobalItem.SpiritLevel;
            float factor = (float)longSwordGlobalItem.Spirit / longSwordGlobalItem.SpiritMax;
            float factor1 = (float)longSwordGlobalItem.Time / 255;
            Rectangle drawRect = new(0, 0, 58, 12);
            Rectangle drawRect_EdgeLight = new(0, 12,(int)(58 * factor1), 12);
            Color color = SpiritLevel switch
            {
                1 => Color.White,
                2 => Color.Gold,
                3 => Color.Red,
                _ => default
            };
            Rectangle lineRect = new(0,0, (int)(41 * factor), 3);

            if (color != default) // 绘制气刃外框的颜色框
            {
                spriteBatch.Draw(tex, rect.Center(), drawRect_EdgeLight, color, 0f, new Vector2(29f, 7f), 2f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(tex,rect,drawRect, Color.White,0,Vector2.Zero,SpriteEffects.None,0f); // 绘制气刃外框
            spriteBatch.Draw(ModContent.Request<Texture2D>("WeaponSkill/UI/SpiritUI/SpiritUITex_DrawSpiritLine").Value, rect.Center(), lineRect, new Color(200,30,30,20), 0, new Vector2(15f, 1.5f),2f, SpriteEffects.None, 0f); ; // 绘制气刃条

            if (ContainsPoint(Main.MouseScreen))
            {
                //ItemSlot.MouseHover(ref AmmoItem);
                Main.hoverItemName = longSwordGlobalItem.Spirit + "/" +longSwordGlobalItem.SpiritMax.ToString();
            }

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer,null,Main.UIScaleMatrix);
        }
    }
}
