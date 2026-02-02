using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Domain.Enums;

public enum SellerStatus
{
    Pending = 0,    // Chờ duyệt
    Active = 1,     // Đang hoạt động
    Inactive = 2,   // Tạm nghỉ/Ẩn gian hàng
    Suspended = 3,  // Bị đình chỉ (có thời hạn hoặc chờ xử lý vi phạm)
    Banned = 4,     // Cấm vĩnh viễn
    Rejected = 5    // Từ chối đơn đăng ký
}
