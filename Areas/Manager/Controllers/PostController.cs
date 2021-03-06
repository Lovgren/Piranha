﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Models.Manager.PostModels;

namespace Piranha.Areas.Manager.Controllers
{
    public class PostController : ManagerController
    {
		/// <summary>
		/// Default constructor. Gets the post list.
		/// </summary>
		[Access(Function="ADMIN_POST")]
	    public ActionResult Index() {
			return View("Index", ListModel.Get());
        }

		/// <summary>
		/// Creates a new post.
		/// </summary>
		/// <param name="im">The insert model</param>
		[HttpPost()]
		[Access(Function="ADMIN_POST")]
		public ActionResult Insert(InsertModel im) {
			EditModel pm = EditModel.CreateByTemplate(im.TemplateId) ;

			ViewBag.Title = Piranha.Resources.Post.EditTitleNew + pm.Template.Name.ToLower() ;

			return View("Edit", pm) ;
		}

		/// <summary>
		/// Edits the post with the given id.
		/// </summary>
		/// <param name="id">The post id</param>
		[Access(Function="ADMIN_POST")]
		public ActionResult Edit(string id) {
			EditModel m = EditModel.GetById(new Guid(id)) ;
			ViewBag.Title = Piranha.Resources.Post.EditTitleExisting ;

			return View("Edit", m) ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		[Access(Function="ADMIN_POST")]
		public ActionResult Edit(bool draft, EditModel m) {
			if (ModelState.IsValid) {
				if (m.SaveAll(draft)) {
					ModelState.Clear() ;
					if (!draft)
						SuccessMessage(Piranha.Resources.Post.MessagePublished) ;
					else SuccessMessage(Piranha.Resources.Post.MessageSaved) ;
				} else ErrorMessage(Piranha.Resources.Post.MessageNotSaved) ;
			}
			m.Refresh() ;

			if (m.Post.IsNew)
				ViewBag.Title = Piranha.Resources.Post.EditTitleNew + m.Template.Name.ToLower() ;
			else ViewBag.Title = Piranha.Resources.Post.EditTitleExisting ;

			return View("Edit", m) ;
		}

		/// <summary>
		/// Deletes the post.
		/// </summary>
		/// <param name="id">The post id</param>
		[Access(Function="ADMIN_POST")]
		public ActionResult Delete(string id) {
			EditModel pm = EditModel.GetById(new Guid(id)) ;

			if (pm.DeleteAll())
				SuccessMessage(Piranha.Resources.Post.MessageDeleted) ;
			else ErrorMessage(Piranha.Resources.Post.MessageNotDeleted) ;

			return Index() ;
		}

		/// <summary>
		/// Reverts to latest published verison.
		/// </summary>
		/// <param name="id">The post id.</param>
		[Access(Function="ADMIN_POST")]
		public ActionResult Revert(string id) {
			EditModel.Revert(new Guid(id)) ;

			SuccessMessage(Piranha.Resources.Post.MessageReverted) ;

			return Edit(id) ;
		}

		/// <summary>
		/// Unpublishes the specified page.
		/// </summary>
		/// <param name="id">The post id</param>
		[Access(Function="ADMIN_POST")]
		public ActionResult Unpublish(string id) {
			EditModel.Unpublish(new Guid(id)) ;

			SuccessMessage(Piranha.Resources.Post.MessageUnpublished) ;

			return Edit(id) ;
		}
    }
}
