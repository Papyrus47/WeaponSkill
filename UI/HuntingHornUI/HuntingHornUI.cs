using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using Terraria.UI.Chat;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.HuntingHorn;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.UI.HuntingHornUI
{
    public class HuntingHornUI : UIState
    {
        public override void OnInitialize()
        {
            Width = new(154, 0);
            Height = new(38, 0);
        }
        public override void Update(GameTime gameTime)
        {
            WS_Configs_UI wS_Configs_UI = WS_Configs_UI.Init;
            Left.Percent = wS_Configs_UI.SpiritUI_Pos.X;
            Top.Percent = wS_Configs_UI.SpiritUI_Pos.Y;
            Recalculate();

            Item item = Main.LocalPlayer.HeldItem;
            if(item.TryGetGlobalItem<HuntingHornGlobalItem>(out var huntingHornGlobalItem))
                huntingHornGlobalItem.hornMelody.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem != null && Main.LocalPlayer.HeldItem.active && !Main.playerInventory && Main.LocalPlayer.HeldItem.TryGetGlobalItem<HuntingHornGlobalItem>(out _))
            {
                base.Draw(spriteBatch);
            }
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item item = Main.LocalPlayer.HeldItem;
            HuntingHornGlobalItem huntingHornGlobalItem = item.GetGlobalItem<HuntingHornGlobalItem>();
            #region 关于旋律的绘制
            Texture2D tex = ModAsset.HuntingHornUI.Value;
            Rectangle destinationRectangle = GetDimensions().ToRectangle();
            //huntingHornGlobalItem.hornMelody.Update();
            spriteBatch.Draw(tex, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            #region 绘制旋律
            var melodies = huntingHornGlobalItem.hornMelody.melodies.ToArray();
            destinationRectangle.X += 160;
            for (int i = huntingHornGlobalItem.hornMelody.melodies.Count - 1; i >= 0;i--)
            {
                tex = GetTex(melodies, i);
                destinationRectangle.Width = tex.Width;
                destinationRectangle.Height = tex.Height;
                destinationRectangle.X -= 30;
                spriteBatch.Draw(tex, destinationRectangle, null, huntingHornGlobalItem.hornMelody.DrawColor(melodies[i]), 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
            #endregion
            #endregion
            #region 绘制需要演奏的音乐
            float scale = 0.5f;
            destinationRectangle.X = (int)(GetDimensions().ToRectangle().X + GetDimensions().ToRectangle().Width * 1.5f);
            if (huntingHornGlobalItem.huntingHornBuffs.Count > 3)
                huntingHornGlobalItem.huntingHornBuffs.Dequeue();
            HuntingHornBuff[] huntingHornBuffs = huntingHornGlobalItem.huntingHornBuffs.ToArray();
            for (int i = 0; i < huntingHornGlobalItem.huntingHornBuffs.Count; i++)
            {
                HuntingHornBuff buff = huntingHornBuffs[i];
                if (buff == null)
                    continue;
                HuntingHornMelody.MelodyType[] melodiesArray = buff.melodyTypes.ToArray();
                for (int j = 0; j < buff.melodyTypes.Count; j++)
                {
                    tex = GetTex(melodiesArray, j);

                    destinationRectangle.Width = (int)(tex.Width * scale);
                    destinationRectangle.Height = (int)(tex.Height * scale);
                    spriteBatch.Draw(tex, destinationRectangle, null, huntingHornGlobalItem.hornMelody.DrawColor(melodiesArray[j]), 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    destinationRectangle.X += (int)(destinationRectangle.Width * 2);
                }

                spriteBatch.DrawString(FontAssets.MouseText.Value, buff.Name, destinationRectangle.Left(), Color.White, 0f,new(0f, FontAssets.MouseText.Value.MeasureString(buff.Name).Y * 0.5f),scale * 2,SpriteEffects.None,0f);
                destinationRectangle.Y += (int)(destinationRectangle.Height * 1.5f);
                destinationRectangle.X = (int)(GetDimensions().ToRectangle().X + GetDimensions().ToRectangle().Width * 1.5f);
            }
            #endregion
            #region 绘制乐谱
            destinationRectangle = GetDimensions().ToRectangle();
            destinationRectangle.Y += destinationRectangle.Height * 2; // 向下移动

            huntingHornBuffs = huntingHornGlobalItem.hornMelody.DefHuntingHornBuff.Values.ToArray();
            for (int i = 0; i < huntingHornGlobalItem.hornMelody.DefHuntingHornBuff.Count; i++)
            {
                HuntingHornBuff buff = huntingHornBuffs[i];
                if (buff == null)
                    continue;
                HuntingHornMelody.MelodyType[] melodiesArray = buff.melodyTypes.ToArray();
                destinationRectangle.X = (int)(GetDimensions().ToRectangle().X + GetDimensions().ToRectangle().Width * 0f);
                for (int j = 0; j < buff.melodyTypes.Count; j++)
                {
                    tex = GetTex(melodiesArray, j);

                    destinationRectangle.Width = (int)(tex.Width * scale);
                    destinationRectangle.Height = (int)(tex.Height * scale);
                    spriteBatch.Draw(tex, destinationRectangle, null, huntingHornGlobalItem.hornMelody.DrawColor(melodiesArray[j]), 0f, Vector2.Zero, SpriteEffects.None, 0f);
                    destinationRectangle.X += (int)(destinationRectangle.Width * 2);
                }

                spriteBatch.DrawString(FontAssets.MouseText.Value, buff.Name, destinationRectangle.Left(), Color.White, 0f, new(0f, FontAssets.MouseText.Value.MeasureString(buff.Name).Y * 0.5f), scale * 2, SpriteEffects.None, 0f);
                destinationRectangle.Y += (int)(destinationRectangle.Height * 1.5f);
                
            }
            #endregion
                //#region 绘制乐铺
                //huntingHornGlobalItem.hornMelody.DrawUI(spriteBatch,GetDimensions().Position()); // 传入左上角
                //#endregion
        }

        public static Texture2D GetTex(HuntingHornMelody.MelodyType[] melodies, int i)
        {
            Texture2D tex;
            switch (melodies[i])
            {
                case HuntingHornMelody.MelodyType.Left:
                    tex = ModAsset.HuntingHornMelody2.Value;
                    break;
                case HuntingHornMelody.MelodyType.Right:
                    tex = ModAsset.HuntingHornMelody1.Value;
                    break;
                case HuntingHornMelody.MelodyType.LeftAndRight:
                    tex = ModAsset.HuntingHornMelody3.Value;
                    break;
                case HuntingHornMelody.MelodyType.SP:
                    tex = ModAsset.HuntingHornMelody4.Value;
                    break;
                default:
                    tex = ModAsset.HuntingHornMelody4.Value;
                    break;
            }

            return tex;
        }
    }
}
