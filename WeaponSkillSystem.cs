using StarBreaker.Content.Appraise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponSkill.Command.ResidueSwing;
using WeaponSkill.UI.ChangeAmmoUI;
using WeaponSkill.UI.ChargeBladeUI;
using WeaponSkill.UI.CrossbowPartUI;
using WeaponSkill.UI.DualBladesUI;
using WeaponSkill.UI.GunBulletUI;
using WeaponSkill.UI.HuntingHornUI;
using WeaponSkill.UI.SlashAxeUI;
using WeaponSkill.UI.SpiritUI;
using WeaponSkill.UI.StaminaUI;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.UI.StarBreakerUI.TalkUI;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.HuntingHorn;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.SlashAxe;
using WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes;
using WeaponSkill.Weapons.StarBreakerWeapon.General;
using WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade;
using WeaponSkill.Command;

namespace WeaponSkill
{
    public class WeaponSkillSystem : ModSystem
    {
        public List<UserInterface> userInterfaces;
        public ChangeAmmoUI changeAmmo;
        public StaminaUI stamina;
        public SpiritUI spiritUI;
        public CrossbowPartUI crossbowPartUI;
        public DualBladesUI bladesUI;
        public ChargeBladeBottle chargeBladeBottle;
        public SkillsTreeUI skillsTreeUI;
        public TalkUI talkUI;
        public GunBulletUI gunBulletUI;
        public SlashAxeUI slashAxeUI;
        public HuntingHornUI huntingHornUI;
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

            slashAxeUI = new();
            slashAxeUI.Initialize();

            chargeBladeBottle = new ChargeBladeBottle();
            chargeBladeBottle.Initialize();

            huntingHornUI = new();
            huntingHornUI.Initialize();

            skillsTreeUI = new();
            skillsTreeUI.Initialize();
            UserInterface userInterface3 = new UserInterface();
            userInterfaces.Add(userInterface3);
            userInterface3.SetState(skillsTreeUI);

            #region 取消这段注释显示对话UI
            //talkUI = new();
            //talkUI.Initialize();
            //UserInterface userInterface_talk = new UserInterface();
            //userInterfaces.Add(userInterface_talk);
            //userInterface_talk.SetState(talkUI);
            #endregion

            crossbowPartUI = new();
            UserInterface userInterface4 = new UserInterface();
            userInterfaces.Add(userInterface4);
            crossbowPartUI.Initialize();
            userInterface4.SetState(crossbowPartUI);

            gunBulletUI = new();
            UserInterface userInterface5 = new UserInterface();
            userInterfaces.Add(userInterface5);
            gunBulletUI.Initialize();
            userInterface5.SetState(gunBulletUI);

            On_NPC.HitModifiers.ToHitInfo += HitModifiers_ToHitInfo;
            On_Main.MouseText_DrawItemTooltip_GetLinesInfo += On_Main_MouseText_DrawItemTooltip_GetLinesInfo;
            Main.OnPostDraw += Main_OnPostDraw;

            //HuntingHornBuff.Load(Mod);
            Type[] type = AssemblyManager.GetLoadableTypes(Mod.Code);
            foreach (Type t in type)
            {
                //HuntingHornBuff.Load(t);
                if (typeof(IMyLoader).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                {
                    IMyLoader loader = (Activator.CreateInstance(t) as IMyLoader);
                    loader.Load();
                }
            }
        }

        private void Main_OnPostDraw(GameTime obj)
        {
            AppraiseSystem.Instance.Draw();
        }
        public override void PostUpdateEverything()
        {
            AppraiseSystem.Instance.Update();
        }
        private static void On_Main_MouseText_DrawItemTooltip_GetLinesInfo(On_Main.orig_MouseText_DrawItemTooltip_GetLinesInfo orig, Item item, ref int yoyoLogo, ref int researchLine, float oldKB, ref int numLines, string[] toolTipLine, bool[] preFixLine, bool[] badPreFixLine, string[] toolTipNames, out int prefixlineIndex)
        {
            bool flag = false;
            if(item.ModItem is StarBreakerMoreItemPart)
            {
                flag = true;
                //Array.Resize(ref toolTipLine, toolTipLine.Length + 1);
                //Array.Resize(ref toolTipNames, toolTipNames.Length + 1);
                //Array.Resize(ref preFixLine, preFixLine.Length + 1);
            }
            orig.Invoke(item,ref yoyoLogo,ref researchLine,oldKB,ref numLines,toolTipLine,preFixLine,badPreFixLine,toolTipNames,out prefixlineIndex);

            if(flag) // 用于增加额外的说明
            {
                StarBreakerMoreItemPart starBreakerMoreItemPart = item.ModItem as StarBreakerMoreItemPart;
                if (Main.keyState.PressingShift()) // 按住Shift键时
                {
                    toolTipNames[numLines] = "ModifiedByWeaponSkill_Show";
                    toolTipLine[numLines] = starBreakerMoreItemPart.PartText.Value;
                }
                else
                {
                    toolTipNames[numLines] = "ModifiedByWeaponSkill_NoShow";
                    toolTipLine[numLines] = Language.GetTextValue("Mods.WeaponSkill.Items.General.ShowMoreText");
                }
                numLines++;
            }
        }

        private NPC.HitInfo HitModifiers_ToHitInfo(On_NPC.HitModifiers.orig_ToHitInfo orig, ref NPC.HitModifiers self, float baseDamage, bool crit, float baseKnockback, bool damageVariation, float luck)
        {
            self.SourceDamage *= SlashDamage.GetSlashDamageMultiple();
            self.SourceDamage *= HitDamage.GetHitDamageMultiple_Physics();
            self.SourceDamage *= SpurtsDamage.GetHitDamageMultiple_Physics();
            SpurtsDamage.ModifySpurtsHit(ref self);
            return orig.Invoke(ref self, baseDamage, crit, baseKnockback, damageVariation, luck);
        }

        public override void Unload()
        {
            userInterfaces.Clear();
            // 就用这个检测Mod有没有加载
            if(changeAmmo != null)
            {
                On_NPC.HitModifiers.ToHitInfo -= HitModifiers_ToHitInfo;
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
            if (userInterface.CurrentState == bladesUI || userInterface.CurrentState == spiritUI || userInterface.CurrentState == chargeBladeBottle || userInterface.CurrentState == slashAxeUI || userInterface.CurrentState == huntingHornUI)
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
                else if (Main.LocalPlayer.HeldItem.TryGetGlobalItem<SlashAxeGlobalItem>(out _))
                {
                    userInterface.SetState(slashAxeUI);
                }
                else if (Main.LocalPlayer.HeldItem.TryGetGlobalItem<HuntingHornGlobalItem>(out _))
                {
                    userInterface.SetState(huntingHornUI);
                }
            }
        }
    }
}
