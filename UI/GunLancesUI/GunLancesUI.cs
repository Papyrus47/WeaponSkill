using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.GunLances;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.UI.GunLancesUI
{
    public class GunLancesUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(1, 0);
            Height = new(1, 0);
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
            if (Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.active && !Main.playerInventory && Main.LocalPlayer.HeldItem.TryGetGlobalItem<GunLancesGlobalItem>(out _))
            {
                base.Draw(spriteBatch);
            }
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.TryGetGlobalItem(out GunLancesGlobalItem gunLancesGlobalItem))
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointWrap,DepthStencilState.Default,Main.Rasterizer,null,Main.UIScaleMatrix);
                int scale = 4;
                Rectangle Rect = GetDimensions().ToRectangle();
                Rectangle drawRect = Rect;
                Texture2D AmmoTex = ModAsset.GunLancesUI_Ammo.Value;
                Texture2D LongHangTex = ModAsset.GunLancesUI_LongHang.Value;
                Texture2D DrogueHitTex = ModAsset.GunLancesUI_DrogueHit.Value;
                #region 绘制弹药
                for(int i = 0; i < gunLancesGlobalItem.MaxAmmo; i++)
                {
                    Rectangle drawAmmoRect = new Rectangle(0, 0, AmmoTex.Width, AmmoTex.Height / 2);
                    drawRect.Width = drawAmmoRect.Width * scale;
                    drawRect.Height = drawAmmoRect.Height * scale;
                    if (i >= gunLancesGlobalItem.Ammo)
                    {
                        drawAmmoRect.Y += drawAmmoRect.Height;
                    }
                    spriteBatch.Draw(AmmoTex,drawRect, drawAmmoRect, Color.White, 0f, drawAmmoRect.Size() * 0.5f, SpriteEffects.None, 0f);
                    drawRect.X += drawAmmoRect.Width * scale;
                }
                drawRect = Rect;
                Rectangle drawLongHangRect = new Rectangle(0, 0, LongHangTex.Width, LongHangTex.Height / 2);
                drawRect.Width = drawLongHangRect.Width * scale;
                drawRect.Height = drawLongHangRect.Height * scale;
                Color drawColor = Color.White;
                drawRect.Y += (int)(drawLongHangRect.Height * scale * 1.5f);
                drawRect.X += drawLongHangRect.Width * scale / 2;
                if (!gunLancesGlobalItem.HasLongHang)
                {
                    drawLongHangRect.Y += drawLongHangRect.Height;
                    drawColor = Color.Red * (Main.DiscoG / 255f);
                    spriteBatch.Draw(LongHangTex, drawRect, drawLongHangRect, Color.White * 0.5f, 0f, drawLongHangRect.Size() * 0.5f, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(LongHangTex, drawRect, drawLongHangRect, drawColor, 0f, drawLongHangRect.Size() * 0.5f, SpriteEffects.None, 0f);

                drawColor = Color.White;
                drawRect = Rect;
                Rectangle drawDrougeHitRect = new Rectangle(0, DrogueHitTex.Height / 2, DrogueHitTex.Width,DrogueHitTex.Height / 2);
                drawRect.Y += (int)(drawDrougeHitRect.Height * scale * 7f);
                drawRect.X += (int)(drawDrougeHitRect.Width * 1.5f * scale / 2);
                drawRect.Width = (int)(DrogueHitTex.Width * scale * 1.5f);
                drawRect.Height = DrogueHitTex.Height * scale;
                spriteBatch.Draw(DrogueHitTex, drawRect, drawDrougeHitRect, drawColor, 0f, drawDrougeHitRect.Size() * 0.5f, SpriteEffects.None, 0f);
                if (gunLancesGlobalItem.DrogueHitTime > 0)
                {
                    drawDrougeHitRect.Y = 0;
                    //drawDrougeHitRect.Width = ;
                    float time = (1f - gunLancesGlobalItem.DrogueHitTime / 1800f);
                    int factor = (int)(time * DrogueHitTex.Width * scale * 1.5f);
                    drawRect.Width -= factor;
                    drawRect.X -= (int)(factor * 0.45f);
                    //drawRect.X -= drawDrougeHitRect.Width;
                    spriteBatch.Draw(DrogueHitTex, drawRect, drawDrougeHitRect, drawColor, 0f, drawDrougeHitRect.Size() * 0.5f, SpriteEffects.None, 0f);
                }
                
                #endregion
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, null, Main.UIScaleMatrix);
            }
        }
    }
}
