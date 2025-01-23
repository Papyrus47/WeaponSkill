using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.UI;
using WeaponSkill.Configs;
using WeaponSkill.Command;

namespace WeaponSkill.UI.StarBreakerUI.SkillsTreeUI
{
    /// <summary>
    /// 用类似AddSkill一样的方法注册这个技能树
    /// </summary>
    public class SkillsTreeUI : UIState
    {
        public struct SkillsControl
        {
            public bool InSky;
            public bool IsSP1Click;
            public bool IsSP2Click;
            public bool IsSP3Click;
            public bool IsLeftClick;
            public bool IsRightClick;
            /// <summary>
            /// 连续向前移动
            /// </summary>
            public bool IsDoubleForwardMove;
            /// <summary>
            /// 连续向后移动
            /// </summary>
            public bool IsDoubleBackwardMove;
            /// <summary>
            /// 需要蓄力
            /// </summary>
            public bool IsChannel;
            /// <summary>
            /// 需要等待的攻击
            /// </summary>
            public bool IsStopAtk;

            public SkillsControl(bool isLeftClick, bool isRightClick, bool isSP1Click, bool isDoubleForwardMove, bool isDoubleBackwardMove, bool isChannel, bool isStopAtk, bool inSky, bool isSP2Click = false, bool isSP3Click = false)
            {
                IsLeftClick = isLeftClick;
                IsRightClick = isRightClick;
                IsSP1Click = isSP1Click;
                IsDoubleForwardMove = isDoubleForwardMove;
                IsDoubleBackwardMove = isDoubleBackwardMove;
                IsChannel = isChannel;
                IsStopAtk = isStopAtk;
                InSky = inSky;
                IsSP2Click = isSP2Click;
                IsSP3Click = isSP3Click;
            }
        }
        /// <summary>
        /// 静态类用于一直保存
        /// 一个技能对应一个类
        /// </summary>
        public static Dictionary<ProjSkill_Instantiation, List<(SkillsControl,string)>> skillTreeDict = new();
        /// <summary>
        /// 现在的技能
        /// </summary>
        public static ProjSkill_Instantiation nowSkill;
        public static List<(SkillsControl, string)> GetNewSkillsTreeTextList() => new();
        public static bool TryAddSkillTree(ProjSkill_Instantiation skill, List<(SkillsControl, string)> values)
        {
            if (skillTreeDict.ContainsKey(skill))
            {
                skillTreeDict[skill].AddRange(values);
                return true;
            }
            skillTreeDict.Add(skill, values);
            return true;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
        }
        public static void UnLoad()
        {
            skillTreeDict = null;
        }
        public override void OnActivate()
        {
            base.OnActivate();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Vector2 pos = WS_Configs_UI.Init.SkillsTreeUI_Pos;
            Left.Precent = pos.X;
            Top.Precent = pos.Y;
            nowSkill = null;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            string drawText = "";
            if (nowSkill == null) return;
            if (skillTreeDict.TryGetValue(nowSkill, out var skillsText))
            {
                skillsText.ForEach(x =>
                {
                    SkillsControl controlText = x.Item1;
                    #region 控制判定
                    if (controlText.InSky)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.InSky));
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.Then");
                        drawText += " ";
                    }

                    if (controlText.IsChannel)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsChannel));
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.Then");
                        drawText += " ";
                    }

                    if (controlText.IsDoubleBackwardMove)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsDoubleBackwardMove));
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.Then");
                        drawText += " ";
                    }
                    else if (controlText.IsDoubleForwardMove)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsDoubleForwardMove));
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.Then");
                        drawText += " ";
                    }

                    if (controlText.IsStopAtk)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsStopAtk));
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.Then");
                        drawText += " ";
                    }

                    if (controlText.IsSP1Click)
                    {
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsSP1Click));
                        drawText += " ";
                    }
                    if (controlText.IsLeftClick)
                    {
                        if (controlText.IsSP1Click) drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.And");
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsLeftClick));
                        drawText += " ";
                    }
                    if(controlText.IsRightClick)
                    {
                        if(controlText.IsLeftClick || controlText.IsSP1Click) drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl.And");
                        drawText += Language.GetTextValue("Mods.WeaponSkill.SkillsControl." + nameof(controlText.IsRightClick));
                        drawText += " ";
                    }
                    #endregion
                    drawText += ":" + x.Item2;
                    drawText += "\n";
                });

                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, drawText, Main.screenWidth * Left.Precent, Main.screenHeight * Top.Precent, Color.White, Color.Black, FontAssets.MouseText.Value.MeasureString(drawText) * 0.5f, 0.7f);
            }
            else
                return;
        }
    }
}
