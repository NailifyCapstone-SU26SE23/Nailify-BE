namespace Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs
{
    /// <summary>
    /// Dữ liệu để thêm một phương án trả lời cho câu hỏi Quiz.
    /// optionValues là mảng string — truyền nhiều giá trị qua query: ?optionValues=val1&amp;optionValues=val2
    /// </summary>
    public class AddQuizOptionRequestDTO
    {
        public string Label { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> OptionValues { get; set; } = new();
    }
}
