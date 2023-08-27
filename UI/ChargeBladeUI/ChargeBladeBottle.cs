using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.DualBlades;

namespace WeaponSkill.UI.ChargeBladeUI
{
    public class ChargeBladeBottle : UIState
    {
        public override void OnInitialize()
        {
            Width = new(100, 0);
            Height = new(30, 0);
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
            if (!Main.playerInventory && Main.LocalPlayer.HeldItem?.TryGetGlobalItem<ChargeBladeGlobalItem>(out _) == true)
            {
                base.Draw(spriteBatch);
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            var item = Main.LocalPlayer.HeldItem.GetGlobalItem<ChargeBladeGlobalItem>();
            Vector2 Center = GetDimensions().Position();
            Texture2D tex = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/ChargeBladeBottle").Value;
            Color color = Color.White;
            color.A = 255;
            for (int i =0;i < item.StatChargeBottleMax; i++)
            {
                if(i < item.StatChargeBottle) // 额外绘制装填
                {
                    Rectangle bottleChargeRect = new Rectangle(0,71,10,10);
                    spriteBatch.Draw(tex, Center + new Vector2(0,-4), bottleChargeRect, color, 0f, bottleChargeRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                }
                Rectangle rect = new(0, 0, 10, 15);
                if (item.StatCharge >= 23)
                {
                    rect.Y = 52;
                }
                else if (item.StatCharge >= 16)
                {
                    rect.Y = 34;
                }
                else if(item.StatCharge >= 10)
                {
                    rect.Y = 16;
                }
                spriteBatch.Draw(tex, Center, rect, color, 0f, rect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                Center.X += 20;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        }
    }
}
