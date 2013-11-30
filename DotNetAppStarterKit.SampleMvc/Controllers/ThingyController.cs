// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetAppStarterKit.Core.Mapping;
using DotNetAppStarterKit.SampleMvc.DataProject.Command.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.Interface;
using DotNetAppStarterKit.SampleMvc.DataProject.Query.QueryDto;
using DotNetAppStarterKit.SampleMvc.Models;

namespace DotNetAppStarterKit.SampleMvc.Controllers
{
    public partial class ThingyController : Controller
    {
        public readonly IMapper<ThingyQueryDto, ThingyModel> DtoToModelMapper;
        public readonly IGetThingyQuery GetThingyQuery;
        public readonly ISaveThingyCommand SaveThingyCommand;

        public ThingyController(IGetThingyQuery getThingyQuery, IMapper<ThingyQueryDto, ThingyModel> dtoToModelMapper,
            ISaveThingyCommand saveThingyCommand)
        {
            GetThingyQuery = getThingyQuery;
            DtoToModelMapper = dtoToModelMapper;
            SaveThingyCommand = saveThingyCommand;
        }

        public virtual async Task<ActionResult> Index(Guid? id)
        {
            var model = new ThingyModel();
            if (id != null && id != Guid.Empty)
            {
                var res = GetThingyQuery.ExecuteCached(id.Value) ?? await GetThingyQuery.ExecuteAsync(id.Value);
                if (res != null)
                {
                    model = DtoToModelMapper.Map(res, new ThingyModel());
                }
            }
            return View(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Index(ThingyModel model)
        {
            if (ModelState.IsValid)
            {
                await SaveThingyCommand.ExecuteAsync(model);
                return RedirectToAction(MVC.Home.Index());
            }
            return View(model);
        }
    }
}