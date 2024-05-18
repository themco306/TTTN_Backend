using System.Text;
using backend.Models;

public static class TableUtils
{
    public static string CreateHtmlTable(Order orderData,string link)
    {
        StringBuilder tableHtml = new StringBuilder();
        tableHtml.AppendLine("<table border=\"1\">");
        tableHtml.AppendLine("<thead>");
        tableHtml.AppendLine("<tr>");
        tableHtml.AppendLine("<th>STT</th>");
        tableHtml.AppendLine("<th>Mã sản phẩm</th>");
        tableHtml.AppendLine("<th>Tên sản phẩm</th>");
        tableHtml.AppendLine("<th>Số lượng</th>");
        tableHtml.AppendLine("<th>Đơn giá</th>");
        tableHtml.AppendLine("<th>Thành tiền</th>");
        tableHtml.AppendLine("</tr>");
        tableHtml.AppendLine("</thead>");
        tableHtml.AppendLine("<tbody>");

        foreach (var detail in orderData.OrderDetails)
        {
            tableHtml.AppendLine("<tr>");
            tableHtml.AppendLine($"<td>{detail.Id}</td>");
            tableHtml.AppendLine($"<td>{detail.ProductId}</td>");
            tableHtml.AppendLine($"<td>{detail.Product.Name}</td>");
            tableHtml.AppendLine($"<td>{detail.Quantity}</td>");
            tableHtml.AppendLine($"<td>{detail.Price.ToString("F2")}</td>");
            tableHtml.AppendLine($"<td>{detail.TotalPrice.ToString("F2")}</td>");
            tableHtml.AppendLine("</tr>");
        }

        tableHtml.AppendLine("</tbody>");
        tableHtml.AppendLine("<tfoot>");
        tableHtml.AppendLine($"<tr><td colspan=\"4\">Tổng tiền:</td><td colspan=\"2\">{orderData.Total.ToString("F2")}</td></tr>");
        tableHtml.AppendLine($"<tr><td colspan=\"4\">Ghi chú:</td><td colspan=\"2\">{orderData.Note}</td></tr>");
        tableHtml.AppendLine($"<tr><td colspan=\"4\">Phương thức thanh toán:</td><td colspan=\"2\">{orderData.PaymentType}</td></tr>");
        tableHtml.AppendLine($"<tr><td colspan=\"4\">Địa chỉ giao hàng:</td><td colspan=\"2\">{orderData.OrderInfo.DeliveryAddress}, {orderData.OrderInfo.DeliveryWard}, {orderData.OrderInfo.DeliveryDistrict}, {orderData.OrderInfo.DeliveryProvince}</td></tr>");
        tableHtml.AppendLine($"<tr><td colspan=\"6\">Xác nhận email của bạn, Vui lòng xác nhận email của bạn bằng cách nhấp vào liên kết này: <a href=\"{link}\">liên kết này</a></td></tr>");
        tableHtml.AppendLine("</tfoot>");
        tableHtml.AppendLine("</table>");

        return tableHtml.ToString();
    }
}