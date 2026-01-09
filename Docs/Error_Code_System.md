# 錯誤代碼系統說明

## 概述

本專案使用統一的錯誤代碼系統，每個錯誤代碼都包含：
- **Code**：錯誤代碼字串（如 `PRODUCT_NOT_FOUND`）
- **Description**：錯誤代碼的描述（如 `產品不存在`）

## 回應格式

### 錯誤回應結構

所有錯誤回應都包含 `code` 和 `description`：

```json
{
  "success": false,
  "error": {
    "message": "Product with ProductId {id} not found",
    "code": "PRODUCT_NOT_FOUND",
    "description": "產品不存在"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 驗證錯誤回應

```json
{
  "success": false,
  "error": {
    "message": "Validation failed",
    "code": "VALIDATION_ERROR",
    "description": "驗證失敗"
  },
  "errors": [
    {
      "field": "Name",
      "message": "Product name is required",
      "attemptedValue": null
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 錯誤代碼結構

### ErrorCodes 類別

所有錯誤代碼定義在 `ErrorCodes` 類別中，按功能模組分組：

```csharp
ErrorCodes
├── BadRequest          // 400 錯誤
├── Validation          // 驗證錯誤
├── Argument            // 參數錯誤
├── Domain              // 領域錯誤
├── NotFound            // 404 錯誤
├── Conflict            // 409 錯誤
├── Business            // 業務邏輯錯誤
└── ... (其他分類)
```

### 描述字典

所有錯誤代碼的描述定義在 `_descriptions` 字典中，使用繁體中文描述。

## 使用方式

### 方式 1：使用錯誤代碼常數（推薦）

```csharp
// 自動包含描述
return ApiResponseHelper.NotFound(
    $"Product with ProductId {productId} not found",
    ErrorCodes.NotFound.ProductNotFound);
```

**回應：**
```json
{
  "error": {
    "code": "PRODUCT_NOT_FOUND",
    "description": "產品不存在"
  }
}
```

### 方式 2：使用 ErrorCodeInfo

```csharp
var errorCode = ErrorCodes.GetErrorCodeInfo(ErrorCodes.NotFound.ProductNotFound);
return ApiResponseHelper.NotFound("產品不存在", errorCode);
```

### 方式 3：手動指定描述（覆蓋預設）

```csharp
return ApiResponseHelper.BadRequest(
    "Insufficient inventory",
    ErrorCodes.Business.InsufficientInventory,
    "自訂描述：庫存數量不足");
```

### 取得錯誤代碼描述

```csharp
// 取得描述
var description = ErrorCodes.GetDescription(ErrorCodes.NotFound.ProductNotFound);
// 結果: "產品不存在"

// 取得完整的錯誤代碼資訊
var errorInfo = ErrorCodes.GetErrorCodeInfo(ErrorCodes.NotFound.ProductNotFound);
// errorInfo.Code = "PRODUCT_NOT_FOUND"
// errorInfo.Description = "產品不存在"
```

## 錯誤代碼列表

### NotFound（404）

| Code | Description |
|------|-------------|
| `PRODUCT_NOT_FOUND` | 產品不存在 |
| `SKU_NOT_FOUND` | SKU 不存在 |
| `STORAGE_NOT_FOUND` | 倉庫不存在 |
| `INVENTORY_NOT_FOUND` | 庫存記錄不存在 |

### Business（400）

| Code | Description |
|------|-------------|
| `INSUFFICIENT_INVENTORY` | 庫存不足 |
| `INVALID_QUANTITY` | 無效的數量 |
| `PRICE_MISMATCH` | 價格不匹配 |
| `CURRENCY_MISMATCH` | 貨幣不匹配 |

### Conflict（409）

| Code | Description |
|------|-------------|
| `SKU_CODE_EXISTS` | SKU 代碼已存在 |
| `PRODUCT_NAME_EXISTS` | 產品名稱已存在 |
| `RESOURCE_EXISTS` | 資源已存在 |

### Validation（400）

| Code | Description |
|------|-------------|
| `VALIDATION_ERROR` | 驗證失敗 |
| `FIELD_REQUIRED` | 欄位為必填 |
| `FIELD_TOO_LONG` | 欄位長度超過限制 |

## 擴展錯誤代碼

當需要新增錯誤代碼時：

1. 在對應的分類中新增常數
2. 在 `_descriptions` 字典中新增描述

**範例：**

```csharp
public static class Business
{
    // 1. 新增錯誤代碼常數
    public const string ProductOutOfStock = "PRODUCT_OUT_OF_STOCK";
}

// 2. 在 _descriptions 字典中新增描述
private static readonly Dictionary<string, string> _descriptions = new()
{
    // ...
    { Business.ProductOutOfStock, "產品已缺貨" }
};
```

## 優勢

1. **自動包含描述**：使用錯誤代碼常數時，描述會自動加入回應
2. **集中管理**：所有錯誤代碼和描述集中管理
3. **類型安全**：使用常數避免拼寫錯誤
4. **易於維護**：修改描述只需更新字典
5. **前端友好**：前端可以根據 code 和 description 進行統一處理

## 前端處理建議

前端可以根據 `code` 和 `description` 進行統一處理：

```typescript
// TypeScript 範例
interface ApiError {
  message: string;
  code: string;
  description: string;
}

function handleError(error: ApiError) {
  // 使用 code 進行邏輯判斷
  switch (error.code) {
    case 'PRODUCT_NOT_FOUND':
      showMessage(error.description); // "產品不存在"
      break;
    case 'INSUFFICIENT_INVENTORY':
      showMessage(error.description); // "庫存不足"
      break;
    default:
      showMessage(error.description || error.message);
  }
}
```

## 總結

錯誤代碼系統現在包含：
- ✅ **Code**：錯誤代碼字串
- ✅ **Description**：錯誤代碼的描述
- ✅ 自動映射：使用錯誤代碼常數時自動包含描述
- ✅ 集中管理：所有錯誤代碼和描述統一管理
- ✅ 易於擴展：新增錯誤代碼只需兩步
