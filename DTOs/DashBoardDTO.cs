namespace backend.DTOs
{
    public class GetNewCustomerDTO
    {
        public int Today { get; set; }           // Số lượng mới hôm nay
        public double PercentIncreaseToday { get; set; }  // Phần trăm tăng so với ngày hôm qua
        public int ThisMonth { get; set; }       // Số lượng mới trong tháng này
        public double PercentIncreaseMonth { get; set; }  // Phần trăm tăng so với tháng trước
        public int ThisYear { get; set; }        // Số lượng mới trong năm nay
    }
        public class DatetimeQueryDTO
    {
        public DateTimeOffset StartDate {get;set;}
        public DateTimeOffset EndDate {get;set;}
    }
    public class OrderSummaryDTO
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

}
