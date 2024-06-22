using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.UI.ChangeAmmoUI;
using WeaponSkill.UI.ChargeBladeUI;
using WeaponSkill.UI.CrossbowAddPartUI;
using WeaponSkill.UI.DualBladesUI;
using WeaponSkill.UI.SpiritUI;
using WeaponSkill.UI.StaminaUI;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.UI.StarBreakerUI.TalkUI;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill
{
    public class WeaponSkillSystem : ModSystem
    {
        public List<UserInterface> userInterfaces;
        public ChangeAmmoUI changeAmmo;
        public StaminaUI stamina;
        public SpiritUI spiritUI;
        public CrossbowAddPartUI crossbowAddPartUI;
        public DualBladesUI bladesUI;
        public ChargeBladeBottle chargeBladeBottle;
        public SkillsTreeUI skillsTreeUI;
        public TalkUI talkUI;
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

            bladesUI = new();
            bladesUI.Initialize();

            chargeBladeBottle = new ChargeBladeBottle();
            chargeBladeBottle.Initialize();

            skillsTreeUI = new();
            skillsTreeUI.Initialize();
            UserInterface userInterface3 = new UserInterface();
            userInterfaces.Add(userInterface3);
            userInterface3.SetState(skillsTreeUI);

            #region 取消这段注释显示对话UI
            //talkUI = new();
            //talkUI.Initialize();
            //UserInterface userInterface4 = new UserInterface();
            //userInterfaces.Add(userInterface4);
            //userInterface4.SetState(talkUI);
            #endregion

            //crossbowAddPartUI = new();
            //UserInterface userInterface3 = new UserInterface();
            //userInterfaces.Add(userInterface3);
            //crossbowAddPartUI.Initialize();
            //userInterface3.SetState(crossbowAddPartUI);
            On_NPC.HitModifiers.ToHitInfo += HitModifiers_ToHitInfo;
        }


        private NPC.HitInfo HitModifiers_ToHitInfo(On_NPC.HitModifiers.orig_ToHitInfo orig, ref NPC.HitModifiers self, float baseDamage, bool crit, float baseKnockback, bool damageVariation, float luck)
        {
            self.SourceDamage *= SlashDamage.GetSlashDamageMultiple();
            return orig.Invoke(ref self, baseDamage, crit, baseKnockback, damageVariation, luck);
        }

        public override void Unload()
        {
            userInterfaces.Clear();
            // 就用这个检测Mod有没有加载
            if(changeAmmo != null)
            {
            }
            changeAmmo = null;
            stamina = null;
            spiritUI = null;
            SkillsTreeUI.UnLoad();
        }
        public override void PostUpdatePlayers()
        {
            SlashDamage.UpdateSlashDamageCount();
        }
        public override void UpdateUI(GameTime gameTime)
        {
            userInterfaces.ForEach(x =>
            {
                TryChangeTheUserInterfacesSetState(x);
                x.Update(gameTime);
            });
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
        public void TryChangeTheUserInterfacesSetState(UserInterface userInterface)
        {
            if (userInterface.CurrentState == bladesUI || userInterface.CurrentState == spiritUI || userInterface.CurrentState == chargeBladeBottle)
            {
                if (Main.LocalPlayer.HeldItem.TryGetGlobalItem<DualBladesGlobalItem>(out _))
                {
                    userInterface.SetState(bladesUI);
                }
                else if(Main.LocalPlayer.HeldItem.TryGetGlobalItem<LongSwordGlobalItem>(out _))
                {
                    userInterface.SetState(spiritUI);
                }
                else if (Main.LocalPlayer.HeldItem.TryGetGlobalItem<ChargeBladeGlobalItem>(out _))
                {
                    userInterface.SetState(chargeBladeBottle);
                }
            }
        }
    }
}
