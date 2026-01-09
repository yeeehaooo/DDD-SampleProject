# API 回應格式說明

## 統一回應結構

本專案使用統一的 API 回應格式，確保所有端點的回應結構一致，便於前端處理和維護。

## 回應格式

### 成功回應

#### 有資料的成功回應（200 OK）

```json
{
  "success": true,
  "data": {
    // 實際資料內容
  },
  "message": "操作成功", // 可選
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**範例：**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "productId": "550e8400-e29b-41d4-a716-446655440000",
    "name": "iPhone 15",
    "description": "最新款 iPhone",
    "basePrice": 32900.00
  },
  "message": "Product retrieved successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### 建立成功回應（201 Created）

```json
{
  "success": true,
  "data": {
    // 建立的資源資料
  },
  "message": "Resource created successfully", // 可選
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### 無內容回應（204 No Content）

HTTP 204 No Content，無回應主體。

### 錯誤回應

#### 一般錯誤回應（400 Bad Request）

```json
{
  "success": false,
  "error": {
    "message": "錯誤訊息",
    "code": "ERROR_CODE",
    "description": "錯誤代碼的描述" // 自動從 ErrorCodes 取得
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**範例：**
```json
{
  "success": false,
  "error": {
    "message": "Page number and page size must be greater than 0",
    "code": "INVALID_PAGE_NUMBER",
    "description": "無效的頁碼"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### 驗證錯誤回應（400 Bad Request）

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
    },
    {
      "field": "BasePrice",
      "message": "BasePrice cannot be negative",
      "attemptedValue": -100
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### 未找到回應（404 Not Found）

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

#### 內部伺服器錯誤（500 Internal Server Error）

```json
{
  "success": false,
  "error": {
    "message": "An error occurred while processing your request",
    "code": "INTERNAL_SERVER_ERROR",
    "description": "內部伺服器錯誤"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 錯誤代碼

所有錯誤代碼定義在 `ErrorCodes` 類別中，按功能模組分組。使用錯誤代碼常數可以：
- 避免硬編碼字串
- 提供 IntelliSense 支援
- 確保錯誤代碼一致性
- 方便維護和擴展

### 錯誤代碼分類

#### 通用錯誤（400 Bad Request）
- `ErrorCodes.BadRequest.General` - 一般請求錯誤
- `ErrorCodes.BadRequest.InvalidInput` - 無效輸入
- `ErrorCodes.BadRequest.InvalidParameter` - 無效參數
- `ErrorCodes.BadRequest.MissingRequiredField` - 缺少必填欄位

#### 驗證錯誤（400 Bad Request）
- `ErrorCodes.Validation.ValidationFailed` - 驗證失敗
- `ErrorCodes.Validation.FieldRequired` - 欄位必填
- `ErrorCodes.Validation.FieldTooLong` - 欄位過長
- `ErrorCodes.Validation.InvalidValue` - 無效值

#### 參數錯誤（400 Bad Request）
- `ErrorCodes.Argument.ArgumentNull` - 參數為空
- `ErrorCodes.Argument.ArgumentInvalid` - 參數無效
- `ErrorCodes.Argument.ArgumentOutOfRange` - 參數超出範圍

#### 領域錯誤（400 Bad Request）
- `ErrorCodes.Domain.DomainError` - 領域層錯誤
- `ErrorCodes.Domain.BusinessRuleViolation` - 業務規則違反
- `ErrorCodes.Domain.InvalidState` - 無效狀態

#### 資源錯誤（404 Not Found）
- `ErrorCodes.NotFound.General` - 資源不存在
- `ErrorCodes.NotFound.ProductNotFound` - 產品不存在
- `ErrorCodes.NotFound.SkuNotFound` - SKU 不存在
- `ErrorCodes.NotFound.StorageNotFound` - 倉庫不存在

#### 衝突錯誤（409 Conflict）
- `ErrorCodes.Conflict.General` - 資源衝突
- `ErrorCodes.Conflict.ResourceExists` - 資源已存在
- `ErrorCodes.Conflict.DuplicateEntry` - 重複項目
- `ErrorCodes.Conflict.SkuCodeExists` - SKU 代碼已存在

#### 業務邏輯錯誤（400 Bad Request）
- `ErrorCodes.Business.InsufficientInventory` - 庫存不足
- `ErrorCodes.Business.InvalidQuantity` - 無效數量
- `ErrorCodes.Business.PriceMismatch` - 價格不匹配

#### 系統錯誤（500 Internal Server Error）
- `ErrorCodes.InternalServer.General` - 內部伺服器錯誤
- `ErrorCodes.InternalServer.DatabaseError` - 資料庫錯誤
- `ErrorCodes.InternalServer.ExternalServiceError` - 外部服務錯誤

### 使用範例

```csharp
// 使用特定錯誤代碼
return ApiResponseHelper.NotFound(
    $"Product with ProductId {productId} not found",
    ErrorCodes.NotFound.ProductNotFound);

// 使用業務邏輯錯誤代碼
return ApiResponseHelper.BadRequest(
    "Insufficient inventory",
    ErrorCodes.Business.InsufficientInventory);

// 使用衝突錯誤代碼
return ApiResponseHelper.Conflict(
    "SkuCode already exists",
    ErrorCodes.Conflict.SkuCodeExists);
```

## 使用方式

### 在 Controller 中使用

```csharp
using SampleProject.Api.Helpers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    // 成功回應
    return ApiResponseHelper.Ok(data);
    return ApiResponseHelper.Ok(data, "操作成功");

    // 建立回應
    return ApiResponseHelper.Created(data, actionName, routeValues, "建立成功");

    // 錯誤回應（自動包含描述）
    return ApiResponseHelper.BadRequest("錯誤訊息", ErrorCodes.BadRequest.General);
    return ApiResponseHelper.NotFound("資源不存在", ErrorCodes.NotFound.ProductNotFound);

    // 使用 ErrorCodeInfo（包含 Code 和 Description）
    var errorCode = ErrorCodes.GetErrorCodeInfo(ErrorCodes.NotFound.ProductNotFound);
    return ApiResponseHelper.NotFound("產品不存在", errorCode);

    // 驗證錯誤（自動包含描述）
    return ApiResponseHelper.BadRequest(validationResult);
}
```

## 優點

1. **一致性**：所有 API 端點使用相同的回應格式
2. **可預測性**：前端可以統一處理回應
3. **易於除錯**：錯誤訊息和代碼清晰明確
4. **時間戳記**：所有回應包含時間戳記，便於追蹤
5. **驗證錯誤詳情**：驗證錯誤提供詳細的欄位級別資訊

## 注意事項

1. **204 No Content**：刪除操作使用 204，無回應主體
2. **時間戳記**：所有回應自動包含 UTC 時間戳記
3. **錯誤代碼**：建議使用標準錯誤代碼，便於前端處理
4. **訊息本地化**：錯誤訊息可以根據需求進行本地化
