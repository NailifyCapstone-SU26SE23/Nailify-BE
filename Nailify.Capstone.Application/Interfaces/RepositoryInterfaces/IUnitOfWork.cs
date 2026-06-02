using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ICategoryTypeRepository CategoryTypeRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        INailDesignRepository NailDesignRepository { get; }
        ISalonOperatingHourRepository SalonOperatingHourRepository { get; }
        ISalonRepository SalonRepository { get; }
        INailArtistRepository NailArtistRepository { get; }
        IScheduleRepository ScheduleRepository { get; }
        IComponentRepository ComponentRepository { get; }
        INailShapeRepository NailShapeRepository { get; }
        INailSurfaceRepository NailSurfaceRepository { get; }
        INailVariantRepository NailVariantRepository { get; }
        INailComponentRepository NailComponentRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
