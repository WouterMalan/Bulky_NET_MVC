using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> productList = unitOfWork.Product.GetAll(includeProperties:"Category").OrderBy(x => x.Description).ToList();
        
            return View(productList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = unitOfWork.Product.Get(x => x.Id == id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = webHostEnvironment.WebRootPath;

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(webRootPath, "images/product");

                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    // delete the old image
                    var oldImagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }

                if (productVM.Product.Id != 0)
                {
                    unitOfWork.Product.Update(productVM.Product);
                TempData["Success"] = "Product updated successfully";
                }
                else
                {
                    unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = "Product created successfully";
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(productVM);
            }
        }

        // public IActionResult Delete(int? id)
        // {
        //     if (id == 0)
        //     {
        //         return NotFound();
        //     }

        //     Product productFromDb = unitOfWork.Product.Get(x => x.Id == id);

        //     if (productFromDb == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(productFromDb);
        // }

        // [HttpPost,
        //  ActionName("Delete")]
        //   public IActionResult DeletePost(int? id)
        // {
        //     if (id == 0)
        //     {
        //         return NotFound();
        //     }

        //     Product productFromDb = unitOfWork.Product.Get(x => x.Id == id);

        //     if (productFromDb == null)
        //     {
        //         return NotFound();
        //     }

        //     unitOfWork.Product.Remove(productFromDb);
        //     unitOfWork.Save();
        //     TempData["Success"] = "Product deleted successfully";
        //     return RedirectToAction("Index");
        // }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> allObjProduct = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return Json(new { data = allObjProduct });
        }

         [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productFromDbToBeDeleted = unitOfWork.Product.Get(x => x.Id == id);

            if (productFromDbToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, productFromDbToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            unitOfWork.Product.Remove(productFromDbToBeDeleted);

            unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}