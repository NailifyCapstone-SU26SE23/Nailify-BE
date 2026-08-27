using Google.Apis.Auth;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IGoogleConfiguration _googleConfiguration;

        public GoogleAuthService(IGoogleConfiguration googleConfiguration)
        {
            _googleConfiguration = googleConfiguration;
        }

        public async Task<GoogleUserInfoDTO?> VerifyTokenAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(_googleConfiguration.ClientId))
            {
                throw new InvalidOperationException("Google ClientId is not configured.");
            }
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _googleConfiguration.ClientId }
                    });
                if (payload == null) return null;
                return new GoogleUserInfoDTO
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    Picture = payload.Picture
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
