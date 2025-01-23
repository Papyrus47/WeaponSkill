namespace WeaponSkill.Command
{
    public struct CustomVertexInfo : IVertexType
    {
        public static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        });
        /// <summary>
        /// 绘制位置(世界坐标)
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 绘制的颜色
        /// </summary>
        public Color Color;
        /// <summary>
        /// 前两个是纹理坐标，最后一个是自定义的
        /// </summary>
        public Vector3 TexCoord;

        public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }
}
