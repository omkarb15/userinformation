using Microsoft.EntityFrameworkCore;

namespace UserInformation.Model
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }

        public DbSet<AssetHobby> AssetHobbies { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        public DbSet<QuestionOpt> QuestionOpts { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<UserOption> UserOptions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Tree> Trees { get; set; }
        public DbSet<TreeDragDrop> TreeDragDrops { get; set; }
        public DbSet<CheckBoxTree> CheckBoxTrees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleData> SalesData { get; set; }
        public DbSet<StackChart> StackCharts { get; set; }
        //public DbSet<StackChartDto> stackChartDto { get; set; } do not use dto as dbset


        protected override void OnModelCreating(ModelBuilder modelBuilder) //it is only used for projection
        {
            // Mark StackChartDto as keyless
            modelBuilder.Entity<StackChartDto>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<PieChart> PieCharts { get; set; }
        public DbSet<SankeyFlow>SankeyFlows { get; set; }

        public DbSet<ColumnLine> ColumnLines { get; set; }


    }
}
