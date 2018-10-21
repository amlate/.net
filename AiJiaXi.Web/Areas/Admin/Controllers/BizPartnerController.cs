using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Enums;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Project.Web.Provider;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class BizPartnerController : Controller
    {
        private IRepository<BizPartner> _bizPatneRepository;

        public BizPartnerController(IRepository<BizPartner> bizPatneRepository)
        {
            _bizPatneRepository = bizPatneRepository;
        }

        public ActionResult Index(int id = 1)
        {
            var model = _bizPatneRepository.FindList<JoinApplication>(item => true).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_BizPartnerSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult BizPartnerSearchRPost(string name, int id = 1)
        {
            return BizPartnerSearchResult(name,  id);
        }
        private ActionResult BizPartnerSearchResult(string name, int id = 1)
        {
            var predicate = PredicateBuilder.True<BizPartner>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            
            var model = _bizPatneRepository.FindList<JoinApplication>(predicate).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_BizPartnerSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(long id = 0)
        {
            if (id == 0)
            {
                ViewBag.Second = "新增合作伙伴";
                return View(new BizPartner());
            }

            var entity = _bizPatneRepository.Find(item => item.Id == id);
            ViewBag.Second = "编辑合作伙伴";

            return entity == null ? View(new BizPartner()) : View(entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(BizPartner model, HttpPostedFileBase b_thumbnail, HttpPostedFileBase b_image, FormCollection form)
        {
            string thumbnailName = b_thumbnail == null ? string.Empty : FileUploadHelper.ProcessUpload(b_thumbnail, Server.MapPath("~/Upload"));
            string bImageName = b_image == null ? string.Empty : FileUploadHelper.ProcessUpload(b_image, Server.MapPath("~/Upload"));
          
            if (model.Id == 0)
            {
                model.AddTime = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(thumbnailName))
                {
                    model.ThumbNail = thumbnailName;
                }

                if (!string.IsNullOrWhiteSpace(bImageName))
                {
                    model.ImageEntity = new ImageEntity { ImgUrl = bImageName, Height = 0, Width = 0 };
                }

                model.Desc = string.Empty;
                await _bizPatneRepository.AddAsync(model);
            }
            else
            {
                var entity = _bizPatneRepository.Find(item => item.Id == model.Id);
                if (this.TryUpdateModel(entity))
                {
                    if (!string.IsNullOrWhiteSpace(thumbnailName))
                    {
                        entity.ThumbNail = thumbnailName;
                    }

                    if (!string.IsNullOrWhiteSpace(bImageName))
                    {
                        if (entity.ImageEntity != null)
                        {
                            entity.ImageEntity.ImgUrl = bImageName;
                        }
                        else
                        {
                            entity.ImageEntity = new ImageEntity { ImgUrl = bImageName, Height = 0, Width = 0 };

                        }

                    }
                    
                    
                    model.Desc = string.Empty;
                    await _bizPatneRepository.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(long id = 0)
        {
            var entity = await _bizPatneRepository.FindAsync(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('不存在的记录，删除失败！');");
            }

            var result = await _bizPatneRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }


            var model = _bizPatneRepository.FindList<BizPartner>(item => true).OrderByDescending(a => a.AddTime).ToPagedList(1, 15);

            return PartialView("_BizPartnerSearchResult", model);
        }

        public ActionResult Shows()
        {
            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "显示"},
                new Select2Item() {value = "false", text = "不显示"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Post(FormCollection form)
        {
            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _bizPatneRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (string.Equals("flag", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.IsShow = bool.Parse(value);
            }

            var result = await _bizPatneRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}