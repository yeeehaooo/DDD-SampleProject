# 錯誤代碼使用指南

## 概述

本專案使用統一的錯誤代碼系統，所有錯誤代碼定義在 `ErrorCodes` 類別中。使用錯誤代碼可以：
- ✅ 避免硬編碼字串
- ✅ 提供 IntelliSense 支援
- ✅ 確保錯誤代碼一致性
- ✅ 方便維護和擴展
- ✅ 便於前端統一處理錯誤

## 錯誤代碼結構

錯誤代碼按功能模組分組，使用巢狀靜態類別組織：

```csharp
ErrorCodes
├── BadRequest          // 400 錯誤
├── Validation          // 驗證錯誤
├── Argument            // 參數錯誤
├── Domain              // 領域錯誤
├── Operation           // 操作錯誤
├── NotFound            // 404 錯誤
├── Conflict            // 409 錯誤
├── Unauthorized        // 401 錯誤
├── Forbidden           // 403 錯誤
├── InternalServer      // 500 錯誤
├── Business            // 業務邏輯錯誤
└── Pagination          // 分頁錯誤
```

## 使用方式

### 在 Controller 中使用

```csharp
using SampleProject.Api.Helpers;

// 方式 1：使用錯誤代碼字串（自動包含描述）
return ApiResponseHelper.NotFound(
    $"Product with ProductId {productId} not found",
    ErrorCodes.NotFound.ProductNotFound);
// 回應會自動包含：
// {
//   "error": {
//     "code": "PRODUCT_NOT_FOUND",
//     "description": "產品不存在"
//   }
// }

// 方式 2：使用 ErrorCodeInfo（明確取得 Code 和 Description）
var errorCode = ErrorCodes.GetErrorCodeInfo(ErrorCodes.NotFound.ProductNotFound);
return ApiResponseHelper.NotFound("產品不存在", errorCode);

// 方式 3：手動指定描述（覆蓋預設描述）
return ApiResponseHelper.BadRequest(
    "Insufficient inventory",
    ErrorCodes.Business.InsufficientInventory,
    "自訂描述：庫存數量不足，無法完成操作");

// 業務邏輯錯誤
return ApiResponseHelper.BadRequest(
    "Insufficient inventory",
    ErrorCodes.Business.InsufficientInventory);

// 衝突錯誤
return ApiResponseHelper.Conflict(
    "SkuCode already exists",
    ErrorCodes.Conflict.SkuCodeExists);

// 分頁錯誤
return ApiResponseHelper.BadRequest(
    "Page number must be greater than 0",
    ErrorCodes.Pagination.InvalidPageNumber);
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

### 在 ExceptionHandlingMiddleware 中使用

Middleware 已自動使用錯誤代碼：

```csharp
// 自動映射
ArgumentNullException → ErrorCodes.Argument.ArgumentNull
DomainException → ErrorCodes.Domain.DomainError
KeyNotFoundException → ErrorCodes.NotFound.General
```

## 錯誤代碼列表

### BadRequest（400）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.BadRequest.General` | 一般請求錯誤 | 預設錯誤代碼 |
| `ErrorCodes.BadRequest.InvalidInput` | 無效輸入 | 輸入格式錯誤 |
| `ErrorCodes.BadRequest.InvalidParameter` | 無效參數 | 參數值不正確 |
| `ErrorCodes.BadRequest.MissingRequiredField` | 缺少必填欄位 | 必填欄位為空 |

### Validation（400）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.Validation.ValidationFailed` | 驗證失敗 | FluentValidation 失敗 |
| `ErrorCodes.Validation.FieldRequired` | 欄位必填 | 必填欄位驗證 |
| `ErrorCodes.Validation.FieldTooLong` | 欄位過長 | 長度驗證 |
| `ErrorCodes.Validation.InvalidValue` | 無效值 | 值驗證失敗 |

