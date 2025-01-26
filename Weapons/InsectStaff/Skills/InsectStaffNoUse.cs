using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaffNoUse : BasicInsectStaffSkill
    {
        public InsectStaffNoUse(InsectStaffProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            Projectile.spriteDirection = -player.direction;
            swingHelper.Change_Lerp(Vector2.UnitY.RotatedBy(0.4), 1, Vector2.One, 1f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.5f);
            swingHelper.SetSwingActive();
            swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, 0);

        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition() => true;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null);
            return false;
        }
    }
}