﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SkillUp.DAL.Context;
using SkillUp.DAL.UnitOfWorks;
using SkillUp.Entity.Entities;
using SkillUp.Entity.Entities.Relations.ManyToMany;
using SkillUp.Entity.ViewModels;
using SkillUp.Service.Helpers;
using SkillUp.Service.Services.Abstractions;

namespace SkillUp.Service.Services.Concretes
{
    public class ProductService : IProductService
    {
        readonly IUnitOfWork _unitOfWork;   
        readonly IWebHostEnvironment _env;
        readonly AppDbContext _context;

        public ProductService(IWebHostEnvironment env, IUnitOfWork unitOfWork, AppDbContext context)
        {
            _env = env;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task CreateProductAsync(CreateProductVM productVM)
        {
            var categories = _context.Categories.Where(ctg => productVM.CategoryIds.Contains(ctg.Id));
            var authors = _context.Authors.Where(auth => productVM.AuthorIds.Contains(auth.Id));
            Product product = new Product 
            { 
                Price = productVM.Price,
                DiscountPrice = productVM.DiscountPrice,
                SKU = productVM.SKU,
                Quantity = productVM.Quantity,
                Name = productVM.Name,
                Description = productVM.Description,
                ImageUrl = productVM.Image.SaveFile(Path.Combine(_env.WebRootPath, "user", "assets", "productimg")),
            };
            foreach (var item in categories)
            {
                _context.ProductCategories.Add(new ProductCategory { Product = product, CategoryId = item.Id });
            }
            foreach (var item in authors)
            {
                _context.ProductAuthors.Add(new ProductAuthor { Product = product, AuthorId = item.Id });
            }

            await _unitOfWork.GetRepository<Product>().AddAsync(product);
            await _unitOfWork.SaveAsync();
        }

        public async  Task DeleteProductAsync(int id)
        {
            await _unitOfWork.GetRepository<Product>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ICollection<Product>> GetAllProductAsync()
        {
            var product = await _context.Products.Include(pc=>pc.ProductCategories).ThenInclude(c=>c.Category)
                .Include(pa=>pa.ProductAuthors).ThenInclude(a=>a.Author).Include(ap=>ap.AppUserProducts)
                .ThenInclude(a=>a.AppUser).ToListAsync();
            return product;
        }

        public Task<Product> GetProductById(int id)
        {
            throw new NotImplementedException();
        }

        public async  Task<bool> UpdateProductAsync(int id ,UpdateProductVM productVM)
        {
            var product = await _context.Products.Include(cc => cc.ProductCategories).ThenInclude(c => c.Category)
                .Include(pa=>pa.ProductAuthors).ThenInclude(a=>a.Author).FirstOrDefaultAsync(x => x.Id == id);
            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.Price = productVM.Price;
            product.DiscountPrice = productVM.DiscountPrice;
            product.SKU = productVM.SKU;    
            product.Quantity = productVM.Quantity;  

            foreach (var category in product.ProductCategories)
            {
                if (productVM.CategoryIds.Contains(category.CategoryId))
                {
                    productVM.CategoryIds.Remove(category.CategoryId);
                }
                else
                {
                    _context.ProductCategories.Remove(category);
                }
            }

            foreach (var categoryId in productVM.CategoryIds)
            {
                _context.ProductCategories.Add(new ProductCategory { Product = product, CategoryId = categoryId });
            }

            foreach (var author in product.ProductAuthors)
            {
                if (productVM.AuthorIds.Contains(author.AuthorId))
                {
                    productVM.AuthorIds.Remove(author.AuthorId);
                }
                else
                {
                    _context.ProductAuthors.Remove(author);
                }
            }
            foreach (var authorId in productVM.AuthorIds)
            {
                _context.ProductAuthors.Add(new ProductAuthor { Product = product, AuthorId = authorId });
            }

            await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<UpdateProductVM> UpdateProductById(int id)
        {
            var product = _context.Products.Include(cc => cc.ProductCategories).Include(pa=>pa.ProductAuthors).FirstOrDefault(c => c.Id == id);
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,  
                CategoryIds = new List<int>(),
                AuthorIds= new List<int>(),
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl,
            };
            foreach (var category in product.ProductCategories)
            {
                productVM.CategoryIds.Add(category.CategoryId);
            }
            foreach (var author in product.ProductAuthors)
            {
                productVM.AuthorIds.Add(author.AuthorId);
            }

            return productVM;

        }
    }
}