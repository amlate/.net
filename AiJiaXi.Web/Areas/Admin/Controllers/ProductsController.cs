using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Common.Helpers;
using Project.Domain.Entities.Orders;
using Project.Domain.Enums;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Project.Web.Provider;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {
        private IRepository<OrderItem> _orderItemRepository;
        private ApplicationUserManager _userManager;

        public ProductsController(IRepository<OrderItem> orderItemRepository, ApplicationUserManager userManager)
        {
            _orderItemRepository = orderItemRepository;
            _userManager = userManager;
        }
        
        // GET: Admin/Products
        public async Task<ActionResult> Index(string productClass,int id = 1)
        {
            var predicate = PredicateBuilder.True<OrderItem>().And(item => item.IsValid && item.ItemClass.IsValid);
            
            if (string.IsNullOrWhiteSpace(productClass))
            {
                throw new Exception("不存在的类别Id");
            }
            else if (!productClass.IsInt())
            {
                throw new Exception("非法参数");
            }
            else
            {
                var classId_long = long.Parse(productClass);
                ViewBag.ClassId = classId_long;
                predicate = predicate.And(item => item.ItemClassId == classId_long);
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                // 代理商只能看到自己的产品
                var cityId = user.Agency.CityId;
                predicate = predicate.And(item => item.ItemClass.CityId == cityId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("前台会员无权查看！");
            }
            
            var model = this._orderItemRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderItemSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderItemSearchPost(string name, int id = 1)
        {
            return OrderItemSearchResult(name, id);
        }

        private ActionResult OrderItemSearchResult(string name, int id = 1)
        {
            var predicate = PredicateBuilder.True<OrderItem>().And(item => item.IsValid && item.ItemClass.IsValid);
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var cityId = user.Agency.CityId;
                predicate = predicate.And(item => item.ItemClass.CityId == cityId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("前台会员无权查看！");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            var model = this._orderItemRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderItemSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(string classId, long id = 0)
        {
            long classId_long = 0;
            if (string.IsNullOrWhiteSpace(classId))
            {
                throw new Exception("不存在的类别Id");
            }
            else if (!classId.IsInt())
            {
                throw new Exception("非法参数");
            }
            else
            {
                classId_long = long.Parse(classId);
            }
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权管理产品");
            }

            if (id == 0)
            {
                ViewBag.Second = "新增产品类别";
                return View(new OrderItem() { ItemClassId = classId_long});
            }

            var entity = _orderItemRepository.Find(item => item.Id == id);
            if (entity.ItemClassId != classId_long)
            {
                throw new Exception("非法参数");
            }
            ViewBag.Second = "编辑产品类别";

            return entity == null ? View(new OrderItem() { ItemClassId = classId_long }) : View(entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(OrderItem model, HttpPostedFileBase c_icon, HttpPostedFileBase c_icon_hover)
        {
            string c_iconName = c_icon == null ? string.Empty : FileUploadHelper.ProcessUpload(c_icon, Server.MapPath("~/Upload"));
            string c_icon_hoverName = c_icon_hover == null ? string.Empty : FileUploadHelper.ProcessUpload(c_icon_hover, Server.MapPath("~/Upload"));

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权管理产品");
            }

            var entity = _orderItemRepository.Find(item => item.Id == model.Id);
            if (entity == null)
            {
                if (!string.IsNullOrWhiteSpace(c_iconName))
                {
                    model.ImageUrl = c_iconName;
                }

                if (!string.IsNullOrWhiteSpace(c_icon_hoverName))
                {
                    model.HoverImageUrl = c_icon_hoverName;
                }
                
                model.AddTime = DateTime.Now;
                model.AddUser = user.UserName;
                await _orderItemRepository.AddAsync(model);
            }
            else
            {
                if (this.TryUpdateModel(entity))
                {
                    if (!string.IsNullOrWhiteSpace(c_iconName))
                    {
                        entity.ImageUrl = c_iconName;
                    }

                    if (!string.IsNullOrWhiteSpace(c_icon_hoverName))
                    {
                        entity.HoverImageUrl = c_icon_hoverName;
                    }
                    
                    entity.ModifyTime = DateTime.Now;
                    entity.ModifyUser = user.UserName;
                    await _orderItemRepository.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index", new { productClass = model.ItemClassId, id = 1});
        }

        [HttpPost]
        public async Task<ActionResult> Recycle(long id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权删除管理产品类别");
            }

            var entity = _orderItemRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该产品类别不存在，删除失败！');");
            }

            entity.IsValid = false;
            var result = await _orderItemRepository.UpdateAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._orderItemRepository.FindList<OrderItemClass>(item => item.IsValid && item.ItemClass.IsValid)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_OrderItemSearchResult", model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权删除产品");
            }

            var entity = _orderItemRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该产品类别不存在，删除失败！');");
            }
            
            var result = await _orderItemRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._orderItemRepository.FindList<OrderItem>(item => item.IsValid && item.ItemClass.IsValid)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_orderItemRepository", model);
        }
    }
}