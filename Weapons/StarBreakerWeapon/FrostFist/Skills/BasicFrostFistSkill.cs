using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class BasicFrostFistSkill : ProjSkill_Instantiation
    {
        public FrostFistProj FrostFist => modProjectile as FrostFistProj;
        public Player Player;
        /// <summary>
        /// 可以用于切换停止技能
        /// </summary>
        public bool CanChangeToStopActionSkill
        {
            get
            {
                return FrostFist.CanChangeToStopActionSkill;
            }
            set
            {
                FrostFist.CanChangeToStopActionSkill = value;
            }
        }

        /// <summary>
        /// 攻击前摇判定
        /// </summary>
        public bool PreAtk;
        public BasicFrostFistSkill(FrostFistProj modProjectile) : base(modProjectile)
        {
            Player = modProjectile.Player;
        }
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
        }
        public Dust FrostFistDust()
        {
            if (Player.HandPosition.HasValue)
            {
                Vector2 center = Player.HandPosition.Value - Player.velocity;
                Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FrostStaff, Player.direction * 2, 0, 150, default, 1.3f);
                dust.position = center;
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.fadeIn = 1;
                dust.velocity += Player.velocity * 0.1f;
                if (Player.velocity.LengthSquared() < 10) dust.velocity.Y -= 3f;
                if (Main.rand.NextBool())
                {
                    dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
                    dust.scale += Main.rand.NextFloat();
                    //if (Main.rand.NextBool())
                    //{
                    //    dust.customData = Player;
                    //}
                }
                return dust;
            }
            return null;
        }
        /// <summary>
        /// 返回false则是没有站在物块上
        /// </summary>
        /// <returns></returns>
        public bool GetPlayerStandTile()
        {
            bool flag = false;
            Point point = (Player.Bottom / 16).ToPoint();
            for (int i = 0; i < 2; i++)
            {
                Point p = point;
                p.Y += i;
                Tile tile = Main.tile[p];
                if (tile.HasTile && tile.HasUnactuatedTile)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }
}
