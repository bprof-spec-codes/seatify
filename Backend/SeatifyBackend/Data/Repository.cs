using Entities.Helpers;

namespace Data
{
    public class Repository<T> where T : class, IIdEntity
    {
        AppDbContext ctx;
        public Repository(AppDbContext ctx)
        {
            this.ctx = ctx;
        }
        public Repository()
        {
        }

        public virtual void Add(T entity)
        {
            ctx.Set<T>().Add(entity);
            ctx.SaveChanges();
        }

        public virtual T FindById(string id)
        {
            return ctx.Set<T>().First(e => e.Id == id);
        }

        public virtual void DeleteById(string id)
        {
            var entity = FindById(id);
            ctx.Set<T>().Remove(entity);
            ctx.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            ctx.Set<T>().Remove(entity);
            ctx.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            var old = FindById(entity.Id);
            foreach (var prop in typeof(T).GetProperties())
            {
                prop.SetValue(old, prop.GetValue(entity));
            }
            ctx.Set<T>().Update(old);
            ctx.SaveChanges();
        }

        public virtual IQueryable<T> GetAll()
        {
            return ctx.Set<T>();
        }
    }
}
