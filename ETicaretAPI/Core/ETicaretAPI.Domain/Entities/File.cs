using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Storage { get; set; }
        [NotMapped]
        public override DateTime UpdatedDate { get => base.UpdatedDate; set => base.UpdatedDate = value; }
        //BaseEntity'deki UpdatedDate'e bu entityde gerek yok. O nedenle BaseEntity'de UpdatedDate'i virtual olarak tanımladık ve burada override edip [NotMapped] ile etiketliyerek oluşturulmasını engelleddik.
    }
}
