using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.DualBlades;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Items.DualBlades
{
    public class ChlorophyteBlades : BasicDualBlades
    {
        //public class ChlorophyteBladesSwingHelper : DualBladesSwingHelper
        //{
        //    public ChlorophyteBladesSwingHelper(DualBladesProj proj, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(proj, oldVelLength, swingItemTex)
        //    {
        //    }
        //}
        //public class ChlorophyteBladesProj_Blades : DualBladesProj.DualBlades
        //{
        //    public ChlorophyteBladesProj_Blades(DualBladesProj dualBladesProj, SwingHelper swingHelper) : base(dualBladesProj, swingHelper)
        //    {
        //    }
        //    public override void AI(float Time)
        //    {
        //        Time = Math.Clamp(Time, 0f, 1.2f);
        //        if (Time > 1)
        //        {
        //            SwingHelper.SetNotSaveOldVel();
        //        }
        //        if (SwingHelper is DualBladesSwingHelper helper)
        //        {
        //            helper.spriteDir = spDir;
        //        }
        //        //Vector2 vector2 = SwingHelper.VelScale;
        //        //SwingHelper.VelScale *= 2;
        //        //HammerProj.DrawHammerSwingShader_Index.Add(dualBladesProj.Projectile.whoAmI);
        //        SwingHelper.ProjFixedPlayerCenter(dualBladesProj.Player, -dualBladesProj.SwingLength * 0.35f,true,true);
        //        SwingHelper.SwingAI(dualBladesProj.SwingLength, dualBladesProj.Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
        //        //SwingHelper.VelScale = vector2;
        //    }
        //    public override void Draw(SpriteBatch sb, Color drawColor)
        //    {
        //        //SwingHelper.Swing_Draw_ItemAndAfterimage(drawColor, (x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);
        //        Effect effect = WeaponSkill.SwingEffect.Value;
        //        var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        //        effect.Parameters["uTransform"].SetValue(model * projection);
        //        effect.Parameters["uColorChange"].SetValue(0.95f);
        //        Main.graphics.GraphicsDevice.Textures[1] = dualBladesProj.SpawnItem.GetGlobalItem<DualBladesGlobalItem>().DrawColorTex;
        //        if (dualBladesProj.InArchdemonMode || dualBladesProj.InDemonMode) drawColor = Color.Lerp(Color.Red, drawColor, 0.9f);
        //        SwingHelper.Swing_Draw_ItemAndTrailling(drawColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0), effect);
        //    }
        //}
        public override void InitDefault()
        {
            Item.Size = new(44, 72);
            Item.damage = 74;
            Item.knockBack = 2;
            Item.crit = 7;
            Item.rare = ItemRarityID.Lime;
            Item.scale = 0.6f;
        }
        //public override void HoldItem(Player player)
        //{
        //    Item.scale = 1.8f;
        //    var blades = GetDualBlades(player,true);
        //    //if (HammerSwingRenderDraw.CanUseHammerSwingRender.Contains(Type))
        //    //{
        //    //    HammerSwingRenderDraw.CanUseHammerSwingRender.Add(Type);
        //    //}
        //    if (blades != null)
        //    {
        //        blades.Projectile.scale = 1;
        //        if (blades.BackBlades is not ChlorophyteBladesProj_Blades)
        //        {
        //            blades.BackBlades = new ChlorophyteBladesProj_Blades(blades, new DualBladesSwingHelper(blades, 18, blades.DrawProjTex));
        //        }
        //        if (blades.HeldBlades is not ChlorophyteBladesProj_Blades)
        //        {
        //            blades.HeldBlades = new ChlorophyteBladesProj_Blades(blades, new DualBladesSwingHelper(blades, 18, blades.DrawProjTex));
        //        }
        //    }
        //}
        //public override void AddRecipes()
        //{
        //    CreateRecipe().AddIngredient(ItemID.AntlionMandible, 20).AddRecipeGroup(RecipeGroupID.IronBar).AddTile(TileID.Anvils).Register();
        //}
    }
}
