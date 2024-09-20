using ETicaretAPI.Application.Repositories.ProductImageFileRepository;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.ProductImageFileRepository
{
    public class ProductImageFileWriteRepository : WriteRepository<ProductImageFile>, IProductImageFileWriteRepository
    {
        public ProductImageFileWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
