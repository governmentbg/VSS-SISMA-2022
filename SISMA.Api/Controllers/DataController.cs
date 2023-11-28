using Microsoft.AspNetCore.Mvc;
using SISMA.Core.Contracts;
using SISMA.Infrastructure.Contracts.Data;

namespace SISMA.Api.Controllers
{
    [Route("data")]
    public class DataController : BaseController
    {
        private readonly IDataService _dataService;
        private readonly ILogger<DataController> _logger;

        public DataController(
            IDataService dataService,
            ILogger<DataController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/text")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult Get()
        {
            //var fileContent = System.IO.File.ReadAllBytes(@"d:\tmp\5-05_obstini-2022-4.xml");
            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SismaModel));

            //var model = x.Deserialize(new MemoryStream(fileContent));


            return Content("SISMA.API works!");
        }

        [HttpPost]
        [Route("submit")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(SismaResponseModel))]
        public async Task<IActionResult> Submit([FromBody] SismaModel model)
        {
            var saveResult = await _dataService.SaveData(model);
            if (!saveResult.IsSuccessfull)
            {
                return Ok(new SismaResponseModel()
                {
                    ResultCode = saveResult.ErrorCode,
                    Message = saveResult.ErrorMessage
                });
            }
            return Ok(new SismaResponseModel()
            {
                ResultCode = SismaConstants.ErrorCodes.OK,
                Message = saveResult.ErrorMessage
            });
        }

        [HttpPost]
        [Route("test")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(SismaResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(SismaResponseModel))]
        public async Task<IActionResult> Test(int catId, int year)
        {
            var model = await _dataService.TestModelTotal(catId, year);
            var saveResult = await _dataService.SaveData(model);
            if (!saveResult.IsSuccessfull)
            {
                return Ok(new SismaResponseModel()
                {
                    ResultCode = saveResult.ErrorCode,
                    Message = saveResult.ErrorMessage
                });
            }
            return Ok(new SismaResponseModel()
            {
                ResultCode = SismaConstants.ErrorCodes.OK,
                Message = saveResult.ErrorMessage
            });
        }

        [HttpPost]
        [Route("getrequest")]
        //[Produces("application/json", "application/xml")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SismaResponseModel))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(SismaResponseModel))]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(SismaResponseModel))]
        //[ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(SismaResponseModel))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(SismaResponseModel))]
        public async Task<IActionResult> GetRequest()
        {
            var model = await _dataService.TestModelCourts();
            var saveResult = await _dataService.SaveData(model);
            return Ok(saveResult);
        }

        //[HttpGet]
        //[Route("xml")]
        //public async Task<IActionResult> xml()
        //{
        //    var model = await _dataService.TestModelCourts();
        //    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SismaModel));
        //    var ms = new MemoryStream();
        //    x.Serialize(ms, model);
        //    var xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
        //    return Ok(xml);
        //}
    }
}