namespace Rocket.Models
{
  public enum ProductCategory
  {
    遊戲主機 = 0,
    遊戲配件 = 1,
    遊戲片 = 2
  }
  public enum ProductStatus
  {
    可出租 = 0,
    已出租 = 1
  }
  public enum Star
  {
    一星 = 1,
    二星 = 2,
    三星 = 3,
    四星 = 4,
    五星 = 5,
  }
  public enum OrderStatus
  {
    等待確認 = 0,
    訂單取消 = 1,
    已完成 = 2
  }
  public enum GenderType
  {
    女 = 0,
    男 = 1
  }
}