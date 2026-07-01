using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest request);
    }
}
