using Terraria.ModLoader;
using Terraria.Localization;

namespace WeaponSkill
{
	public class WeaponSkill : Mod
	{
        public static Asset<Effect> SwingEffect;
        public static Asset<Effect> SpurtsShader;
        public static Asset<Texture2D> ChooseAmmoUITex;
        public static Asset<Texture2D> StaminaUITex;
        public static Asset<Texture2D> SwingTex;
        public static ModKeybind RangeChange;
        public static ModKeybind BowSlidingStep;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                SwingEffect = Assets.Request<Effect>("Effects/" + nameof(SwingEffect));
                SpurtsShader = Assets.Request<Effect>("Effects/" + nameof(SpurtsShader));
                SwingTex = Assets.Request<Texture2D>("Images/" + nameof(SwingTex));
                ChooseAmmoUITex = Assets.Request<Texture2D>("UI/ChangeAmmoUI/" + nameof(ChooseAmmoUITex));
                StaminaUITex = Assets.Request<Texture2D>("UI/StaminaUI/" + nameof(StaminaUITex));
            }
        }
        public override void PostSetupContent()
        {
            RangeChange = KeybindLoader.RegisterKeybind(this, Language.GetTextValue("Mods.WeaponSkill.ModKey.RangeChange"), Microsoft.Xna.Framework.Input.Keys.G);
            BowSlidingStep = KeybindLoader.RegisterKeybind(this, Language.GetTextValue("Mods.WeaponSkill.ModKey.BowSlidingStep"), Microsoft.Xna.Framework.Input.Keys.F);
        }
        public override object Call(params object[] args)
        {
            if(args == null)
            {
                throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
            }
            bool CallSucceed = false;
            /* ��� : ��һ����ָ������
                 * 0 ��ʾ�� 
                 * 1 ��ʾ�̽�
                 * 2 ��ʾ��ì
            * �ڶ�������ƷID,һ������½�ֹ�ֳֵ�Ļ
            *
            *
            */
            if (args[0] is int weaponType && args[1] is int ItemType)
            {
                CallSucceed = true;
                switch (weaponType)
                {
                    case 0: // ��
                        {
                            Weapons.BroadSword.BroadSwordGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    case 1: // �̽�
                        {
                            Weapons.Shortsword.ShortswordGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    case 2: // ��ì
                        {
                            Weapons.Spears.SpearsGlobalItem.WeaponID.Add(ItemType);
                            break;
                        }
                    default:CallSucceed = false;break;
                }
            }
            return CallSucceed;
        }
    }
}