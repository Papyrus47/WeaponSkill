using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing_SkyFall : FrostFist_SwordSwing
    {
        public FrostFist_SwordSwing_SkyFall(FrostFistProj modProjectile, Func<bool> changeCondition) : base(modProjectile, changeCondition)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Player.position -= Collision.TileCollision(Player.position, -Vector2.UnitY * 5, Player.width, Player.height);
                    if (GetPlayerStandTile()) break;
                }
                if(!GetPlayerStandTile() && Projectile.ai[1] > AtkTime - 2)
                {
                    Projectile.ai[1] = AtkTime - 1;
                    Player.velocity.Y = -1;
                }
                else if (GetPlayerStandTile() && Projectile.ai[1] == AtkTime)
                {
                    Player.position.Y -= 16;
                    for (int i = 0; i < 30; i++)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Player.Center + new Vector2(i / 30f * FrostFist.SwordLength * Player.direction, 0), 1, 1, DustID.FrostStaff, Player.direction * 2, 0, 150, default, 1.3f);
                            dust.velocity.Y = -j * 0.8f;
                            dust.velocity.X = 0;
                            dust.velocity = dust.velocity.RotatedByRandom(0.2);
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && !GetPlayerStandTile();
        }
    }
}
