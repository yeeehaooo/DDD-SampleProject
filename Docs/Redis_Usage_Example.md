# Redis 快取使用範例

## 基本使用

在 Handler 中注入 `IRedisCacheService` 來使用 Redis 快取：

```csharp
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;
    private readonly IRedisCacheService _cache;

    public GetProductByIdQueryHandler(
        IProductRepository repository,
        IRedisCacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ProductDto?> HandleAsync(
        GetProductByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        // 嘗試從快取取得
        var cacheKey = $"product:{request.Id}";
        var cachedProduct = await _cache.GetAsync<ProductDto>(cacheKey, cancellationToken);

        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        // 從資料庫取得
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return null;

        var productDto = new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CreatedAt,
            product.UpdatedAt);

        // 存入快取（5 分鐘過期）
        await _cache.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(5), cancellationToken);

        return productDto;
    }
}
```

## 清除快取

當資料更新時，記得清除相關快取：

```csharp
public async Task<ProductDto> HandleAsync(
    UpdateProductCommand request,
    CancellationToken cancellationToken = default)
{
    // ... 更新邏輯 ...

    // 清除快取
    var cacheKey = $"product:{request.Id}";
    await _cache.RemoveAsync(cacheKey, cancellationToken);

    return productDto;
}
```

## 快取方法說明

- `GetAsync<T>(string key)` - 取得快取值
- `SetAsync<T>(string key, T value, TimeSpan? expiration)` - 設定快取值（可選過期時間）
- `RemoveAsync(string key)` - 移除指定鍵
- `ExistsAsync(string key)` - 檢查鍵是否存在
