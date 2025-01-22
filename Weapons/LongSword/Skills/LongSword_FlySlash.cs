using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public class LongSword_FlySlash : BasicLongSwordSkill
    {
        public LongSword_FlySlash(LongSwordProj longSword) : base(longSword)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((int)Projectile.ai[0] == 2)
            {
                Vector2 vel = player.velocity.SafeNormalize(default).RotatedByRandom(MathHelper.TwoPi);
                for (int i = 0; i < 7; i++)
                {
                    var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), target.Center - vel.RotatedBy(MathHelper.PiOver4 * i) * LongSword.SwingLength * 1.5f, vel.RotatedBy(MathHelper.PiOver4 * i), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, LongSword.SwingLength * 2.3f, 80, TextureAssets.Heart.Value);
                    proj.FixedPos = false;
                }
            }
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 0 || (int)Projectile.ai[0] == 2;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override void AI()
        {
            LongSword.swordScabbard.DrawPos = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0);
            LongSword.swordScabbard.Dir = player.direction;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 登龙突刺
                    player.velocity.X = player.direction * 10;
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.2f);
                    swingHelper.SetNotSaveOldVel();
                    swingHelper.SwingAI(LongSword.SwingLength,player.direction,0);
                    if(Projectile.ai[1]++ > 15) 
                    {
                        player.velocity.X = 0;
                        if(Projectile.numHits > 0)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                            player.velocity.Y = -15;
                            var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), player.Center + Vector2.UnitX, Vector2.UnitX, (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, LongSword.SwingLength * 2.3f, 80, TextureAssets.Heart.Value);
                            proj.FixedPos = false;
                        }
                        else
                        {
                            Projectile.ai[0] = 3;
                            swingHelper.Change(-Vector2.UnitY, Vector2.One);
                        }
                    }
                    break;
                case 1: // 跳起
                    swingHelper.Change_Lerp(-Vector2.UnitY, 0.2f, Vector2.One, 0.2f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SetNotSaveOldVel();
                    swingHelper.SwingAI(LongSword.SwingLength, player.direction, 0);
                    if(player.velocity.Y > 0)
                    {
                        Projectile.ai[0]++;
                        TheUtility.ResetProjHit(Projectile);
                    }
                    break;
                case 2: // 下落
                    if (player.velocity.Y == 0)
                    {
                        Projectile.ai[0]++;
                    }
                    Projectile.extraUpdates = 2;
                    player.velocity.Y = 20;
                    player.GetModPlayer<WeaponSkillPlayer>().playerFallSpeed = 50;
                    float Time = LongSword.TimeChange(Math.Min(1,Projectile.ai[1]++ / 15f));
                    Projectile.spriteDirection = player.direction;
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(LongSword.SwingLength, player.direction, MathHelper.PiOver2 * Time);
                    LongSwordProj.DrawLongSwordSwingShader_Index.Add(Projectile.whoAmI);
                    break;
                case 3: // 发呆50帧
                    player.velocity *= 0;
                    Projectile.extraUpdates = 0;
                    Projectile.spriteDirection = player.direction;
                    swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                    swingHelper.SwingAI(LongSword.SwingLength, player.direction, MathHelper.PiOver2);
                    if (Projectile.ai[1]++ > 50)
                    {
                        SkillTimeOut = true;
                    }
                    break;
            }
        }
        public override bool ActivationCondition() => player.controlUseItem && WeaponSkill.BowSlidingStep.Current;
        public override bool SwitchCondition() => false;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => !LongSword.InSpiritAttack ? new Color(0.2f, 0.2f, 0.2f, 0f) : new Color(2f, 2f, 2f, 0f));
            return false;
        }
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.numHits = 0;
            Projectile.rotation = 0;
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
