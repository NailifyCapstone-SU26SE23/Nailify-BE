# Staff Transfer Across Salons — Full Code Plan

## Tóm tắt

Điều chuyển (tăng cường) thợ A từ salon B → salon C theo mô hình đã chốt:
1. **Entity `StaffTransfer` theo khoảng ngày** (ArtistId, FromSalonId, ToSalonId, StartDate–EndDate, Status). Salon hiệu lực của thợ theo ngày = ToSalon nếu có transfer Active phủ ngày đó, ngược lại salon gốc (`User.SalonId` KHÔNG đổi, hết hạn tự về salon gốc).
2. **Booking Approved tại salon B trong khoảng điều chuyển**: tái sử dụng luồng Emergency Off — đổi thợ cùng salon đủ skill → dời giờ ±30/60p → hủy + thông báo; trả báo cáo chi tiết (đổi thợ / dời / hủy) cho manager.
3. **Trong khoảng tăng cường, khách book thợ A** → booking thuộc **salon C** (`Booking.SalonId = C`), thợ bị ẩn khỏi danh sách/availability salon B, hiện ở salon C.

> ⚠️ Phát hiện: `IStaffTransferRepository.cs` **đã tồn tại trong workspace nhưng chưa hoàn chỉnh** (dòng 16 bị cắt giữa chừng `Task<List<Guid>> GetTransferredOutArtist`, và tham chiếu entity `StaffTransfer` chưa tồn tại → project hiện KHÔNG build được). Plan sẽ **hoàn thiện file này** thay vì tạo mới (§4).

---

## §1. [MỚI] Enum trạng thái — `Nailify.Capstone.Domain\Enums\StaffTransferStatus.cs`

```csharp
namespace Nailify.Capstone.Domain.Enums
{
    public enum StaffTransferStatus
    {
        Scheduled,   // Đã lên lịch / đang hiệu lực (theo khoảng ngày)
        Completed,   // Đã kết thúc (hết EndDate hoặc kết thúc sớm)
        Cancelled    // Bị hủy trước khi bắt đầu
    }
}
```

## §2. [MỚI] Entity — `Nailify.Capstone.Domain\Entities\StaffTransfer.cs`

```csharp
using Nailify.Capstone.Domain.Enums;
using System;

namespace Nailify.Capstone.Domain.Entities
{
    public class StaffTransfer
    {
        public Guid StaffTransferId { get; set; }
        public Guid NailArtistId { get; set; }
        public Guid FromSalonId { get; set; }
        public Guid ToSalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public StaffTransferStatus Status { get; set; } = StaffTransferStatus.Scheduled;
        public string? Reason { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual NailArtist NailArtist { get; set; } = null!;
        public virtual Salon FromSalon { get; set; } = null!;
        public virtual Salon ToSalon { get; set; } = null!;
    }
}
```

## §3. [SỬA] `Nailify.Capstone.Infrastructure\DBContext\NailifyDbContext.cs`

Thêm DbSet (cạnh các DbSet hiện có ~dòng 21–35):

```csharp
public DbSet<StaffTransfer> StaffTransfers { get; set; }
```

Thêm Fluent config trong `OnModelCreating` (sau block config NailArtist ~dòng 568):

```csharp
modelBuilder.Entity<StaffTransfer>(entity =>
{
    entity.HasKey(x => x.StaffTransferId);
    entity.HasIndex(x => new { x.NailArtistId, x.StartDate, x.EndDate });
    entity.Property(x => x.Reason).HasMaxLength(500);

    entity.HasOne(x => x.NailArtist)
        .WithMany()
        .HasForeignKey(x => x.NailArtistId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.FromSalon)
        .WithMany()
        .HasForeignKey(x => x.FromSalonId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.ToSalon)
        .WithMany()
        .HasForeignKey(x => x.ToSalonId)
        .OnDelete(DeleteBehavior.Restrict);
});
```

(2 FK cùng trỏ Salon → bắt buộc `Restrict` để tránh multiple cascade paths.)

## §4. [HOÀN THIỆN FILE ĐANG DỞ] `Application\Interfaces\RepositoryInterfaces\IStaffTransferRepository.cs`

File đã tồn tại nhưng bị cắt dở → thay bằng bản đầy đủ:

