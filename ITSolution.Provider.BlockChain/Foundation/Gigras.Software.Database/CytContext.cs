using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Database;

public partial class CytContext : DbContext
{
    public CytContext(DbContextOptions<CytContext> options) : base(options)
    {
    }

    public virtual DbSet<ITCountry> Countries { get; set; }
    public virtual DbSet<ITAdmin> DynamicAdmins { get; set; }
    public virtual DbSet<ITUser> DynamicUsers { get; set; }
    public virtual DbSet<ITCity> Cities { get; set; }
    public virtual DbSet<ITState> States { get; set; }
    public DbSet<Form> DynamicForms { get; set; }
    public DbSet<FormsSection> DynamicFormsSections { get; set; }
    public DbSet<FormField> DynamicFormFields { get; set; }
    public DbSet<FieldType> DynamicFieldTypes { get; set; }
    public DbSet<FieldOption> DynamicFieldOptions { get; set; }
    public DbSet<FieldOptionValue> DynamicFieldOptionValues { get; set; }
    public DbSet<FieldValidation> DynamicFieldValidations { get; set; }
    public DbSet<FieldTypeValidation> DynamicFieldTypeValidations { get; set; }
    public DbSet<DynamicCtrl> DynamicCtrl { get; set; }
    public DbSet<DynamicValidation> DynamicValidation { get; set; }
    public DbSet<DynamicBlockChainField> DynamicBlockChainField { get; set; }
    public DbSet<DynamicUserData> DynamicUserData { get; set; }
    public DbSet<SmartContractAbi> SmartContractAbi { get; set; }
    public DbSet<SmartContractAddress> SmartContractAddress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder.Entity<FieldType>()
                .Property(e => e.Steps)
                .HasColumnType("DECIMAL(18, 4)");  // Configure precision and scale
            // Your model configuration code here
            base.OnModelCreating(modelBuilder);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine("Error in OnModelCreating: " + ex.Message);
            // Optionally rethrow the exception or handle accordingly
            throw;
        }
    }
}