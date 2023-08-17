using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.General
{
    public class SpurtsProj_JudgmentCut : SpurtsProj
    {
        public static List<int> JudgmentCutProj;
        public bool CanUpdateScale;
        public bool CanKill;
        public override void Load()
        {
            base.Load();
            JudgmentCutProj = new();
        }
        public override void Unload()
        {
            base.Unload();
            JudgmentCutProj = null;
        }
        public override void AI()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
            player ??= Main.player[Projectile.owner];
            if (Projectile.ai[2] < 8) Projectile.ai[2]++;
            if(CanUpdateScale) Projectile.ai[1] *= 0.55f;
            if (CanKill)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[1] <= 0.3f)
            {
                CanKill = true;
            }
            if (FixedPos) Projectile.Center = player.Center;
            JudgmentCutProj.Add(Projectile.whoAmI);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is SpurtsProj_JudgmentCut_RenderDraw))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new SpurtsProj_JudgmentCut_RenderDraw());
            }
            return false;
        }
        public override bool? CanDamage() => CanKill;
        public static new SpurtsProj_JudgmentCut NewSpurtsProj(IEntitySource source, Vector2 pos, Vector2 vel, int damage, float kn, int onwer, float width, float height, Texture2D DrawColorTex = null)
        {
            int v = Projectile.NewProjectile(source, pos, vel, ModContent.ProjectileType<SpurtsProj_JudgmentCut>(), damage, kn, onwer, width, height);
            (Main.projectile[v].ModProjectile as SpurtsProj).DrawColorTex = DrawColorTex;
            return Main.projectile[v].ModProjectile as SpurtsProj_JudgmentCut;
        }
    }
}
