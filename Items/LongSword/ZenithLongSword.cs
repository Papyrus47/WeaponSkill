using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword.Skills;
using Terraria.ID;
using WeaponSkill.Weapons.General;
using Terraria.Graphics;

namespace WeaponSkill.Items.LongSword
{
    public class ZenithLongSword : BasicLongSwordItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Zenith;
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Zenith);
            Item.scale *= 5;
            Item.shoot = ProjectileID.None;
            Item.shootSpeed = 0;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var longSword = GetLongSwordProj(player);
            if (longSword != null)
            {
                int proj = Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center,(Main.MouseWorld - player.Center) * 0.5f, 933, hit.SourceDamage / 30, hit.Knockback, player.whoAmI, Main.rand.Next(-20, 21), FinalFractalHelper.GetRandomProfileIndex());
                if(longSword.CurrentSkill is LongSword_SakuraSlashed || longSword.CurrentSkill is LongSword_Naknotsu_RotSlash)
                {
                    Main.projectile[proj].velocity.Y *= 0f;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vel;
                        vel = (Main.MouseWorld - player.Center) * 0.5f;
                        proj = Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center + vel, vel, 933, hit.SourceDamage / 30, hit.Knockback, player.whoAmI, Main.rand.Next(-20, 21), FinalFractalHelper.GetRandomProfileIndex());
                        Main.projectile[proj].velocity.Y *= 0f;

                        vel = Vector2.One.RotatedByRandom(MathHelper.TwoPi);

                        SpurtsProj_JudgmentCut proj1 = SpurtsProj_JudgmentCut.NewSpurtsProj(player.GetSource_OnHit(target), target.Center - (vel * 300), vel.RotatedBy(0.3),0, hit.Knockback / 5, player.whoAmI, 600, Main.rand.Next(3, 6) * 5);
                        proj1.FixedPos = false;
                        proj1.AIAction = (jc) =>
                        {
                            if (jc.Projectile.ai[2]++ > 30)
                            {
                                jc.CanUpdateScale = true;
                            }
                        };
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Zenith).Register();
            Recipe recipe = Recipe.Create(ItemID.Zenith);
            recipe.AddIngredient(Type).Register();
        }
    }
}
