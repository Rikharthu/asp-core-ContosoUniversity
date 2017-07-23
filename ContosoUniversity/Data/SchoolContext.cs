using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Data
{
    // Database Context is a main class that coordinates the Entity Framework functionality for a give data model
    // To create one extend Microsoft.EntityFrameworkCore.DbContext class, and specify which entities are included
    // in the data model
    // Database Contexts are usually injected as services inside Startup.cs
    public class SchoolContext : DbContext
    {
        // See https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/read-related-data
        // to check how data can be retrieved

        // Specify entity models
        // Tables will have the same names as DbSet props (can be overridden)
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        /* DbSet corresponds to a database table, and an entity corresponds to a row in the table */
        // DbSet Enrollments and Students could have been omitted, since Student entity references
        // the Enrollment entity, and the Enrollment entity references the Course entity
        // thus they would be included automatically
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Person> People { get; set; } // inherited by Students and Instructors


        // To initialize Database Context we need to tell information about how to connect to the database provide (Connection String)
        // It can be done in two ways:
        // 1) override onConfiguring method
        // 2) call base constructor upon DbContextOptions with connection string in it
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
            // do nothing
        }

        // Replaced with a constructor
        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ActorDb;Trusted_Connection=True;");
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Override the default naming behavior and specify Table names explicitly
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
            modelBuilder.Entity<Person>().ToTable("Person");

            // specify the tracking property for the Department Entity
            modelBuilder.Entity<Department>()
                .Property(p => p.RowVersion).IsConcurrencyToken();

            // CourseAssigment doesn't have it's own PK property, so tell the EF how to generate it
            // InstructorID and CourseID should function as a composite primary key:
            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });

        }
    }
}
