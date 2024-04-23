using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;
        private readonly GalleryService _galleryService;

        public ProductService(IProductRepository productRepository, IMapper mapper, Generate generate, AccountService accountService,GalleryService galleryService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _galleryService=galleryService;
        }

        public async Task<List<ProductGetDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<List<ProductGetDTO>>(products);
        }

        public async Task<ProductGetDTO> GetProductByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }
            return _mapper.Map<ProductGetDTO>(product);
        }

        public async Task<Product> CreateProductAsync(ProductInputDTO productInputDTO)
        {
            await _accountService.CUExistingUser(productInputDTO.CreatedById, productInputDTO.UpdatedById);
            
            // Mapping ProductInputDTO to Product
            var product = _mapper.Map<Product>(productInputDTO);

            // Generating slug from Name
            string slug = _generate.GenerateSlug(productInputDTO.Name);
            product.Slug = slug;

            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<ProductGetDTO> UpdateProductAsync(long id, ProductInputDTO productInputDTO)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }

            await _accountService.CUExistingUser(productInputDTO.CreatedById, productInputDTO.UpdatedById);
            
            // Mapping properties from productInputDTO to existingProduct
            _mapper.Map(productInputDTO, existingProduct);

            // Generating slug from Name
            string slug = _generate.GenerateSlug(productInputDTO.Name);
            existingProduct.Slug = slug;

            await _productRepository.UpdateAsync(existingProduct);

            return _mapper.Map<ProductGetDTO>(existingProduct);
        }
        public async Task DeleteProductsAsync(List<long> ids){
                foreach (var id in ids)
            {
                    await DeleteProductAsync(id);
            }
        }
        public async Task DeleteProductAsync(long id)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }
            await _galleryService.DeleteGalleryByProductIdAsync(id);
            await _productRepository.DeleteAsync(id);
        }
        public async Task<ProductGetDTO> UpdateProductStatusAsync(long id)
{
    var existing = await _productRepository.GetByIdAsync(id);

    if (existing == null)
    {
        throw new NotFoundException("Sản phẩm không tồn tại");
    }

    // Cập nhật trạng thái mới (nếu hiện tại là 0 thì cập nhật thành 1, và ngược lại)
    existing.Status = existing.Status == 0 ? 1 : 0;

    await _productRepository.UpdateAsync(existing);

    return _mapper.Map<ProductGetDTO>(existing);
}
    }
}
