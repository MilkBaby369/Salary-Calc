using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UiDesktopApp2.Models;

public partial class EsolutionDatabaseContext : DbContext
{
    public EsolutionDatabaseContext()
    {
    }

    public EsolutionDatabaseContext(DbContextOptions<EsolutionDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EsolutionPopulation> EsolutionPopulations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost; Port=5433; Database=ESolutionDatabase;Username=postgres;Password=dlwlgus1004@");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EsolutionPopulation>(entity =>
        {
            entity.HasKey(e => e.EmployeeNumber).HasName("Esolution_population_pkey");

            entity.ToTable("ESolution_population");

            entity.Property(e => e.EmployeeNumber)
                .ValueGeneratedNever()
                .HasColumnName("employee_number");
            entity.Property(e => e.Basepay).HasColumnName("basepay");
            entity.Property(e => e.DateE)
                .HasColumnType("character varying")
                .HasColumnName("date_e");
            entity.Property(e => e.DateS)
                .HasColumnType("character varying")
                .HasColumnName("date_s");
            entity.Property(e => e.Department)
                .HasColumnType("character varying")
                .HasColumnName("department");
            entity.Property(e => e.EmploymentInsurance).HasColumnName("employment_insurance");
            entity.Property(e => e.EmploymentInsuranceYearEndSettlement).HasColumnName("employment_insurance_year_end_settlement");
            entity.Property(e => e.HealthInsurance).HasColumnName("health_insurance");
            entity.Property(e => e.IncomeTax).HasColumnName("income_tax");
            entity.Property(e => e.LastPaymentDate).HasColumnName("last_payment_date");
            entity.Property(e => e.LoanPayment).HasColumnName("loan_payment");
            entity.Property(e => e.LocalIncomeTax).HasColumnName("local_income_tax");
            entity.Property(e => e.LongTermCareInsurance).HasColumnName("long_term_Care_Insurance ");
            entity.Property(e => e.MealExpenses).HasColumnName("meal_expenses");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.NationalPensionPlan).HasColumnName("national_pension_plan");
            entity.Property(e => e.NationalPensionRetroactive).HasColumnName("national_pension_retroactive");
            entity.Property(e => e.OvertimePay).HasColumnName("overtime_pay");
            entity.Property(e => e.PerformancePay).HasColumnName("performance_pay");
            entity.Property(e => e.Position)
                .HasMaxLength(25)
                .HasColumnName("position");
            entity.Property(e => e.RDExpenses).HasColumnName("R&D_expenses");
            entity.Property(e => e.SelfDrivingSubsidy).HasColumnName("self_driving_subsidy");
            entity.Property(e => e.TotalDeductions).HasColumnName("total_deductions");
            entity.Property(e => e.TotalPayment).HasColumnName("total_payment");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
