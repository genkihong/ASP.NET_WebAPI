﻿namespace Rocket.Models
{
  using System.Data.Entity;

  public class FirstModel : DbContext
  {
    // 您的內容已設定為使用應用程式組態檔 (App.config 或 Web.config)
    // 中的 'FirstModel' 連接字串。根據預設，這個連接字串的目標是
    // 您的 LocalDb 執行個體上的 'Rocket.Models.FirstModel' 資料庫。
    // 
    // 如果您的目標是其他資料庫和 (或) 提供者，請修改
    // 應用程式組態檔中的 'FirstModel' 連接字串。
    public FirstModel()
        : base("name=FirstModel")
    {
    }

    // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
    // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Evaluation> Evaluations { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
  }
}