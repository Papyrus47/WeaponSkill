namespace StarBreaker.Content.Appraise
{
    /// <summary>
    /// 评价系统
    /// <para>评价的计算是这样的:根据伤害和对应攻击的伤害乘率提升评价</para>
    /// <para>NPC也可以有这个系统,所以扩展面很广</para>
    /// </summary>
    public class AppraiseSystem : IUnLoad
    {
        private AppraiseSystem() { }
        private static AppraiseSystem instance;

        public static AppraiseSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new()
                    {
                        appraise = new List<Appraise>(3) //分配三个,目前星击要同时存在三个评价的人,也就只有两小只 打 玩家了
                    };
                }
                return instance;
            }
        }
        public List<Appraise> appraise;
        public void Load(Entity entity, object appTarget)
        {
            appraise.Add(new());
            appraise[^1].Load(entity, appTarget);
        }
        public void Update()
        {
            appraise.RemoveAll(x => x?.AppraiseHasEntity == null);
            appraise.ForEach(x => x?.Update());
        } //更新他们的评价
        public void OnHit(IAppraiseEntity app, Entity target, int damage)
        {
            Appraise a = appraise.Find(x => app.Equals(x.appraiseEntity));
            float? time = a?.appraiseEntity.OnHit(target, damage);
            if (time.HasValue && a != null)
            {
                a.AppraiseTime -= time.Value;
                if (time > 0)
                {
                    a.HitCDTime = 300;
                }

                if (a.AppraiseTime < 0)
                {
                    a.AppraiseTime = 140;
                    if (a.AppraiseID == AppraiseID.None)
                    {
                        a.AppraiseID = AppraiseID.Deadly;
                    }
                    else if (a.AppraiseID < AppraiseID.KillGod)
                    {
                        a.AppraiseID++;
                    }
                    else if (a.AppraiseID == AppraiseID.KillGod)
                    {
                        a.AppraiseTime = 0;
                    }
                }
            }
        }
        public void OnHurt(IAppraiseEntity app, Entity target, int damage)
        {
            Appraise a = appraise.Find(x => app.Equals(x.appraiseEntity));
            float? time = a?.appraiseEntity.OnHurt(target, damage);
            if (time.HasValue && a != null && a.AppraiseID != AppraiseID.None)
            {
                a.AppraiseTime += time.Value;
                if (a.AppraiseTime > 190)
                {
                    a.AppraiseTime = 10;
                    a.AppraiseID--;
                }
            }
        }
        public void Draw()
        {
            appraise.ForEach(x =>
            {
                float? prog = x?.AppraiseTime / 150f;
                x?.appraiseEntity?.Draw(prog.Value, x.AppraiseID);
             });
        }

        public void UnLoad()
        {
            instance = null;
        }
    }
}
