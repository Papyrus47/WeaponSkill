using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Shortsword
{
    public class SpurtsProj : ModProjectile
    {
        public Player player;
        public Texture2D DrawColorTex;
        public bool FixedPos;
        public Action<NPC,NPC.HitInfo,int> OnHit;
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            FixedPos = true;
        }
        public override void AI()
        {
            player ??= Main.player[Projectile.owner];
            if(Projectile.ai[2] < 8) Projectile.ai[2]++;
            Projectile.ai[1] *= 0.85f;
            if (Projectile.ai[1] <= 0) Projectile.Kill();
            if(FixedPos) Projectile.Center = player.Center;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[2] < 8;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),projHitbox.Center(),projHitbox.Center() + Projectile.velocity * Projectile.ai[0], Projectile.ai[1],ref r);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHit?.Invoke(target,hit,damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpurtsDraw spurtsDraw = new()
            {
                Direction = Projectile.velocity,
                Width = Projectile.ai[0],
                DrawPos = Projectile.Center,
                Height = Projectile.ai[1],
                DrawColor = new Color(255,255,255, 255),
                ScreenCorrect = true
            };
            if (DrawColorTex?.IsDisposed == true) return false;
            Main.graphics.GraphicsDevice.Textures[2] = DrawColorTex;
            spurtsDraw.Draw(Main.spriteBatch, WeaponSkill.SpurtsShader.Value);
            return false;
        }
        public static SpurtsProj NewSpurtsProj(IEntitySource source, Vector2 pos, Vector2 vel, int damage, float kn, int onwer, float width, float height, Texture2D DrawColorTex = null)
        {
            int v = Projectile.NewProjectile(source, pos, vel, ModContent.ProjectileType<SpurtsProj>(), damage, kn, onwer, width, height);
            (Main.projectile[v].ModProjectile as SpurtsProj).DrawColorTex = DrawColorTex;
            return Main.projectile[v].ModProjectile as SpurtsProj;
        }
    }
}
