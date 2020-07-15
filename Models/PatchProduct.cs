using Rocket.Common;

namespace Rocket.Models
{
  public class PatchProduct: AbstractPatchStateRequest<PatchProduct, Product>
  {
    public PatchProduct()
    {
      AddPatchStateMapping(x => x.Name);
      AddPatchStateMapping(x => x.OriginPrice);
      AddPatchStateMapping(x => x.Price);
      AddPatchStateMapping(x => x.Deposit);
      AddPatchStateMapping(x => x.Quantity);
      AddPatchStateMapping(x => x.City);
      AddPatchStateMapping(x => x.Zone);
      AddPatchStateMapping(x => x.Store);
      AddPatchStateMapping(x => x.Status);
      AddPatchStateMapping(x => x.Category);
      AddPatchStateMapping(x => x.Period);
      AddPatchStateMapping(x => x.Description);
      AddPatchStateMapping(x => x.PublishDate);
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public int OriginPrice { get; set; }
    public int Price { get; set; }
    public int Deposit { get; set; }
    public int Quantity { get; set; }
    public string City { get; set; }
    public string Zone { get; set; }
    public string Store { get; set; }
    public ProductStatus Status { get; set; }
    public ProductCategory Category { get; set; }
    public string Period { get; set; }
    public string Description { get; set; }
    public string PublishDate { get; set; }
  }
}