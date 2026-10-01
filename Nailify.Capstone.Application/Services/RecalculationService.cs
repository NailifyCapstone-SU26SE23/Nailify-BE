using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Security.Cryptography;

namespace Nailify.Capstone.Application.Services
{
    public class RecalculationService : IRecalculationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecalculationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<NailVariantPriceRecalculationResponseDTO>> RecalculateAllAsync()
        {
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            var updatedVariants = 0;

            foreach (var variant in variants)
            {
                if (await ApplyNailVariantTotalsAsync(variant.NailVariantId))
                {
                    updatedVariants++;
                }
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
                if (await ApplyCustomerNailTotalsAsync(customerNail.CustomerNailId))
                {
                    updatedCustomerNails++;
                }
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

        public async Task RecalculateNailVariantAsync(int nailVariantId)
        {
            await ApplyNailVariantTotalsAsync(nailVariantId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecalculateCustomerNailAsync(int customerNailId)
        {
            await ApplyCustomerNailTotalsAsync(customerNailId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecalculateNailVariantsByComponentIdAsync(int componentId)
        {
            var nailVariantIds = await _unitOfWork.NailComponentRepository.GetNailVariantIdsByComponentIdAsync(componentId);
            foreach (var nailVariantId in nailVariantIds)
            {
                await ApplyNailVariantTotalsAsync(nailVariantId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecalculateCustomerNailsByComponentIdAsync(int componentId)
        {
            var customerNailIds = await _unitOfWork.CustomerNailComponentRepository.GetCustomerNailIdsByComponentIdAsync(componentId);
            foreach (var customerNailId in customerNailIds)
            {
                await ApplyCustomerNailTotalsAsync(customerNailId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecalculateByNailSurfaceIdAsync(int nailSurfaceId)
        {
            var nailVariantIds = await _unitOfWork.NailVariantRepository.GetNailVariantIdsByNailSurfaceIdAsync(nailSurfaceId);
            foreach (var nailVariantId in nailVariantIds)
            {
                await ApplyNailVariantTotalsAsync(nailVariantId);
            }

            var customerNailIds = await _unitOfWork.CustomerNailRepository.GetCustomerNailIdsByNailSurfaceIdAsync(nailSurfaceId);
            foreach (var customerNailId in customerNailIds)
            {
                await ApplyCustomerNailTotalsAsync(customerNailId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> ApplyNailVariantTotalsAsync(int nailVariantId)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(nailVariantId);
            if (variant == null)
            {
                return false;
            }

            var surface = variant.NailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(variant.NailSurfaceId.Value)
                : null;
            var nailComponents = await _unitOfWork.NailComponentRepository.GetByNailVariantIdAsync(nailVariantId);

            var componentPrice = 0m;
            var componentDuration = 0;
            foreach (var nailComponent in nailComponents)
            {
                if (!IsActive(nailComponent.Component?.Status))
                {
                    continue;
                }

                var multiplier = GetFingerPriceMultiplier(nailComponent.FingerIndex);
                componentPrice += (nailComponent.Component?.Price ?? 0m) * multiplier;
                componentDuration += (nailComponent.Component?.Duration ?? 0) * multiplier;
            }

            var recalculatedPrice = (surface?.Price ?? 0m) + componentPrice;
            var recalculatedDuration = (surface?.Duration ?? 0) + componentDuration;
            var hasChanges = variant.Price != recalculatedPrice || variant.Duration != recalculatedDuration;

            variant.Price = recalculatedPrice;
            variant.Duration = recalculatedDuration;
            _unitOfWork.NailVariantRepository.Update(variant);

            return hasChanges;
        }

        private async Task<bool> ApplyCustomerNailTotalsAsync(int customerNailId)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(customerNailId);
            if (customerNail == null)
            {
                return false;
            }

            var surface = customerNail.NailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(customerNail.NailSurfaceId.Value)
                : null;
            var nailComponents = await _unitOfWork.CustomerNailComponentRepository.GetByCustomerNailIdAsync(customerNailId);

            var componentPrice = 0m;
            var componentDuration = 0;
            foreach (var nailComponent in nailComponents)
            {
                var multiplier = GetFingerPriceMultiplier(nailComponent.FingerIndex);

                if (IsActive(nailComponent.Component?.Status))
                {
                    componentPrice += (nailComponent.Component?.Price ?? 0m) * multiplier;
                    componentDuration += (nailComponent.Component?.Duration ?? 0) * multiplier;
                }
            }

            var recalculatedPrice = (surface?.Price ?? 0m) + componentPrice;
            var recalculatedDuration = (surface?.Duration ?? 0) + componentDuration;
            var hasChanges = customerNail.Price != recalculatedPrice || customerNail.Duration != recalculatedDuration;

            customerNail.Price = recalculatedPrice;
            customerNail.Duration = recalculatedDuration;
            _unitOfWork.CustomerNailRepository.Update(customerNail);

            return hasChanges;
        }

        private static bool IsActive(string? status)
        {
            return string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetFingerPriceMultiplier(int fingerIndex)
        {
            return fingerIndex == -1 ? 5 : 1;
        }

        public async Task<ProcessAllBookingsResult> ProcessAllCompletedBookingsAsync()
        {
            var result = new ProcessAllBookingsResult();
            var errors = new List<string>();

            try
            {
                var allBookings = await _unitOfWork.BookingRepository.FindAllAsync();
                var completedBookings = allBookings.Where(b => b.Status == BookingStatus.Completed).ToList();

                result.TotalBookings = completedBookings.Count;

                if (!completedBookings.Any())
                {
                    result.Message = "No completed bookings found to process";
                    result.Success = true;
                    return result;
                }

                var processedCount = 0;
                var skippedCount = 0;

                foreach (var booking in completedBookings)
                {
                    try
                    {


                        var processResult = await ProcessCompletedBookingAsync(booking.BookingId);

                        if (processResult.Success)
                        {
                            processedCount++;
                            result.ProcessedBookingIds.Add(booking.BookingId);
                        }
                        else
                        {
                            errors.Add($"Booking {booking.BookingId}: {processResult.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Booking {booking.BookingId}: {ex.Message}");
                    }
                }
                result.Success = errors.Count == 0;
                result.ProcessedCount = processedCount;
                result.SkippedCount = skippedCount;
                result.ErrorCount = errors.Count;
                result.Errors = errors;
                result.Message = $"Processed {processedCount} bookings, skipped {skippedCount}, errors {errors.Count}";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ProcessBookingResult> ProcessCompletedBookingAsync(Guid bookingId)
        {
            var result = new ProcessBookingResult();

            try
            {
                var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
                if (booking == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Booking with ID {bookingId} not found";
                    return result;
                }

                if (booking.Status != BookingStatus.Completed)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Booking {bookingId} is not completed (Status: {booking.Status})";
                    return result;
                }

                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(booking.CustomerId);
                if (customer == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Customer with ID {booking.CustomerId} not found";
                    return result;
                }

                var transaction = await CreateTransactionFromBookingAsync(booking);
                result.Transaction = transaction;
                result.TransactionCreated = transaction != null;

                var loyaltyTransaction = await CreateLoyaltyTransactionFromBookingAsync(booking, customer);
                result.LoyaltyTransaction = loyaltyTransaction;
                result.LoyaltyTransactionCreated = loyaltyTransaction != null;

                result.Success = true;
                result.Message = "Transaction and LoyaltyTransaction created successfully";
                result.BookingId = bookingId;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<string> GenerateUniqueOrderCodeAsync()
        {
            long orderCode;
            bool exists;

            do
            {
                orderCode = RandomNumberGenerator.GetInt32(100000, 999999);
                exists = await _unitOfWork.TransactionRepository.ExistsAsync(t => t.OrderCode == orderCode.ToString());
            } while (exists);

            return orderCode.ToString();
        }

        private async Task<Transaction> CreateTransactionFromBookingAsync(Booking booking)
        {
            var orderCode = await GenerateUniqueOrderCodeAsync();

            var updatedAt = booking.UpdatedAt ?? DateTime.UtcNow;
            var expiresAt = updatedAt.AddMinutes(15); ;

            var transaction = new Transaction
            {
                BookingId = booking.BookingId,
                OrderCode = orderCode,
                Amount = booking.AmountDue ?? 0m,
                Reference = null,
                PaymentLinkId = null,
                CheckoutUrl = string.Empty,
                QrCode = string.Empty,
                Status = TransactionStatus.Paid,
                CreatedAt = (DateTime)booking.UpdatedAt,
                PaidAt = (DateTime)booking.UpdatedAt, 
                ExpiresAt = expiresAt,
                WebhookPayload = string.Empty
            };

            await _unitOfWork.TransactionRepository.CreateAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return transaction;
        }

        private async Task<LoyaltyTransaction> CreateLoyaltyTransactionFromBookingAsync(Booking booking, Customer customer)
        {
            var earnedPoints = 10;

            customer.LoyaltyPoint += earnedPoints;
            customer.LifetimePoints += earnedPoints;

            var pagedResult = await _unitOfWork.LoyaltyTierRepository.GetPagedAsync(
                pageNumber: 1,
                pageSize: 10
            );
            var allTiers = pagedResult.Items;
            var matchedTier = allTiers
                .Where(t =>
                    (!t.MinLifetimePoints.HasValue || customer.LifetimePoints >= t.MinLifetimePoints.Value) &&
                    (!t.MaxLifetimePoints.HasValue || customer.LifetimePoints <= t.MaxLifetimePoints.Value))
                .OrderByDescending(t => t.MinLifetimePoints ?? 0)
                .FirstOrDefault();

            matchedTier ??= allTiers.FirstOrDefault(t => t.SortOrder == 1);

            if (matchedTier != null)
            {
                customer.LoyaltyTierId = matchedTier.LoyaltyTierId;
            }

            _unitOfWork.CustomerRepository.Update(customer);

            var loyaltyTransaction = new LoyaltyTransaction
            {
                CustomerId = booking.CustomerId,
                BookingId = booking.BookingId,
                Points = earnedPoints,
                TransactionType = LoyaltyTransactionType.Earned,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(loyaltyTransaction);
            await _unitOfWork.SaveChangesAsync();

            return loyaltyTransaction;
        }
    }

    public class ProcessBookingResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid BookingId { get; set; }
        public Transaction? Transaction { get; set; }
        public bool TransactionCreated { get; set; }
        public LoyaltyTransaction? LoyaltyTransaction { get; set; }
        public bool LoyaltyTransactionCreated { get; set; }
    }

    public class ProcessAllBookingsResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int TotalBookings { get; set; }
        public int ProcessedCount { get; set; }
        public int SkippedCount { get; set; }
        public int ErrorCount { get; set; }
        public List<Guid> ProcessedBookingIds { get; set; } = new List<Guid>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}