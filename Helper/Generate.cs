using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace backend.Helper
{
    public class Generate
    {
        public string GenerateSlug(string input)
        {
            // Loại bỏ khoảng trắng ở đầu và cuối chuỗi
            string slug = input.Trim().ToLower();

            // Chuyển đổi các ký tự có dấu thành không dấu
            slug = RemoveAccent(slug);

            // Xóa các ký tự không phải chữ hoặc số
            slug = Regex.Replace(slug, @"[^\p{L}0-9\s-]", "");

            // Thay thế các khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-");

            // Loại bỏ các dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }
public string GenerateOrderCode()
        {
            // Lấy thời gian hiện tại
            DateTime currentTime = DateTime.Now;

            // Tạo mã đơn hàng từ ngày, tháng, năm, giờ, phút và giây
            string orderCode = currentTime.ToString("yyyyMMddHHmmss");

            return orderCode;
        }

       private string RemoveAccent(string text)
{
    var normalizedString = text.Normalize(NormalizationForm.FormD);
    var stringBuilder = new StringBuilder();

    foreach (var c in normalizedString)
    {
        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        {
            if (c == 'đ' || c == 'Đ')
            {
                stringBuilder.Append('d');
            }
            else
            {
                stringBuilder.Append(c);
            }
        }
    }

    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
}

    }
}
