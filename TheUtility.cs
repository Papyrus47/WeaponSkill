using StarBreaker.Content.Appraise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Weapons.General;

namespace WeaponSkill
{
    public static class TheUtility
    {
        public static bool InBegin()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            FieldInfo field = spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null && field.GetValue(spriteBatch) is bool canDraw)
            {
                return canDraw;
            }
            return false;
        }
        public static T DeepClone<T>(this T obj)
        {
            #region 空检查判定
            if (obj == null)
                return obj;
            #endregion

            #region type判定
            var type = obj.GetType();
            if (obj is string || type.IsValueType)
                return obj;
            #endregion

            var result = Activator.CreateInstance(type); // 创建T的实例
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic); // 获得所有字段
            foreach (var field in fields)
            {
                field.SetValue(result, field.GetValue(obj)); // Clone下来所有的字段
            }
            return (T)result;
        }
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
        public static int GetItemFrameCount(Item item)
        {
            if(Main.itemAnimations[item.type] != null)
            {
                return Main.itemAnimations[item.type].FrameCount;
            }
            return 1;
        }
        public static void SetProjFrameWithItem(Projectile proj, Item item)
        {
            if (Main.itemAnimations[item.type] != null) proj.frame = Main.itemAnimations[item.type].Frame;
        }
        public static void SetPlayerImmune(Player player,int immuneTime = 12)
        {
            if (player.immuneTime < immuneTime) player.SetImmuneTimeForAllTypes(immuneTime);
        }
        /// <summary>
        /// 用这个直接获取当前实例的命名空间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetInstancePart(this object obj)
        {
            string nameSpace = obj.GetType().Namespace;
            nameSpace = nameSpace.Replace('.', '/');
            nameSpace += "/";
            return nameSpace;
        }
        /// <summary>
        /// 用这个直接获取当前实例的命名空间附带名字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetInstancePartWithName(this object obj)
        {
            string nameSpace = GetInstancePart(obj);
            string name = obj.GetType().Name;
            return nameSpace + name;
        }
        /// <summary>
        /// 会心特效
        /// </summary>
        /// <param name="target"></param>
        public static SpurtsProj CritProj(Projectile Projectile, NPC target, Vector2 vel)
        {
            SpurtsProj proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_OnHit(target), target.Center - vel * 75f, vel, 0, 0, Projectile.owner, 150, 50, TextureAssets.Heart.Value);
            proj.FixedPos = false;
            return proj;
        }
        public static bool TryAdd<T>(this HashSet<T> hash, T[] Array)
        {
            for(int i =  0; i < Array.Length; i++)
            {
                if (!hash.Add(Array[i]))
                    return false;
            }
            return true;
        }
        public static string GetAppraiseDrawFont(AppraiseID appraiseID)
        {
            string drawFont = null;
            switch (appraiseID)
            {
                case AppraiseID.Deadly: drawFont = "D"; break;
                case AppraiseID.Crazily: drawFont = "C"; break;
                case AppraiseID.Blast: drawFont = "B"; break;
                case AppraiseID.A: drawFont = "A"; break;
                case AppraiseID.S: drawFont = "S"; break;
                case AppraiseID.SS: drawFont = "SS"; break;
                case AppraiseID.SSS: drawFont = "SSS"; break;
                case AppraiseID.KillGod: drawFont = "KG!"; break;
            }
            return drawFont;
        }
        public static Color GetAppraiseDrawColor(AppraiseID appraiseID)
        {
            Color color = Color.Transparent;
            switch (appraiseID)
            {
                case AppraiseID.Deadly: color = Color.White * 0.5f; break;
                case AppraiseID.Crazily: color = Color.White * 0.8f; break;
                case AppraiseID.Blast: color = Color.SkyBlue * 1.3f; break;
                case AppraiseID.A: color = Color.OrangeRed; break;
                case AppraiseID.S: color = Color.Yellow; break;
                case AppraiseID.SS: color = Color.Lerp(Color.Yellow, Color.Gold, 0.5f); break;
                case AppraiseID.SSS: color = Color.Gold; break;
                case AppraiseID.KillGod: color = Color.BlueViolet; break;
            }
            return color;
        }
        public static Rectangle GetApprasieDrawRect(AppraiseID appraiseID)
        {
            Rectangle rect = default;
            rect.Height = 20;
            switch (appraiseID)
            {
                case AppraiseID.Deadly:
                    rect.Width = 22;
                    break;
                case AppraiseID.Crazily:
                    rect.X = 27;
                    rect.Width = 19;
                    break;
                case AppraiseID.Blast:
                    rect.X = 51;
                    rect.Width = 18;
                    break;
                case AppraiseID.A:
                    rect.X = 74;
                    rect.Width = 19;
                    break;
                case AppraiseID.S:
                    rect.X = 98;
                    rect.Width = 16;
                    break;
                case AppraiseID.SS:
                    rect.X = 119;
                    rect.Width = 31;
                    break;
                case AppraiseID.SSS:
                    rect.X = 155;
                    rect.Width = 46;
                    break;
                case AppraiseID.KillGod:
                    rect.X = 206;
                    rect.Width = 52;
                    break;
            }
            return rect;
        }
    }
}
