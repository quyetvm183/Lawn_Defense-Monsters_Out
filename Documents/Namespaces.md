# Namespace & lý do dùng `RGame`

Khái niệm:
- Namespace trong C# dùng để nhóm các lớp, tránh xung đột tên (name collisions) khi project có nhiều file hoặc khi import thư viện ngoài.

Tại sao project này dùng `namespace RGame`:
- Tổ chức: gói tất cả class game vào `RGame` để dễ nhận biết source từ game này.
- Tránh xung đột: nếu bạn import package bên thứ 3 có class `GameManager`, `MenuManager`... thì `RGame.GameManager` khác với `ThirdPartyNamespace.GameManager`.
- Đóng gói: dễ đọc trong code (`using RGame;` trên đầu file) và khi refactor.

Khi thêm script mới:
- Đặt trong `namespace RGame` nếu bạn muốn script trở thành 1 phần của codebase cùng namespace.
- Nếu script là tool/editor hoặc dùng chung cho nhiều project, bạn có thể tạo namespace riêng như `RGame.Tools` hoặc `RGame.Editor`.

Lưu ý khi sửa code:
- Khi đổi namespace, nhớ update file nơi import class (thêm `using <yournamespace>;`) hoặc dùng fully-qualified name `YourNamespace.YourClass`.
