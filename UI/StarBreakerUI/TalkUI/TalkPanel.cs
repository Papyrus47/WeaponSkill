using ReLogic.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;
using WeaponSkill.Configs;

namespace WeaponSkill.UI.StarBreakerUI.TalkUI
{
    /// <summary>
    /// 边框聊天面板
    /// </summary>
    public class TalkPanel : UIElement
    {
        public float Scale = 1.5f;
        /// <summary>
        /// 越大越慢
        /// </summary>
        public float TalkSpeed = 2;
        public string TalkText = "这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！这都是你的错！管理者！";
        public byte TalkTime;
        /// <summary>
        /// 停止对话
        /// </summary>
        public bool StopTalk;
        /// <summary>
        /// 正在对话用的文本
        /// </summary>
        protected string OnTalkText = "";
        protected List<string> DrawTalkText = new();
        /// <summary>
        /// 正在对话的索引
        /// </summary>
        protected int OnTalkTextIndex;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 135 * 3;
            Height.Pixels = 66 * 3;
        }
        public override void Update(GameTime gameTime)
        {
            Left.Pixels = 0;
            Left.Precent = WS_Configs_UI.Init.TalkUIPos.X;
            Top.Precent = WS_Configs_UI.Init.TalkUIPos.Y;
            base.Update(gameTime);
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            string Name = "星辰大废物";
            Texture2D drawTex = WeaponSkill.TalkUI.Value;
        roundAndroundWeGo:
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 drawPos = calculatedStyle.Position() + new Vector2(0,calculatedStyle.Height * 0.5f);
            Rectangle texRect = new(0, 0, drawTex.Width, drawTex.Height);
            bool IsChangeTextScale = false; // 是否改变了大小
            Vector2 textSize = ChatManager.GetStringSize(font, OnTalkText, Vector2.One * Scale);
            Vector2 defTextSize = ChatManager.GetStringSize(font, "B", Vector2.One * Scale);
            bool roundAndround = false;
            //List<int> textEnterCount = new();

            while (textSize.Y * Scale * DrawTalkText.Count > Height.Pixels * 0.57f)
            {
                Scale *= 0.999f;
                IsChangeTextScale = true;
            }

            if (!StopTalk && TalkTime++ >= TalkSpeed && !Main.gamePaused)
            {
                if (OnTalkTextIndex > TalkText.Length - 1)
                {
                    OnTalkTextIndex = 0;
                    OnTalkText = "";
                    TalkTime = 0;
                    Scale = 1.5f;
                    DrawTalkText.Clear();
                }
                else
                {
                    TalkTime = 0;
                    //OnTalkText = TalkText.Remove(OnTalkTextIndex);
                    OnTalkText += TalkText[OnTalkTextIndex];
                    if (textSize.X + defTextSize.X * 1.5f > Width.Pixels * 0.6f)
                    {
                        string ce_talkText = OnTalkText;
                        DrawTalkText.Add(ce_talkText);
                        OnTalkText = "";

                        //textSize = ChatManager.GetStringSize(font, OnTalkText, Vector2.One * Scale);
                        //while (textSize.Y * Scale * DrawTalkText.Count > Height.Pixels * 0.56f)
                        //{
                        //    Scale *= 0.99f;
                        //}
                        IsChangeTextScale = true;
                    }
                    OnTalkTextIndex++;
                    if (OnTalkTextIndex > TalkText.Length - 1)
                    {
                        StopTalk = true;
                    }
                }

            }
            else if (StopTalk)
            {
                if(Main.mouseLeft && calculatedStyle.ToRectangle().Contains(Main.MouseScreen.ToPoint()))
                {
                    StopTalk = false;
                }
            }
            if (IsChangeTextScale)
            {
                DrawTalkText.Clear();

                OnTalkText = "";
                for (int i = 0; i < OnTalkTextIndex; i++)
                {
                    OnTalkText += TalkText[i];
                    textSize = ChatManager.GetStringSize(font, OnTalkText, Vector2.One * Scale);
                    if (textSize.X + defTextSize.X * 1.5f > Width.Pixels * 0.6f)
                    {
                        string ce_talkText = OnTalkText;
                        DrawTalkText.Add(ce_talkText);
                        OnTalkText = "";
                    }
                }
                roundAndround = true;
            }
            if (roundAndround)
                goto roundAndroundWeGo;

            Vector2 scale_panel = new Vector2(Width.Pixels / texRect.Width, Height.Pixels / texRect.Height);
            spriteBatch.Draw(drawTex, drawPos, null, Color.White, 0f, new Vector2(0, texRect.Height * 0.5f), scale_panel, SpriteEffects.None, 0f);
            int posY_offset;
            for (posY_offset = 0; posY_offset < DrawTalkText.Count; posY_offset++)
            {
                spriteBatch.DrawString(font, DrawTalkText[posY_offset], drawPos + new Vector2(0, defTextSize.Y * posY_offset * Scale), Color.White, 0, new Vector2(-45 * scale_panel.X, texRect.Height * 0.77f) * (1f / Scale), Scale, SpriteEffects.None, 0f);
            }
            spriteBatch.DrawString(font, OnTalkText, drawPos + new Vector2(0, defTextSize.Y * posY_offset * Scale), Color.White, 0, new Vector2(-45 * scale_panel.X, texRect.Height * 0.77f) * (1f / Scale), Scale, SpriteEffects.None, 0f);
            Vector2 nameSize = ChatManager.GetStringSize(font, Name, Vector2.One);
            //if (OnTalkText.Length > 2 && IsEnter)
            //{
            //    for (int i = 0; i < textEnterCount.Count; i++)
            //    {
            //        OnTalkText = OnTalkText.Remove(textEnterCount[i], 1);
            //    }
            //}
            //spriteBatch.Draw(TextureAssets.BlackTile.Value, GetDimensions().ToRectangle(), Color.Red);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            #region 粗边名字
            float scale_name = 1.2f;
            while (nameSize.X * scale_name > Width.Pixels * 0.2f || nameSize.Y * scale_name > Height.Pixels * 0.2f)
            {
                scale_name *= 0.99f;
            }
            Utils.DrawBorderStringFourWay(spriteBatch, font, Name, drawPos.X - 20 * -scale_panel.X, drawPos.Y - (texRect.Height * 0.5f + nameSize.Y * 0.75f) * scale_panel.Y * 0.3333f, Color.White, Color.Black,new Vector2(nameSize.X * 0.5f,nameSize.Y * 0.5f), scale_panel.Y * scale_name * 0.43f);
            // spriteBatch.DrawString(font, Name, drawPos - new Vector2(20 * -scale_panel.X, texRect.Height * 0.5f + nameSize.Y * 0.75f), Color.White, 0, nameSize * 0.5f, scale_panel.Y * 0.5f, SpriteEffects.None, 0f);
            #endregion
        }
    }
}