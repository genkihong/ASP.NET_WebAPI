using Rocket.Common;

namespace Rocket.Models
{
  public class PatchMember : AbstractPatchStateRequest<PatchMember, Member>
  {
    public PatchMember()
    {
      AddPatchStateMapping(x => x.Name);
      AddPatchStateMapping(x => x.Password);
      AddPatchStateMapping(x => x.Email);
      AddPatchStateMapping(x => x.Identity);
      AddPatchStateMapping(x => x.Phone);
      AddPatchStateMapping(x => x.Address);
      AddPatchStateMapping(x => x.Photo);
      AddPatchStateMapping(x => x.Reply);
      AddPatchStateMapping(x => x.StoreImage);
      AddPatchStateMapping(x => x.StoreDescription);
    }
    //public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Identity { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Photo { get; set; }
    public string Reply { get; set; }
    public string StoreImage { get; set; }
    public string StoreDescription { get; set; }
  }
}