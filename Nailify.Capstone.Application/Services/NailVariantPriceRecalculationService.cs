using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Application.Services
{
    public class NailVariantPriceRecalculationService : INailVariantPriceRecalculationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NailVariantPriceRecalculationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<NailVariantPriceRecalculationResponseDTO>> RecalculateAllAsync()
        {
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            var updatedVariants = 0;

            foreach (var variant in variants)
            {
                var recalculatedPrice = (variant.NailSurface?.Price ?? 0m)
                    + variant.NailComponents.Sum(nailComponent =>
                        nailComponent.Component.Price * GetFingerPriceMultiplier(nailComponent.FingerIndex));

                if (variant.Price != recalculatedPrice)
                {
                    variant.Price = recalculatedPrice;
                    updatedVariants++;
                }

                _unitOfWork.NailVariantRepository.Update(variant);
            }

            await _unitOfWork.SaveChangesAsync();

            var response = new NailVariantPriceRecalculationResponseDTO
            {
                TotalVariants = variants.Count,
                UpdatedVariants = updatedVariants
            };

            return new ApiSuccessResult<NailVariantPriceRecalculationResponseDTO>(
                response,
                "Recalculate all nail variant prices successfully.");
        }

        public async Task<ApiResult<CustomerNailPriceRecalculationResponseDTO>> RecalculateAllCustomerNailsAsync()
        {
            var customerNails = await _unitOfWork.CustomerNailRepository.GetAllCustomerNailsAsync();
            var updatedCustomerNails = 0;

            foreach (var customerNail in customerNails)
            {
                var recalculatedPrice = (customerNail.NailSurface?.Price ?? 0m)
                    + customerNail.CustomerNailComponents.Sum(component =>
                        ((component.Component?.Price ?? 0m) + (component.CustomerComponent?.Price ?? 0m))
                        * GetFingerPriceMultiplier(component.FingerIndex));

                if (customerNail.Price != recalculatedPrice)
                {
                    customerNail.Price = recalculatedPrice;
                    updatedCustomerNails++;
                }

                _unitOfWork.CustomerNailRepository.Update(customerNail);
            }

            await _unitOfWork.SaveChangesAsync();

            var response = new CustomerNailPriceRecalculationResponseDTO
            {
                TotalCustomerNails = customerNails.Count,
                UpdatedCustomerNails = updatedCustomerNails
            };

            return new ApiSuccessResult<CustomerNailPriceRecalculationResponseDTO>(
                response,
                "Recalculate all customer nail prices successfully.");
        }

        private static int GetFingerPriceMultiplier(int fingerIndex)
        {
            return fingerIndex == -1 ? 5 : 1;
        }
    }
}
