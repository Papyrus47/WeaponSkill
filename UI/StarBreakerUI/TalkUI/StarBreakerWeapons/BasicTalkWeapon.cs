using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace WeaponSkill.UI.StarBreakerUI.TalkUI.StarBreakerWeapons
{
    public abstract class BasicTalkWeapon
    {
        public string Name => Language.GetTextValue("Mods.WeaponSkill.UI.StarBreakerUI.TalkUI." + thePath + ".Name");
        /// <summary>
        /// 坐标位置
        /// </summary>
        public string thePath;
        /// <summary>
        /// 表情的名字
        /// </summary>
        public List<string> Faces;
        /// <summary>
        /// 使用表情的索引
        /// </summary>
        public int UseFaceIndex;
        /// <summary>
        /// 使用的表情
        /// </summary>
        public Texture2D UsingFace
        {
            get
            {
                if(Faces == null)
                {
                    Faces = new List<string>();
                    int i = 0;
                    while(true)
                    {
                        var face = Language.GetText("Mods.WeaponSkill.UI.StarBreakerUI.TalkUI." + thePath + ".Faces." + i.ToString());
                        if (face != null)
                        {
                            string text = "Mods.WeaponSkill.UI.StarBreakerUI.TalkUI.StarBreakerWeapons.";
                            text += thePath + '.';
                            text += face.Value;
                            text = text.Replace('.', '/');
                            Faces.Add(text);
                        }
                        else
                            break;
                        //Language.GetTextValue("Mods.WeaponSkill.UI.StarBreakerUI.TalkUI." + thePath + ".Faces");
                        i++;
                    }
                }
                return ModContent.Request<Texture2D>(Faces[UseFaceIndex]).Value;
            }
        }

    }
}
