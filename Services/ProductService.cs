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
        private readonly TagService _tagService;
        private readonly IProductTagRepository _productTagRepository;

        public ProductService(IProductRepository productRepository, IMapper mapper, Generate generate, AccountService accountService, GalleryService galleryService, TagService tagService,IProductTagRepository productTagRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _galleryService = galleryService;
            _tagService = tagService;
            _productTagRepository=productTagRepository;
        }
        public async Task UpdateTagProductAsync()
        {
            var topSold = await _productRepository.GetTopProductsByTotalItemsSoldAsync(12);
            var bestSellerTag = await _tagService.GetTagByTypeAsync(TagType.BestSeller);

            // Lấy ra danh sách sản phẩm đã được gắn tag "BestSeller" trước đó
            var previousBestSellers = await _productRepository.GetProductsByTagTypeAsync(TagType.BestSeller);
             foreach (var product in previousBestSellers)
            {
                // Loại bỏ tag "BestSeller" cho các sản phẩm đã được gắn trước đó
                var productTag =new ProductTag{ProductId=product.Id,TagId=bestSellerTag.Id};
                if (productTag != null)
                {
                    await _productRepository.RemoveProductTagAsync(productTag);
                }
            }
            foreach (var product in topSold)
            {
                if(product.TotalItemsSold>0){
                    var productTag=new ProductTag{
                        ProductId=product.Id,
                        TagId=bestSellerTag.Id
                    };
                    await _productTagRepository.AddAsync(productTag);
                }
            }

           
        }

        public async Task<List<ProductGetDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<List<ProductGetDTO>>(products);
        }
        public async Task<List<ProductGetDTO>> GetAllProductsByTagAsync(long tagId)
        {
            var tag=await _tagService.GetTagByIdAsync(tagId);
            var products = await _productRepository.GetProductsByTagTypeAsync(tag.Type);
            return _mapper.Map<List<ProductGetDTO>>(products);
        }
        public async Task<PagedResult<ProductGetDTO>> GetFilteredProductsAsync(ProductFilterDTO filter)
{
    var pagedResult = await _productRepository.GetFilteredProductsAsync(filter);
    var productDTOs = _mapper.Map<List<ProductGetDTO>>(pagedResult.Items);
    return new PagedResult<ProductGetDTO>
    {
        Items = productDTOs,
        TotalCount = pagedResult.TotalCount,
        PageSize = pagedResult.PageSize,
        CurrentPage = pagedResult.CurrentPage
    };
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
        public async Task<ProductGetDTO> GetProductBySlugAsync(string slug)
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            if (product == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }
            return _mapper.Map<ProductGetDTO>(product);
        }
        public async Task<List<ProductGetDTO>> GetSameProductsAsync(string slug)
        {   var product=await GetProductBySlugAsync(slug);
        var products = await _productRepository.GetSameProductsAsync(product.Id,product.Category.Id,product.Brand.Id);
            return _mapper.Map<List<ProductGetDTO>>(products);
        }
        public async Task<Product> CreateProductAsync(ProductInputDTO productInputDTO)
        {
            await _accountService.CUExistingUser(productInputDTO.CreatedById, productInputDTO.UpdatedById);

            // Mapping ProductInputDTO to Product
            var product = _mapper.Map<Product>(productInputDTO);

            // Generating slug from Name
            string slug = _generate.GenerateSlug(productInputDTO.Name);
            product.Slug = slug;
            var tag = await _tagService.GetTagByTypeAsync(TagType.NewModel);
            product.ProductTags = new List<ProductTag> { new ProductTag { Product = product, Tag = tag } };


            await _productRepository.AddAsync(product);
            var newProduct=await _productRepository.GetProductsByTagTypeAsync(TagType.NewModel);
                if ( newProduct.Count>= 13)
                {
        // Lấy ra sản phẩm cũ nhất có tag "NewModel"
        var oldestProduct = await _productRepository.GetLastProductByTagTypeAsync(TagType.NewModel);

        // Loại bỏ tag "NewModel" khỏi sản phẩm cũ nhất
        var productTag = new ProductTag{ProductId=oldestProduct.Id,TagId=tag.Id};
        await _productRepository.RemoveProductTagAsync(productTag);
    }
            return product;
        }
        public async Task UpdateProductQuantityAsync(long id,int mQuantity){
             var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }
            existingProduct.Quantity=existingProduct.Quantity-mQuantity;
            existingProduct.TotalItemsSold=existingProduct.TotalItemsSold+mQuantity;
            await _productRepository.UpdateAsync(existingProduct);
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
        public async Task DeleteProductsAsync(List<long> ids)
        {
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
