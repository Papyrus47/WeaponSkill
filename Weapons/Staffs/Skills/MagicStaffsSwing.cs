using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;
using WeaponSkill.Helper;
using Microsoft.Xna.Framework.Graphics;

namespace WeaponSkill.Weapons.Staffs.Skills
{
    public class MagicStaffsSwing : BasicMagicStaffsSkill
    {
        // 绘制209为拖尾
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float SwingRot;
        public bool SwingDirectionChange;
        public float ChangeLerpSpeed;
        public float VisualRotation;
        /// <summary>
        /// 挥舞所需要时间
        /// </summary>
        public float SwingTime;
        /// <summary>
        /// 挥舞变化方式
        /// </summary>
        public Func<float,float> TimeChange;
        /// <summary>
        /// 如何使用这个技能
        /// </summary>
        public Func<Player,bool> ChangeCondition;
        /// <summary>
        /// 在这里调用额外Shoot
        /// </summary>
        public Action<MagicStaffsSwing> Shoot;
        /// <summary>
        /// 在这里使用特定AI
        /// </summary>
        public Action<MagicStaffsSwing> OnUse;
        public MagicStaffsSwing(MagicStaffsProj modProjectile, Func<float, float> timeChange, Func<Player, bool> changeCondition) : base(modProjectile)
        {
            TimeChange = timeChange;
            ChangeCondition = changeCondition;
        }
        public override void AI()
        {
            Projectile.extraUpdates = 2;
            Player.itemLocation = Projectile.Center;
            OnUse.Invoke(this);
            SwingHelper.Change(StartVel, VelScale, VisualRotation);
            Projectile.spriteDirection = Player.direction * SwingDirectionChange.ToDirectionInt();
            Projectile.ai[0] += Player.GetWeaponAttackSpeed(staffsProj.SpawnItem) / 2;
            float Time = TimeChange.Invoke(Projectile.ai[0] / SwingTime);
            if (Time > 1)
            {
                Projectile.extraUpdates = 0;
                if (Projectile.ai[1]++ == 1)
                {
                    Shoot.Invoke(this);

                    Player.HeldItem.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                    ItemLoader.Shoot(Player.HeldItem, Player, (EntitySource_ItemUse_WithAmmo)Player.GetSource_ItemUse_WithPotentialAmmo(Player.HeldItem, -1), Projectile.Center, (Main.MouseWorld - Player.Center).SafeNormalize(default) * Player.HeldItem.shootSpeed, Player.HeldItem.shoot, Projectile.damage, Projectile.knockBack);
                    TheUtility.Player_ItemCheck_Shoot(Player, Player.HeldItem, Projectile.damage);
                    Player.HeldItem.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                }
                Time = 1 + (Projectile.ai[0] / SwingTime) * 0.01f;
                SwingHelper.SetNotSaveOldVel();
            }
            SwingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
            SwingHelper.SwingAI(staffsProj.SwingLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

            if (Time > 1.02f)
            {
                SkillTimeOut = true;
            }
        }
        public override bool SwitchCondition()
        {
            float Time = TimeChange.Invoke(Projectile.ai[0] / SwingTime);
            if (Time > 1)
                Time = 1 + (Projectile.ai[0] / SwingTime) * 0.01f;
            if (Time > 1.01)
                return true;
            return false;
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke(Player);
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (f) => new Color(50, 100, 255, 0) * f);
            return false;
        }
        public override bool? CanDamage() => true;
        public override void OnSkillActive()
        {
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.extraUpdates = 0;
            TheUtility.ResetProjHit(Projectile);
        }
    }
}
