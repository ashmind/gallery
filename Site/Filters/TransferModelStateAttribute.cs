using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Exigen.StudentManagement.Filters {
    public class TransferModelStateAttribute : FilterAttribute, IActionFilter {
        private const string TempDataKey = "TransferModelState.TempData";

        public void OnActionExecuted(ActionExecutedContext filterContext) {
            if (!(filterContext.Result is RedirectResult) && !(filterContext.Result is RedirectToRouteResult))
                return;

            var modelState = filterContext.Controller.ViewData.ModelState;
            if (modelState.IsValid)
                return;

            var tempData = filterContext.Controller.TempData;
            tempData[TempDataKey] = modelState;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            if (!filterContext.Controller.TempData.ContainsKey(TempDataKey))
                return;

            var preservedModelState = (ModelStateDictionary)filterContext.Controller.TempData[TempDataKey];
            var modelState = filterContext.Controller.ViewData.ModelState;
            foreach (var pair in preservedModelState) {
                modelState.Add(pair.Key, pair.Value);
            }
        }
    }
}
