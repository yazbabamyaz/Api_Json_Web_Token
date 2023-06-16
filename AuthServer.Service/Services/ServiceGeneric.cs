using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{    
    //savechanges burada çalışacak - unitofwork vs    
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            this._genericRepository = genericRepository;
        }
        
        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity=ObjectMapper.Mapper.Map<TEntity>(entity);//dto yu tentity e çevirdik.
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();//savechanges.
            var newDto=ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            //getall db den datayı aldı memory e yükledi.
            return Response<IEnumerable<TDto>>.Success(products, 200);
            
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product=_genericRepository.GetByIdAsync(id);
            if (product==null)//business kodu
            {
                return Response<TDto>.Fail("Id Not Found", 404, true);
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product),200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product==null)
            {
                return Response<NoDataDto>.Fail("Id not found",404,true);
            }
            _genericRepository.Remove(product);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);//204:No content.body de data yok.sadece status code
        }

        public async Task<Response<NoDataDto>> Update(TDto entity,int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product==null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }
            var updateProduct=ObjectMapper.Mapper.Map<TEntity>(entity);
            _genericRepository.Update(updateProduct);//STATE inin modified olarak işaretledik.
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);//db ye yansımadı şimdi order by ya da diğer sorgulamaları yapabiliriz.
                                                           // list.Take(10);//10 tanesini alabiliriz. hala db ye yansımadı
                                                           //yukarıdaki satırlar memoryde ef tarafından trac ediliyor enson tolist denilince db den çekiyor.
                                                           //list.ToListAsync();//bu esnada db ye yansır.IQueryable
            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
