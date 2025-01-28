using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Core;

namespace WeaponSkill.Weapons.HuntingHorn
{
    /// <summary>
    /// 旋律Buff
    /// </summary>
    public abstract class HuntingHornBuff
    {
        public HuntingHornBuff(List<HuntingHornMelody.MelodyType> melodyTypes)
        {
            this.melodyTypes = melodyTypes;
        }
        /// <summary>
        /// 名字
        /// </summary>
        public virtual string Name => TheUtility.RegisterText("Mods." + GetType().Namespace + "." + GetType().Name);
        public List<HuntingHornMelody.MelodyType> melodyTypes = new();
        /// <summary>
        /// 演奏效果
        /// </summary>
        public virtual void OnPlay(Player player,Projectile projectile) { }
        public virtual void Load()
        {
            _ = Name;
        }
        /// <summary>
        /// 加载笛子buff
        /// </summary>
        /// <param name="mod"></param>
        public static void Load(Mod mod)
        {
            Type[] type = AssemblyManager.GetLoadableTypes(mod.Code);
            foreach (Type t in type)
            {
                if(t.IsSubclassOf(typeof(HuntingHornBuff)) && !t.IsAbstract)
                {
                    (Activator.CreateInstance(t, new List<HuntingHornMelody.MelodyType>() { HuntingHornMelody.MelodyType.Left }) as HuntingHornBuff).Load();
                }
            }
        }
        public virtual void Register()
        {

        }
        //public void Unload()
        //{
        //}
    }
}
