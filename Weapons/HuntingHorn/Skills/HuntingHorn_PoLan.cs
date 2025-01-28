using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;
using static WeaponSkill.Weapons.HuntingHorn.HuntingHornMelody;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_PoLan : BasicHuntingHornSkill
    {
        public HuntingHorn_PoLan(HuntingHornProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            switch ((int)Projectile.ai[0])
            {
                case 0: // 缓慢的手持
                    swingHelper.Change_Lerp(-Vector2.UnitY, 0.3f, Vector2.One, 0.3f);
                    swingHelper.ProjFixedPlayerCenter(player,-HuntingHornProj.SwingLength * 0.5f,true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction,0);
                    Projectile.position.X += player.direction * Projectile.width * 0.2f;
                    player.itemRotation += MathHelper.PiOver2 * player.direction;
                    Projectile.ai[1]++;
                    Projectile.spriteDirection = player.direction;
                    if (Projectile.ai[1] > 30)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 1;
                    }
                    break;
                case 1: // 波浪鼓
                    swingHelper.Change_Lerp(-Vector2.UnitY, 0.3f, Vector2.One, 0.3f);
                    swingHelper.ProjFixedPlayerCenter(player, -HuntingHornProj.SwingLength * 0.5f, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    Projectile.position.X += player.direction * Projectile.width * 0.2f;
                    player.itemRotation += MathHelper.PiOver2 * player.direction;
                    if (Projectile.ai[1]++ % 5 == 0)
                    {
                        TheUtility.ResetProjHit(Projectile);
                        Projectile.spriteDirection = -Projectile.spriteDirection;
                        for (int i = 0; i < 20; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity * 0.75f, 1, 1,DustID.FireworksRGB,0,0,100);
                            dust.noGravity = true;
                            dust.velocity = Main.rand.NextVector2Unit() * HuntingHornProj.SwingLength * 0.01f;
                            dust.color = Color.SkyBlue with { A = 0} * 0.3f;
                        }
                    }
                    else if (Projectile.ai[1] > 30)
                    {
                        Projectile.ai[0] = 2;
                    }
                    break;
                case 2: // 等待跳出
                    if (Projectile.ai[1]++ > 30)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.Change_Lerp(-Vector2.UnitY, 0.3f, Vector2.One, 0.3f);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    break;
            }
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition() => WeaponSkill.BowSlidingStep.Current;
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndAfterimage(lightColor, (x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 2;
            player.SetImmuneTimeForAllTypes(8);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            if (ActivationCondition())
            {
                HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
                HuntingHornMelody hornMelody = huntingHornGlobalItem.hornMelody;
                hornMelody.melodies.Enqueue(MelodyType.SP);
                HuntingHornBuff item = hornMelody.FindMelody(player, Projectile);
                if (item != null)
                    huntingHornGlobalItem.huntingHornBuffs.Enqueue(item);
            }
        }
    }
}
