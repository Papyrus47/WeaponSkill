using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.GunLances;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.General
{
    public class DrawShieldAndWeaponSystem : ILoadable
    {
        public static List<int> DrawShieldAndWeapon;
        public void Load(Mod mod)
        {
            DrawShieldAndWeapon = new();
            On_Main.DrawPlayers_AfterProjectiles += On_Main_DrawPlayers_AfterProjectiles;
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc += On_PlayerDrawLayers_DrawPlayer_10_BackAcc;
            Main.OnPostDraw += Main_OnPostDraw;
        }

        public void Unload()
        {
            if (DrawShieldAndWeapon == null)
                return;
            On_Main.DrawPlayers_AfterProjectiles -= On_Main_DrawPlayers_AfterProjectiles;
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc -= On_PlayerDrawLayers_DrawPlayer_10_BackAcc;
            Main.OnPostDraw -= Main_OnPostDraw;
        }
        private static void Main_OnPostDraw(GameTime obj)
        {
            DrawShieldAndWeapon.Clear();
        }
        public void On_PlayerDrawLayers_DrawPlayer_10_BackAcc(On_PlayerDrawLayers.orig_DrawPlayer_10_BackAcc orig, ref PlayerDrawSet drawinfo)
        {
            orig.Invoke(ref drawinfo);
            if (DrawShieldAndWeapon.Count > 0)
            {
                for (int i = 0; i < DrawShieldAndWeapon.Count; i++) // 剑绘制
                {
                    //if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    //var shield = modProj.shield;
                    //shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.swingHelper.center / 16).ToPoint()));

                    if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is ChargeBladeProj chargeBlade)
                    {
                        if (chargeBlade.chargeBladeGlobal.InAxe) continue;

                        Color color = Lighting.GetColor((chargeBlade.Projectile.Center / 16).ToPoint());
                        chargeBlade.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                    }
                    else if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is LancesProj lancesProj)
                    {
                        Color color = Lighting.GetColor((lancesProj.Projectile.Center / 16).ToPoint());
                        lancesProj.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                    }
                    else if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is GunLancesProj gunLancesProj)
                    {
                        Color color = Lighting.GetColor((gunLancesProj.Projectile.Center / 16).ToPoint());
                        gunLancesProj.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                    }
                }
            }
        }

        public void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig.Invoke(self);
            if (DrawShieldAndWeapon.Count > 0)
            {
                for (int i = 0; i < DrawShieldAndWeapon.Count; i++) // 盾绘制
                {
                    if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is ChargeBladeProj chargeBlade)
                    {
                        if (!chargeBlade.shieldCanDraw) continue;

                        var shield = chargeBlade.shield;
                        shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.swingHelper.center / 16).ToPoint()));
                    }
                    else if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is LancesProj lancesProj)
                    {
                        var shield = lancesProj.shield;
                        shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.Pos / 16).ToPoint()));
                    }
                    else if (Main.projectile[DrawShieldAndWeapon[i]].ModProjectile is GunLancesProj gunLancesProj)
                    {
                        var shield = gunLancesProj.shield;
                        shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.Pos / 16).ToPoint()));
                    }
                    //if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    //Color color = Lighting.GetColor((modProj.Projectile.Center / 16).ToPoint());
                    //modProj.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                }
            }
        }
    }
}
