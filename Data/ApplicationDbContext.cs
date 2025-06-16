using EHRsystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EHRsystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet definitions for all your entities (ensure all entities with collections are listed)
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<LabTest> LabTests { get; set; } = null!;
        public DbSet<TestResult> TestResults { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<UserAuditLog> UserAuditLogs { get; set; } = null!;
        // DbSets for entities referenced in Patient's collections (if they are separate tables)
        public DbSet<LabResult> LabResults { get; set; } = null!;
        public DbSet<MedicalHistory> MedicalHistories { get; set; } = null!;
        public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
        public DbSet<MedicalFile> MedicalFiles { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method from IdentityDbContext
            base.OnModelCreating(modelBuilder);

            // Configure Doctor-User relationship
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Assuming ApplicationUser doesn't have a specific collection for Doctors
                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(d => d.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Patient-User relationship
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId); // Patient primary key is PatientId

                // Assuming ApplicationUser doesn't have a specific collection for Patients
                entity.HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<Patient>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Appointment relationships
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relationship with Doctor
                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments) // Explicitly point to Doctor's Appointments collection
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Patient
                entity.HasOne(a => a.Patient)
                    .WithMany(p => p.Appointments) // Explicitly point to Patient's Appointments collection
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Enum conversion to string for database storage
                entity.Property(a => a.Status)
                    .HasConversion<string>();

                entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Prescription relationships
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relationship with Doctor
                entity.HasOne(p => p.Doctor)
                    .WithMany(d => d.Prescriptions) // Explicitly point to Doctor's Prescriptions collection
                    .HasForeignKey(p => p.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Patient
                entity.HasOne(p => p.Patient)
                    .WithMany(p => p.Prescriptions) // Explicitly point to Patient's Prescriptions collection
                    .HasForeignKey(p => p.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure LabTest
            modelBuilder.Entity<LabTest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(lt => lt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(lt => lt.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure TestResult relationships
            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relationship with LabTest
                entity.HasOne(t => t.LabTest)
                    .WithMany() // Assuming LabTest does NOT have a collection for TestResults
                                // If LabTest DOES have a collection (e.g., ICollection<TestResult> Results),
                                // change to .WithMany(lt => lt.Results)
                    .HasForeignKey(t => t.LabTestId) // <-- CORRECTED: Changed from TestResultId to LabTestId
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Patient
                entity.HasOne(t => t.Patient)
                    .WithMany(p => p.TestResults) // Explicitly point to Patient's TestResults collection
                    .HasForeignKey(t => t.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Doctor who ordered the test
                entity.HasOne(t => t.OrderedByDoctor)
                    .WithMany(d => d.OrderedTests) // Explicitly point to Doctor's OrderedTests collection
                    .HasForeignKey(t => t.OrderedByDoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add default values for auditing properties
                entity.Property(tr => tr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(tr => tr.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure MedicalRecord relationships
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relationship with Doctor
                entity.HasOne(m => m.Doctor)
                    .WithMany(d => d.MedicalRecords) // Explicitly point to Doctor's MedicalRecords collection
                    .HasForeignKey(m => m.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Patient
                modelBuilder.Entity<MedicalRecord>()
                        .HasOne(m => m.Patient)
                        .WithMany(p => p.MedicalRecords) // Explicitly point to Patient's MedicalRecords collection
                        .HasForeignKey(m => m.PatientId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.Property(mr => mr.RecordDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(mr => mr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(mr => mr.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure UserAuditLog
            modelBuilder.Entity<UserAuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(u => u.User)
                    .WithMany() // Assuming ApplicationUser doesn't have a specific collection for UserAuditLogs
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure constraints for ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_User_YearsOfExperience",
                    "[YearsOfExperience] >= 0"));
            });

            // Add indexes for performance (good practice)
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.AppointmentDate);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.Status);

            modelBuilder.Entity<TestResult>()
                .HasIndex(t => t.TestDate);

            modelBuilder.Entity<UserAuditLog>()
                .HasIndex(u => u.Timestamp);
        }
    }
}