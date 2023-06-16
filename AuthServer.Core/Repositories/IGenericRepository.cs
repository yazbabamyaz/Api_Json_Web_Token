using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    //generic olarak TEntity alacak(tipi class olan) 
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        //asenkron metot olduğu belli olsun diye Async yazdım. 
        Task<IEnumerable<TEntity>> GetAllAsync();
        //Tüm dataları alıp üzerinde sorgulama yapmayacaksan IEnumerable yapabiliriz.
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        //function alacak:bir delegedir.Yani bir metodu işaret eder.Tentity alacak geriye de bool dönen metodu işaret edecek.where(x=>x.id>5) x=tentity e karşılık gelir.
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);//remove asenkron metodu yokmuş.
         TEntity Update(TEntity entity);
        //MODELSTATE ini modified olarak belirlicez. savechanges diyince ef gidecek memorydeki tüm entitylerin statenine bakacak modified varsa güncellicek. 
                                                
    }
}