### NotFound（404）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.NotFound.General` | 資源不存在 | 預設 404 錯誤 |
| `ErrorCodes.NotFound.ProductNotFound` | 產品不存在 | 查詢產品時 |
| `ErrorCodes.NotFound.SkuNotFound` | SKU 不存在 | 查詢 SKU 時 |
| `ErrorCodes.NotFound.StorageNotFound` | 倉庫不存在 | 查詢倉庫時 |

### Conflict（409）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.Conflict.General` | 資源衝突 | 預設衝突錯誤 |
| `ErrorCodes.Conflict.ResourceExists` | 資源已存在 | 建立重複資源 |
| `ErrorCodes.Conflict.SkuCodeExists` | SKU 代碼已存在 | SKU 代碼重複 |
| `ErrorCodes.Conflict.ProductNameExists` | 產品名稱已存在 | 產品名稱重複 |

### Business（400）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.Business.InsufficientInventory` | 庫存不足 | 庫存操作時 |
| `ErrorCodes.Business.InvalidQuantity` | 無效數量 | 數量驗證 |
| `ErrorCodes.Business.PriceMismatch` | 價格不匹配 | 價格比較 |
| `ErrorCodes.Business.CurrencyMismatch` | 貨幣不匹配 | 貨幣比較 |

### Pagination（400）

| 錯誤代碼 | 說明 | 使用場景 |
|---------|------|---------|
| `ErrorCodes.Pagination.InvalidPageNumber` | 無效頁碼 | 頁碼驗證 |
| `ErrorCodes.Pagination.InvalidPageSize` | 無效頁面大小 | 頁面大小驗證 |

## 擴展錯誤代碼

當需要新增錯誤代碼時：

1. 在 `ErrorCodes` 類別中新增常數
2. 使用描述性名稱
3. 遵循命名規則（大寫字母和底線）
4. 按功能模組分組

**範例：**

```csharp
public static class ErrorCodes
{
    public static class Business
    {
        // 新增錯誤代碼
        public const string ProductOutOfStock = "PRODUCT_OUT_OF_STOCK";
        public const string InvalidDiscount = "INVALID_DISCOUNT";
    }
}
```

## 最佳實踐

1. **優先使用特定錯誤代碼**
   ```csharp
   // ✅ 好的做法
   return ApiResponseHelper.NotFound(
       "Product not found",
       ErrorCodes.NotFound.ProductNotFound);

   // ❌ 避免使用
   return ApiResponseHelper.NotFound(
       "Product not found",
       "NOT_FOUND");
   ```

2. **錯誤訊息要清晰**
   ```csharp
   // ✅ 好的做法
   return ApiResponseHelper.BadRequest(
       "Page number must be greater than 0",
       ErrorCodes.Pagination.InvalidPageNumber);

   // ❌ 避免使用
   return ApiResponseHelper.BadRequest(
       "Error",
       ErrorCodes.Pagination.InvalidPageNumber);
   ```

3. **錯誤代碼要一致**
   - 相同類型的錯誤使用相同的錯誤代碼
   - 避免為相同錯誤建立多個錯誤代碼

4. **文件化錯誤代碼**
   - 在錯誤代碼常數上方添加 XML 註解
   - 說明使用場景和範例

## 前端處理建議

前端可以根據錯誤代碼進行統一處理：

```typescript
// TypeScript 範例
switch (error.code) {
  case 'PRODUCT_NOT_FOUND':
    showMessage('產品不存在');
    break;
  case 'INSUFFICIENT_INVENTORY':
    showMessage('庫存不足');
    break;
  case 'VALIDATION_ERROR':
    showValidationErrors(error.errors);
    break;
  default:
    showMessage('發生錯誤，請稍後再試');
}
```

## 總結

使用統一的錯誤代碼系統可以：
- 提高程式碼可維護性
- 減少拼寫錯誤
- 提供更好的開發體驗
- 便於前端統一處理錯誤
- 方便錯誤追蹤和分析
