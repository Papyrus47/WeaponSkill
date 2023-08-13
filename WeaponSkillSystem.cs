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
        public List<UserInterface> userInterfaces;
        public ChangeAmmoUI changeAmmo;
        public StaminaUI stamina;
        public SpiritUI spiritUI;
        public override void Load()
        {
            userInterfaces = new();
            changeAmmo = new ChangeAmmoUI();
            UserInterface userInterface0 = new UserInterface();
            userInterfaces.Add(userInterface0);
            changeAmmo.Initialize();
            userInterface0.SetState(changeAmmo);

            stamina = new StaminaUI();
            UserInterface userInterface1 = new UserInterface();
            userInterfaces.Add(userInterface1);
            stamina.Initialize();
            userInterface1.SetState(stamina);

            spiritUI = new();
            UserInterface userInterface2 = new UserInterface();
            userInterfaces.Add(userInterface2);
            spiritUI.Initialize();
            userInterface2.SetState(spiritUI);

            //crossbow_AddPartUIState = new Crossbow_AddPartUIState();
            //UserInterface userInterface3 = new UserInterface();
            //userInterfaces.Add(userInterface3);
            //crossbow_AddPartUIState.Initialize();
            //userInterface3.SetState(crossbow_AddPartUIState);
        }
        public override void Unload()
        {
            userInterfaces.Clear();
            changeAmmo = null;
            stamina = null;
            spiritUI = null;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            userInterfaces.ForEach(x => x.Update(gameTime));
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("WeaponSkillUI", () =>
                {
                    for (int i = 0; i < userInterfaces.Count; i++)
                    {
                        userInterfaces[i].Draw(Main.spriteBatch, new());
                    }
                    return true;
                }, InterfaceScaleType.UI));

            }
        }
    }
}
