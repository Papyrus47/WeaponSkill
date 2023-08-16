using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill
{
    public static class TheUtility
    {
        public static void VillagesItemOnHit(Item item,Player player, Rectangle itemRectangle, int damage, float knockBack, int npcIndex, int dmgRandomized, int dmgDone)
        {
            Type type = player.GetType();
            type.GetMethod("ApplyNPCOnHitEffects", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(player,new object[] {item, itemRectangle ,damage,knockBack,npcIndex,dmgRandomized,dmgDone});
        }
        public static void Player_ItemCheck_Shoot(Player player, Item sItem, int weaponDamage)
        {
            Type type = player.GetType();
            type.GetMethod("ItemCheck_Shoot",BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(player,new object[] {player.whoAmI,sItem,weaponDamage});
        }
        public static void Player_ItemCheck_EmitUseVisuals(Player player,Item item,Rectangle hitBox)
        {
            Type type = player.GetType();
            type.GetMethod("ItemCheck_EmitUseVisuals", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(player, new object[] { item,hitBox });
        }
        public static void ResetProjHit(Projectile proj)
        {
            for (int i = 0; i < proj.localNPCImmunity.Length; i++)
            {
                proj.localNPCImmunity[i] = 0;
            }
        }
        public static List<CustomVertexInfo> GenerateTriangle(List<CustomVertexInfo> customs)
        {
            List<CustomVertexInfo> triangleList = new();
            for (int i = 0; i < customs.Count - 2; i += 2)
            {
                triangleList.Add(customs[i]);
                triangleList.Add(customs[i + 2]);
                triangleList.Add(customs[i + 1]);

                triangleList.Add(customs[i + 1]);
                triangleList.Add(customs[i + 2]);
                triangleList.Add(customs[i + 3]);
            }
            return triangleList;
        }
        public static void GetWeaponDrawColor(Texture2D DrawColorTex,Asset<Texture2D> texture,float factor = 0.9f,float colorScale = 1.95f)
        {
            Main.QueueMainThreadAction(() =>
            {
                Color[] TexColor = new Color[texture.Width() * texture.Height()];
                int width = texture.Width();
                texture.Value.GetData(TexColor);
                Color[] colors = new Color[texture.Height()];
                for (int i = colors.Length - 1; i > 0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Color value1 = TexColor[(colors.Length - i) * width + j];
                        if (value1 == default) continue;
                        colors[i] = Color.Lerp(value1, colors[i],factor);
                    }
                    colors[i] *= colorScale;
                }
                DrawColorTex.SetData(colors);
            });
        }
        public static void SetPlayerImmune(Player player,int immuneTime = 12)
        {
            if (player.immuneTime < immuneTime) player.SetImmuneTimeForAllTypes(immuneTime);
        }
    }
}
