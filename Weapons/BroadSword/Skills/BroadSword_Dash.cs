using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.BroadSword.Skills
{
    public class BroadSword_Dash : BasicBroadSwordSkill
    {
        public BroadSword_Dash(BroadSwordProj broadSwordProj) : base(broadSwordProj)
        {
        }
        public byte Level;
        public BroadSwordSwing ChangeSkill;
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 15) // 铁山靠持续时间
            {
                player.endurance += 0.3f; // 提升免疫伤害率
                player.velocity.X = player.direction * 5;
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitX * player.direction, 2, 2, 6));
            }
            else if (Projectile.ai[0] > 35) // 跳出,回归
            {
                SkillTimeOut = true;
            }
            Projectile.damage = (int)(Projectile.originalDamage * (Level * 0.4f + 0.5f));
            #region 剑拿着
            Projectile.spriteDirection = player.direction * ChangeSkill.SwingDirectionChange.ToDirectionInt();
            swingHelper.Change_Lerp(ChangeSkill.StartVel, ChangeSkill.ChangeLerpSpeed, ChangeSkill.VelScale, ChangeSkill.ChangeLerpSpeed, ChangeSkill.VisualRotation, ChangeSkill.ChangeLerpSpeed);
            swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
            swingHelper.SwingAI(broadSword.SwingLength, player.direction, 0);
            #endregion
        }
        public override bool ActivationCondition() => false;
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (player.controlUseTile && player.controlUseItem) // 在右键的时候强制切换到铁山靠
            {
                Level = broadSword.ChangeLevel;
                ChangeSkill = nowSkill as BroadSwordSwing;
                return true;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 0.2f * Level;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Effect effect = WeaponSkill.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = broadSword.DrawColorTex;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0), effect, (_) => 0);
            return false;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[0] > 20;
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => targetHitbox.Intersects(player.Hitbox);
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Level = 0;
            Projectile.damage = Projectile.originalDamage;
        }
    }
}
