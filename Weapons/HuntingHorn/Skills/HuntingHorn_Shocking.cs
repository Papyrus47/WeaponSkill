using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.HuntingHorn.HuntingHornMelody;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_Shocking : BasicHuntingHornSkill
    {
        public HuntingHorn_Shocking(HuntingHornProj proj) : base(proj)
        {
        }
        public List<NPC> HitNPCs = new();
        public override void AI()
        {
            Projectile.spriteDirection = player.direction;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 预备锤打
                    swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.2f);
                    swingHelper.ProjFixedPlayerCenter(player, -HuntingHornProj.SwingLength * 0.5f, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    if (Projectile.ai[1]++ > 30)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                    }
                    break;
                case 1: // 锤打
                    Projectile.extraUpdates = 3;
                    swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.2f);
                    swingHelper.ProjFixedPlayerCenter(player, (Projectile.ai[1] / 30f * HuntingHornProj.SwingLength) - (HuntingHornProj.SwingLength * 0.5f), true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    if (Projectile.ai[1]++ > 30)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        if (HitNPCs.Count <= 0)
                        {
                            Projectile.ai[0] = 3;
                        }
                    }
                    break;
                case 2: // 电吉他启动
                    Projectile.extraUpdates = 0;
                    swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.2f);
                    swingHelper.ProjFixedPlayerCenter(player, -HuntingHornProj.SwingLength * 0.5f, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    player.itemRotation += Projectile.ai[1] * player.direction * 0.3f;
                    if (Projectile.ai[1]++ > 3)
                    {
                        Projectile.ai[1] = 0;
                        if (Projectile.ai[2]++ > 8)
                        {
                            Projectile.ai[1] = Projectile.ai[2] = 0;
                            Projectile.ai[0]++;
                            break;
                        }
                        SoundEngine.PlaySound(SoundID.Item135 with { Pitch = -0.3f, Volume = 0.3f }, player.position);
                        HitNPCs.ForEach(x => player.ApplyDamageToNPC(x, Projectile.damage * 5, Projectile.knockBack * 5, player.direction, Main.rand.NextBool(Projectile.CritChance, 100), DamageClass.Magic));
                    }
                    break;
                case 3: // 等待切换
                    if (Projectile.ai[1]++ > 60)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.Change_Lerp(Vector2.UnitX, 0.2f, Vector2.One, 0.2f);
                    swingHelper.ProjFixedPlayerCenter(player, -HuntingHornProj.SwingLength * 0.5f, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    break;
            }
        }
        public override bool? CanDamage() => (int)Projectile.ai[0] == 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition() => WeaponSkill.SpKeyBind.Current;
        public override bool SwitchCondition() => Projectile.ai[0] >= 3;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndAfterimage(lightColor, (x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);
            for(int i = 0; i < HitNPCs.Count; i++)
            {
                if (HitNPCs[i].active)
                    Utils.DrawLine(sb, HitNPCs[i].Center, player.MountedCenter, Color.SkyBlue, Color.SkyBlue,2);
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 4;
            player.SetImmuneTimeForAllTypes(8);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            HitNPCs.Add(target);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            HitNPCs.Clear();
            TheUtility.ResetProjHit(Projectile);
            //if (WeaponSkill.BowSlidingStep.Current && !ChangeIsTrue)
            //{
            //    melodyType = HuntingHornMelody.MelodyType.SP;
            //}
            //else
        }
    }
}
