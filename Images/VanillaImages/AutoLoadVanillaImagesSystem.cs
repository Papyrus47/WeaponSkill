using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Images.VanillaImages
{
    public class AutoLoadVanillaImagesSystem : ILoadable
    {
        public enum AutoLoadVanillaImagesEnum
        {
            Proj = 0,Item = 1
        }
        public Dictionary<(int,AutoLoadVanillaImagesEnum), Asset<Texture2D>> VanillaImagesSave;

        public void Load(Mod mod)
        {
            VanillaImagesSave = new();
            string Path = GetType().Namespace.Replace('.','/').Replace("WeaponSkill/","");
            #region 弹幕
            string ProjPath = Path + "/Proj/";
            for(int ID = 0;ID < ProjectileID.Count;ID++)
            {
                if (!mod.RootContentSource.HasAsset(ProjPath + "Proj_" + ID)) continue;
                VanillaImagesSave.Add((ID,AutoLoadVanillaImagesEnum.Proj), TextureAssets.Projectile[ID]);
                TextureAssets.Projectile[ID] = mod.Assets.Request<Texture2D>(ProjPath + "Proj_" + ID);
            }
            #endregion
            #region 物品
            string ItemPath = Path + "/Item/";
            for (int ID = 0; ID < ItemID.Count; ID++)
            {
                if (!mod.RootContentSource.HasAsset(ItemPath + "Item_" + ID)) continue;
                VanillaImagesSave.Add((ID, AutoLoadVanillaImagesEnum.Item), TextureAssets.Item[ID]);
                TextureAssets.Item[ID] = mod.Assets.Request<Texture2D>(ItemPath + "Item_" + ID);
            }
            #endregion
        }

        public void Unload()
        {
            foreach(var item in VanillaImagesSave)
            {
                if(item.Key.Item2 == AutoLoadVanillaImagesEnum.Proj)
                {
                    TextureAssets.Projectile[item.Key.Item1] = item.Value;
                }
                else if(item.Key.Item2 == AutoLoadVanillaImagesEnum.Item)
                {
                    TextureAssets.Item[item.Key.Item1] = item.Value;
                }
            }
            VanillaImagesSave.Clear();
            VanillaImagesSave = null;
        }
    }
}
