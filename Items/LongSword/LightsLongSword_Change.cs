using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.LongSword.Skills;
using Terraria.ID;

namespace WeaponSkill.Items.LongSword
{
    public class LightsLongSword_Change: LightsLongSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage += 30;
            Item.rare = ItemRarityID.Lime;
            Item.scale = 1.4f; 
            if (Item.TryGetGlobalItem<LongSwordGlobalItem>(out var globalItem))
            {
                globalItem.ScabbardAction.Add((scabbard) =>
                {
                    scabbard.DrawOrigin.Y += 4;
                    scabbard.DrawOrigin.X -= 4 * scabbard.Dir;
                });
            }
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/LightsLongSwordScabbard");
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var longSword = GetLongSwordProj(player);
            //if (longSword != null && (longSword.CurrentSkill is LongSword_Naknotsu_RotSlash || longSword.CurrentSkill is LongSword_SerenePose) && player.ownedProjectileCounts[ModContent.ProjectileType<SpurtsProj_JudgmentCut>()] < 30)
            //{
            //    for (int i = player.ownedProjectileCounts[ModContent.ProjectileType<SpurtsProj_JudgmentCut>()]; i < 30; i++)
            //    {
            //        Vector2 vel = Main.rand.NextVector2Unit();
            //        SpurtsProj_JudgmentCut proj = SpurtsProj_JudgmentCut.NewSpurtsProj(player.GetSource_OnHit(target), Main.screenPosition + new Vector2(Main.rand.Next(Main.screenWidth),Main.rand.Next(Main.screenHeight)) - (vel * 800), vel, hit.Damage, hit.Knockback / 5, player.whoAmI, 3000, Main.rand.Next(3,6) * 10, SpurtsTex);
            //        proj.FixedPos = false;
            //        proj.AIAction = (jc) =>
            //        {
            //            if (jc.Projectile.ai[2]++ > 30)
            //            {
            //                jc.CanUpdateScale = true;
            //            }
            //        };
            //    }
            //}
            if (longSword != null && (longSword.CurrentSkill is LongSword_Naknotsu_RotSlash || longSword.CurrentSkill is LongSword_SerenePose))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel =Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                    SpurtsProj proj = SpurtsProj.NewSpurtsProj(player.GetSource_OnHit(target), target.Center - (vel * 300), vel, hit.Damage, hit.Knockback / 5, player.whoAmI, 600,50, SpurtsTex);
                    proj.FixedPos = false;
                    //proj.AIAction = (jc) =>
                    //{
                    //    if (jc.Projectile.ai[2]++ > 30)
                    //    {
                    //        jc.CanUpdateScale = true;
                    //    }
                    //};

                    SpurtsProj_JudgmentCut proj1 = SpurtsProj_JudgmentCut.NewSpurtsProj(player.GetSource_OnHit(target), target.Center - (vel * 300), vel.RotatedBy(0.3), hit.Damage, hit.Knockback / 5, player.whoAmI, 600, Main.rand.Next(3, 6) * 5);
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
            base.OnHitNPC(player, target, hit, damageDone);
        }
    }
}
