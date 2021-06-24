using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IMapper mapper, IDiscountRepository repository, ILogger<DiscountService> logger)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if(coupon==null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found"));
            }

            _logger.LogInformation("Discount is retrieved for productName: {productName}, Amount : { amount}", coupon.ProductName, coupon.Amount);
            return _mapper.Map<Coupon, CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<CouponModel, Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is successfully created.ProductName: {productName}", coupon.ProductName);
            return _mapper.Map<Coupon, CouponModel>(coupon);
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<CouponModel, Coupon>(request.Coupon);
             await _repository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated.ProductName: {productName}", coupon.ProductName);
            return _mapper.Map<Coupon, CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var result = await _repository.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = result
            };

            _logger.LogInformation("Discount is deleted successfully for productName: {productName}",request.ProductName);
            return response;
        }

    }
}
