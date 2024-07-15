using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.NPCs;
using WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    /// <summary>
    /// 拳追加爆炸
    /// </summary>
    public class FrostFist_FistBoom : WeaponSkillGlobalNPCComponent
    {
        public int Time;
        public Player player;
        public Vector2 Vel;
        public int Dmg;
        public Action<NPC> ExtraAI;
        public FrostFist_FistBoom(int time, Player player, Vector2 vel, int dmg)
        {
            Time = time;
            this.player = player;
            Vel = vel;
            Dmg = dmg;
        }

        public override void AI(NPC target)
        {
            if (Time-- < 0)
            {
                ExtraAI?.Invoke(target);

                target.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime /= 10;
                int def = target.defense;
                target.defense = 0;
                player.ApplyDamageToNPC(target, Dmg, 0f, player.direction, false);
                for (int i = 0; i < 50; i++)
                {
                    Dust dust = Dust.NewDustDirect(target.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue, 1.3f);
                    dust.position = target.Center;
                    dust.velocity *= 0;
                    dust.noGravity = true;
                    dust.fadeIn = 1;
                    dust.velocity -= Vel.RotatedByRandom(0.4).SafeNormalize(default) * Main.rand.NextFloat(0.2f, 1f) * 10f;
                }
                target.defense = def;
                if (target.knockBackResist != 0)
                    target.velocity = Vel;
                Remove = true;

                #region 属性伤害
                player.addDPS((int)ElementDamageSystem.Instance.ElementDamageApply(new IceElementDamage() { baseDamage = Dmg * 5 }, target));
                #endregion
            }
        }
    }
}
