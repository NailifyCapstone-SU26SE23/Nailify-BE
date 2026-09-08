namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IOrderCodeGenerator
    {
        Task<long> GenerateUniqueOrderCodeAsync();
    }
}
