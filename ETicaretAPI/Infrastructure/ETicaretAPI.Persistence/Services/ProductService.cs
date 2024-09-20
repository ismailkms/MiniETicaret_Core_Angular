using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories.ProductRepository;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class ProductService : IProductService
    {
        readonly IQRCodeService _qrCodeService;
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;

        public ProductService(IQRCodeService qrCodeService, IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _qrCodeService = qrCodeService;
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task<byte[]> QRCodeToProductAsync(string productId)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var plainObject = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CreatedDate
            };
            string plainText = JsonSerializer.Serialize(plainObject);

            return _qrCodeService.GenerateQRCode(plainText);
        }

        public async Task StockUpdateToProductAsync(string productId, int stock)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            product.Stock = stock;
            await _productWriteRepository.SaveAsync();
        }
    }
}
