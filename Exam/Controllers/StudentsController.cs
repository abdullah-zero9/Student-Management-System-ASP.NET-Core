using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Models.ViewModels;

namespace Exam.Controllers
{
    public class StudentsController : Controller
    {
        private readonly EnrollDb_CoreContext _context;
        private readonly IWebHostEnvironment _he;
        public StudentsController(EnrollDb_CoreContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Students.Include(x => x.Enrolls).ThenInclude(y => y.Course).ToListAsync());
            return View(await _context.Students.Include(x => x.Enrolls).ThenInclude(y => y.Course).OrderByDescending(x => x.StudentId).ToListAsync());
        }
        public IActionResult AddNewCourse(int? id)
        {
            ViewBag.course = new SelectList(_context.Courses, "CourseId", "CourseName", id.ToString() ?? "");
            return PartialView("_AddNewCourse");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentVM studentVM, int[] CourseId)
        {
            if (ModelState.IsValid)
            {
                Student student = new Student
                {
                    StudentName = studentVM.StudentName,
                    DateOfBirth = studentVM.DateOfBirth,
                    Phone = studentVM.Phone,
                    StudentStatus = studentVM.StudentStatus
                };
                var file = studentVM.ImagePath;
                if (file != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string fileToSave = Path.Combine(webroot, folder, Path.GetFileName(file.FileName));
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        student.Image = "/" + folder + "/" + Path.GetFileName(file.FileName);
                    }
                }
                foreach (var item in CourseId)
                {
                    Enroll enroll = new Enroll()
                    {
                        Student = student,
                        StudentId = student.StudentId,
                        CourseId = item
                    };
                    _context.Enrolls.Add(enroll);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.StudentId == id);
            StudentVM studentVM = new StudentVM()
            {
                StudentId = student!.StudentId,
                StudentName = student.StudentName,
                DateOfBirth = student.DateOfBirth,
                Phone = student.Phone,
                Image = student.Image,
                StudentStatus = student.StudentStatus
            };
            var existCourse = _context.Enrolls.Where(x => x.StudentId == id).ToList();
            foreach (var item in existCourse)
            {
                studentVM.CourseList.Add(item.CourseId);
            }
            return View(studentVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(StudentVM studentVM, int[] CourseId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Student student = new Student
        //        {
        //            StudentId = studentVM.StudentId,
        //            StudentName = studentVM.StudentName,
        //            DateOfBirth = studentVM.DateOfBirth,
        //            Phone = studentVM.Phone,
        //            StudentStatus = studentVM.StudentStatus,
        //            Image = studentVM.Image
        //        };
        //        var file = studentVM.ImagePath;
        //        if (file != null)
        //        {
        //            string webroot = _he.WebRootPath;
        //            string folder = "Images";
        //            string imgFileName = Path.GetFileName(studentVM.ImagePath.FileName);
        //            string fileToSave = Path.Combine(webroot, folder, imgFileName);
        //            using (var stream = new FileStream(fileToSave, FileMode.Create))
        //            {
        //                studentVM.ImagePath.CopyTo(stream);
        //                student.Image = "/" + folder + "/" + imgFileName;
        //            }
        //        }

        //        var existCourse = _context.Enrolls.Where(x => x.StudentId == student.StudentId).ToList();
        //        foreach (var item in existCourse)
        //        {
        //            _context.Enrolls.Remove(item);
        //        }
        //        foreach (var item in CourseId)
        //        {
        //            Enroll enroll = new Enroll()
        //            {
        //                StudentId = student.StudentId,
        //                CourseId = item
        //            };
        //            _context.Enrolls.Add(enroll);
        //        }
        //        _context.Update(student);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentVM studentVM, int[] CourseId, bool removePhoto)
        {
            if (ModelState.IsValid)
            {
                Student student = await _context.Students.FindAsync(studentVM.StudentId);
                if (student == null)
                {
                    return NotFound();
                }

                student.StudentName = studentVM.StudentName;
                student.DateOfBirth = studentVM.DateOfBirth;
                student.Phone = studentVM.Phone;
                student.StudentStatus = studentVM.StudentStatus;

                if (removePhoto)
                {
                    // Remove the existing image from the student object and database
                    student.Image = null;
                    // Delete the image file from the file system
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string imagePath = studentVM.Image?.TrimStart('/');
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        string filePath = Path.Combine(webroot, imagePath);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                else
                {
                    var file = studentVM.ImagePath;
                    if (file != null)
                    {
                        string webroot = _he.WebRootPath;
                        string folder = "Images";
                        string imgFileName = Path.GetFileName(studentVM.ImagePath.FileName);
                        string fileToSave = Path.Combine(webroot, folder, imgFileName);
                        using (var stream = new FileStream(fileToSave, FileMode.Create))
                        {
                            studentVM.ImagePath.CopyTo(stream);
                            student.Image = "/" + folder + "/" + imgFileName;
                        }
                    }
                }

                var existCourse = _context.Enrolls.Where(x => x.StudentId == student.StudentId).ToList();
                foreach (var item in existCourse)
                {
                    _context.Enrolls.Remove(item);
                }
                foreach (var item in CourseId)
                {
                    Enroll enroll = new Enroll()
                    {
                        StudentId = student.StudentId,
                        CourseId = item
                    };
                    _context.Enrolls.Add(enroll);
                }
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.StudentId == id);
            var existCourse = _context.Enrolls.Where(x => x.StudentId == id).ToList();
            foreach (var item in existCourse)
            {
                _context.Enrolls.Remove(item);
            }
            _context.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
