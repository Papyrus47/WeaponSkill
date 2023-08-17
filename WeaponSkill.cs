using Terraria.ModLoader;
using Terraria.Localization;
using ReLogic.Content.Sources;
using Terraria.Graphics.Effects;
using WeaponSkill.Helper;
using WeaponSkill.Items.DualBlades;

namespace WeaponSkill
{
	public class WeaponSkill : Mod
	{
        public static Asset<Effect> SwingEffect;
        public static Asset<Effect> SpurtsShader;
        public static Asset<Effect> OffsetShader;
        public static Asset<Texture2D> ChooseAmmoUITex;
        public static Asset<Texture2D> StaminaUITex;
        public static Asset<Texture2D> SwingTex;
        public static Asset<Texture2D> SwingTex_Offset;
        public static Asset<Texture2D> SpiritUITex;
        public static ModKeybind RangeChange;
        public static ModKeybind BowSlidingStep;
        public static ModKeybind SpKeyBind;
        public static RenderTargetShaderSystem RenderTargetShaderSystem;
        public static RenderTarget2D MyRender;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                SwingEffect = Assets.Request<Effect>("Effects/" + nameof(SwingEffect));
                SpurtsShader = Assets.Request<Effect>("Effects/" + nameof(SpurtsShader));
                OffsetShader = Assets.Request<Effect>("Effects/" + nameof(OffsetShader));
                SwingTex = Assets.Request<Texture2D>("Images/" + nameof(SwingTex));
                ChooseAmmoUITex = Assets.Request<Texture2D>("UI/ChangeAmmoUI/" + nameof(ChooseAmmoUITex),AssetRequestMode.ImmediateLoad);
                StaminaUITex = Assets.Request<Texture2D>("UI/StaminaUI/" + nameof(StaminaUITex));
                SpiritUITex = Assets.Request<Texture2D>("UI/SpiritUI/" + nameof(SpiritUITex));
                SwingTex_Offset = Assets.Request<Texture2D>("Images/" + nameof(SwingTex_Offset));
            }
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;
            On_Main.LoadWorlds += On_Main_LoadWorlds;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            RenderTargetShaderSystem = new();
            Main.OnPostDraw += Main_OnPostDraw;
        }
        public static void Main_OnPostDraw(GameTime obj)
        {
            RenderTargetShaderSystem?.ResetData();
        }

        private static void Main_OnResolutionChanged(Vector2 obj)
        {
            Main.QueueMainThreadAction(() =>
            {
                MyRender = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }

        private static void On_Main_LoadWorlds(On_Main.orig_LoadWorlds orig)
        {
            Main.QueueMainThreadAction(() =>
            {
                MyRender = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
            orig.Invoke();
        }
        public static void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            RenderTargetShaderSystem.Draw();
            orig.Invoke(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        public override void Unload()
        {
            if (RenderTargetShaderSystem != null)
            {
                Main.QueueMainThreadAction(() =>
                {
                    MyRender?.Dispose();
                });
                MyRender = null;
                RenderTargetShaderSystem = null;
                On_FilterManager.EndCapture -= On_FilterManager_EndCapture;
                Main.OnResolutionChanged -= Main_OnResolutionChanged;
                On_Main.LoadWorlds -= On_Main_LoadWorlds;
            }
        }

        public override void PostSetupContent()
        {
            RangeChange = KeybindLoader.RegisterKeybind(this, Language.GetTextValue("Mods.WeaponSkill.ModKey.RangeChange"), Microsoft.Xna.Framework.Input.Keys.G);
            BowSlidingStep = KeybindLoader.RegisterKeybind(this, Language.GetTextValue("Mods.WeaponSkill.ModKey.BowSlidingStep"), Microsoft.Xna.Framework.Input.Keys.F);
            SpKeyBind = KeybindLoader.RegisterKeybind(this, Language.GetTextValue("Mods.WeaponSkill.ModKey.SpKeyBind"), Microsoft.Xna.Framework.Input.Keys.X);
        }
        public override object Call(params object[] args)
        {
            if(args == null)
            {
                throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
            }
            bool CallSucceed = false;
            /* 规格 : 第一个是指定类型
                 * 0 表示大剑 
                 * 1 表示短剑
                 * 2 表示长矛
                 * 3 表示弓
                 * 4 表示太刀
            * 第二个是物品ID,一般情况下禁止手持弹幕
            * 第三个是额外,一般用于特殊物品
            *
            *
            */
            if (args[0] is int weaponType && args[1] is int ItemType)
            {
                CallSucceed = true;
                switch (weaponType)
                {
                    case 0: // 大剑
                        {
                            Weapons.BroadSword.BroadSwordGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    case 1: // 短剑
                        {
                            Weapons.Shortsword.ShortswordGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    case 2: // 长矛
                        {
                            Weapons.Spears.SpearsGlobalItem.WeaponID.Add(ItemType);
                            if (args[2].Equals("Spears_SpType"))
                            {
                                Weapons.Spears.SpearsGlobalItem.WeaponID_SP.Add(ItemType);
                            }
                            break;
                        }
                    case 3: // 弓
                        {
                            Weapons.Bows.BowsGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    case 4: // 太刀
                        {
                            Weapons.LongSword.LongSwordGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    default:CallSucceed = false;break;
                }
            }
            return CallSucceed;
        }
    }
}