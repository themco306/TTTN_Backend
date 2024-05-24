using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class MenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly AccountService _accountService;
        private readonly Generate _generate;
        private readonly IMapper _mapper;
        private readonly CategoryService _categoryService;
        private readonly BrandService _brandService;
        private readonly PostService _postService;

        public MenuService(IMenuRepository menuRepository, AccountService accountService, Generate generate, IMapper mapper, CategoryService categoryService, BrandService brandService, PostService postService)
        {
            _menuRepository = menuRepository;
            _accountService = accountService;
            _generate = generate;
            _mapper = mapper;
            _categoryService = categoryService;
            _brandService = brandService;
            _postService = postService;
        }

        public async Task<List<MenuGetDTO>> GetMenusAsync()
        {
            var menus = await _menuRepository.GetAllAsync();

            return _mapper.Map<List<MenuGetDTO>>(menus);

        }
        public async Task<List<MenuGetShortDTO>> GetMenusHeaderAsync()
        {
            var menus = await _menuRepository.GetMenuHeaderAsync();
            return _mapper.Map<List<MenuGetShortDTO>>(menus);
        }
                public async Task<List<MenuGetShortDTO>> GetSubMenusAsync(long id)
        {
            var menus = await _menuRepository.GetSubMenusAsync(id);
            return _mapper.Map<List<MenuGetShortDTO>>(menus);
        }
        // public async Task<List<MenuGetDTO>> GetMenusActiveAsync()
        // {
        //     var menus = await _menuRepository.GetAllAsync(true);
        //     return _mapper.Map<List<MenuGetDTO>>(menus);
        // }
        public async Task<MenuGetDTO> GetMenuByIdAsync(long menuId)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);
            if (menu == null)
            {
                throw new NotFoundException("Menu không tồn tại");
            }

            return _mapper.Map<MenuGetDTO>(menu);
        }
        //         public async Task<Menu> GetMenuShowByIdAsync(long menuId)
        // {
        //     var menu = await _menuRepository.GetByIdAsync(menuId);
        //     if (menu == null)
        //     {
        //         throw new NotFoundException("Hình ảnh không tồn tại");
        //     }

        //     return menu;
        // }
        public async Task<List<MenuGetDTO>> GetParentsAsync(long id)
        {
            var currentCategory = await _menuRepository.GetByIdAsync(id);
            if (currentCategory == null)
            {
                throw new NotFoundException("Menu không tồn tại.");
            }

            List<Menu> parentCategories;
            // Nếu danh mục hiện tại là một danh mục gốc
            parentCategories = await _menuRepository.GetAllAsync(); // Lấy tất cả danh mục



            // Loại bỏ danh mục hiện tại và tất cả các danh mục con của nó khỏi danh sách danh mục cha
            parentCategories = parentCategories.Where(c => c.Id != id && !IsDescendant(currentCategory, c)).ToList();

            return _mapper.Map<List<MenuGetDTO>>(parentCategories);
        }

        private bool IsDescendant(Menu parent, Menu child)
        {
            // Kiểm tra xem child có phải là một con của parent không
            if (child.ParentId == null)
            {
                return false;
            }
            if (child.ParentId == parent.Id)
            {
                return true;
            }
            return IsDescendant(parent, child.Parent); // Đệ quy kiểm tra các danh mục cha của child
        }
        public async Task<Menu> CreateCustomMenuAsync(MenuCustomInputDTO menuDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var menu = new Menu
            {
                Name = menuDTO.Name,
                Link = menuDTO.Link,
                Position = menuDTO.Position
            };
            menu.Type = "custom";
            menu.CreatedById = user.Id;
            menu.UpdatedById = user.Id;
            await _menuRepository.AddAsync(menu);
            return menu;
        }
        public async Task<Menu> CreateMenuAsync(MenuInputDTO menuDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var existingMenu = await _menuRepository.GetByTableIdAsync(menuDTO.TableId, menuDTO.Type);
            if (existingMenu != null)
            {
                throw new BadRequestException("Menu đã tồn tại vui lòng sửa thay vì thêm mới");
            }
            var menu = new Menu
            {
                Position = menuDTO.Position,
                Type = menuDTO.Type,
                TableId = menuDTO.TableId
            };
            switch (menuDTO.Type)
            {
                case "category":
                    {
                        var category = await _categoryService.GetCategoryByIdAsync(menuDTO.TableId);
                        menu.Name = category.Name;
                        menu.Link = category.Slug;
                    }
                    break;
                case "brand":
                    {
                        var category = await _brandService.GetBrandByIdAsync(menuDTO.TableId);
                        menu.Name = category.Name;
                        menu.Link = category.Slug;
                    }
                    break;
                case "post":
                    {
                        var category = await _postService.GetPostByIdAsync(menuDTO.TableId);
                        menu.Name = category.Name;
                        menu.Link = category.Slug;
                    }
                    break;
                case "page":
                    {
                        var category = await _postService.GetPostByIdAsync(menuDTO.TableId);
                        menu.Name = category.Name;
                        menu.Link = category.Slug;
                    }
                    break;
                default:
                    throw new NotFoundException("Không tồn tại");
            }
            menu.CreatedById = user.Id;
            menu.UpdatedById = user.Id;
            await _menuRepository.AddAsync(menu);
            return menu;
        }
        public async Task<Menu> UpdateMenuAsync(long id, MenuInputUpdateDTO menuDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var existingMenu = await _menuRepository.GetByIdAsync(id);
            if (existingMenu == null)
            {
                throw new NotFoundException("Menu không tồn tại");
            }
            if (menuDTO.ParentId != 0 && menuDTO.ParentId != existingMenu.Id)
            {
                var existingParent = await _menuRepository.GetByIdAsync(menuDTO.ParentId);
                if (existingParent == null)
                {
                    throw new NotFoundException("Menu cha không tồn tại");
                }
                existingMenu.ParentId = menuDTO.ParentId;
            }

            if (existingMenu.TableId == 0)
            {
                existingMenu.Name = menuDTO.Name;
                existingMenu.Link = menuDTO.Link;
            }
            existingMenu.Position = menuDTO.Position;
            existingMenu.SortOrder = menuDTO.SortOrder;

            existingMenu.Status = menuDTO.Status;
            existingMenu.UpdatedById = existingUser.Id;
            await _menuRepository.UpdateAsync(existingMenu);

            return existingMenu;

        }

        public async Task DeleteMenuAsync(long menuId)
        {
            var existingMenu = await _menuRepository.GetByIdAsync(menuId);
            if (existingMenu == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }
            await _menuRepository.DeleteAsync(existingMenu);
        }
        public async Task DeleteMenusAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingCategory = await _menuRepository.GetByIdAsync(id);
                if (existingCategory != null)
                {
                    await _menuRepository.DeleteAsync(existingCategory);
                }
            }
        }
        public async Task UpdateMenuStatusAsync(long id)
        {
            var existing = await _menuRepository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new NotFoundException("Menu không tồn tại");
            }
            existing.Status = existing.Status == 0 ? 1 : 0;

            await _menuRepository.UpdateAsync(existing);
        }
    }
}
