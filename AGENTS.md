# AGENTS

Hướng dẫn cho mọi AI/coding agent làm việc trong repository này.

## Bắt buộc đọc trước khi code

1. `MEMORY.md`
2. `system_rules.md`
3. File liên quan trong `docs/`
4. Code thật trong module sắp sửa

## Nguyên tắc làm việc

- Dự án là ASP.NET Core MVC + EF Core + SQL Server.
- Schema gốc nằm ở `btl_table.sql`.
- Không đoán tên bảng/cột/trạng thái. Luôn đối chiếu database/entity.
- Không hard-code connection string, mật khẩu, token.
- Không commit `appsettings.Development.json`, `.env`, `.vs/`, `bin/`, `obj/`.
- Giữ giao diện và thông báo bằng tiếng Việt.
- Sau khi sửa code, chạy `dotnet build` nếu có thể.

## Tài liệu nhanh

- Schema: `docs/database_schema.md`
- Luồng nghiệp vụ: `docs/business_flows.md`
- Backlog module: `docs/feature_backlog.md`
- Checklist làm việc với AI: `docs/ai_workflow.md`
