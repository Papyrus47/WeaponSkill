using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.DualBlades;

namespace WeaponSkill.UI.ChargeBladeUI
{
    // 红 226 53 26
    // 黄 245 219 27
    public class ChargeBladeBottle : UIState
    {
        public int OldBottle;
        public int[] Times;
        public int[] frameCount;
        public bool[] IsAdd;
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
            if (!Main.playerInventory && Main.LocalPlayer.HeldItem?.TryGetGlobalItem<ChargeBladeGlobalItem>(out var item) == true)
            {
                if (OldBottle < item.StatChargeBottle && Times != null) // 增加瓶子的情况
                {
                    for (int i = OldBottle; i < item.StatChargeBottle; i++)
                    {
                        if (i >= item.StatChargeBottleMax) continue;
                        Times[i] = 30;
                        frameCount[i] = 6;
                        IsAdd[i] = true;
                    }
                }
                else if(OldBottle > item.StatChargeBottle && Times != null) // 消耗瓶子的情况
                {
                    for (int i = OldBottle - 1; i >= item.StatChargeBottle; i--)
                    {
                        if (i >= item.StatChargeBottleMax) continue;
                        Times[i] = 20;
                        frameCount[i] = 5;
                        IsAdd[i] = false;
                    }
                }
                OldBottle = item.StatChargeBottle;
            }
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
            if(Times == null || Times.Length != item.StatChargeBottleMax)
            {
                Times = new int[item.StatChargeBottleMax];
                IsAdd = new bool[item.StatChargeBottleMax];
                frameCount = new int[item.StatChargeBottleMax];
            }
            Vector2 Center = GetDimensions().Position();
            Texture2D tex = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/ChargeBladeBottle").Value;
            Texture2D tex_add = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/ChargeBladeBottle_OnAdd").Value;
            Texture2D tex_use = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/ChargeBladeBottle_OnUse").Value;
            Texture2D StateTex = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/ChargeBladeShowStateTex").Value;
            Color color = Color.White;
            color.A = 255;
            #region 红剑绘制
            Rectangle SwordRect = new Rectangle(0, 27, 9, 22);
            spriteBatch.Draw(StateTex, Center, SwordRect, color, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            if (item.SwordStrengthening > 0) // 剑强化
            {
                Color drawColor = color;
                if(item.SwordStrengthening < 600)
                {
                    drawColor.A = (byte)(item.SwordStrengthening % 40 / 20f * 255);
                }
                SwordRect.Y = 0;
                spriteBatch.Draw(StateTex, Center, SwordRect, drawColor, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            }
            Center.X += 25;
            #endregion
            #region 红盾绘制
            Rectangle ShieldRect = new Rectangle(16, 27, 17, 23);
            spriteBatch.Draw(StateTex, Center, ShieldRect, color, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            if (item.ShieldStrengthening > 0) // 剑强化
            {
                Color drawColor = color;
                if (item.ShieldStrengthening < 600)
                {
                    drawColor.A = (byte)(item.SwordStrengthening % 40 / 20f * 255);
                }
                ShieldRect.Y = 0;
                spriteBatch.Draw(StateTex, Center, ShieldRect, drawColor, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            }
            Center.X += 40;
            #endregion
            #region 红斧绘制
            if (item.AxeStrengthening) // 剑强化
            {
                Rectangle AxeRect = new Rectangle(37, 27, 17, 21);
                spriteBatch.Draw(StateTex, Center, AxeRect, color, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                Color drawColor = color;
                if (item.AxeStrengtheningTime < 100 && item.StatChargeBottle == 1)
                {
                    drawColor.A = (byte)(item.SwordStrengthening % 40 / 20f * 255);
                }
                AxeRect.Y = 0;
                spriteBatch.Draw(StateTex, Center, AxeRect, drawColor, 0f, SwordRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                Center.X += 40;
            }
            #endregion
            #region 瓶子绘制
            for (int i =0;i < item.StatChargeBottleMax; i++)
            {
                if (Times[i] <= 0) // 正常瓶子绘制的代码
                {
                    if (i < item.StatChargeBottle) // 额外绘制装填
                    {
                        Rectangle bottleChargeRect = new(0, 53, 10, 10);
                        if (item.StatCharge >= 16)
                        {
                            color = Color.Red;
                        }
                        else if (item.StatCharge >= 10)
                        {
                            color = Color.Yellow;
                        }
                        spriteBatch.Draw(tex, Center + new Vector2(0, -4), bottleChargeRect, color, 0f, bottleChargeRect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                    }
                    Rectangle rect = new(0, 0, 10, 15);
                    if (item.StatCharge >= 23)
                    {
                        rect.Y = 34;
                        color = new Color(226, 53, 26);
                    }
                    else if (item.StatCharge >= 16)
                    {
                        rect.Y = 16;
                        color = new Color(226, 53, 26);
                    }
                    else if (item.StatCharge >= 10)
                    {
                        rect.Y = 16;
                        color = new Color(245, 219, 27);
                    }
                    spriteBatch.Draw(tex, Center, rect, color, 0f, rect.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
                }
                else // 瓶子动画
                {
                    Times[i]--;
                    int frame = frameCount[i] - (Times[i] / 5) - 1; // 帧图
                    Rectangle rect = new(0, frame * 16, 10, 16);
                    Texture2D drawTex = tex_add;
                    if (IsAdd[i])
                    {
                        drawTex = tex_add;
                    }
                    else
                    {
                        drawTex = tex_use;
                    }
                    #region 颜色处理
                    if (item.StatCharge >= 16)
                    {
                        color = new Color(226, 53, 26);
                    }
                    else if (item.StatCharge >= 10)
                    {
                        color = new Color(245, 219, 27);
                    }
                    #endregion
                    spriteBatch.Draw(drawTex, Center, rect, color, 0f, new Vector2(10, 15) / 2, 2f, SpriteEffects.None, 0f);
                }
                Center.X += 20;
            }
            #endregion
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        }
    }
}
