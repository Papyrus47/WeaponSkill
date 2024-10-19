using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;

namespace WeaponSkill.Weapons.StarBreakerWeapon
{
    public class StarBreakerSkills : ProjSkill_Instantiation
    {
        public Player Player;
        public StarBreakerWeaponProj StarBreakerWeaponProj => modProjectile as StarBreakerWeaponProj;
        public string Name;
        public SkillsTreeUI.SkillsControl skillsControl;
        public StarBreakerSkills(StarBreakerWeaponProj modProjectile) : base(modProjectile)
        {
        }
        public void AddBySkill(params StarBreakerSkills[] skill)
        {
            foreach (var skillItem in skill)
            {
                skillItem.AddSkill(this);
            }
        }
        public StarBreakerSkills AddSkill(StarBreakerSkills projSkill)
        {
            switchToSkill.Add(projSkill);
            if(projSkill.Name == null)
            {
                return projSkill;
            }
            SkillsTreeUI.TryAddSkillTree(this,
            [
                (projSkill.skillsControl,projSkill.Name)
            ]);
            return projSkill;
        }
        public List<StarBreakerSkills> AddSkilles(params StarBreakerSkills[] projSkill)
        {
            switchToSkill.AddRange(projSkill);
            for(int i = 0; i < projSkill.Length; i++)
            {
                if (projSkill[i].Name == null)
                    continue;
                
                SkillsTreeUI.TryAddSkillTree(this,
                [
                    (projSkill[i].skillsControl,projSkill[i].Name)
                ]);
            }
            return new List<StarBreakerSkills>(projSkill);
        }
        /// <summary>
        /// 可以用于切换停止技能
        /// </summary>
        public bool CanChangeToStopActionSkill
        {
            get => StarBreakerWeaponProj.CanChangeToStopActionSkill;
            set => StarBreakerWeaponProj.CanChangeToStopActionSkill = value;
        }
    }
}
