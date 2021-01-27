﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCS.Logging.DotNetFramework.Web.Attributes
{
    public sealed class TrackUsageAttribute : ActionFilterAttribute
    {
        private readonly string _productName;
        private readonly string _layerName;
        private readonly string _name;

        public TrackUsageAttribute(string product, string layer, string name)
        {
            _productName = product;
            _layerName = layer;
            _name = name;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Helpers.LogWebUsage(_productName, _layerName, _name);
        }
    }
}
