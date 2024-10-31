using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.Lances.Skills
{
    /// <summary>
    /// 飞升跃入突刺
    /// </summary>
    public class LancesSpurts_FlySpurts : LancesSpurts
    {
        public LancesSpurts_FlySpurts(LancesProj lancesProj) : base(lancesProj)
        {
            ActivationConditionFunc = () => player.controlUseTile;
        }
        public override void AI()
        {
            //if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(Vector2.UnitX, new Vector2(1f, 0f), 0);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
            }
            if (Projectile.ai[0] < 20)
            {
                player.velocity.X = 8 * player.direction;
                player.velocity.Y = -2;
                PreAttack = true;
            }
            else if (Projectile.ai[0] == 20)
            {
                for (int i = 0; i < 3; i++)
                {
                    SpurtsProj proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromThis(), player.Center, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * ActionDmg), 0, Projectile.owner, lancesProj.SwingLength * 2.5f, 100, TextureAssets.Heart.Value);
                }
            }
            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(player.Center, player.direction);

            swingHelper.ProjFixedPlayerCenter(player, (5 - Projectile.ai[0] * Projectile.ai[0] * Projectile.ai[0] / 250000f) * 2f, true);
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[0] > 150)
            {
                Projectile.ai[0] = 0;
                Projectile.extraUpdates = 0;
                SkillTimeOut = true;
            }
        }
    }
}
