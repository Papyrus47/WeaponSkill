namespace StarBreaker.Content.Appraise
{
    public interface IAppraiseEntity
    {
        public object[] UseAttack { get; set; }
        public float OnHit(Entity target, int damage);
        public float OnHurt(Entity target, int damage);
        public void Draw(float progress, AppraiseID id);
    }
}
