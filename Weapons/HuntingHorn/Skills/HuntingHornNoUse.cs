using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.HuntingHorn.HuntingHornMelody;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHornNoUse : BasicHuntingHornSkill
    {
        public HuntingHornNoUse(HuntingHornProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            Projectile.ai[0] += player.velocity.X * 0.01f;
            swingHelper.Change_Lerp(-Vector2.UnitX.RotatedBy(-0.2 + MathF.Sin(Projectile.ai[0]) * 0.1f), 0.5f, Vector2.One, 0.5f);
            swingHelper.ProjFixedPlayerCenter(player, HuntingHornProj.SwingLength * -0.1f);
            swingHelper.SetSwingActive();
            swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
            player.heldProj = Projectile.whoAmI;
            Projectile.position.X -= player.direction * Projectile.width * 0.1f;
            Projectile.position.Y -= Projectile.height * 0.2f;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi - 0.3f * player.direction);

            //HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
            //HuntingHornMelody hornMelody = huntingHornGlobalItem.hornMelody;
            ////hornMelody.melodies.Enqueue(melodyType);
            //HuntingHornBuff item = hornMelody.FindMelody(player, Projectile);
            //if (item != null)
            //    huntingHornGlobalItem.huntingHornBuffs.Enqueue(item);
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition() => true;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null);
            return false;
        }
    }
}