```csharp
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IStaffTransferRepository : IGenericRepository<StaffTransfer>
    {
        // Transfer đang hiệu lực của thợ tại 1 ngày (Scheduled và StartDate <= date <= EndDate)
        Task<StaffTransfer?> GetActiveTransferByArtistAndDateAsync(Guid artistId, DateTime date);
        // Các transfer tăng cường VÀO salon tại 1 ngày (kèm NailArtist.Account + Skills)
        Task<List<StaffTransfer>> GetTransfersIntoSalonByDateAsync(Guid salonId, DateTime date);
        // Danh sách ArtistId bị điều RA KHỎI salon tại 1 ngày (để ẩn khỏi salon gốc)
        Task<List<Guid>> GetTransferredOutArtistIdsAsync(Guid salonId, DateTime date);
        // Kiểm tra thợ đã có transfer Scheduled trùng khoảng ngày chưa
        Task<bool> HasOverlappingTransferAsync(Guid artistId, DateTime startDate, DateTime endDate);
        // Danh sách transfer phân trang cho manager
        Task<PagedList<StaffTransfer>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, StaffTransferStatus? status);
    }
}
```

## §5. [MỚI] `Nailify.Capstone.Infrastructure\Repository\StaffTransferRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class StaffTransferRepository : GenericRepository<StaffTransfer>, IStaffTransferRepository
    {
        public StaffTransferRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<StaffTransfer?> GetActiveTransferByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            var d = date.Date;
            return await FindByCondition(x => x.NailArtistId == artistId
                    && x.Status == StaffTransferStatus.Scheduled
                    && x.StartDate.Date <= d
                    && x.EndDate.Date >= d, trackChanges: false)
                .FirstOrDefaultAsync();
        }

        public async Task<List<StaffTransfer>> GetTransfersIntoSalonByDateAsync(Guid salonId, DateTime date)
        {
            var d = date.Date;
            return await FindByCondition(x => x.ToSalonId == salonId
                    && x.Status == StaffTransferStatus.Scheduled
                    && x.StartDate.Date <= d
                    && x.EndDate.Date >= d, trackChanges: false)
                .Include(x => x.NailArtist)
                    .ThenInclude(a => a.Account)
                .Include(x => x.NailArtist)
                    .ThenInclude(a => a.NailArtistSkills)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetTransferredOutArtistIdsAsync(Guid salonId, DateTime date)
        {
            var d = date.Date;
            return await FindByCondition(x => x.FromSalonId == salonId
                    && x.Status == StaffTransferStatus.Scheduled
                    && x.StartDate.Date <= d
                    && x.EndDate.Date >= d, trackChanges: false)
                .Select(x => x.NailArtistId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingTransferAsync(Guid artistId, DateTime startDate, DateTime endDate)
        {
            var s = startDate.Date;
            var e = endDate.Date;
            return await FindByCondition(x => x.NailArtistId == artistId
                    && x.Status == StaffTransferStatus.Scheduled
                    && x.StartDate.Date <= e
                    && x.EndDate.Date >= s, trackChanges: false)
                .AnyAsync();
        }

        public async Task<PagedList<StaffTransfer>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, StaffTransferStatus? status)
        {
            return await GetPagedAsync(pageNumber, pageSize,
                x => (!salonId.HasValue || x.FromSalonId == salonId.Value || x.ToSalonId == salonId.Value)
                    && (!artistId.HasValue || x.NailArtistId == artistId.Value)
                    && (!status.HasValue || x.Status == status.Value),
                x => x.NailArtist, x => x.FromSalon, x => x.ToSalon);
        }
    }
}
```

Lưu ý: `GetPagedAsync` nhận `params Expression<Func<T, object>>[] includes` — nếu cần include `NailArtist.Account` để lấy tên thợ, có thể thay bằng query thủ công với `.Include(x => x.NailArtist).ThenInclude(a => a.Account)` + `PagedList<StaffTransfer>.ToPagedList(...)` theo pattern các repo khác.

## §6. [SỬA] `IUnitOfWork.cs` + `UnitOfWork.cs`

`Application\Interfaces\RepositoryInterfaces\IUnitOfWork.cs` — thêm property:

```csharp
IStaffTransferRepository StaffTransferRepository { get; }
```

`Nailify.Capstone.Infrastructure\UnitOfWork.cs` — theo pattern lazy `??=` hiện có:

```csharp
private IStaffTransferRepository _staffTransferRepository = null!;
public IStaffTransferRepository StaffTransferRepository => _staffTransferRepository ??= new StaffTransferRepository(_context);
```

## §7. [SỬA] `Nailify.Capstone.Infrastructure\Configuration\DependencyInjection.cs`

```csharp
// Cạnh các repo khác (~sau dòng 130)
services.AddScoped<IStaffTransferRepository, StaffTransferRepository>();
// Cạnh các service khác (~sau dòng 187, gần INailArtistEmergencyService)
services.AddScoped<IStaffTransferService, StaffTransferService>();
```

## §8. [MỚI] DTOs

`Application\DTOs\RequestDTOs\StaffTransferRequestDTOs\CreateStaffTransferRequestDTO.cs` (FromSalonId suy từ `User.SalonId` của thợ phía server, không cho client truyền):

```csharp
using System;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs
{
    public class CreateStaffTransferRequestDTO
    {
        public Guid NailArtistId { get; set; }
        public Guid ToSalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
```

`Application\DTOs\ResponseDTOs\StaffTransferResponseDTOs\StaffTransferResponseDTO.cs`:

```csharp
using AutoMapper;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.StaffTransferResponseDTOs
{
    public class StaffTransferResponseDTO : IMapFrom<StaffTransfer>
    {
        public Guid StaffTransferId { get; set; }
        public Guid NailArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public Guid FromSalonId { get; set; }
        public string FromSalonName { get; set; } = string.Empty;
        public Guid ToSalonId { get; set; }
        public string ToSalonName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public StaffTransferStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<StaffTransfer, StaffTransferResponseDTO>()
                .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.NailArtist != null && s.NailArtist.Account != null
                    ? s.NailArtist.Account.FirstName + " " + s.NailArtist.Account.LastName : string.Empty))
                .ForMember(d => d.FromSalonName, opt => opt.MapFrom(s => s.FromSalon != null ? s.FromSalon.Name : string.Empty))
                .ForMember(d => d.ToSalonName, opt => opt.MapFrom(s => s.ToSalon != null ? s.ToSalon.Name : string.Empty))
                .IgnoreAllNonExisting();
        }
    }

    // Kết quả tạo transfer: transfer + báo cáo xử lý booking bị ảnh hưởng cho manager
    public class StaffTransferResultDTO
    {
        public StaffTransferResponseDTO Transfer { get; set; } = null!;
        public int TotalAffectedBookings { get; set; }
        public int AutoReassignedCount { get; set; }
        public int RescheduleSuggestedCount { get; set; }
        public int CancelledCount { get; set; }
        public List<EmergencyBookingHandlingDetailDTO> ProcessingDetails { get; set; } = new();
    }
}
```

(Kiểm tra tên property `Salon.Name` khi implement — nếu là `SalonName` thì sửa mapping tương ứng.)

## §9. [REFACTOR] `NailArtistEmergencyService` — tách hàm xử lý booking theo ngày để tái sử dụng

`INailArtistEmergencyService.cs` — thêm method:

```csharp
// Xử lý toàn bộ booking Approved của thợ trong 1 ngày: đổi thợ -> dời giờ -> hủy.
// KHÔNG SaveChanges bên trong; caller chịu trách nhiệm save.
Task<EmergencyOffResultDTO> ProcessAffectedBookingsForDateAsync(Guid artistId, DateTime targetDate, string reason);
```

`NailArtistEmergencyService.cs`:
- Di chuyển NGUYÊN VĂN khối xử lý booking hiện tại (dòng ~64–268: lấy `GetApprovedBookingsWithDetailsByArtistAndDateAsync` → sort StartTime → candidates từ `GetActiveArtistsWithSchedulesAndSkillsBySalonAsync(salonId, artistId)` → per booking: reassign nếu shift phủ + không trùng break + `CheckSkillMatrixAndCustomNailLevelAsync` + không `HasSimulationConflictAsync` → else Nearest Slot Search offsets `{30,-30,60,-60}` → else `x.Cancel(...)`, BookingHistory + notification fire-and-forget) vào `ProcessAffectedBookingsForDateAsync`.
- Chỉ 2 thay đổi: `request.Reason` → tham số `reason`; **bỏ** `SaveChangesAsync` bên trong (caller save).
- `SetArtistOffDutyAsync` còn lại: validate artist + tạo NailArtistBreak `[EMERGENCY OFF]` + gọi `ProcessAffectedBookingsForDateAsync(artistId, targetDate, request.Reason)` + `SaveChangesAsync` → hành vi Emergency Off không đổi.

## §10. [MỚI] Service — `IStaffTransferService` + `StaffTransferService`

`Application\Interfaces\ServiceInterfaces\IStaffTransferService.cs`:

```csharp
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.StaffTransferResponseDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IStaffTransferService
    {
        Task<ApiResult<StaffTransferResultDTO>> CreateTransferAsync(CreateStaffTransferRequestDTO request, Guid actorId);
        Task<ApiResult<StaffTransferResponseDTO>> CancelTransferAsync(Guid transferId);
        Task<ApiResult<PagedList<StaffTransferResponseDTO>>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, StaffTransferStatus? status);
        Task<ApiResult<StaffTransferResponseDTO>> GetTransferByIdAsync(Guid transferId);
    }
}
```

`Application\Services\StaffTransferService.cs`:

```csharp
using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.StaffTransferResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class StaffTransferService : IStaffTransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INailArtistEmergencyService _emergencyService;
        private readonly INotificationService _notificationService;

        public StaffTransferService(IUnitOfWork unitOfWork, IMapper mapper,
            INailArtistEmergencyService emergencyService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emergencyService = emergencyService;
            _notificationService = notificationService;
        }

        public async Task<ApiResult<StaffTransferResultDTO>> CreateTransferAsync(CreateStaffTransferRequestDTO request, Guid actorId)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date;
            var todayLocal = DateTime.UtcNow.AddHours(7).Date;

            if (startDate > endDate)
                return new ApiErrorResult<StaffTransferResultDTO>("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            if (startDate < todayLocal)
                return new ApiErrorResult<StaffTransferResultDTO>("Không thể tạo điều chuyển trong quá khứ.");

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NailArtistId);
            if (artist == null || artist.Status != "Active")
                return new ApiErrorResult<StaffTransferResultDTO>("Không tìm thấy thợ hoặc thợ không hoạt động.");
            if (artist.Account?.SalonId == null)
                return new ApiErrorResult<StaffTransferResultDTO>("Thợ chưa được gán salon gốc.");

            var fromSalonId = artist.Account.SalonId.Value;
            if (fromSalonId == request.ToSalonId)
                return new ApiErrorResult<StaffTransferResultDTO>("Salon đích phải khác salon gốc của thợ.");

            var toSalon = await _unitOfWork.SalonRepository.GetByIdAsync(request.ToSalonId);
            if (toSalon == null)
                return new ApiErrorResult<StaffTransferResultDTO>("Không tìm thấy salon đích.");

            if (await _unitOfWork.StaffTransferRepository.HasOverlappingTransferAsync(request.NailArtistId, startDate, endDate))
                return new ApiErrorResult<StaffTransferResultDTO>("Thợ đã có lịch điều chuyển trùng khoảng thời gian này.");

            var transfer = new StaffTransfer
            {
                StaffTransferId = Guid.NewGuid(),
                NailArtistId = request.NailArtistId,
                FromSalonId = fromSalonId,
                ToSalonId = request.ToSalonId,
                StartDate = startDate,
                EndDate = endDate,
                Status = StaffTransferStatus.Scheduled,
                Reason = request.Reason,
                CreatedBy = actorId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.StaffTransferRepository.CreateAsync(transfer);

            // Xử lý toàn bộ booking Approved của thợ tại salon gốc trong khoảng điều chuyển
            var result = new StaffTransferResultDTO();
            var reason = $"Thợ được điều động tăng cường sang chi nhánh khác ({request.Reason ?? "theo kế hoạch"})";
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                var dayResult = await _emergencyService.ProcessAffectedBookingsForDateAsync(request.NailArtistId, d, reason);
                result.TotalAffectedBookings += dayResult.TotalAffectedBookings;
                result.AutoReassignedCount += dayResult.AutoReassignedCount;
                result.RescheduleSuggestedCount += dayResult.RescheduleSuggestedCount;
                result.CancelledCount += dayResult.CancelledAndRefundedCount;
                result.ProcessingDetails.AddRange(dayResult.ProcessingDetails);
            }

            await _unitOfWork.SaveChangesAsync();

            // Thông báo cho thợ (fire-and-forget theo pattern hiện có)
            _ = _notificationService.SendNotificationToUserAsync(artist.AccountId,
                "Lịch điều động tăng cường",
                $"Bạn được điều động sang chi nhánh khác từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}.");

            var created = await _unitOfWork.StaffTransferRepository.GetPagedTransfersAsync(1, 1, null, null, null); // hoặc query lại theo Id kèm include
            result.Transfer = _mapper.Map<StaffTransferResponseDTO>(transfer);
            return new ApiSuccessResult<StaffTransferResultDTO>(result, "Tạo điều chuyển thành công.");
        }

        public async Task<ApiResult<StaffTransferResponseDTO>> CancelTransferAsync(Guid transferId)
        {
            var transfer = await _unitOfWork.StaffTransferRepository.GetByIdAsync(transferId);
            if (transfer == null)
                return new ApiErrorResult<StaffTransferResponseDTO>("Không tìm thấy điều chuyển.");
            if (transfer.Status != StaffTransferStatus.Scheduled)
                return new ApiErrorResult<StaffTransferResponseDTO>("Chỉ có thể hủy điều chuyển đang ở trạng thái Scheduled.");

            var todayLocal = DateTime.UtcNow.AddHours(7).Date;
            if (transfer.StartDate.Date > todayLocal)
            {
                // Chưa bắt đầu -> hủy hẳn
                transfer.Status = StaffTransferStatus.Cancelled;
            }
            else
            {
                // Đang giữa kỳ -> kết thúc sớm từ hôm nay (booking đã xử lý trước đó KHÔNG tự khôi phục)
                var newEnd = todayLocal.AddDays(-1);
                transfer.EndDate = newEnd < transfer.StartDate.Date ? transfer.StartDate.Date : newEnd;
                transfer.Status = StaffTransferStatus.Completed;
            }
            transfer.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<StaffTransferResponseDTO>(_mapper.Map<StaffTransferResponseDTO>(transfer), "Hủy/kết thúc điều chuyển thành công.");
        }

        public async Task<ApiResult<PagedList<StaffTransferResponseDTO>>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, StaffTransferStatus? status)
        {
            var paged = await _unitOfWork.StaffTransferRepository.GetPagedTransfersAsync(pageNumber, pageSize, salonId, artistId, status);
            var dtos = _mapper.Map<System.Collections.Generic.List<StaffTransferResponseDTO>>(paged);
            var result = new PagedList<StaffTransferResponseDTO>(dtos, paged.MetaData.TotalCount, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<StaffTransferResponseDTO>>(result);
        }

        public async Task<ApiResult<StaffTransferResponseDTO>> GetTransferByIdAsync(Guid transferId)
        {
            var transfer = await _unitOfWork.StaffTransferRepository
                .FindByCondition(x => x.StaffTransferId == transferId, trackChanges: false)
                .Include(x => x.NailArtist).ThenInclude(a => a.Account)
                .Include(x => x.FromSalon)
                .Include(x => x.ToSalon)
                .FirstOrDefaultAsync();
            if (transfer == null)
                return new ApiErrorResult<StaffTransferResponseDTO>("Không tìm thấy điều chuyển.");
            return new ApiSuccessResult<StaffTransferResponseDTO>(_mapper.Map<StaffTransferResponseDTO>(transfer));
        }
    }
}
```

Lưu ý khi implement: đối chiếu tên method thực tế của repo hiện có (`GetNailArtistWithProfileAsync`, `GetByIdAsync`, `CreateAsync`, chữ ký `SendNotificationToUserAsync`, cách `PagedList` construct/MetaData) — chỉnh theo đúng codebase.

## §11. [SỬA] `BookingAssignmentService` — ẩn thợ ở salon B, hiện ở salon C

Thêm helper private:

```csharp
// Áp dụng điều chuyển: loại thợ bị điều đi khỏi salon, thêm thợ tăng cường vào salon theo ngày
private async Task<List<NailArtist>> ApplyStaffTransfersAsync(List<NailArtist> homeArtists, Guid salonId, DateTime date)
{
    var transferredOutIds = await _unitOfWork.StaffTransferRepository.GetTransferredOutArtistIdsAsync(salonId, date);
    var incoming = await _unitOfWork.StaffTransferRepository.GetTransfersIntoSalonByDateAsync(salonId, date);

    var result = homeArtists.Where(x => !transferredOutIds.Contains(x.NailArtistId)).ToList();
    foreach (var t in incoming)
    {
        if (t.NailArtist != null && t.NailArtist.Status == "Active"
            && result.All(x => x.NailArtistId != t.NailArtist.NailArtistId))
        {
            result.Add(t.NailArtist);
        }
    }
    return result;
}
```

Gọi helper này ngay sau khi lấy danh sách thợ theo salon trong các luồng: `GetRandomArtistAsync`, `GetSuggestedArtistAsync`, `GetAvailableArtistsForBookingAsync` (và `GetSalonAvailableSlotsAsync` của plan salon-slots nếu triển khai). Thợ tăng cường được include sẵn `NailArtistSkills` (§5) nên các filter skill hiện có hoạt động bình thường.

## §12. [SỬA] `BookingCreationService.CreateBookingAsync` — chặn book sai salon

Trong nhánh `if (request.NailArtistId.HasValue)`, ngay sau `GetArtistWithLockAsync` (~dòng 254–297, đã có `localDate` UTC+7):

```csharp
var activeTransfer = await _unitOfWork.StaffTransferRepository
    .GetActiveTransferByArtistAndDateAsync(request.NailArtistId.Value, localDate);
