namespace SampleProject.Api.Helpers;

/// <summary>
/// 統一的錯誤代碼定義
///
/// 用途：
/// 1. 集中管理所有錯誤代碼，避免硬編碼字串
/// 2. 提供 IntelliSense 支援，減少拼寫錯誤
/// 3. 方便維護和擴展
/// 4. 確保錯誤代碼的一致性
/// 5. 提供錯誤代碼的描述資訊
///
/// 命名規則：
/// - 使用大寫字母和底線
/// - 按功能模組分組
/// - 使用描述性名稱
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// 通用錯誤代碼（400 Bad Request）
    /// </summary>
    public static class BadRequest
    {
        public const string General = "BAD_REQUEST";
        public const string InvalidInput = "INVALID_INPUT";
        public const string InvalidParameter = "INVALID_PARAMETER";
        public const string MissingRequiredField = "MISSING_REQUIRED_FIELD";
        public const string InvalidFormat = "INVALID_FORMAT";
    }

    /// <summary>
    /// 驗證錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Validation
    {
        public const string ValidationFailed = "VALIDATION_ERROR";
        public const string FieldRequired = "FIELD_REQUIRED";
        public const string FieldTooLong = "FIELD_TOO_LONG";
        public const string FieldTooShort = "FIELD_TOO_SHORT";
        public const string InvalidValue = "INVALID_VALUE";
        public const string ValueOutOfRange = "VALUE_OUT_OF_RANGE";
        public const string InvalidFormat = "INVALID_FORMAT";
    }

    /// <summary>
    /// 參數錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Argument
    {
        public const string ArgumentNull = "ARGUMENT_NULL";
        public const string ArgumentInvalid = "ARGUMENT_INVALID";
        public const string ArgumentOutOfRange = "ARGUMENT_OUT_OF_RANGE";
    }

    /// <summary>
    /// 領域錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Domain
    {
        public const string DomainError = "DOMAIN_ERROR";
        public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
        public const string InvalidState = "INVALID_STATE";
        public const string ConstraintViolation = "CONSTRAINT_VIOLATION";
    }

    /// <summary>
    /// 操作錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Operation
    {
        public const string InvalidOperation = "INVALID_OPERATION";
        public const string OperationNotAllowed = "OPERATION_NOT_ALLOWED";
        public const string OperationFailed = "OPERATION_FAILED";
    }

    /// <summary>
    /// 資源錯誤代碼（404 Not Found）
    /// </summary>
    public static class NotFound
    {
        public const string General = "NOT_FOUND";
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string SkuNotFound = "SKU_NOT_FOUND";
        public const string StorageNotFound = "STORAGE_NOT_FOUND";
        public const string InventoryNotFound = "INVENTORY_NOT_FOUND";
        public const string SpecificationNotFound = "SPECIFICATION_NOT_FOUND";
        public const string SpecificationValueNotFound = "SPECIFICATION_VALUE_NOT_FOUND";
    }

    /// <summary>
    /// 衝突錯誤代碼（409 Conflict）
    /// </summary>
    public static class Conflict
    {
        public const string General = "CONFLICT";
        public const string ResourceExists = "RESOURCE_EXISTS";
        public const string DuplicateEntry = "DUPLICATE_ENTRY";
        public const string SkuCodeExists = "SKU_CODE_EXISTS";
        public const string ProductNameExists = "PRODUCT_NAME_EXISTS";
    }

    /// <summary>
    /// 授權錯誤代碼（401 Unauthorized）
    /// </summary>
    public static class Unauthorized
    {
        public const string General = "UNAUTHORIZED";
        public const string InvalidToken = "INVALID_TOKEN";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string AuthenticationRequired = "AUTHENTICATION_REQUIRED";
    }

    /// <summary>
    /// 權限錯誤代碼（403 Forbidden）
    /// </summary>
    public static class Forbidden
    {
        public const string General = "FORBIDDEN";
        public const string InsufficientPermissions = "INSUFFICIENT_PERMISSIONS";
        public const string AccessDenied = "ACCESS_DENIED";
    }

    /// <summary>
    /// 系統錯誤代碼（500 Internal Server Error）
    /// </summary>
    public static class InternalServer
    {
        public const string General = "INTERNAL_SERVER_ERROR";
        public const string DatabaseError = "DATABASE_ERROR";
        public const string ExternalServiceError = "EXTERNAL_SERVICE_ERROR";
        public const string ConfigurationError = "CONFIGURATION_ERROR";
        public const string UnexpectedError = "UNEXPECTED_ERROR";
    }

    /// <summary>
    /// 業務邏輯錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Business
    {
        public const string InsufficientInventory = "INSUFFICIENT_INVENTORY";
        public const string InvalidQuantity = "INVALID_QUANTITY";
        public const string PriceMismatch = "PRICE_MISMATCH";
        public const string CurrencyMismatch = "CURRENCY_MISMATCH";
        public const string ProductInUse = "PRODUCT_IN_USE";
        public const string CannotDeleteActiveProduct = "CANNOT_DELETE_ACTIVE_PRODUCT";
    }

    /// <summary>
    /// 分頁錯誤代碼（400 Bad Request）
    /// </summary>
    public static class Pagination
    {
        public const string InvalidPageNumber = "INVALID_PAGE_NUMBER";
        public const string InvalidPageSize = "INVALID_PAGE_SIZE";
        public const string PageNumberOutOfRange = "PAGE_NUMBER_OUT_OF_RANGE";
    }

    /// <summary>
    /// 取得錯誤代碼的描述
    /// </summary>
    public static string GetDescription(string code)
    {
        return _descriptions.TryGetValue(code, out var description)
            ? description
            : "Unknown error code";
    }

    /// <summary>
    /// 取得錯誤代碼資訊（包含 Code 和 Description）
    /// </summary>
    public static ErrorCodeInfo GetErrorCodeInfo(string code)
    {
        return new ErrorCodeInfo(code, GetDescription(code));
    }

    private static readonly Dictionary<string, string> _descriptions = new()
    {
        // BadRequest
        { BadRequest.General, "一般請求錯誤" },
        { BadRequest.InvalidInput, "無效的輸入資料" },
        { BadRequest.InvalidParameter, "無效的參數" },
        { BadRequest.MissingRequiredField, "缺少必填欄位" },
        { BadRequest.InvalidFormat, "格式不正確" },

        // Validation
        { Validation.ValidationFailed, "驗證失敗" },
        { Validation.FieldRequired, "欄位為必填" },
        { Validation.FieldTooLong, "欄位長度超過限制" },
        { Validation.FieldTooShort, "欄位長度不足" },
        { Validation.InvalidValue, "無效的值" },
        { Validation.ValueOutOfRange, "值超出允許範圍" },
        { Validation.InvalidFormat, "格式不正確" },

        // Argument
        { Argument.ArgumentNull, "參數不能為空" },
        { Argument.ArgumentInvalid, "參數無效" },
        { Argument.ArgumentOutOfRange, "參數超出範圍" },

        // Domain
        { Domain.DomainError, "領域層錯誤" },
        { Domain.BusinessRuleViolation, "違反業務規則" },
        { Domain.InvalidState, "無效的狀態" },
        { Domain.ConstraintViolation, "約束條件違反" },

        // Operation
        { Operation.InvalidOperation, "無效的操作" },
        { Operation.OperationNotAllowed, "操作不被允許" },
        { Operation.OperationFailed, "操作失敗" },

        // NotFound
        { NotFound.General, "資源不存在" },
        { NotFound.ProductNotFound, "產品不存在" },
        { NotFound.SkuNotFound, "SKU 不存在" },
        { NotFound.StorageNotFound, "倉庫不存在" },
        { NotFound.InventoryNotFound, "庫存記錄不存在" },
        { NotFound.SpecificationNotFound, "規格不存在" },
        { NotFound.SpecificationValueNotFound, "規格值不存在" },

        // Conflict
        { Conflict.General, "資源衝突" },
        { Conflict.ResourceExists, "資源已存在" },
        { Conflict.DuplicateEntry, "重複的項目" },
        { Conflict.SkuCodeExists, "SKU 代碼已存在" },
        { Conflict.ProductNameExists, "產品名稱已存在" },

        // Unauthorized
        { Unauthorized.General, "未授權" },
        { Unauthorized.InvalidToken, "無效的令牌" },
        { Unauthorized.TokenExpired, "令牌已過期" },
        { Unauthorized.AuthenticationRequired, "需要身份驗證" },

        // Forbidden
        { Forbidden.General, "禁止存取" },
        { Forbidden.InsufficientPermissions, "權限不足" },
        { Forbidden.AccessDenied, "存取被拒絕" },

        // InternalServer
        { InternalServer.General, "內部伺服器錯誤" },
        { InternalServer.DatabaseError, "資料庫錯誤" },
        { InternalServer.ExternalServiceError, "外部服務錯誤" },
        { InternalServer.ConfigurationError, "設定錯誤" },
        { InternalServer.UnexpectedError, "未預期的錯誤" },

        // Business
        { Business.InsufficientInventory, "庫存不足" },
        { Business.InvalidQuantity, "無效的數量" },
        { Business.PriceMismatch, "價格不匹配" },
        { Business.CurrencyMismatch, "貨幣不匹配" },
        { Business.ProductInUse, "產品正在使用中" },
        { Business.CannotDeleteActiveProduct, "無法刪除啟用的產品" },

        // Pagination
        { Pagination.InvalidPageNumber, "無效的頁碼" },
        { Pagination.InvalidPageSize, "無效的頁面大小" },
        { Pagination.PageNumberOutOfRange, "頁碼超出範圍" }
    };
}
