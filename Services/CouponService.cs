using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public CouponService(ICouponRepository couponRepository, IMapper mapper, Generate generate, AccountService accountService)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
        }

        public async Task<List<CouponGetDTO>> GetAllCouponsAsync()
        {
            var coupons = await _couponRepository.GetAllAsync();
            return _mapper.Map<List<CouponGetDTO>>(coupons);
        }

        public async Task<CouponGetDTO> GetCouponByIdAsync(long id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }
            return _mapper.Map<CouponGetDTO>(coupon);
        }
        public async Task<CouponGetDTO> GetCouponByCodeAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }
            
            return _mapper.Map<CouponGetDTO>(coupon);
        }
        public async Task<CouponGetDTO> SubmitCodeAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }
            DateTime currentDate = DateTime.Now;
            if (currentDate < coupon.StartDate)
            {
                throw new Exception("Chưa đến ngày giảm giá.");
            }
            if (currentDate > coupon.EndDate)
            {
                throw new Exception("Mã giảm giá đã hết hạn.");
            }
            if(coupon.UsageLimit==0)
            {
                throw new Exception("Mã giảm giá đã hết số lượt sử dụng.");
            }

            return _mapper.Map<CouponGetDTO>(coupon);
        }
        public async Task<CouponGetDTO> CreateCouponAsync(CouponInputDTO couponInputDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            // Generate a unique code for the coupon if not provided
            var existingCoupon = await _couponRepository.GetByCodeAsync(couponInputDTO.Code);
            if (existingCoupon != null)
            {
                throw new BadRequestException("Mã này đã tồn tại");
            }




            var coupon = _mapper.Map<Coupon>(couponInputDTO);
            coupon.Status = 0;
            if (couponInputDTO.DiscountType == DiscountType.Percentage)
            {
                coupon.Description = $"Giảm {couponInputDTO.DiscountValue}% cho đơn hàng từ {couponInputDTO.MinimumOrderValue} VND";
            }
            else if (couponInputDTO.DiscountType == DiscountType.FixedAmount)
            {
                coupon.Description = $"Giảm {couponInputDTO.DiscountValue} VND cho đơn hàng từ {couponInputDTO.MinimumOrderValue} VND";
            }
            coupon.CreatedById = existingUser.Id;
            coupon.UpdatedById = existingUser.Id;
            // Add coupon to the database
            await _couponRepository.AddAsync(coupon);

            return _mapper.Map<CouponGetDTO>(coupon);
        }
                public async Task UpdateCouponUsageLimitAsync(long id, int limit)
        {
            var existingCoupon = await _couponRepository.GetByIdAsync(id);
            existingCoupon.UsageLimit=limit;
            await _couponRepository.UpdateAsync(existingCoupon);
        }
        public async Task<CouponGetDTO> UpdateCouponAsync(long id, CouponInputDTO couponInputDTO)
        {
            var existingCoupon = await _couponRepository.GetByIdAsync(id);

            if (existingCoupon == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }


            // Map CouponInputDTO to existingCoupon
            _mapper.Map(couponInputDTO, existingCoupon);

            // Update coupon in the database
            await _couponRepository.UpdateAsync(existingCoupon);

            return _mapper.Map<CouponGetDTO>(existingCoupon);
        }

        public async Task DeleteCouponAsync(long id)
        {
            var existingCoupon = await _couponRepository.GetByIdAsync(id);

            if (existingCoupon == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }

            await _couponRepository.DeleteAsync(id);
        }
    }
}
