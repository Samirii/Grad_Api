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

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating( modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC073AE4FFCA");

            entity.ToTable("Course");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CategoryId).HasColumnName("CategoryId ");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("Description ");
            entity.Property(e => e.InstructorId).HasColumnName("InstructorId ");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(50)
                .HasColumnName("ThumbnailUrl ");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("Title ");
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
      

        //modelBuilder.Entity<IdentityRole>().HasData(
        //    new IdentityRole
        //    {
        //        Name = "Student" , 
        //        NormalizedName = "STUDENT",
        //        Id= "0cfbe64f-8d26-4b72-8a66-4655477ffdb1",
        //        ConcurrencyStamp = "0cfbe64f-8d26-4b72-8a66-4655477ffdb1"
        //    },
        //    new IdentityRole
        //    {
        //        Name = "Administrator",
        //        NormalizedName="ADMINISTRATOR",
        //        Id = "40247282-b48e-4140-a75c-48a104e3ba27",
        //        ConcurrencyStamp = "40247282-b48e-4140-a75c-48a104e3ba27"
        //    },
        //    new IdentityRole
        //    {
        //        Name = "Teacher",
        //        NormalizedName = "TEACHER",
        //        Id = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3",
        //        ConcurrencyStamp = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3"
        //    }
        //    );
        //  var hasher = new PasswordHasher<ApiUser>();
        
        //      modelBuilder.Entity<ApiUser>().HasData(
        //        new ApiUser
        //           {
                  
        //               Id = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
        //               Email= "admin@samir.com",
        //               NormalizedEmail= "ADMIN@SAMIR.COM",
        //               UserName = "admin@samir.com",
        //               NormalizedUserName= "ADMIN@SAMIR.COM",
        //               FirstName="System",
        //               LastName="Admin",
        //               PasswordHash = hasher.HashPassword(null, "P@ssword1"),
        //               EmailConfirmed = true,
        //               SecurityStamp = "a566bd1e-536d-4a83-96d4-ac3824012fc3",
        //               ConcurrencyStamp = "a566bd1e-536d-4a83-96d4-ac3824012fc3", 
        //               LockoutEnabled = true 

        //        },
        //    new ApiUser
        //       {
                  
        //            Id = "0cdff28c-16b3-406c-8660-7deac47f3545",  
        //            Email = "teacher@samir.com",
        //            NormalizedEmail = "TEACHER@SAMIR.COM",
        //            UserName = "teacher@samir.com",
        //            NormalizedUserName = "TEACHER@SAMIR.COM",
        //            FirstName = "System",
        //            LastName = "Teacher",
        //            PasswordHash = hasher.HashPassword(null, "P@ssword1"),
        //        EmailConfirmed = true,
        //        SecurityStamp = "0cdff28c-16b3-406c-8660-7deac47f3545",
        //        ConcurrencyStamp = "0cdff28c-16b3-406c-8660-7deac47f3545",
        //        LockoutEnabled = true
        //    },
        //    new ApiUser
        //    {
        //        Id = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
        //        Email = "student@samir.com",
        //        NormalizedEmail = "STUDENT@SAMIR.COM",
        //        UserName = "student@samir.com",
        //        NormalizedUserName = "STUDENT@SAMIR.COM",
        //        FirstName = "System",
        //        LastName = "Student",
        //        PasswordHash = hasher.HashPassword(null, "P@ssword1"),
        //        EmailConfirmed = true,
        //        SecurityStamp = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
        //        ConcurrencyStamp = "26d2fc9b-2420-4bd4-bc15-402364f3c489",
        //        LockoutEnabled = true
        //    }
        //       );
        //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
        //    new IdentityUserRole<string>
        //    {
        //        RoleId = "0cfbe64f-8d26-4b72-8a66-4655477ffdb1",
        //        UserId = "26d2fc9b-2420-4bd4-bc15-402364f3c489"

        //    },
        //    new IdentityUserRole<string>
        //    {
        //        RoleId = "40247282-b48e-4140-a75c-48a104e3ba27",
        //        UserId = "a566bd1e-536d-4a83-96d4-ac3824012fc3"

        //    },
        //    new IdentityUserRole<string>
        //    {
        //        RoleId = "ca88f2e0-79ab-4aea-833b-bcfbe53a8bc3",
        //        UserId = "0cdff28c-16b3-406c-8660-7deac47f3545"

        //    }

        //    );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
