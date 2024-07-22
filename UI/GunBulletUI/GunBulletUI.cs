using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using WeaponSkill.Weapons.Guns;

namespace WeaponSkill.UI.GunBulletUI
{
    public class GunBulletUI : UIState
    {
        public Asset<Texture2D> bulletTex;
        public override void OnInitialize()
        {
            bulletTex = ModContent.Request<Texture2D>(this.GetInstancePartWithName());
            Width.Set(12, 0);
            Height.Set(20, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!GunsGlobalItem.ShowUI)
                return;
            Left.Set(Width.Pixels * 0.5f + Main.LocalPlayer.Top.X - Main.screenPosition.X, 0);
            Top.Set(Height.Pixels * -2 + Main.LocalPlayer.Top.Y - Main.screenPosition.Y, 0);
            GunsGlobalItem.ShowUI = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!GunsGlobalItem.ShowUI)
                return;
            CalculatedStyle calculated = GetDimensions();
            Rectangle rectangle = calculated.ToRectangle();

            spriteBatch.Draw(bulletTex.Value, rectangle, null, Color.White, 0f, rectangle.Size() * 0.5f, SpriteEffects.None, 0f);

            if (Main.LocalPlayer.HeldItem.TryGetGlobalItem<GunsGlobalItem>(out GunsGlobalItem item)) 
            {
                rectangle.X += 30;
                var font = FontAssets.MouseText.Value;
                string text = (item.GunType.HasBullet > 0 ? item.GunType.HasBullet : 0) + "/" + item.GunType.MaxBullet;
                Utils.DrawBorderStringFourWay(spriteBatch, font, text, rectangle.X, rectangle.Y, Color.White, Color.Black, new Vector2(10,rectangle.Height * 0.5f));
            }
        }
    }
}
