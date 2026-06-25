using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class NailArtistService : INailArtistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailArtistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResult<bool>> DeleteNailArtistAsync(Guid artistId)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if (artist == null)
            {
                return new ApiResult<bool>(false, "Không tìm thấy thợ nail với ID đã cho.");
            }

            _unitOfWork.NailArtistRepository.Delete(artist);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResult<bool>(true, true, "Thợ nail đã được xóa thành công.");
        }

        public async Task<ApiResult<NailArtistResponseDTO>> GetNailArtistByIdAsync(Guid artistId)
        {
            var arstist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(artistId);
            if (arstist == null)
            {
                return new ApiResult<NailArtistResponseDTO>(false, "Không tìm thấy thợ nail với ID đã cho.");
            }
            var response = _mapper.Map<NailArtistResponseDTO>(arstist);
            return new ApiSuccessResult<NailArtistResponseDTO>(response, "Lấy thông tin thợ nail thành công.");
        }

        public async Task<ApiResult<PagedList<NailArtistResponseDTO>>> GetPagedNailArtistsAsync(int pageNumber, int pageSize, Guid? salonId = null)
        {
            var pagedArtists = await _unitOfWork.NailArtistRepository.GetPagedAsync(pageNumber, pageSize, salonId.HasValue ? (x => x.Account.SalonId == salonId.Value) : null, x => x.Account);

            var mappedItems = _mapper.Map<List<NailArtistResponseDTO>>(pagedArtists.Items);

            var response = new PagedList<NailArtistResponseDTO>(
                mappedItems,
                pagedArtists.MetaData.TotalItems,
                pagedArtists.MetaData.CurrentPage,
                pagedArtists.MetaData.PageSize
            );

            return new ApiSuccessResult<PagedList<NailArtistResponseDTO>>(response, "Lấy danh sách thợ nail phân trang thành công.");
        }

        public async Task<ApiResult<NailArtistResponseDTO>> PatchNailArtistAsync(Guid artistId, NailArtistPatchRequest request)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if (artist == null)
            {
                return new ApiResult<NailArtistResponseDTO>(false, "Không tìm thấy thợ nail với ID đã cho.");
            }

            if (request.SalonId.HasValue)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(artist.AccountId);
                if (user != null)
                {
                    user.SalonId = request.SalonId.Value;
                    _unitOfWork.UserRepository.Update(user);
                }
            }

            _mapper.Map(request, artist);
            _unitOfWork.NailArtistRepository.Update(artist);
            await _unitOfWork.SaveChangesAsync();
            var updatedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(artistId);
            var response = _mapper.Map<NailArtistResponseDTO>(updatedArtist);
            return new ApiSuccessResult<NailArtistResponseDTO>(response, "Cập nhật một phần thông tin Thợ nail thành công.");
        }

        public async Task<ApiResult<NailArtistResponseDTO>> UpdateNailArtistAsync(Guid artistId, NailArtistUpdateRequest request)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if (artist == null)
                return new ApiErrorResult<NailArtistResponseDTO>("Không tìm thấy Thợ nail.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(artist.AccountId);
            if (user != null)
            {
                user.SalonId = request.SalonId;
                _unitOfWork.UserRepository.Update(user);
            }

            _mapper.Map(request, artist);
            _unitOfWork.NailArtistRepository.Update(artist);
            await _unitOfWork.SaveChangesAsync();
            var updatedArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(artistId);
            var response = _mapper.Map<NailArtistResponseDTO>(updatedArtist);
            return new ApiSuccessResult<NailArtistResponseDTO>(response, "Cập nhật Thợ nail thành công.");
        }
    }
}
