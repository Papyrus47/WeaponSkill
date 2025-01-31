using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public class GunLancesSpurts : BasicGunLancesSkill
    {
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;
        public Func<bool> ActivationConditionFunc;
        public bool IsMove = true;

        public GunLancesSpurts(GunLancesProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile)
        {
            ActivationConditionFunc = activationConditionFunc;
        }

        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(Vector2.UnitX, new Vector2(1f, 0f), 0);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
                SoundEngine.PlaySound(SoundID.Item7.WithPitchOffset(0.5f));
            }
            if (Projectile.ai[0] < 20)
            {
                if (IsMove)
                    player.velocity.X = 5 * player.direction;
                PreAttack = true;
            }
            else if (Projectile.ai[0] == 20)
            {
                SpurtsProj proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromThis(), player.Center, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * ActionDmg), Projectile.knockBack, Projectile.owner, GunLancesProj.SwingLength * 2.5f, 100, TextureAssets.Heart.Value);
            }
            #region 盾更新
            var lancesShield = GunLancesProj.shield;
            lancesShield.Update(player.Center, player.direction);
            #endregion

            swingHelper.ProjFixedPlayerCenter(player, (5 - Projectile.ai[0] * Projectile.ai[0] * Projectile.ai[0] / 250000f) * 2f, true);
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.SwingAI(GunLancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[0] > 150)
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

            //swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Gun", out var gun))
                gun.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Handle", out var handle))
                handle.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 130;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            Projectile.extraUpdates = 4;
        }
    }
}
