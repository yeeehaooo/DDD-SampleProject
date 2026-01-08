# SampleProject API

## 專案概述

這是一個基於 .NET 8 的 Web API 專案，採用以下架構模式：

- **DDD (Domain-Driven Design)** - 輕量級領域驅動設計
- **Clean Architecture** - 乾淨架構（實用主義，非學術派）
- **CQRS** - 命令查詢職責分離（在需要時使用）
- **自定義 Mediator** - 不使用 MediatR 依賴

## 專案結構

```
src/
├── SampleProject.Domain/          # 領域層（核心業務邏輯）
├── SampleProject.Application/     # 應用層（用例編排）
├── SampleProject.Infrastructure/  # 基礎設施層（持久化、外部服務）
└── SampleProject.Api/             # API 層（入口點）

tests/
├── SampleProject.Domain.Tests/
└── SampleProject.Application.Tests/
```

## 技術棧

- .NET 8
- ASP.NET Core Web API
- Dapper (資料庫存取)
- SQL Server
- Redis (快取)
- Serilog (日誌記錄)
- FluentValidation
- Swagger/OpenAPI
- xUnit (測試)

## 架構圖

詳細架構圖請參考：[Diagrams/Architecture.drawio](./Diagrams/Architecture.drawio)

## 快速開始

### 前置需求

- .NET 8 SDK
- SQL Server (或 LocalDB)
- Redis Server
- Visual Studio 2022 或 VS Code

### 設定步驟

1. 還原 NuGet 套件
   ```bash
   dotnet restore
   ```

2. 更新連線字串（appsettings.json）
   - 資料庫連線字串
   - Redis 連線字串

3. 啟動 Redis Server
   ```bash
   # Windows (如果使用 Redis for Windows)
   redis-server

   # 或使用 Docker
   docker run -d -p 6379:6379 redis
   ```

4. 建立資料庫和資料表
   - 執行 SQL 腳本：`Docs/Scripts/CreateDatabase.sql`
   - 或使用 SQL Server Management Studio 執行腳本

5. 執行專案
   ```bash
   cd src/SampleProject.Api
   dotnet run
   ```

6. 開啟 Swagger UI
   - 瀏覽器開啟：`https://localhost:5001/swagger`

## 日誌記錄

專案使用 Serilog 進行日誌記錄：

- **開發環境**：日誌寫入到 `logs/log-{date}.txt` 檔案
- **生產環境**：可設定同時寫入檔案和 Seq（在 `appsettings.Production.json` 中啟用）

### 啟用 Seq（可選）

1. 安裝 Seq：https://datalust.co/seq
2. 更新 `appsettings.Production.json` 中的 Seq 設定
3. 重新啟動應用程式

詳細設定請參考 `appsettings.Production.json` 中的 Serilog 設定。

## Redis 快取

專案已整合 Redis 快取功能，使用方式請參考：[Redis_Usage_Example.md](./Redis_Usage_Example.md)

## API 端點

### Products

- `GET /api/products` - 取得所有產品
- `GET /api/products/{id}` - 取得單一產品
- `GET /api/products/page/{pageNumber}/{pageSize}` - 分頁取得產品
- `POST /api/products` - 建立產品
- `PUT /api/products/{id}` - 更新產品
- `DELETE /api/products/{id}` - 刪除產品

## 設計原則

### 領域層 (Domain)

- 包含業務實體和值物件
- 實體包含業務不變量驗證
- 不依賴基礎設施或框架

### 應用層 (Application)

- 編排用例
- 不包含業務規則
- 透過介面與領域層溝通
- 使用自定義 Mediator 進行命令/查詢分發

### 基礎設施層 (Infrastructure)

- 實作持久化（Dapper）
- 實作 Repository 介面
- 實作 Redis 快取服務
- 可替換，不影響業務邏輯

### API 層

- 使用 Minimal API 或 Controller
- 統一例外處理
- 請求日誌記錄
- Swagger 文件

## 測試

執行所有測試：
```bash
dotnet test
```

執行特定測試專案：
```bash
dotnet test tests/SampleProject.Domain.Tests
```

## 注意事項

- API 設計為無狀態，支援水平擴展
- 所有依賴透過依賴注入管理
- 業務邏輯集中在領域層
- 使用 FluentValidation 進行輸入驗證

## 授權

[您的授權資訊]
