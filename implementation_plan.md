# Kế hoạch sửa lỗi tính toán thời gian Quy trình (BookingProcedure Duration)

## 1. Tổng quan vấn đề & Nguyên nhân gốc rễ (Root Cause)

### Vấn đề:
Khi người dùng đặt lịch 1 món dịch vụ/mẫu móng (`BookingItem`) bao gồm nhiều tùy chọn (`NailVariant`, `ShapeMethodConfig`, `CustomerNailRequest`, `Service`), thời gian của `BookingItem` được tính bằng tổng thời gian các tùy chọn đó.
Tuy nhiên, khi sinh ra các bước quy trình `BookingProcedure`:
1. **Lệch thời gian quy trình so với tổng thời gian BookingItem**: Code cũ lấy nguyên thời gian tĩnh mặc định trong Catalog (`x.Procedure.Duration ?? 0`) cho từng bước (ví dụ Catalog gồm 4 bước x 10p = 40p). Trong khi đó, mẫu móng (Variant/CustomNail) có thể có thời gian thợ ấn định hoặc tổng thời gian món đó chỉ là 10p. Kết quả là tổng thời gian quy trình (40p) bị chênh lệch lớn so với tổng thời gian đơn đặt lịch (10p).
2. **Bị bỏ sót bước quy trình khi là Combo**: Trong `DuplicateProceduresForBookingItemAsync` (`BookingProcedureService.cs`), code sử dụng cấu trúc `if (NailVariantId) ... else if (ServiceId) ... else if (CustomerNailRequestId)`. Vì dùng `if...else if`, nếu 1 `BookingItem` có cả `NailVariantId` và `ShapeMethodConfigId` hoặc `ServiceId`, hệ thống chỉ sinh quy trình cho `NailVariantId` và **bỏ qua hoàn toàn** các bước quy trình của `ShapeMethodConfig` và `Service`!

---

## 2. Các thay đổi đề xuất (Proposed Changes)

### Component: Application - BookingProcedureService
#### [MODIFY] [BookingProcedureService.cs](file:///c:/Users/Lenovo/Desktop/Capstone/Nailify-BE/Nailify.Capstone.Application/Services/BookingProcedureService.cs)
- Thay đổi hàm `DuplicateProceduresForBookingItemAsync(BookingItem item)`:
  1. **Xử lý nối tiếp các phần của Combo (Combo support)**: Thay thế `if...else if` bằng các khối kiểm tra độc lập (`if (item.NailVariantId.HasValue)`, `if (item.CustomerNailRequestId.HasValue)`, `if (item.ShapeMethodConfigId.HasValue)`, `if (item.ServiceId.HasValue)`). Tăng dần `StepOrder` để sinh đầy đủ tất cả quy trình cho từng thành phần trong món.
  2. **Tỷ lệ hóa thời gian (Proportional Scaling)**:
     - Lấy thời gian mục tiêu của mẫu thiết kế $D_{target}$ (từ `item.Duration` hoặc thời gian do thợ thẩm định/variant ấn định).
     - Lấy tổng thời gian catalog $D_{cat} = \sum P_{i, catalog}$.
     - Nếu $D_{cat} > 0$ và $D_{target} > 0$, tính hệ số tỷ lệ $k = \frac{D_{target}}{D_{cat}}$.
     - Tính lại thời gian cho từng bước:
       - $d_{i, new} = \text{Math.Max}(1, \text{Math.Round}(d_{i, cat} \times k))$
       - $a_{i, new} = \text{Math.Min}(d_{i, new}, \text{Math.Max}(1, \text{Math.Round}(a_{i, cat} \times k)))$
       - $p_{i, new} = \text{Math.Max}(0, d_{i, new} - a_{i, new})$
     - Điều chỉnh vi phân ở bước cuối cùng để tổng $\sum d_{i, new}$ bằng chính xác $D_{target}$.

---

### Component: Application - BookingSchedulingService
#### [MODIFY] [BookingSchedulingService.cs](file:///c:/Users/Lenovo/Desktop/Capstone/Nailify-BE/Nailify.Capstone.Application/Services/BookingSchedulingService.cs)
- Cập nhật hàm `GenerateMockBookingProceduresAsync`:
  - Áp dụng công thức tỷ lệ hóa tương tự cho danh sách mock procedures khi giả lập xếp lịch / kiểm tra xung đột / tính thời gian chờ hàng chờ WalkIn.

---

## 3. Kế hoạch kiểm thử & Xác minh (Verification Plan)

### Kế hoạch kiểm thử thủ công (Manual Verification)
1. **Kiểm tra mẫu móng có thời gian rút ngắn/thay đổi:**
   - Đặt 1 mẫu móng có tổng thời gian 10 phút, trong khi catalog quy trình mẫu đó gồm 4 bước mặc định (40 phút).
   - Kiểm tra kết quả trong DB: Hệ thống sinh ra 4 bước quy trình có tổng thời gian đúng bằng **10 phút** (mỗi bước ~2-3 phút).
2. **Kiểm tra Combo (NailVariant + ShapeMethodConfig + Service):**
   - Đặt 1 BookingItem bao gồm cả `NailVariantId`, `ShapeMethodConfigId`, và `ServiceId`.
   - Kiểm tra danh sách `BookingProcedure`: Tất cả các bước của Mẫu nail, Dáng móng, và Dịch vụ đi kèm đều được sinh ra đầy đủ theo đúng thứ tự `StepOrder`.
