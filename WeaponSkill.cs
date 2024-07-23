using Terraria.ModLoader;
using Terraria.Localization;
using ReLogic.Content.Sources;
using Terraria.Graphics.Effects;
using WeaponSkill.Helper;
using WeaponSkill.Items.DualBlades;
using WeaponSkill.UI.StarBreakerUI.TalkUI;
using WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill
{
	public class WeaponSkill : Mod
	{
        public static Asset<Effect> SwingEffect;
        public static Asset<Effect> SpurtsShader;
        public static Asset<Effect> OffsetShader;
        public static Asset<Effect> SwordHot;
        public static Asset<Effect> HammerChannelShader;
        public static Asset<Texture2D> CritTex;
        //public static Asset<Effect> SP_SwingEffect;
        public static Asset<Texture2D> ChooseAmmoUITex;
        public static Asset<Texture2D> StaminaUITex;
        public static Asset<Texture2D> SwingTex;
        public static Asset<Texture2D> SwingTex_Offset;
        public static Asset<Texture2D> SpiritUITex;
        public static Asset<Texture2D> HotTex;
        public static Asset<Texture2D> TalkUI;
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
                SwordHot = Assets.Request<Effect>("Effects/" + nameof(SwordHot));
                //SP_SwingEffect = Assets.Request<Effect>("Effects/" + nameof(SP_SwingEffect));
                HammerChannelShader = Assets.Request<Effect>("Effects/" + nameof(HammerChannelShader));
                SwingTex = Assets.Request<Texture2D>("Images/" + nameof(SwingTex));
                ChooseAmmoUITex = Assets.Request<Texture2D>("UI/ChangeAmmoUI/" + nameof(ChooseAmmoUITex),AssetRequestMode.ImmediateLoad);
                StaminaUITex = Assets.Request<Texture2D>("UI/StaminaUI/" + nameof(StaminaUITex));
                SpiritUITex = Assets.Request<Texture2D>("UI/SpiritUI/" + nameof(SpiritUITex));
                SwingTex_Offset = Assets.Request<Texture2D>("Images/" + nameof(SwingTex_Offset));
                CritTex = Assets.Request<Texture2D>("Images/" + nameof(CritTex));
                TalkUI = Assets.Request<Texture2D>("UI/StarBreakerUI/TalkUI/" + nameof(TalkUI));
                HotTex = Assets.Request<Texture2D>("Images/" + nameof(HotTex));
            }
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;
            On_Main.LoadWorlds += On_Main_LoadWorlds;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            RenderTargetShaderSystem = new();
            ElementDamageSystem.Load();
            Main.OnPostDraw += Main_OnPostDraw;

            SlashDamage.Load();
            HitDamage.Load();
            SpurtsDamage.Load();
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
            if (!Main.dedServ)
            {
                SwingEffect = null;
                SpurtsShader = null;
                OffsetShader = null;
                SwordHot = null;
                HammerChannelShader = null;
                SwingTex = null;
                ChooseAmmoUITex = null;
                StaminaUITex = null;
                SpiritUITex = null;
                SwingTex_Offset = null;
                HotTex = null;
                TalkUI = null;
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
            return CallSucceed;
        }
    }
}