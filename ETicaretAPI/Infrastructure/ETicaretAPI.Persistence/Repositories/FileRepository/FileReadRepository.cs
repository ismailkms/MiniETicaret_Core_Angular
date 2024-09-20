using ETicaretAPI.Application.Repositories.FileRepository;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.FileRepository
{
    public class FileReadRepository : ReadRepository<Domain.Entities.File>, IFileReadRepository
    {
        public FileReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
