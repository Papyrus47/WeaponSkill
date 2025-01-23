using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.SwordShield
{
    public class SwordShield_Shield : BasicShield
    {
        public float ShieldRot;
        public Vector2 Pos;
        public int Dir;
        public float Rot;
        public readonly Asset<Texture2D> drawShieldTex;

        public SwordShield_Shield(SwordShieldProj proj, Asset<Texture2D> DrawShieldTex)
        {
            //this.chargeBladeProj = proj;
            //width = DrawShieldTex.Width() * 1.5f;
            //height = DrawShieldTex.Height() * 1.5f;
            Size = DrawShieldTex.Size() * proj.Projectile.scale;
            drawShieldTex = DrawShieldTex;
        }
        public override float GetDefence()
        {
            float def = base.GetDefence();
            return def / 10;
        }
        public override void ModifyHit(ref Player.HurtModifiers hurtModifiers)
        {
            base.ModifyHit(ref hurtModifiers);
            hurtModifiers.FinalDamage *= 0.6f;
        }
        public virtual void Update(Vector2 Pos, int Dir,float rot)
        {
            InDef = false;
            this.Pos = Pos;
            this.Dir = Dir;
            this.Rot = rot;
        }
        public override bool GetDefSucced(Rectangle hitbox) => true;
        public virtual void Draw(SpriteBatch spriteBatch,Color DrawColor)
        {
            spriteBatch.Draw(drawShieldTex.Value, Pos - Main.screenPosition, null, DrawColor, Rot, drawShieldTex.Size() * 0.5f, Size / drawShieldTex.Size(), SpriteEffects.None, 0);
        }
    }
}
