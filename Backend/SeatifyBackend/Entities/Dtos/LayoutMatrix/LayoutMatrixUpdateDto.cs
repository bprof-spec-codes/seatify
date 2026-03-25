using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.LayoutMatrix
{
    public class LayoutMatrixUpdateDto
    {
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 200)]
        public int Rows { get; set; }

        [Range(1, 200)]
        public int Columns { get; set; }
    }
}
