namespace StarBreaker.Content.Appraise
{
    public class Appraise
    {
        public AppraiseID AppraiseID { get => appraiseID; set => appraiseID = value; }
        private AppraiseID appraiseID;
        /// <summary>
        /// 评价存在的对象
        /// </summary>
        public Entity AppraiseHasEntity;
        public IAppraiseEntity appraiseEntity;
        public float AppraiseTime;
        public int HitCDTime;
        public void Load(Entity entity, object appTarget)
        {
            AppraiseHasEntity = entity;
            appraiseID = AppraiseID.None;
            if (appTarget is IAppraiseEntity appraise)
            {
                appraiseEntity = appraise;
            }
        }
        public void Update()
        {
            if (AppraiseID.None != appraiseID && HitCDTime == 0)
            {
                AppraiseTime++;
            }
            else if (HitCDTime > 0)
            {
                HitCDTime--;
            }

            if (AppraiseTime > 150)
            {
                AppraiseTime = 0;
                if (appraiseID > AppraiseID.Deadly && appraiseID != AppraiseID.None)
                {
                    appraiseID--;
                }
                else
                {
                    appraiseID = AppraiseID.None;
                }
            }
        }
    }
}