var effectiveSalonId = activeTransfer?.ToSalonId ?? artist.Account.SalonId;
if (effectiveSalonId != request.SalonId)
{
    await _unitOfWork.RollbackTransactionAsync();
    return new ApiErrorResult<BookingResponseDTO>(
        "Thợ đang được điều động sang chi nhánh khác trong ngày này. Vui lòng chọn thợ khác hoặc đặt tại chi nhánh thợ đang làm việc.");
}
```

→ Trong khoảng tăng cường: book thợ A tại salon B bị chặn (400); book tại salon C hợp lệ và `Booking.SalonId = C` (giữ nguyên theo request). Hết hạn transfer, mọi thứ tự về salon gốc mà không cần job nào (kiểm tra theo ngày).

## §13. [MỚI] `Nailify.Capstone.Presentation\Controllers\StaffTransfersController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffTransfersController : BaseApiController
    {
        private readonly IStaffTransferService _staffTransferService;

        public StaffTransfersController(IStaffTransferService staffTransferService)
        {
            _staffTransferService = staffTransferService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTransfer([FromBody] CreateStaffTransferRequestDTO request)
        {
            var actorId = GetCurrentUserId();
            var result = await _staffTransferService.CreateTransferAsync(request, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{transferId}/cancel")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CancelTransfer(Guid transferId)
        {
            var result = await _staffTransferService.CancelTransferAsync(transferId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> GetTransfers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] Guid? salonId = null, [FromQuery] Guid? artistId = null, [FromQuery] StaffTransferStatus? status = null)
        {
            var result = await _staffTransferService.GetPagedTransfersAsync(pageNumber, pageSize, salonId, artistId, status);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{transferId}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> GetTransferById(Guid transferId)
        {
            var result = await _staffTransferService.GetTransferByIdAsync(transferId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
```

## §14. Migration

```
dotnet ef migrations add AddStaffTransferTable --project Nailify.Capstone.Infrastructure --startup-project Nailify.Capstone.Presentation
```

---

## Test Plan

1. Build sạch + migration tạo bảng `StaffTransfers` đúng (3 FK Restrict, index composite).
2. Tạo transfer: overlap / ToSalon == FromSalon / StartDate quá khứ / StartDate > EndDate → 400 với message tiếng Việt tương ứng.
3. Tạo transfer khi thợ có booking Approved tại salon B trong khoảng ngày → response chứa báo cáo: N reassigned / M reschedule-suggested / K cancelled + ProcessingDetails từng đơn; khách nhận notification.
4. Trong khoảng transfer: thợ A biến mất khỏi available-artists của salon B, xuất hiện ở salon C (đủ skill filter); sau EndDate tự về salon B (không cần job).
5. Book thợ A tại salon B trong khoảng transfer → 400; book tại salon C → thành công, `Booking.SalonId = C`.
6. Cancel transfer chưa bắt đầu → Cancelled; cancel giữa kỳ → EndDate co lại hôm qua + Completed, booking đã xử lý không khôi phục.
7. Regression: Emergency Off (`SetArtistOffDutyAsync`) hoạt động y hệt sau refactor §9.

## Giả định đã chốt

- Chỉ booking **Approved** được auto-xử lý (Pending để manager xử lý tay) — giống Emergency Off hiện tại.
- `Schedule` giữ nguyên salon-agnostic: thợ mang ca làm của mình sang salon tăng cường.
- Hủy transfer giữa kỳ KHÔNG tự khôi phục các booking đã bị đổi/dời/hủy.
- Voucher đền bù khi hủy đơn giữ nguyên TODO như Emergency Off.
- Khi implement, đối chiếu chữ ký thực tế: `Salon.Name`, `SendNotificationToUserAsync(userId, title, body)`, method repo (`GetByIdAsync`/`CreateAsync`), constructor `PagedList`.
