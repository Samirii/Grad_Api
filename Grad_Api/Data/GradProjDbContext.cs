using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Data;

public partial class GradProjDbContext : IdentityDbContext<ApiUser>
{
    public GradProjDbContext()
    {
    }

    public GradProjDbContext(DbContextOptions<GradProjDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating( modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(250);

            entity.Property(e => e.TeacherName).HasMaxLength(100);
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC07415C3794");

            entity.ToTable("CourseCategory");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("Name ");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lesson__3214EC07751A4E00");

            entity.ToTable("Lesson");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(50)
                .HasColumnName("Content ");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("Title ");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(50)
                .HasColumnName("VideoUrl ");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Lesson_ToTable");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC079DB2C3F8");

            entity.ToTable("Question");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.QuestionText)
                .HasMaxLength(50)
                .HasColumnName("QuestionText ");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_Question_ToTable");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Quiz__3214EC0747465A37");

            entity.ToTable("Quiz");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("Title ");

            entity.HasOne(d => d.Course).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Quiz_ToTable");
        });
      

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(s => s.Teachers)
                  .WithOne(t => t.Subject)
                  .HasForeignKey(t => t.SubjectId)
                  .OnDelete(DeleteBehavior.SetNull);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
