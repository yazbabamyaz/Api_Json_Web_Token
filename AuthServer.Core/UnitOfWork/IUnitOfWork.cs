using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync();//genelde SaveChanges isminde metot derler karışmasın diye bunu dedim.
        void Commit();//asenkron olmayan metotlar için.aynı işi yapar.
    }
}
