using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace GrandElementApi.Data
{
    public class ApplicationContext : DbContext
    {
        public static readonly ILoggerFactory _consoleLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarCategory> CarCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<DeliveryContact> DeliveryContacts { get; set; }
        public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierProduct> SuppliersProducts { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<User> Users{ get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<PartRequest> PartRequests { get; set; }
        public ApplicationContext()
        {
            //Database.EnsureCreated();
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseLoggerFactory(_consoleLoggerFactory);
            optionsBuilder.UseNpgsql(conn);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnCarCategoryModelCreating(modelBuilder);
            OnProductModelCreating(modelBuilder);
            OnCarModelCreating(modelBuilder);
            OnDeliveryContactCreating(modelBuilder);
            OnDeliveryAddressCreating(modelBuilder);
            OnClientCreating(modelBuilder);
            OnSupplierCreating(modelBuilder);
            OnSupplierProductCreating(modelBuilder);
            OnRequestCreating(modelBuilder);
            OnPartRequestCreating(modelBuilder);
            OnUserCreating(modelBuilder);
            OnSessionCreating(modelBuilder);
        }
        protected void OnCarModelCreating(ModelBuilder mb) {

            mb.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.HasComment("Перевозчики");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarCategoryId)
                    .HasColumnName("car_category_id")
                    .HasComment("Категория машины");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("character varying")
                    .HasComment("Примечания к водителю");

                entity.Property(e => e.Contacts)
                    .HasColumnName("contacts")
                    .HasColumnType("character varying")
                    .HasComment("Контактные данные водителя");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.FreightPrice)
                    .HasColumnName("freight_price")
                    .HasColumnType("numeric");

                entity.Property(e => e.Owner)
                    .HasColumnName("owner")
                    .HasColumnType("character varying")
                    .HasComment("Владелец автомобиля");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasColumnType("character varying")
                    .HasComment("Еденица измерения");

                entity.Property(e => e.Vat)
                    .HasColumnName("vat")
                    .HasComment("НДС(value added tax). 1-включен; 0-невключен");

                entity.HasOne(d => d.CarCategory)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.CarCategoryId)
                    .HasConstraintName("cars_car_categories_id_fk");
            });
        }
        protected void OnCarCategoryModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarCategory>()
                .ToTable("car_categories");
            modelBuilder.Entity<CarCategory>()
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<CarCategory>()
                .Property(c => c.Name).HasColumnName("name");
        }
        protected void OnProductModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .ToTable("products");
            modelBuilder.Entity<Product>()
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Product>()
                .Property(c => c.Name).HasColumnName("name");
        }
        protected void OnDeliveryContactCreating(ModelBuilder mb) {

            mb.Entity<DeliveryContact>(entity =>
            {
                entity.ToTable("delivery_contacts");

                entity.HasIndex(e => e.Id)
                    .HasName("delivery_contacts_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Communication)
                    .HasColumnName("communication")
                    .HasColumnType("character varying")
                    .HasComment("Способ связи");

                entity.Property(e => e.DeliveryAddressId).HasColumnName("delivery_address_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.DeliveryAddress)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.DeliveryAddressId)
                    .HasConstraintName("delivery_contacts_delivery_address_id_fk");
            });
        }
        protected void OnDeliveryAddressCreating(ModelBuilder mb)
        {

            mb.Entity<DeliveryAddress>(entity =>
            {
                entity.ToTable("delivery_address");

                entity.HasComment("адрес доставки");

                entity.HasIndex(e => e.Id)
                    .HasName("delivery_address_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying")
                    .HasComment("Адрес");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("delivery_address_clients_id_fk");
            });
        }
        protected void OnClientCreating(ModelBuilder mb)
        {

            mb.Entity<Client>(entity =>
            {
                entity.ToTable("clients");

                entity.HasIndex(e => e.Id)
                    .HasName("clients_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

            });

        }
        protected void OnSupplierCreating(ModelBuilder mb) {
            mb.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");

                entity.HasComment("Поставщики");

                entity.HasIndex(e => e.Id)
                    .HasName("suppliers_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("character varying");

                entity.Property(e => e.LegalEntity)
                    .HasColumnName("legal_entity")
                    .HasColumnType("character varying")
                    .HasComment("Юридическое лицо");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Vat)
                    .HasColumnName("vat")
                    .HasComment("НДС; 1-включен в стоимость; 0-нет");
            });
        }
        protected void OnSupplierProductCreating(ModelBuilder mb) {
            mb.Entity<SupplierProduct>(entity =>
            {
                entity.ToTable("supplier_product");

                entity.HasIndex(e => e.Id)
                    .HasName("supplier_product_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasComment("Товары поставщиков");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("supplier_product_products_id_fk");

                entity.HasOne(d => d.Supplier)
                    .WithMany(x=>x.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("supplier_product_suppliers_id_fk");
            });
        }
        protected void OnRequestCreating(ModelBuilder mb)
        {

            mb.Entity<Request>(entity =>
            {
                entity.ToTable("requests");

                entity.HasComment("Заявки");

                entity.HasIndex(e => e.DeliveryStart)
                    .HasName("requests_delivery_start_index");

                entity.HasIndex(e => e.Id)
                    .HasName("requests_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric")
                    .HasComment("Заказанный объем");

                entity.Property(e => e.AmountComplete)
                    .HasColumnName("amount_complete")
                    .HasDefaultValueSql("0")
                    .HasComment("вывезенный объем");

                entity.Property(e => e.AmountIn)
                    .HasColumnName("amount_in")
                    .HasColumnType("numeric");

                entity.Property(e => e.AmountOut)
                    .HasColumnName("amount_out")
                    .HasColumnType("numeric")
                    .HasComment("Количество товара");

                entity.Property(e => e.CarCategoryId).HasColumnName("car_category_id");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.CarVat)
                    .HasColumnName("car_vat")
                    .HasDefaultValueSql("1")
                    .HasComment("1-ндс вкл, 0-ндс выкл");

                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("character varying");

                entity.Property(e => e.DeliveryAddressId).HasColumnName("delivery_address_id");

                entity.Property(e => e.DeliveryEnd)
                    .HasColumnName("delivery_end")
                    .HasColumnType("date");

                entity.Property(e => e.DeliveryStart)
                    .HasColumnName("delivery_start")
                    .HasColumnType("date");

                entity.Property(e => e.FreightCost)
                    .HasColumnName("freight_cost")
                    .HasColumnType("numeric")
                    .HasComment("Стоимость доставки. freight_price * amount");

                entity.Property(e => e.Status)
                     .HasColumnName("status")
                     .HasColumnType("numeric");

                entity.Property(e => e.FreightPrice)
                    .HasColumnName("freight_price")
                    .HasColumnType("numeric")
                    .HasComment("Цена перевозки за еденицу(час, объем и тд). ");

                entity.Property(e => e.IsLong)
                    .HasColumnName("is_long")
                    .HasComment("Признак долгосрочного заказа");

                entity.Property(e => e.ManagerId).HasColumnName("manager_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Profit)
                    .HasColumnName("profit")
                    .HasColumnType("numeric")
                    .HasComment("Прибыль. amount * (selling_price - purchase_price - freight_price)");

                entity.Property(e => e.PurchasePrice)
                    .HasColumnName("purchase_price")
                    .HasColumnType("numeric")
                    .HasComment("Цена закупки товара");

                entity.Property(e => e.Reward)
                    .HasColumnName("reward")
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SellingCost)
                    .HasColumnName("selling_cost")
                    .HasColumnType("numeric");

                entity.Property(e => e.SellingPrice)
                    .HasColumnName("selling_price")
                    .HasColumnType("numeric")
                    .HasComment("Цена продажи товара");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.SupplierVat)
                    .HasColumnName("supplier_vat")
                    .HasComment("1-ндс вкл, 0-ндс выкл");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasColumnType("character varying")
                    .HasComment("Единица измерения");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("requests_clients_id_fk");

                entity.HasOne(d => d.DeliveryAddress)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.DeliveryAddressId)
                    .HasConstraintName("requests_delivery_address_id_fk");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("requests_products_id_fk");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("requests_suppliers_id_fk");

                entity.HasOne(d => d.Manager)
                    .WithMany(m => m.Requests)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("requests_users_id_fk");
            });

        }
        protected void OnPartRequestCreating(ModelBuilder mb)
        {

            mb.Entity<PartRequest>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.ToTable("part_requests");

                entity.HasComment("Соответствие долгосрочных заявок");

                entity.Property(e => e.ChildRequestId)
                    .HasColumnName("child_request_id")
                    .HasComment("Поражденная заявка");

                entity.Property(e => e.ParentRequestId)
                    .HasColumnName("parent_request_id")
                    .HasComment("Долгосрочная заявка");

                entity.HasOne(d => d.ChildRequest)
                    .WithMany(r=>r.Parts)
                    .HasForeignKey(d => d.ChildRequestId)
                    .HasConstraintName("part_requests_requests_id_fk_2");

                entity.HasOne(d => d.ParentRequest)
                    .WithMany()
                    .HasForeignKey(d => d.ParentRequestId)
                    .HasConstraintName("part_requests_requests_id_fk");
            });
        }
        protected void OnUserCreating(ModelBuilder mb)
        {
            mb.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasComment("Пользователи");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasColumnType("character varying");

                entity.Property(e => e.Pass)
                    .HasColumnName("pass")
                    .HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });
        }
        protected void OnSessionCreating(ModelBuilder mb)
        {
            mb.Entity<Session>(entity =>
            {
                entity.ToTable("sessions");

                entity.HasComment("Сессии");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("character varying");

                entity.Property(e => e.LoginDate)
                    .HasColumnName("login_date")
                    .HasColumnType("timestamp without time zone");

                entity.Property(e => e.LogoutDate)
                    .HasColumnName("logout_date")
                    .HasColumnType("timestamp without time zone");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Sessions)
                    .HasForeignKey(e=>e.UserId)
                    .HasConstraintName("sessions_users_id_fk");
            });
        }
    }
}
