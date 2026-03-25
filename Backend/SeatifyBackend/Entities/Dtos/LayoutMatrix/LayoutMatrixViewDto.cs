namespace Entities.Dtos.LayoutMatrix
{
    public class LayoutMatrixViewDto
    {
        public string Id { get; set; } = string.Empty;

        public string AuditoriumId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Rows { get; set; }

        public int Columns { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
