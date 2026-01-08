namespace SampleProject.Domain.Entities;

public class SkuSpecification
{
    public int SkuId { get; private set; }
    public int SpecificationValueId { get; private set; }

    // 私有建構函式（用於 Dapper）
    private SkuSpecification() { }

    // 公開建構函式（業務邏輯）
    public SkuSpecification(int skuId, int specificationValueId)
    {
        SkuId = skuId;
        SpecificationValueId = specificationValueId;
    }
}
