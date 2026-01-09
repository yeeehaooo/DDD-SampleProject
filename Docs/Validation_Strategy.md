# 驗證策略說明

## 驗證層級架構

本專案採用**雙層驗證策略**，確保資料在進入領域層前後都經過驗證。

```
┌─────────────────────────────────────────┐
│  API Layer (Controller)                 │
│  └─ FluentValidation (早期驗證)        │ ← 第一層：輸入驗證
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│  Application Layer (Handler)            │
│  └─ 建立 Value Objects                 │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│  Domain Layer (Value Objects)           │
│  └─ 領域不變量驗證                     │ ← 第二層：領域驗證
└─────────────────────────────────────────┘
```

## 驗證職責劃分

### 1. FluentValidation（應用層）

**職責：**
- ✅ 早期輸入驗證（在進入 Handler 前）
- ✅ 提供結構化的 API 錯誤訊息
- ✅ 基本格式檢查（非空、長度、範圍等）
- ✅ 快速失敗，避免不必要的處理

**範例：**
```csharp
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Product name is required")
    .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");
```

**優點：**
- 在 Controller 層就能返回清晰的錯誤訊息
- 避免進入 Handler 後才發現明顯錯誤
- 提供更好的 API 使用體驗

### 2. Value Objects（領域層）

**職責：**
- ✅ **必須包含所有領域驗證**（不變量）
- ✅ 確保領域物件始終有效
- ✅ 封裝業務規則驗證
- ✅ 即使繞過 FluentValidation，仍能保護領域

**範例：**
```csharp
public ProductName(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new DomainException("Product name cannot be empty");

    if (value.Length > 200)
        throw new DomainException("Product name cannot exceed 200 characters");

    Value = value;
}
```

**優點：**
- 領域層自保護，無法建立無效的 Value Object
- 驗證邏輯集中在 Value Object 中，易於維護
- 符合 DDD 原則：領域物件保護自己的不變量

## 驗證流程範例

### 建立 Product 的驗證流程

```
1. API Controller 接收 CreateProductCommand
   ↓
2. FluentValidation 驗證（早期檢查）
   ├─ Name 非空？✅
   ├─ Name 長度 ≤ 200？✅
   ├─ BasePrice ≥ 0？✅
   └─ 如果失敗 → 返回 400 BadRequest
   ↓
3. Handler 建立 Product Entity
   ↓
4. Product 建構函式建立 ProductName Value Object
   ↓
5. ProductName 建構函式驗證（領域驗證）
   ├─ Name 非空？✅
   ├─ Name 長度 ≤ 200？✅
   └─ 如果失敗 → 拋出 DomainException
   ↓
6. Product 建構函式建立 Money Value Object
   ↓
7. Money 建構函式驗證（領域驗證）
   ├─ Amount ≥ 0？✅
   ├─ Currency 非空？✅
   └─ 如果失敗 → 拋出 DomainException
   ↓
8. 驗證通過，建立 Product
```

## 為什麼需要雙層驗證？

### 場景 1：正常 API 呼叫
- FluentValidation 先檢查，提供清晰的錯誤訊息
- 如果通過，Value Objects 再次驗證（防禦性編程）

### 場景 2：內部呼叫（繞過 API）
- 如果直接建立 Value Objects（例如測試、背景任務）
- Value Objects 仍會驗證，保護領域完整性

### 場景 3：資料庫映射（Dapper）
- Dapper 透過 Type Handler 建立 Value Objects
- Value Objects 驗證確保從資料庫讀取的資料有效

## 驗證規則一致性

**重要原則：** FluentValidation 和 Value Objects 的驗證規則必須一致！

### ✅ 正確做法

```csharp
// FluentValidation
RuleFor(x => x.Name)
    .MaximumLength(200);

// Value Object
if (value.Length > 200)
    throw new DomainException("Product name cannot exceed 200 characters");
```

### ❌ 錯誤做法

```csharp
// FluentValidation 允許 300 字元
RuleFor(x => x.Name)
    .MaximumLength(300);

// Value Object 只允許 200 字元
if (value.Length > 200)
    throw new DomainException("Product name cannot exceed 200 characters");
// 這會導致不一致！
```

## 最佳實踐建議

### 1. 驗證規則集中管理

考慮建立常數類別來管理驗證規則：

```csharp
public static class ProductValidationRules
{
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;
    public const decimal MinBasePrice = 0;
}
```

### 2. FluentValidation 可選簡化

如果 Value Objects 已經有完整的驗證，FluentValidation 可以只做最基本的檢查：

```csharp
// 簡化版本：只檢查非空，詳細驗證交給 Value Objects
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Product name is required");
    // 長度檢查由 ProductName Value Object 處理
```

### 3. 錯誤訊息一致性

確保兩層驗證的錯誤訊息一致或互補：

```csharp
// FluentValidation：用戶友好的訊息
"Product name is required"

// Value Object：技術性訊息（用於日誌）
"Product name cannot be empty"
```

## 總結

| 驗證層級 | 職責 | 必須性 | 時機 |
|---------|------|--------|------|
| **FluentValidation** | 早期輸入驗證 | 建議 | API 請求時 |
| **Value Objects** | 領域不變量驗證 | **必須** | 建立 Value Object 時 |

**核心原則：**
- ✅ Value Objects 的驗證是**必須的**（領域保護）
- ✅ FluentValidation 的驗證是**建議的**（更好的 API 體驗）
- ✅ 兩者規則必須**一致**
- ✅ Value Objects 是最終防線，即使繞過 FluentValidation 也能保護領域
