using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        //ortak hata classı
        public List<string> Errors { get; private set; }//sadece bu class içerisinde set edilebilir.
        //dışarıdan set için constructor kullan. eğer sadece constructorda kullanıyorsan hiç yazmayabilirsin.Ama yazmazsan contructur dışındaki metotta kullanamazsın.
        public bool IsShow { get; private set; }//gelen hata kullanıcıya gösterilsin mi gösterilmesin mi
        //developper-yazılımcının anlayacağı hataları false a set etcez.  kullanıcıların göreceği hataları true.
        public ErrorDto()
        {
            Errors = new List<string>();
        }
        public ErrorDto(string error, bool isShow)//tek hata
        {
            Errors.Add(error);
            IsShow = isShow;//hata olabilir hocanın ki
        }
        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }

    }
}
