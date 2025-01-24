using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.Items.SlashAxe
{
    public abstract class BasicSlashAxe : ModItem
    {
        public abstract Asset<Texture2D> SwordTex { get; }
        public abstract Asset<Texture2D> AxeTex { get; }
        public abstract Asset<Texture2D> DefTex { get; }
        public Vector2 SwordSize,AxeSize;
        public Asset<Texture2D> GetSwordTex => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Sword");
        public Asset<Texture2D> GetAxeTex => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Axe");
        public Asset<Texture2D> GetDefTex => ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/" + GetType().Name + "_Def");
        public sealed override void SetDefaults()
        {
            _ = SwordTex;
            _ = AxeTex;
            _ = DefTex;
            SlashAxeGlobalItem.WeaponID ??= new();
            SlashAxeGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
    }
}
