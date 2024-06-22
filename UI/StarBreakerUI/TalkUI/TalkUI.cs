using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace WeaponSkill.UI.StarBreakerUI.TalkUI
{
    public class TalkUI : UIState
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            TalkPanel talkPanel = new TalkPanel();
            talkPanel.Top = new(0, 0.5f);
            talkPanel.Left = new(0, 0.5f);
            Append(talkPanel);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
