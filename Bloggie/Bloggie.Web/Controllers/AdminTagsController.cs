﻿using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [Authorize]
        //Tag ekleme get metodu
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize]
        //tag ekleme post metodu
        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            //var name= Request.Form["name"];
            //var displayName = Request.Form["displayName"];

            //var name = addTagRequest.Name;
            //var display = addTagRequest.DisplayName;
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [Authorize(Roles = "admin,SuperAdmin")]
        //tagleri listeleme metodu 
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tags = await tagRepository.GetAllAsync();
            return View(tags);
        }

        [Authorize(Roles = "admin,SuperAdmin")]
        //Edit Sayfasının görüntülenme(get) actionu
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //1. metot
            //var tag = bloggieDbContext.Tags.Find(id);

            //2.metot
            var tag = await tagRepository.GetAsync(id);

            if (tag != null)
            {
                var editTagReq = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagReq);
            }

            return View(null);

        }

        [Authorize(Roles = "admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };


            var updatedTag = await tagRepository.UpdateAsync(tag);
            if (updatedTag != null)
            {
                //olur notificationları
                return RedirectToAction("List");
            }
            else
            {
                //hata notificationları
            }

            return RedirectToAction("Edit", new { id = editTagRequest.Id });

        }

        [Authorize(Roles = "admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);
            if (deletedTag != null)
            {

                //success notification
                return RedirectToAction("List");
            }

            //error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

    }
}
