using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<Tentity> : IGenericRepository<Tentity> where Tentity : class
    {

        private readonly DbContext _context;//AppDbContext de diyebilirdik?
        private readonly DbSet<Tentity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Tentity>();            
        }
        public async Task AddAsync(Tentity entity)
        {
           await _dbSet.AddAsync(entity);
            //ŞUAN SAVECHANGES ÇAĞIRMADIM ONU SERVİCE KATMANINDA YAPCAM.
            //Sadece memory e entity ekledik db ye yansımadı.UNITofWORK
            
        }

        public async Task<IEnumerable<Tentity>> GetAllAsync()
        {
            //best practise olan burada IQueryabledır eğitim için kullanış hoca.
            
            //IEnumerable: Tüm datayı memory e aldık.Bundan sonra yapacağın orderby ve diğer sorgulamalar memorydeki datada gerçekleşir.buda performansı etkiler.
           return  await _dbSet.ToListAsync();
            
        }
        //IQueryable metod hali de böyle olur.
        //public IQueryable<Tentity> GetAllAsync()
        //{
        //    //best practise olan burada IQueryabledır eğitim için kullanış hoca.

        //    //IEnumerable: Tüm datayı memory e aldık.Bundan sonra yapacağın orderby ve diğer sorgulamalar memorydeki datada gerçekleşir.buda performansı etkiler.
        //    return  _dbSet.AsQueryable();
        //}
        public async Task<Tentity> GetByIdAsync(int id)
        {
            var entity=await _dbSet.FindAsync(id);//pk üzerinden arama yapar (Not:sql de 2 tane pk olabilir)
            if (entity!=null)
            {
                //bu entity nin durumunu entitystate lerden detached. yani bu memoryde takip edilmesin.Trace edilmesin
                _context.Entry(entity).State = EntityState.Detached;                
            }
            return entity;

        }

        public void Remove(Tentity entity)
        {
            _dbSet.Remove(entity);// aslında entitystate i deleted ettik. 2 si de aynı
        }

        public Tentity Update(Tentity entity)
        {
            //dbye bir şey eklemiyor yani asenkron bir işlem yok.Tred bloklanmaz yani
            _context.Entry(entity).State= EntityState.Modified;//bütün propertylerini güncelleyecek update sorgusunu hazırladı.dezavantajdır. tek bir property değiştirme ihtiyacımız olabilir.O yüzden çok büyük projelerde domain d design kullanılıyor.
            return entity;
        }

        public IQueryable<Tentity> Where(Expression<Func<Tentity, bool>> predicate)
        {
            //mümkün olduğunca IQueryable kullan.ÖNCE SORGULAMALAR YAPILIR. O VERİLER MEMORY E ALINIR.
            return _dbSet.Where(predicate);
        }
    }
}
