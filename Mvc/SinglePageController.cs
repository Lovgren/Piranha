﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;
using Piranha.Web;

namespace Piranha.Mvc
{
	public class SinglePageController : Controller
	{
		#region Members
		internal Models.Page page = null ;
		internal Models.PageModel model = null ;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the model.
		/// </summary>
		/// protected T Model { get ; set ; }
		#endregion

		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			string permalink = Request["permalink"] ;
			bool   draft = false ;
			bool   cached = false ;

			// Check if we want to see the draft
			if (User.HasAccess("ADMIN_PAGE")) {
				if (!String.IsNullOrEmpty(Request["draft"])) {
					try {
						draft = Convert.ToBoolean(Request["draft"]) ;
					} catch {}
				}
			}
			
			// Load the current page
			if (!String.IsNullOrEmpty(permalink))
				page = Models.Page.GetByPermalink(permalink, draft) ;
			else page = Models.Page.GetStartpage(draft) ;

			// Check permissions
			if (page.GroupId != Guid.Empty) {
				if (!User.IsMember(page.GroupId)) {
					SysParam param = SysParam.GetByName("LOGIN_PAGE") ;
					if (param != null)
						Response.Redirect(param.Value) ;
					else Response.Redirect("~/") ;
				}
				Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
			} else {
				// Only cache public pages
				DateTime mod = GetLastModified() ;
				cached = false; // ClientCache.HandleClientCache(HttpContext.Current, page.Id.ToString(), mod) ;
			}
			// Load the model if the page wasn't cached
			if (!cached)
				model = PageModel.Get(page) ;
			base.OnActionExecuting(filterContext); 
		}

		/// <summary>
		/// Gets the lastest modification date for caching.
		/// </summary>
		/// <returns></returns>
		protected virtual DateTime GetLastModified() {
			return page.Updated ;
		}

		/// <summary>
		/// Creates the model of the specified type and fills it with the current page data.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <returns>The model</returns>
		protected T CreateModel<T>() where T : PageModel {
			T m = Activator.CreateInstance<T>() ;

			m.Page = model.Page ;
			m.Regions = model.Regions ;
			m.Attachments = model.Attachments ;
			m.Properties = model.Properties ;

			return m ;
		}
	}
}
