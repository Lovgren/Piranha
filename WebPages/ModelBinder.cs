﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Piranha.WebPages
{
	/// <summary>
	/// Binds models from the http form data.
	/// </summary>
	public class ModelBinder
	{
		/// <summary>
		/// Binds the model from the current form data.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="name">The model name</param>
		/// <param name="prefix">Optional name prefix</param>
		/// <returns>The model</returns>
		public static T BindModel<T>(string name, string prefix = "") {
			return (T)BindModel(typeof(T), name, prefix) ;
		}

		/// <summary>
		/// Binds the model of the given type and name from the current form data.
		/// </summary>
		/// <param name="type">The model type</param>
		/// <param name="name">The model name</param>
		/// <param name="prefix">Optional form prefix</param>
		/// <returns>The model</returns>
		public static object BindModel(Type type, string name, string prefix = "") {
			return BindModel(HttpContext.Current.Request.Form, type, name, prefix) ;
		}

		#region Private members
		/// <summary>
		/// Binds the model of the given type from the given name value collection.
		/// </summary>
		/// <param name="form">The form data</param>
		/// <param name="type">The model type</param>
		/// <param name="name">The model name</param>
		/// <param name="prefix">Optional name prefix</param>
		/// <returns>The model</returns>
		private static object BindModel(NameValueCollection form, Type type, string name, string prefix = "") {
			if (form.AllKeys.Contains(prefix + name)) {
				if (type == typeof(HtmlString))
					return new HtmlString(form[prefix + name]) ;
				else if (typeof(Enum).IsAssignableFrom(type))
					return Enum.Parse(type, form[prefix + name]) ;
				return Convert.ChangeType(form[prefix + name], type) ;
			} else {
				var subform = new NameValueCollection() ;

				form.AllKeys.Each((i, e) => {
					if (e.StartsWith(prefix + name))
						subform.Add(e, form[e]) ;
				});
				if (subform.Count > 0) {
					object ret = Activator.CreateInstance(type) ;
					foreach (PropertyInfo p in ret.GetType().GetProperties()) {
						if (p.CanWrite)
							p.SetValue(ret, BindModel(form, p.PropertyType, p.Name, prefix + name + "."), null) ; 
					}
					return ret ;
				} else if (prefix == "") {
					// There field is missing in the form collection. Let's try to create a
					// default value.
					try {
						return Activator.CreateInstance(type) ;
					} catch {
						return null ;
					}
				}
				return null ;
			}
		}
		#endregion
	}
}
