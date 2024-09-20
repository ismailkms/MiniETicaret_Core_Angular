using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateStockQrCodeToProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowCaseImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IProductService _productService;
        readonly IMediator _mediator;
        //Application içerisinde oluşturduğumuz ServiceRegitration ile bunun içini IoC container sayesinde doldurduk.

        public ProductsController(IMediator mediator, IProductService productService)
        {
            _mediator = mediator;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetQRCodeToProduct([FromRoute] string productId)
        {
            byte[] data = await _productService.QRCodeToProductAsync(productId);
            return File(data, "image/png");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStockQrCodeToProduct([FromBody] UpdateStockQrCodeToProductCommandRequest updateStockQrCodeToProductCommandRequest)
        {
            UpdateStockQrCodeToProductCommandResponse response = await _mediator.Send(updateStockQrCodeToProductCommandRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Writing, Definition = "Create Product")]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Updating, Definition = "Update Product")]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Deleting, Definition = "Delete Product")]
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Writing, Definition = "Upload Product File")]
        //id değeri QueryString'den gelecektir. Ör/ ...com/api/products/upload?id=123
        //Upload işlemi ileride id'den farklı bir parametrede alabilir ondan dolayı QueryString'i kullandık.
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
        {
            uploadProductImageCommandRequest.Files = Request.Form.Files;
            //Gönderilen file nesnesini Request.Form.Files; ile yakalıyoruz.
            UploadProductImageCommandResponse response = await _mediator.Send(uploadProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Reading, Definition = "Get Product Images")]
        //id değer RouteData'dan gelecektir. Ör/...com/api/products/getproductsimages/123
        //GetProductImage'de her daim ürün id'si yeterli olacaktır ondan dolayı RouteData'yı kullandık.
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
        {
            List<GetProductImagesQueryResponse> response = await _mediator.Send(getProductImagesQueryRequest);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Deleting, Definition = "Delete Product Image")]
        //id değer RouteData'dan gelecektir. Ör/...com/api/products/getproductsimages/123
        //GetProductImage'de her daim ürün id'si yeterli olacaktır ondan dolayı RouteData'yı kullandık.
        public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest removeProductImageCommandRequest, [FromQuery] string imageId)
        {
            removeProductImageCommandRequest.ImageId = imageId;
            RemoveProductImageCommandResponse response = await _mediator.Send(removeProductImageCommandRequest);
            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Updating, Definition = "Change Showcase Image")]
        public async Task<IActionResult> ChangeShowCaseImage([FromQuery]ChangeShowCaseImageCommandRequest changeShowCaseImageCommandRequest)
        {
            ChangeShowCaseImageCommandResponse response = await _mediator.Send(changeShowCaseImageCommandRequest);
            return Ok(response);
        }
    }
}
