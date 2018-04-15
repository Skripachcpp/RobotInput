using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace WorkingTools.Map
{
    public static class Mapper
    {
        private static readonly DefaultMapConfig Config = new DefaultMapConfig();

        static Mapper()
        {
        }

        public static TTarget Map<TTarget>(object source)
        {
            var target = (TTarget)ObjectMapperManager.DefaultInstance.GetMapperImpl(source.GetType(), typeof(TTarget), Config).Map(source);
            return target;
        }

        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        {
            ObjectMapperManager.DefaultInstance.GetMapper<TSource, TTarget>(Config).Map(source, target);
            return target;
        }
    }
}
