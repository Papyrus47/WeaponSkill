using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Items.LongSword
{
    public class LightsLongSword : BasicLongSwordItem
    {
        public Texture2D SpurtsTex;
        public override void SetDefaults()
        {
            Item.Size = new(28, 66);
            Item.damage = 23;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 8;
            Item.rare = ItemRarityID.Blue;
        }
        public override void HoldItem(Player player)
        {
            if(SpurtsTex == null)
            {
                SpurtsTex = new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[Type].Height());
                TheUtility.GetWeaponDrawColor(SpurtsTex, TextureAssets.Item[Type], 0.9f, 2f);
                //Main.QueueMainThreadAction(() =>
                //{
                //    //SpurtsTex = new Texture2D(Main.graphics.GraphicsDevice,2, TextureAssets.Item[Type].Height());
                //    //Color[] TexColor = new Color[TextureAssets.Item[Type].Height() * TextureAssets.Item[Type].Width()];
                //    //int width = TextureAssets.Item[Type].Width();
                //    //TextureAssets.Item[Type].Value.GetData(TexColor);
                //    //Color[] colors = new Color[TextureAssets.Item[Type].Height()];
                //    //for (int i = colors.Length - 1; i > 0; i--)
                //    //{
                //    //    for (int j = 0; j < width; j++)
                //    //    {
                //    //        Color value1 = TexColor[(colors.Length - i) * width + j];
                //    //        if (value1 == default) continue;
                //    //        colors[i] = Color.Lerp(value1, colors[i], 0.95f);
                //    //    }
                //    //    colors[i] *= 3f;
                //    //}
                //    //SpurtsTex.SetData(colors);
                //    //colors = new Color[2 * TextureAssets.Item[Type].Height()];
                //    //SpurtsTex.GetData(colors);
                //    //for(int i = 0;i < colors.Length;i++)
                //    //{
                //    //    colors[i].R = (byte)(colors[i].R * 0.7f);
                //    //    colors[i].G = (byte)(colors[i].G * 0.3f);
                //    //    colors[i].B = (byte)(colors[i].B * 1.2f);
                //    //}
                //    //SpurtsTex.SetData(colors);
                //    //int height = 80;
                //    //SpurtsTex = new Texture2D(Main.graphics.GraphicsDevice,1, height);
                //    //Color[] colors = new Color[height];
                //    //for (int i = 0; i < height; i++)
                //    //{
                //    //    Color color = Color.Lerp(Color.MediumPurple * 0.4f, Color.Purple, 1f - ((float)i) / height);
                //    //    colors[i] = color;
                //    //}
                //    //SpurtsTex.SetData(colors);
                //});
            }
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/LightsLongSwordScabbard");
            }
        }
        public override void UpdateInventory(Player player)
        {
            if(SpurtsTex != null && player.HeldItem != Item)
            {
                SpurtsTex.Dispose();
                SpurtsTex = null;
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            float size = target.Size.Length();
            Vector2 vel = Main.rand.NextVector2Unit();
            SpurtsProj proj = SpurtsProj.NewSpurtsProj(player.GetSource_OnHit(target), target.Center - (vel * size * 3), vel, hit.Damage / 2, hit.Knockback / 5, player.whoAmI, size * 6,120, SpurtsTex);
            proj.FixedPos = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}
