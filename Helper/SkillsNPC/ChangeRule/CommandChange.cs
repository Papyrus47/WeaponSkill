namespace WeaponSkill.Helper.SkillsNPC.ChangeRule
{
    public abstract class CommandChange : INPCSkillsChangeRule
    {
        /// <summary>
        /// 符合OldSkillList条件即可切换,为null则能直接切换
        /// </summary>
        public List<NPCSkills_Instantiation> OldSkill;
        public NPCSkillsManager skillsManager;
        public NPCSkills_Instantiation TargetSkill;
        public NPCSkills_Instantiation CurrentSkill { get; set; }

        public abstract NPCSkills_Instantiation ChangeSkill();

        public virtual void OnChange(NPCSkills_Instantiation target)
        {
            target.OnSkillActive();
            CurrentSkill.OnSkillDeactivate(target);
        }
    }
}