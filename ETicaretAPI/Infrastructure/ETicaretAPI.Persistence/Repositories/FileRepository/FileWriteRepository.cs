using ETicaretAPI.Application.Repositories.FileRepository;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.FileRepository
{
    public class FileWriteRepository : WriteRepository<Domain.Entities.File>, IFileWriteRepository
    {
        public FileWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
