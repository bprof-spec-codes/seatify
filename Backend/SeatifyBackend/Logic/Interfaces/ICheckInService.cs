using System.Threading.Tasks;
using Entities.Dtos.CheckIn;

namespace Logic.Interfaces
{
    public interface ICheckInService
    {
        Task<CheckInResult> ValidateTicketAsync(string payload);
        Task<CheckInResult> ConfirmCheckInAsync(string ticketId);
    }
}
