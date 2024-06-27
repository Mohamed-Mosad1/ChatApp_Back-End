namespace ChatApp.Application.Persistence.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task DeleteByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task<bool> SaveAllAsync();

    }
}
