using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get;private set; }
        //bı response sınıfı json dataya dönüştürülürken (serialize)bu propertyi ellemesin.
        [JsonIgnore]
        public bool IsSuccessful { get;private set; }//client lara açmayacağım property. clientlar zaten StatusCode dan anlarlar başarılı olup olmadığını.

        //static metotlar üzerinden response oluşturalım.nesne örneği alcaz farklı yolları var.
        public static Response<T> Success(T data,int statusCode)//başarılı olduğunda mesaj istemedik.
        {
            return new Response<T> { Data = data, StatusCode = statusCode,IsSuccessful=true };
        }
        //başarılı olduğunda data da dönmek istemeyebiliriz.
        public static Response<T> Success(int statusCode)//silme güncelleme de geriye data dönmeye gerek yok.
        {
            return new Response<T> {Data=default,  StatusCode = statusCode,IsSuccessful = true }; 
        }
        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful= false
                
            };
        }
        public static Response<T> Fail(string errorMessage, int statusCode,bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T>
            {
                StatusCode = statusCode,
                Error = errorDto,
                IsSuccessful=false
            };
        }
    }
}
