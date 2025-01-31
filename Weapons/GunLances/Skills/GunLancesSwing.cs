using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesSwing : BasicGunLancesSkill
    {
        public GunLancesSwing(GunLancesProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile)
        {
            Rot = MathHelper.Pi + MathHelper.PiOver2;
            ActivationConditionFunc = activationConditionFunc;
        }
        public Func<bool> ActivationConditionFunc;
        public float Rot;
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        public int SwingDir;
        public float ActionDmg = 1f;
        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(StartVel, VelScale, VisualRotation);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
                SoundEngine.PlaySound(
                    SoundID.Item1.WithPitchOffset(-0.3f), // 降低音调更显沉重
                    player.Center
                );
                //swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            }
            if (Projectile.ai[0] < 20)
            {
                player.velocity.X = 2 * player.direction;
                PreAttack = true;
            }
            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(player.Center, player.direction);

            Projectile.spriteDirection = Projectile.direction = player.direction * SwingDir;
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            float factor = Projectile.ai[0] / 80f;
            factor = Math.Min(1, factor);
            factor = MathF.Pow(factor, 4);
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, factor * Rot * SwingDir);
            if (Projectile.ai[0] > 190)
            {
                Projectile.ai[0] = 0;
                Projectile.extraUpdates = 0;
                SkillTimeOut = true;
            }
            #region 部位更新
            Vector2 vel = Vector2.Zero;
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
            {
                handle.Update();
                vel = handle.Size;
            }
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
            {
                gun.OffestCenter = Vector2.Lerp(gun.OffestCenter, Projectile.velocity.SafeNormalize(default) * vel, 1f);
                gun.Update();
                gun.Rot = MathHelper.Lerp(gun.Rot, 0, 0.2f);
            }
            #endregion

        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            swingHelper.DrawTrailing(TextureAssets.Extra[209].Value,(x) => Color.White with { A = 0},null);
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
                gun.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
                handle.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool? CanDamage() => Projectile.ai[0] > 20;
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke();
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => nowSkill is not GunLancesSwing && nowSkill is BasicGunLancesSkill skill && skill.PreAttack && ActivationCondition();
        public override bool SwitchCondition() => Projectile.ai[0] >= 130;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage += ActionDmg - 1;
        }
        public override void OnSkillActive()
        {
            TheUtility.ResetProjHit(Projectile);
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.extraUpdates = 2;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            swingHelper.SetRotVel(0);
            swingHelper.SetNotSaveOldVel();
        }
    }
}
