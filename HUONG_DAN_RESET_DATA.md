# Hướng dẫn sử dụng tính năng Reset Data

## Tổng quan
Tính năng Reset Data cho phép người chơi xóa toàn bộ dữ liệu game và bắt đầu lại từ đầu.

## Cách sử dụng

### Trong Unity Editor
1. Mở scene Menu
2. Chạy game
3. Vào Settings
4. Nhấn nút "Reset Data"
5. Một dialog xác nhận sẽ xuất hiện
6. Chọn "Xóa dữ liệu" để xác nhận hoặc "Hủy" để hủy bỏ

### Trong Game Build
1. Mở game
2. Vào Settings
3. Nhấn nút "Reset Data" lần đầu
   - Text button sẽ chuyển thành màu đỏ: "Nhấn lại để xác nhận!"
4. Nhấn nút "Reset Data" lần thứ 2 trong vòng 3 giây
   - Game sẽ xóa toàn bộ dữ liệu và reload scene

**Lưu ý:** Nếu bạn không nhấn lần 2 trong vòng 3 giây, button sẽ reset về trạng thái ban đầu.

## Setup trong Unity

### Bước 1: Kiểm tra MainMenuHomeScene component
Đảm bảo GameObject "HomeMenu" trong scene Menu có component `MainMenuHomeScene` được gắn.

### Bước 2: Gắn Reset Button Text (Optional nhưng khuyến khích)
1. Mở scene Menu
2. Chọn GameObject "HomeMenu"
3. Trong Inspector, tìm component `MainMenuHomeScene`
4. Kéo Text component của nút Reset vào field "Reset Button Text"
   - Path: Settings/Panel/Reset data/Text

### Bước 3: Kiểm tra Button Event
1. Chọn nút "Reset data" trong Hierarchy
2. Trong Inspector, kiểm tra component Button
3. Đảm bảo OnClick event đã được setup:
   - Target: HomeMenu (MainMenuHomeScene)
   - Function: MainMenuHomeScene.ResetData

## Dữ liệu được xóa
Khi reset data, các thông tin sau sẽ bị xóa:
- Coins (tiền)
- Level đã đạt được
- Số sao của từng level
- Số lượng items (Double Arrow, Triple Arrow, Poison, Freeze)
- Upgrade Wall
- Remove Ads status
- Tất cả PlayerPrefs khác

## Kỹ thuật Implementation

### Cơ chế bảo vệ
- **Unity Editor**: Sử dụng `EditorUtility.DisplayDialog` để hiển thị dialog xác nhận
- **Game Build**: Yêu cầu double-click trong vòng 3 giây để tránh xóa nhầm

### Code Structure
```csharp
// File: MainMenuHomeScene.cs

// Method công khai được gọi từ Button
public void ResetData()

// Method hiển thị cảnh báo (coroutine)
IEnumerator ShowResetWarning()

// Method thực hiện reset
private void PerformReset()
```

### Dependencies
- `GameMode.Instance.ResetDATA()` - Method chính để reset data
- `PlayerPrefs.DeleteAll()` - Fallback nếu GameMode không có
- `SoundManager.Click()` - Play sound khi nhấn nút

## Troubleshooting

### Nút Reset không hoạt động
1. Kiểm tra Button OnClick event đã được setup đúng chưa
2. Kiểm tra GameObject "HomeMenu" có component MainMenuHomeScene không
3. Kiểm tra GameMode.Instance có tồn tại không

### Text không đổi màu khi nhấn lần đầu
1. Kiểm tra field "Reset Button Text" trong MainMenuHomeScene đã được gắn chưa
2. Nếu chưa gắn, chức năng vẫn hoạt động nhưng không có visual feedback

### Game không reload sau khi reset
1. Kiểm tra GameMode.Instance.ResetDATA() có tồn tại không
2. Kiểm tra SceneManager có được import đúng không

## Best Practices

1. **Luôn test trong Build**: Behavior trong Editor và Build khác nhau
2. **Gắn Reset Button Text**: Cải thiện UX bằng visual feedback
3. **Backup data trước khi test**: Tránh mất data quan trọng
4. **Customize delay time**: Có thể thay đổi `resetClickDelay` từ 3 giây sang giá trị khác

## Future Improvements

Một số cải tiến có thể thực hiện:
1. Tạo UI popup confirmation thay vì sử dụng text button
2. Thêm animation cho warning message
3. Thêm sound effect đặc biệt cho warning
4. Lưu backup data trước khi xóa
5. Cho phép restore data từ backup
