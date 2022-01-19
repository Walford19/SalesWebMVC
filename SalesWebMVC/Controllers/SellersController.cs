﻿using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using SalesWebMVC.Services.Exceptions;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        readonly SellerService _sellerService;
        readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();

            return View(list);
        }

        public IActionResult Create()
        {
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            _sellerService.Remove(id.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var obj = _sellerService.FindById(id.Value);
            if (obj == null)
                return NotFound();

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var obj = _sellerService.FindById(id.Value);
            if (obj == null)
                return NotFound();

            var departments = _departmentService.FindAll();
            var sellerView = new SellerFormViewModel { Seller = obj, Departments = departments };

            return View(sellerView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            if (seller.Id != id)
                return BadRequest();

            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
            catch(DbConcurrencyException ex)
            {
                return BadRequest();
            }
        }
    }
}
