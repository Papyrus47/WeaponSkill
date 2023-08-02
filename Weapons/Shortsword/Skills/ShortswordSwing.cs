using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.Shortsword.Skills
{
    public class ShortswordSwing : BasicShortswordSkill
    {
        public ShortswordSwing(ShortswordProj broadSwordProj) : base(broadSwordProj)
        {
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        public Func<float> TimeChange;
        public float TimeChangeMax;
        public float SwingRot;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public override void AI()
        {
            Projectile.extraUpdates = 2;
            player.itemLocation = Projectile.Center;
            swingHelper.Change(StartVel, VelScale, VisualRotation);
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            Projectile.ai[0] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(Shortsword.SpawnItem);
            float Time = Shortsword.TimeChange(Projectile.ai[0] / TimeChangeMax);
            if (Time > 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.ai[1]++;
                Time = 1 + (Projectile.ai[0] / TimeChangeMax) * 0.01f;
                swingHelper.SetNotSaveOldVel();
            }
            else if ((int)(Projectile.ai[0] / TimeChange.Invoke()) % 3 == 0)
            {
                TheUtility.Player_ItemCheck_EmitUseVisuals(player, Shortsword.SpawnItem, Projectile.Hitbox);
            }
            swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
            swingHelper.SwingAI(Shortsword.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

            if (Time > 1.02f)
            {
                SkillTimeOut = true;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Effect effect = WeaponSkill.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = Shortsword.DrawColorTex;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0), effect,(_) => 0);
            return false;
        }
        public override bool? CanDamage() => true;
        public override bool SwitchCondition() => Projectile.ai[1] > 4;
        public override bool ActivationCondition() => player.controlUseTile;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Projectile.damage < Projectile.originalDamage * 3) Projectile.damage += Projectile.originalDamage * 3 / 5;
            ItemLoader.OnHitNPC(Shortsword.SpawnItem, player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(Shortsword.SpawnItem, player, new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), Projectile.damage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
            TheUtility.SetPlayerImmune(player,8);
        }
        public override void OnSkillActive()
        {
            SoundEngine.PlaySound(Shortsword.SpawnItem.UseSound, player.position);
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
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

