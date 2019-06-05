using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PM.Common.Extensions
{
	public static class ListExtensions
	{
		public static SelectList ToSelectList<TEntity>(this List<TEntity> entityItems, string valueField,
			string textField) where TEntity : class
		{
			return new SelectList(entityItems, valueField, textField);
		}
		
	}
}
