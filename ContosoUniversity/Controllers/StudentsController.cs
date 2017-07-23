using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Utils;

namespace ContosoUniversity.Controllers
{
    // This is a scaffolded (The automatic creation of CRUD action methods and views is known as scaffolding)
    // Controller that will use EF to query and save the data
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        // notice that Controller obtains SchoolContext in constructor through DI
        public StudentsController(SchoolContext context)
        {
            // called for each separate request
            Console.WriteLine("StudentsController constructor()");
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            // sortOrder and other params are retrieved from the query string

            ViewData["CurrentSort"] = sortOrder; // current sort order
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString; // current filter

            var students = from s in _context.Students
                           select s;
            // students will be an IQueryable, which is not executed when modified unless ToListAsync() is called

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            // TODO make samples for https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/read-related-data
            // Explicit Loading
            //var departments = _context.Departments;
            //foreach (Department department in departments)
            //{
            //    _context.Entry(department).Collection(p => p.Courses).Load();
            //}

            int pageSize = 3;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));

        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments) // load Student.Enrollments navigation property
                    .ThenInclude(e => e.Course) // and within each Enrollment load the Enrollment.Course property
                .AsNoTracking() // just performance optimization
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] // prevents CSRF attacks (injects token into the view by the FormTagHelper)
        // ModelBinder will instantiate a Student entity from passed form parameters
        // bind property also limits the fields that the model binder uses when it creates a Student instance
        // as such if we have another secret property wo don't want to be set on the frontend, and hacker inject it (with fiddler or etc)
        // then it anyways won't be added because of [Bind] annotation not listing it
        public async Task<IActionResult> Create(
            [Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");

            }

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // read the existing entity
            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(s => s.ID == id);

            // try to update the existing model in the database to match ModelBinder's current bound model (provided by the form here)
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "", // prefix to use with the form fields names
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate)) // list the whitelisted fields that are allowed to change
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);

        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking() // do not keep track whether model is in sync with the database (no caching)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            // This code accepts an optional parameter that indicates whether the method was called after a failure to save changes.
            // This parameter is false when the HttpGet Delete method is called without a previous failure.When it is called by the 
            // HttpPost Delete method in response to a database update error, the parameter is true and an error message is passed to the view.
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if problem persists " +
                    "see your system administrator";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                _context.Students.Remove(student); // sets entity status to "Deleted"
                await _context.SaveChangesAsync(); // generates SQL DELETE command for the "Deleted" entity
                // You could also create a student entity with only the passed ID instead of reading the whole entity
                // and explicitly setting it to "Delete" state
                // Student studentToDelete = new Student() { ID = id };
                // However you should also mind the deletion of related entities (configure cascade delete)
                // _context.Entry(studentToDelete).State = EntityState.Deleted;
                return RedirectToAction("Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true }); // pass our delete bool parameter for errors
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
