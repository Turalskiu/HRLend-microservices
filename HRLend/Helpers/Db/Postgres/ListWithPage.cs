namespace Helpers.Db.Postgres
{
    public interface IEnumerableWithPage<TEntity> : IEnumerable<TEntity>
    {
        int TotalRows { get; set; }
        int FilterRow { get; set; }
        int PageNo { get; set; }
        int PageSize { get; set; }
        string Sort { get; set; }
    }

    public interface IEnumerableWithOffer<TEntity> : IEnumerable<TEntity>
    {
        int TotalRows { get; set; }
        int FilterRow { get; set; }
        int Start { get; set; }
        int Lenght { get; set; }
        string Sort { get; set; }
    }

    public class ListWithPage<TEntity> : List<TEntity>, IEnumerableWithPage<TEntity>
    {
        public int TotalRows { get; set; }
        public int FilterRow { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public new string Sort { get; set; }
    }

    public class ListWithOffer<TEntity> : List<TEntity>, IEnumerableWithOffer<TEntity>
    {
        public int TotalRows { get; set; }
        public int FilterRow { get; set; }
        public int Start { get; set; }
        public int Lenght { get; set; }
        public new string Sort { get; set; }
    }
}