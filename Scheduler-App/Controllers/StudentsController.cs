using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace Scheduler_App.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext DbContext;
        public StudentsController()
        {
            DbContext = new ApplicationDbContext();
        }

        //List of Students
        public ActionResult Index()
        {
            var model = DbContext
                .StudentDatabase
                .ProjectTo<StudentViewModel>()
                .ToList();
            return View(model);
        }

        //GET : CreateStudent
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult CreateStudent()
        {
            return View();
        }

        //Post : CreateStudent
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult CreateStudent(CreateEditStudentViewModel formData)
        {
            return SaveStudent(null, formData);
        }

        private ActionResult SaveStudent(int? id, CreateEditStudentViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View(formData);
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = formData.Email, Email = formData.Email };
            var result = userManager.Create(user, formData.Password);
            var userId = user.Id;
            var student = Mapper.Map<Student>(formData);

            if (!id.HasValue)
            {
                var singleStudent = DbContext.StudentDatabase.ToList();
                if (singleStudent.Count != 0)
                {
                    if (singleStudent.Any(p => p.Email == student.Email))
                    {
                        var studentEmail = DbContext.StudentDatabase.FirstOrDefault(p => p.Email == student.Email).Email;
                        return RedirectToAction(nameof(InstructorController.EmailError),"Instructor");
                    }
                }
                //DbContext.Users.Add(user);
                DbContext.StudentDatabase.Add(student);
                //DbContext.Users.Add(user);
                DbContext.SaveChanges();
                if (!userManager.IsInRole(user.Id, "Student"))
                {
                    userManager.AddToRoleAsync(user.Id, "Student");
                }
                //string code = userManager.GenerateEmailConfirmationToken(user.Id);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //userManager.SendEmailAsync(userId, "Notification",
                //    "Hello, You are registered as student at MITT.Your current Password is Password-1.Please change your password by clicking <a href=\"" + callbackUrl + "\"> here</a>");
            }
            else
            {
                student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == id);
                if (student == null)
                {
                    return RedirectToAction(nameof(StudentsController.Index));
                }
            }
            student.FirstName = formData.FirstName;
            student.LastName = formData.LastName;
            student.Email = formData.Email;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(StudentsController.Details), new { id = student.Id });
        }

        //GET: Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == id);

            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var model = new CreateEditStudentViewModel();
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Email = student.Email;

            return View(model);
        }

        //POST: EditStudent
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(int id, CreateEditStudentViewModel formData)
        {
            return SaveStudent(id, formData);
        }

        // GET: Delete Action for Student
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            Student student = DbContext.StudentDatabase.Find(id);
            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            return View(student);
        }

        // POST: Delete Action for Student
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Student students = DbContext.StudentDatabase.Find(id);
            DbContext.StudentDatabase.Remove(students);
            DbContext.SaveChanges();
            return RedirectToAction(nameof(StudentsController.Index));
        }

        [HttpGet]
        public ActionResult Details(int? id, int? programId)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(StudentsController.Index));

            var userId = User.Identity.GetUserId();

            var student = DbContext.StudentDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == programId);

            var allStudent = new StudentViewModel();
            allStudent.FirstName = student.FirstName;
            allStudent.LastName = student.LastName;
            allStudent.Email = student.Email;
            allStudent.Courses = student.Courses;
            allStudent.ProgramName = student.Courses.Select(p => p.Program.Name).ToString();

            if (allStudent.Courses.Count != 0)
            {
                //allStudent.ProgramId = program.Id;
                var programName = allStudent.Courses.FirstOrDefault(p => p.Students.Contains(student)).Program.Name;
                programName = student.Courses.FirstOrDefault(p => p.Students.Contains(student)).Program.Name;
                allStudent.ProgramId = student.Courses.FirstOrDefault(p => p.Students.Contains(student)).Program.Id;
                allStudent.ProgramName = programName;
            }
            else
            {
                allStudent.ProgramName = null;
                allStudent.ProgramId = null;
            }

            ViewBag.id = id;
            ViewBag.programId = programId;
            return View(allStudent);
        }

        //Get: Create Student through the CSV
        [HttpGet]
        public ActionResult ImportStudent()
        {
            return View(new List<StudentViewModel>());
        }

        //Post: Create Student through the CSV
        [HttpPost]
        public ActionResult ImportStudent(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                //Upload and save the file  
                string csvPath = Server.MapPath("~/Uploads/") + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(csvPath);

                //Create a DataTable.  
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[4] {
        new DataColumn("FirstName", typeof(string)),
        new DataColumn("LastName", typeof(string)),
        new DataColumn("Email", typeof(string)),
        new DataColumn("ProgramName",typeof(string)) });

                //Read the contents of CSV file.  
                List<Student> students = new List<Student>();
                string csvData = System.IO.File.ReadAllText(csvPath);
                //Execute a loop over the rows.  
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.  
                        foreach (string cell in row.Split(','))
                        {
                            //cell.Replace("\r\n", "");
                            if (cell == "\r" || cell == "\n")
                            {
                                return RedirectToAction(nameof(StudentsController.Index));
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell;
                                i++;
                            }

                        }
                        if ( dt.Columns.Count == 4)
                        {

                            var student = (new Student
                            {
                                FirstName = row.Split(',')[0],
                                LastName = row.Split(',')[1],
                                Email = row.Split(',')[2],
                                ProgramName = row.Split(',')[3],
                            });

                            var singleStudent = DbContext.StudentDatabase.ToList();
                            if (singleStudent.Count != 0)
                            {
                                if (singleStudent.Any(p => p.Email == student.Email))
                                {
                                    var studentEmail = DbContext.StudentDatabase.FirstOrDefault(p => p.Email == student.Email).Email;
                                    return View();
                                }
                            }

                            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                            var user = new ApplicationUser { UserName = student.Email, Email = student.Email };
                            var result = userManager.Create(user, student.Password);
                            //programName = student.
                            //DbContext.Users.Add(user);
                            students.Add(student);
                            var programName = students.FirstOrDefault(p => p.ProgramName == student.ProgramName).ProgramName;
                            char[] charsToTrim = { '\r' };
                            programName = student.ProgramName.Trim(charsToTrim);
                            //programName.Courses.Select(p => p.Name);
                            var programCourse = DbContext.ProgramDatabase.FirstOrDefault(p => p.Name == programName).Courses;
                            if (programCourse != null)
                            {
                                var programStudent = programCourse.FirstOrDefault().Students;
                                programStudent.Add(student);
                                foreach (var singleCourse in programCourse)
                                {
                                    student.Courses.Add(singleCourse);
                                }
                            }

                            DbContext.StudentDatabase.Add(student);
                            var model = new StudentViewModel();
                            model.FirstName = student.FirstName;
                            model.LastName = student.LastName;
                            model.Email = student.Email;
                            model.ProgramName = student.ProgramName;
                            model.Courses = student.Courses;
                            DbContext.SaveChanges();
                            DbContext.Users.Add(user);
                            var userId = user.Id;
                            if (!userManager.IsInRole(userId, "Student"))
                            {
                                userManager.AddToRoleAsync(userId, "Student");
                            }
                            string code = userManager.GenerateEmailConfirmationToken(userId);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            userManager.SendEmailAsync(userId, "Notification",
                                "Hello, You are registered as student at MITT.Your current Password is Password-1.Please change your password by clicking <a href=\"" + callbackUrl + "\"> here</a>");
                        }
                    }
                }
            }
                return RedirectToAction("Index");
        }
        //Method to get the Courses and program List
        [HttpGet]
        public ActionResult AssignCourse(int? studentId, int? ProgramId)
        {
            if (ProgramId == null)
            {
                var allPrograms = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

                ViewBag.allProgram = allPrograms;
                if (!studentId.HasValue)
                {
                    return RedirectToAction(nameof(StudentsController.Index));
                }

                var singleStudent = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
                var formData = new AssignCourseToStudentViewModel();
                formData.StudentId = singleStudent.Id;
                formData.ProgramId = ProgramId;
                formData.ProgramList = allPrograms;
                return View(formData);
            }


            if (!studentId.HasValue)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            var model = new AssignCourseToStudentViewModel();
            model.StudentId = student.Id;
            model.ProgramId = ProgramId;

            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == ProgramId);
            var courseList = DbContext.CourseDatabase.ToList();
            var course = program.Courses.Where(p => p.ProgramId != 0).Select(k => new SelectListItem()
            {
                Text = k.Name,
                Value = k.Id.ToString(),
            }).ToList();

            model.AddCourses = course;
            //model.ProgramList = allPrograms;

            return View(model);
        }

        // Method for get the Courses List
        public JsonResult GetCourses(int ProgramId)
        {
            var courseList = DbContext.CourseDatabase.Where(c => c.ProgramId == ProgramId).Select(c => new
            {
                Name = c.Name,
                Id = c.Id,
            }).ToList();
            return Json(courseList, JsonRequestBehavior.AllowGet);
        }

        // Method for the Assign Course to the Student
        [HttpPost]
        public ActionResult AssignCourse(AssignCourseToStudentViewModel model)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == model.StudentId);
            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Details));
            }

            if (model.AddSelectedCourses != null)
            {
                var assigncourse = DbContext.CourseDatabase.FirstOrDefault(p => p.Id.ToString() == model.AddSelectedCourses);
                assigncourse.Students.Add(student);
                student.Courses.Add(assigncourse);
                DbContext.SaveChanges();

                var program = student.Courses.FirstOrDefault(p => p.Id == assigncourse.Id).Program;
                int? programId = program.Id;
                programId = model.ProgramId;
            }

            return RedirectToAction("Details", new { id = student.Id, programId = model.ProgramId });
        }

        [HttpPost]
        public ActionResult RemoveCourse(int? id, int? studentId)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }
            var course = student.Courses.FirstOrDefault(p => p.Id == id);
            if (course != null)
            {
                var AssignedCourse = student.Courses.Remove(course);
                course.Instructor = null;
                DbContext.SaveChanges();
            }
            return RedirectToAction(nameof(StudentsController.Details), new { id = student.Id });
        }

        [HttpGet]
        public ActionResult AssignProgram(int? studentId)
        {
            var allPrograms = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            ViewBag.allProgram = allPrograms;

            if (!studentId.HasValue)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            var model = new AssignCourseToStudentViewModel();
            model.StudentId = student.Id;
            model.ProgramList = allPrograms;

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignProgram(AssignCourseToStudentViewModel model)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == model.StudentId);
            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Details));
            }


            var assignProgram = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == model.ProgramId);
            var courses = assignProgram.Courses.Where(p => p.ProgramId == model.ProgramId).ToList();
            var program = assignProgram.Courses.FirstOrDefault(p => p.ProgramId == model.ProgramId);
            foreach (var singleCourse in courses)
            {
                singleCourse.Students.Add(student);
            }
            DbContext.SaveChanges();
            var studentProgram = student.Courses.FirstOrDefault(p => p.Program == assignProgram).Program;
            var programId = studentProgram.Id;
            studentProgram = assignProgram;
            programId = assignProgram.Id;
            ViewBag.programId = model.ProgramId;
            DbContext.SaveChanges();
            return RedirectToAction("Details", new { id = student.Id, programId = model.ProgramId });
        }

        [HttpPost]
        public ActionResult RemoveProgram(int? id, int? studentId)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }
            var program = student.Courses.FirstOrDefault(p => p.ProgramId == id).Program;
            //var course = DbContext.CourseDatabase.Where(p => p.ProgramId == id).ToList();
            var courses = student.Courses.ToList();

            if (courses != null)
            {
                foreach (var singleCourse in courses)
                {
                    var assignedcourses = student.Courses.Remove(singleCourse);
                }

                DbContext.SaveChanges();
            }
            return RedirectToAction(nameof(StudentsController.Details), new { id = student.Id });
        }
    }
}