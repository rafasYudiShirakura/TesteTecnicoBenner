using System.Collections.Generic;

namespace WpfApp.Services
{
    public interface IDataService<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        T Add(T entity);
        T Update(T entity);
        void Delete(int id);
    }
}
