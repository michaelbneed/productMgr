﻿using System;
using System.Collections.Generic;
using System.Text;
using PM.Common.DtoModels;

namespace PM.Common.ViewModels
{
	public class RequestViewModel
	{
		public List<RequestTypesDto> RequestHistory { get; set; }

		public RequestDto Request { get; set; }

		// TODO - specific data for UI
	}
}
