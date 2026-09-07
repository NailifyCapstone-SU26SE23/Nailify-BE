using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System.Security.Cryptography;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class PayOSHelper : IOrderCodeGenerator
    {
        private readonly IUnitOfWork _unitOfWork;

        public PayOSHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> GenerateUniqueOrderCodeAsync()
        {
            long orderCode;
            bool exists;

            do
            {
                orderCode = RandomNumberGenerator.GetInt32(100000, 999999);
                exists = await _unitOfWork.TransactionRepository.ExistsAsync(t => t.OrderCode == orderCode.ToString());
            } while (exists);

            return orderCode;
        }
    }
}
