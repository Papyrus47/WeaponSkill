using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Staffs.Skills
{
    /// <summary>
    /// 手持法杖
    /// </summary>
    public class HeldMagicStaff : BasicMagicStaffsSkill
    {
        public HeldMagicStaff(MagicStaffsProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.extraUpdates = 0;
            Projectile.rotation = 0;
            SwingHelper.SetRotVel(0);
            #region 处理玩家运动法杖偏移
            float rot = Math.Clamp(Player.velocity.X * 0.2f * Player.direction,-1,1) - MathF.Sin(Projectile.ai[0]) * 0.2f;
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 6.28)
                Projectile.ai[0] -= 6.28f;
            Vector2 vel = (rot + MathHelper.PiOver2).ToRotationVector2();
            SwingHelper.Change(vel, Vector2.One, 0);
            #endregion
            #region 手持法杖
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot * Player.direction);
            Player.heldProj = Projectile.whoAmI;
            SwingHelper.ProjFixedPlayerCenter(Player, 0);
            SwingHelper.SetSwingActive();
            SwingHelper.SwingAI(staffsProj.SwingLength, Player.direction, 0f);
            #endregion
        }
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition() => true;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
        }
        public override void OnSkillDeactivate()
        {
            TheUtility.ResetProjHit(Projectile);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwingHelper.DrawSwingItem(lightColor);
            //SwingHelper.Swing_Draw_ItemAndAfterimage(lightColor,(factor) => new Color(100,100,100,0) * factor,10);
            return false;
        }
    }
}
