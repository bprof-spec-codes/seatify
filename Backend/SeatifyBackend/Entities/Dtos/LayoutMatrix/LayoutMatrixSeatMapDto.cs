using Entities.Dtos.Seat;

namespace Entities.Dtos.LayoutMatrix
{
    public class LayoutMatrixSeatMapDto
    {
        public string Id { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<SeatViewDto> Seats { get; set; } = new List<SeatViewDto>();
    }
}
