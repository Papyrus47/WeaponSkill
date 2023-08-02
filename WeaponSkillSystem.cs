using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.UI.ChangeAmmoUI;
using WeaponSkill.UI.SpiritUI;
using WeaponSkill.UI.StaminaUI;

namespace WeaponSkill
{
    public class WeaponSkillSystem : ModSystem
    {
        public UserInterface userInterface0;
        public UserInterface userInterface1;
        public UserInterface userInterface2;
        public ChangeAmmoUI changeAmmo;
        public StaminaUI stamina;
        public SpiritUI spiritUI;
        public override void Load()
        {
            changeAmmo = new ChangeAmmoUI();
            userInterface0 = new UserInterface();
            changeAmmo.Initialize();
            userInterface0.SetState(changeAmmo);

            stamina = new StaminaUI();
            userInterface1 = new UserInterface();
            stamina.Initialize();
            userInterface1.SetState(stamina);

            spiritUI = new();
            userInterface2 = new();
            spiritUI.Initialize();
            userInterface2.SetState(spiritUI);
        }
        public override void Unload()
        {
            changeAmmo = null;
            userInterface0 = null;

            userInterface1 = null;
            stamina = null;

            spiritUI = null;
            userInterface2 = null;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            userInterface0.Update(gameTime);
            userInterface2.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name.Equals("Vanilla: Mouse Text"));
            if(index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("WSS_ChangeAmmo",() =>
                {
                    userInterface0.Draw(Main.spriteBatch, new());
                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(index, new LegacyGameInterfaceLayer("Stamina",() =>
                {
                    userInterface1.Draw(Main.spriteBatch, new());
                    return true;
                },InterfaceScaleType.UI));

                layers.Insert(index, new LegacyGameInterfaceLayer("spiritUI", () =>
                {
                    userInterface2.Draw(Main.spriteBatch, new());
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
