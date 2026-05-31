# QuizMaker - C# WPF Quiz Management System

## Giới thiệu

QuizMaker là ứng dụng desktop được xây dựng bằng C# WPF, dùng để quản lý và làm bài thi trắc nghiệm. Ứng dụng hỗ trợ ba vai trò chính: Admin, Teacher và Student.

Các chức năng chính:

* Admin quản lý tài khoản người dùng.
* Teacher tạo và quản lý bài kiểm tra.
* Teacher có thể import câu hỏi từ file Word `.docx`.
* Student xem danh sách bài kiểm tra, làm bài và xem kết quả.
* Dữ liệu được lưu cục bộ thông qua file JSON.

---

## Công nghệ sử dụng

* Ngôn ngữ: C#
* Framework: .NET 8
* Giao diện: WPF
* IDE khuyến nghị: Visual Studio 2022
* Quản lý source code: GitHub
* Lưu trữ dữ liệu: JSON

---

## Cấu trúc thư mục

```text
QuizMaker/
├── QuizMaker.sln
└── QuizMaker/
    ├── QuizMaker.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── AssemblyInfo.cs
    ├── Models/
    │   └── Models.cs
    ├── Services/
    │   ├── DatabaseService.cs
    │   └── DocxParser.cs
    └── Views/
        ├── LoginWindow.xaml
        ├── LoginWindow.xaml.cs
        ├── AdminWindow.xaml
        ├── AdminWindow.xaml.cs
        ├── TeacherWindow.xaml
        ├── TeacherWindow.xaml.cs
        ├── StudentWindow.xaml
        ├── StudentWindow.xaml.cs
        ├── QuizExecutionWindow.xaml
        └── QuizExecutionWindow.xaml.cs
```

---

## Hướng dẫn chạy project

### Cách 1: Chạy bằng Visual Studio

1. Mở Visual Studio.
2. Chọn `Open a project or solution`.
3. Mở file `QuizMaker.sln`.
4. Chọn cấu hình `Debug`.
5. Nhấn `F5` hoặc nút `Start` để chạy chương trình.

### Cách 2: Chạy bằng terminal

Tại thư mục chứa file `QuizMaker.sln`, chạy:

```bash
dotnet restore
dotnet build
dotnet run --project QuizMaker/QuizMaker.csproj
```

---

## Định dạng file Word để import câu hỏi

Khi Teacher import câu hỏi từ file `.docx`, mỗi câu hỏi cần được viết trên một dòng theo định dạng:

```text
Tiêu đề|Nội dung câu hỏi|Đáp án A|Đáp án B|Đáp án C|Đáp án D|Đáp án đúng
```

Ví dụ:

```text
Câu 1|C# là gì?|Ngôn ngữ lập trình|Hệ điều hành|Trình duyệt|Phần cứng|A
Câu 2|Từ khóa khai báo lớp trong C# là gì?|class|struct|new|void|A
```

---

## Phân công công việc

| Thành viên   | Branch                            | Module phụ trách               | File chính                                                                                               | Nội dung đóng góp                                                             |
| ------------ | --------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| Lê Thế Vinh | `feature/login-register`          | Login/Register + Project Setup | `QuizMaker.sln`, `QuizMaker.csproj`, `App.xaml`, `LoginWindow.xaml`, `LoginWindow.xaml.cs`, `README.md`  | Upload bộ khung project, xử lý đăng nhập/đăng ký, viết hướng dẫn chạy project |
| Nguyễn Đức Lâm | `feature/admin-user-management`   | Admin + Database + Models      | `AdminWindow.xaml`, `AdminWindow.xaml.cs`, `Models.cs`, `DatabaseService.cs`                             | Quản lý tài khoản, xử lý dữ liệu dùng chung, kiểm soát model và database      |
| Vũ Nguyên Thông | `feature/teacher-quiz-management` | Teacher + Word Import          | `TeacherWindow.xaml`, `TeacherWindow.xaml.cs`, `DocxParser.cs`                                           | Tạo bài kiểm tra, quản lý câu hỏi, import câu hỏi từ Word                     |
| Mai Phước Trí | `feature/student-quiz-execution`  | Student + Quiz Execution       | `StudentWindow.xaml`, `StudentWindow.xaml.cs`, `QuizExecutionWindow.xaml`, `QuizExecutionWindow.xaml.cs` | Hiển thị bài kiểm tra, làm bài, tính điểm, lưu và xem kết quả                 |

---

## Quy trình làm việc GitHub

Nhóm sử dụng các branch riêng cho từng module:

```text
main
├── feature/login-register
├── feature/admin-user-management
├── feature/teacher-quiz-management
└── feature/student-quiz-execution
```

Quy trình:

1. Mỗi thành viên làm việc trên branch được giao.
2. Chỉ chỉnh sửa các file thuộc module của mình.
3. Commit với nội dung rõ ràng.
4. Tạo Pull Request từ branch cá nhân vào `main`.
5. Nhóm trưởng kiểm tra và merge Pull Request.
6. Branch `main` chỉ chứa phiên bản ổn định cuối cùng.

---

## Quy tắc commit

Commit message nên ngắn gọn và mô tả rõ nội dung đã làm.

Ví dụ:

```text
Add project setup and login module
Add admin user management module
Add database service and models
Add teacher quiz management and DOCX parser
Add student quiz execution module
Fix score calculation after submission
Update README instructions
```

Không nên dùng commit message không rõ nội dung như:

```text
update
fix
abc
final
sửa code
```

---

## Không upload lên GitHub

Không upload các thư mục/file sau:

```text
.vs/
bin/
obj/
*.user
*.zip
database.json
```

Các file này là file tạm, file build hoặc dữ liệu cục bộ, không cần đưa lên repository.

---

## Ghi chú

Project được thực hiện cho môn Kỹ thuật lập trình nâng cao. Mục tiêu của project là xây dựng một ứng dụng quản lý bài thi trắc nghiệm, đồng thời sử dụng GitHub để theo dõi quá trình đóng góp của từng thành viên trong nhóm.
