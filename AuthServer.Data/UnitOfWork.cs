using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        public UnitOfWork(AppDbContext context)//bir nesne örneği alıyorum.
        {
            _context= context;
        }
        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()//compiler a bu metot içinde asenkron bir metot var diyoruz.(async)
        {
           await _context.SaveChangesAsync();
        }
    }
}
